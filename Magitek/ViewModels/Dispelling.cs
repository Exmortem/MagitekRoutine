using System.IO;
using System.Linq;
using System.Windows.Input;
using ff14bot;
using Magitek.Commands;
using Magitek.Models.WebResources;
using Magitek.Utilities;
using Magitek.Utilities.Collections;
using Microsoft.Win32;
using Newtonsoft.Json;
using PropertyChanged;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class Dispelling
    {
        private static Dispelling _instance;
        public static Dispelling Instance => _instance ?? (_instance = new Dispelling());

        private readonly string _dispelsFile = @"Settings/" + Core.Me.Name + "/Magitek/Dispels.json";

        private Dispelling()
        {
            StatusList = new List<XivDbItem>();

            if (File.Exists(_dispelsFile))
            {
                Logger.Write("Loading Dispels From Local File");
                StatusList = JsonConvert.DeserializeObject<List<XivDbItem>>(File.ReadAllText(_dispelsFile));
            }

            SearchedStatuses = new List<XivDbItem>(XivDataHelper.XivDbStatuses.Where(r => r.Name.ToLower().Contains("")));
        }

        public List<XivDbItem> StatusList { get; set; }
        public List<XivDbItem> SearchedStatuses { get; set; }

        public ICommand Search => new DelegateCommand<string>(status =>
        {
            SearchedStatuses = new List<XivDbItem>(XivDataHelper.XivDbStatuses.Where(r => r.Name.ToLower().Contains(status.ToLower())));
        });

        public ICommand Add => new DelegateCommand<XivDbItem>(status =>
        {
            if (status == null)
                return;

            if (StatusList == null)
            {
                StatusList = new List<XivDbItem>();
            }

            if (StatusList.Contains(status))
                return;

            StatusList?.Add(status);
        });

        public ICommand Remove => new DelegateCommand<XivDbItem>(status =>
        {
            if (status == null)
                return;

            StatusList.Remove(status);
        });

        public void Save()
        {
            var data = JsonConvert.SerializeObject(StatusList, Formatting.Indented);
            var file = new FileInfo(_dispelsFile);
            file.Directory?.Create();
            File.WriteAllText(_dispelsFile, data);
        }

        private void SaveAs()
        {
            var saveFile = new SaveFileDialog
            {
                Filter = "json files (*.json)|*.json",
                Title = "Save Dispel File",
                OverwritePrompt = true
            };

            if (saveFile.ShowDialog() != true)
                return;

            var data = JsonConvert.SerializeObject(StatusList, Formatting.Indented);
            File.WriteAllText(saveFile.FileName, data);
            Logger.Write($@"Dispels Exported Under {saveFile.FileName} ");
        }
    }
}
