using ff14bot.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Magitek.Models;

[Serializable]
public abstract class JsonSettings : INotifyPropertyChanged
{
#nullable enable
    private readonly object _lock = new();
    private bool _loaded;
    public static string CharacterSettingsDirectory { get; } = ff14bot.Helpers.JsonSettings.CharacterSettingsDirectory;

    protected JsonSettings(string path)
    {
        FilePath = path;
        LoadFrom(FilePath);
    }
    //
    public static string AssemblyPath => Utils.AssemblyDirectory ?? throw new InvalidOperationException();

    [JsonIgnore]
    private string FilePath { get; }

    public static string SettingsPath => Path.Combine(AssemblyPath, "Settings");

    public static string GetSettingsFilePath(params string[] subPathParts)
    {
        var list = new List<string> { SettingsPath };
        list.AddRange(subPathParts);
        return Path.Combine(list.ToArray());
    }

    protected void LoadFrom(string file)
    {
        var properties = GetType().GetProperties();

        foreach (var propertyInfo in properties)
        {
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(DefaultValueAttribute), true);

            if (customAttributes.Length == 0)
            {
                continue;
            }

            foreach (var custom in customAttributes)
            {
                var defaultValueAttribute = custom as DefaultValueAttribute;
                if (propertyInfo.GetSetMethod() != null)
                {
                    propertyInfo.SetValue(this, defaultValueAttribute?.Value, null);
                }
            }
        }

        if (File.Exists(file))
        {
            try
            {
                JsonConvert.PopulateObject(File.ReadAllText(file), this);
            }
            catch (Exception e)
            {
                Logging.WriteException(e);
            }
        }

        _loaded = true;
        if (file != FilePath || !File.Exists(file))
        {
            Save();
        }
    }

    public virtual void Save()
    {
        lock (_lock)
        {
            SaveAs(FilePath);
        }
    }

    public void SaveAs(string file)
    {
        try
        {
            if (!_loaded)
            {
                return;
            }

            if (!File.Exists(file))
            {
                var directoryName = Path.GetDirectoryName(file);
                if (directoryName != null && !Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }

            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        catch (Exception e)
        {
            Logging.WriteException(e);
        }
    }

    public void Reload()
    {
        LoadFrom(FilePath);
        Reloaded?.Invoke(this, EventArgs.Empty);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (_loaded)
            Save();
        else
            return;

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public event EventHandler? Reloaded;

    public event PropertyChangedEventHandler? PropertyChanged;
}

[Serializable]
public class JsonSettings<T> : JsonSettings where T : JsonSettings<T>, new()
{
    private static T? _instance;

    public JsonSettings() : base(GetSettingsFilePath($"{typeof(T).Name}.json"))
    {
    }

    public JsonSettings(string settingsFilePath) : base(settingsFilePath)
    {
    }

    public static T Instance
    {
        get => _instance ??= new T();
        set => _instance = value;
    }
}