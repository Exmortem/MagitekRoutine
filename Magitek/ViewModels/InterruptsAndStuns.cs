using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using ff14bot;
using Magitek.Commands;
using Magitek.Models.WebResources;
using Magitek.Utilities;
using Microsoft.Win32;
using Newtonsoft.Json;
using PropertyChanged;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class InterruptsAndStuns
    {
        private static InterruptsAndStuns _instance;
        public static InterruptsAndStuns Instance => _instance ?? (_instance = new InterruptsAndStuns());

        private readonly string _interruptsAndStunsFile = @"Settings/" + Core.Me.Name + "/Magitek/InterruptsAndStuns.json";

        public InterruptsAndStuns()
        {

            ActionList = new Utilities.Collections.List<XivDbItem>();  
            
            if (File.Exists(_interruptsAndStunsFile))
            {
                Logger.Write("Loading Interrupts And Stuns From Local File");
                ActionList = JsonConvert.DeserializeObject<Utilities.Collections.List<XivDbItem>>(File.ReadAllText(_interruptsAndStunsFile));
            }

            //SearchedActions = new Utilities.Collections.List<XivDbItem>(XivDataHelper.XivDbActions.Where(r => r.Name.ToLower().Contains("")));
            SearchedActions = new Utilities.Collections.List<XivDbItem>();
        }

        public Utilities.Collections.List<XivDbItem> ActionList { get; set; }
        public Utilities.Collections.List<XivDbItem> SearchedActions { get; set; }

        public ICommand Search => new DelegateCommand<string>(status =>
        {
            SearchedActions = new Utilities.Collections.List<XivDbItem>(XivDataHelper.XivDbActions.Where(r => r.Name.ToLower().Contains(status.ToLower())));
        });

        public ICommand Add => new DelegateCommand<XivDbItem>(status =>
        {
            if (status == null)
                return;

            if (ActionList == null)
            {
                ActionList = new Utilities.Collections.List<XivDbItem>();
            }

            if (ActionList.Contains(status))
                return;

            ActionList?.Add(status);
        });

        public ICommand Remove => new DelegateCommand<XivDbItem>(status =>
        {
            if (status == null)
                return;

            ActionList.Remove(status);
        });

        public void Save()
        {
            var data = JsonConvert.SerializeObject(ActionList, Formatting.Indented);
            var file = new FileInfo(_interruptsAndStunsFile);
            file.Directory?.Create();
            File.WriteAllText(_interruptsAndStunsFile, data);
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

            var data = JsonConvert.SerializeObject(ActionList, Formatting.Indented);
            File.WriteAllText(saveFile.FileName, data);
            Logger.Write($@"Dispels Exported Under {saveFile.FileName} ");
        }
    }
}
