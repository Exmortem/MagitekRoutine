using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using Clio.Utilities;
using ff14bot.AClasses;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ICSharpCode.SharpZipLib.Zip;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MagitekLoader;

public class CombatRoutineLoader : CombatRoutine
{
    private const string ProjectName = "Magitek";
    private const string ProjectMainType = "Magitek.Magitek";
    private const string ProjectAssemblyName = "Magitek.dll";
    private const string ZipUrl = "https://github.com/MagitekRB/MagitekRoutine/releases/latest/download/Magitek.zip";
    private const string VersionUrl = "https://github.com/MagitekRB/MagitekRoutine/releases/latest/download/Version.txt";
    private static readonly Color LogColor = Colors.CornflowerBlue;

    private static readonly string ProjectAssembly = Path.Combine(Environment.CurrentDirectory, $@"Routines\{ProjectName}\{ProjectAssemblyName}");
    private static readonly string GreyMagicAssembly = Path.Combine(Environment.CurrentDirectory, @"GreyMagic.dll");
    private static readonly string VersionPath = Path.Combine(Environment.CurrentDirectory, $@"Routines\{ProjectName}\Version.txt");
    private static readonly string BaseDir = Path.Combine(Environment.CurrentDirectory, $@"Routines\{ProjectName}");
    private static readonly string ProjectTypeFolder = Path.Combine(Environment.CurrentDirectory, @"Routines");
    private static string? _latestVersion;

    public CombatRoutineLoader()
    {
        Task.Factory.StartNew(AutoUpdate).Wait();
    }

    public override bool WantButton => true;

    public sealed override CapabilityFlags SupportedCapabilities => CapabilityFlags.All;

    public override float PullRange => Product?.PullRange ?? 25;

    public override ClassJobType[] Class => Product?.Class ?? Array.Empty<ClassJobType>();

    private static CombatRoutine? Product { get; set; }


    public override string Name => ProjectName;
    public override Composite CombatBehavior => Product?.CombatBehavior ?? new Action();
    public override Composite HealBehavior => Product?.HealBehavior ?? new Action();
    public override Composite PullBehavior => Product?.PullBehavior ?? new Action();
    public override Composite PreCombatBuffBehavior => Product?.PreCombatBuffBehavior ?? new Action();
    public override Composite CombatBuffBehavior => Product?.CombatBuffBehavior ?? new Action();
    public override Composite PullBuffBehavior => Product?.PullBuffBehavior ?? new Action();
    public override Composite RestBehavior => Product?.RestBehavior ?? new Action();

    private static string CompiledAssembliesPath => Path.Combine(Utilities.AssemblyDirectory, "CompiledAssemblies");

    public override void Initialize()
    {
        Product?.Initialize();
    }

    public override void OnButtonPress()
    {
        Product?.OnButtonPress();
    }

    public override void Pulse()
    {
        Product?.Pulse();
    }

    public override void ShutDown()
    {
        Product?.ShutDown();
    }

    private static void RedirectAssembly()
    {
        AppDomain.CurrentDomain.AssemblyResolve += Handler;

        AppDomain.CurrentDomain.AssemblyResolve += GreyMagicHandler;
        return;

        Assembly? Handler(object sender, ResolveEventArgs args)
        {
            var name = Assembly.GetEntryAssembly()?.GetName().Name;
            var requestedAssembly = new AssemblyName(args.Name);
            return requestedAssembly.Name != name ? null : Assembly.GetEntryAssembly();
        }

        Assembly? GreyMagicHandler(object sender, ResolveEventArgs args)
        {
            var requestedAssembly = new AssemblyName(args.Name);
            return requestedAssembly.Name != "GreyMagic" ? null : Assembly.LoadFrom(GreyMagicAssembly);
        }
    }

    private static Assembly? LoadAssembly(string path)
    {
        if (!File.Exists(path)) return null;

        if (!Directory.Exists(CompiledAssembliesPath)) Directory.CreateDirectory(CompiledAssembliesPath);

        var t = DateTime.Now.Ticks;
        var name = $"{Path.GetFileNameWithoutExtension(path)}{t}{Path.GetExtension(path)}";
        var pdbPath = path.Replace(Path.GetExtension(path), "pdb");
        var pdb = $"{Path.GetFileNameWithoutExtension(path)}{t}.pdb";
        var capath = Path.Combine(CompiledAssembliesPath, name);
        if (File.Exists(capath))
            try
            {
                File.Delete(capath);
            }
            catch (Exception)
            {
                //
            }

        if (File.Exists(pdb))
            try
            {
                File.Delete(pdb);
            }
            catch (Exception)
            {
                //
            }

        if (!File.Exists(capath)) File.Copy(path, capath);

        if (!File.Exists(pdb) && File.Exists(pdbPath)) File.Copy(pdbPath, pdb);


        Assembly? assembly = null;
        try
        {
            assembly = Assembly.LoadFrom(capath);
        }
        catch (Exception e)
        {
            Logging.WriteException(e);
        }

        return assembly;
    }

    private static CombatRoutine? Load()
    {
        RedirectAssembly();

        var assembly = LoadAssembly(ProjectAssembly);
        if (assembly == null) return null;

        Type baseType;
        try
        {
            baseType = assembly.GetType(ProjectMainType);
        }
        catch (Exception e)
        {
            Log(e.ToString());
            return null;
        }

        CombatRoutine bb;
        try
        {
            bb = (CombatRoutine)Activator.CreateInstance(baseType);
        }
        catch (Exception e)
        {
            Log(e.ToString());
            return null;
        }

        if (bb != null)
            Log(ProjectName + " was loaded successfully.");
        else
            Log("Could not load " + ProjectName + ". This can be due to a new version of Rebornbuddy being released. An update should be ready soon.");

        return bb;
    }

    private static void LoadProduct()
    {
        if (Product != null) return;

        Product = Load();

        if (Product == null)
        {
            Log("Failed to load " + ProjectName + ".");
        }

        //
        //Task.Run(() => Product.Initialize());
    }

    private static void Log(string message)
    {
        message = "[Auto-Updater][" + ProjectName + "] " + message;
        Logging.Write(LogColor, message);
    }

    private static string? GetLocalVersion()
    {
        if (!File.Exists(VersionPath)) return null;

        try
        {
            var version = File.ReadAllText(VersionPath);
            return version;
        }
        catch
        {
            return null;
        }
    }

    private static void AutoUpdate()
    {
        var stopwatch = Stopwatch.StartNew();
        var local = GetLocalVersion();
        _latestVersion = GetLatestVersion().Result;
        var latest = _latestVersion;

        if (local == latest || latest == null || local.StartsWith("pre-"))
        {
            LoadProduct();
            return;
        }

        Log($"Updating to Version: {latest}.");
        var bytes = DownloadLatestVersion().Result;

        if (bytes == null || bytes.Length == 0)
        {
            Log("[Error] Bad product data returned.");
            return;
        }

        if (!Clean(BaseDir))
        {
            Log("[Error] Could not clean directory for update.");
            return;
        }

        if (!Extract(bytes, ProjectTypeFolder + @"\Magitek"))
        {
            Log("[Error] Could not extract new files.");
            return;
        }

        try
        {
            File.WriteAllText(VersionPath, latest);
        }
        catch (Exception e)
        {
            Log(e.ToString());
        }

        stopwatch.Stop();
        Log($"Update complete in {stopwatch.ElapsedMilliseconds} ms.");
        LoadProduct();
    }

    private static async Task<string?> GetLatestVersion()
    {
        using var client = new HttpClient();
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(VersionUrl);
        }
        catch (Exception e)
        {
            Log(e.Message);
            return null;
        }

        if (!response.IsSuccessStatusCode)
            return null;

        string responseMessageBytes;
        try
        {
            responseMessageBytes = await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Log(e.Message);
            return null;
        }

        return responseMessageBytes;
    }

    private static bool Clean(string directory)
    {
        foreach (var file in new DirectoryInfo(directory).GetFiles())
            try
            {
                file.Delete();
            }
            catch
            {
                return false;
            }

        foreach (var dir in new DirectoryInfo(directory).GetDirectories())
            try
            {
                dir.Delete(true);
            }
            catch
            {
                return false;
            }

        return true;
    }

    private static bool Extract(byte[] files, string directory)
    {
        using var stream = new MemoryStream(files);
        var zip = new FastZip();

        try
        {
            zip.ExtractZip(stream, directory, FastZip.Overwrite.Always, null, null, null, false, true);
        }
        catch (Exception e)
        {
            Log(e.ToString());
            return false;
        }

        return true;
    }

    private static async Task<byte[]?> DownloadLatestVersion()
    {
        using var client = new HttpClient();
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(ZipUrl);
        }
        catch (Exception e)
        {
            Log(e.Message);
            return null;
        }

        if (!response.IsSuccessStatusCode)
            return null;

        byte[] responseMessageBytes;
        try
        {
            responseMessageBytes = await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            Log(e.Message);
            return null;
        }

        return responseMessageBytes;
    }
}