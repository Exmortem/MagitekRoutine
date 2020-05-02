/*using System.IO;
using System.Linq;
using System.Windows.Input;
using ff14bot;
using Magitek.Commands;
using Magitek.Models.WebResources;
using Magitek.Utilities;
using Magitek.Utilities.Collections;
using Newtonsoft.Json;
using PropertyChanged;

namespace Magitek.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class TankBusters
    {
        private static TankBusters _instance;
        public static TankBusters Instance => _instance ?? (_instance = new TankBusters());

        private readonly string _tankBustersHealersFile = @"Settings/" + Core.Me.Name + "/Magitek/TankBustersHealers.json";
        private readonly string _tankBustersTanksFile = @"Settings/" + Core.Me.Name + "/Magitek/TankBustersTanks.json";

        public TankBusters()
        {
            ActionListTanks = new List<XivDbItem>();
            ActionListHealers = new List<XivDbItem>();
            ActionDb = new List<XivDbItem>(XivDataHelper.XivDbActions);
            
            if (File.Exists(_tankBustersHealersFile))
            {
                Logger.Write("Loading Healer Tank Busters From Local File");
                ActionListHealers = JsonConvert.DeserializeObject<List<XivDbItem>>(File.ReadAllText(_tankBustersHealersFile));
            }
            else
            {
                Logger.Error("Local Healer Tank Busters File Does Not Exist");
            }

            if (File.Exists(_tankBustersTanksFile))
            {
                Logger.Write("Loading Tank Busters From Local File");
                ActionListTanks = JsonConvert.DeserializeObject<List<XivDbItem>>(File.ReadAllText(_tankBustersTanksFile));
            }
            else
            {
                Logger.Error("Local Tank Busters File Does Not Exist");
            }

            SearchedActionsHealers = new List<XivDbItem>(ActionDb.Where(r => r.Name.ToLower().Contains("")));
            SearchedActionsTanks = new List<XivDbItem>(ActionDb.Where(r => r.Name.ToLower().Contains("")));
        }

        public List<XivDbItem> ActionDb { get; set; }
        public List<XivDbItem> ActionListTanks { get; set; }
        public List<XivDbItem> ActionListHealers { get; set; }
        public List<XivDbItem> SearchedActionsTanks { get; set; }
        public List<XivDbItem> SearchedActionsHealers { get; set; }

        public ICommand SearchTanks => new DelegateCommand<string>(status => { SearchedActionsTanks = new List<XivDbItem>(ActionDb.Where(r => r.Name.ToLower().Contains(status.ToLower()))); });
        public ICommand SearchHealers => new DelegateCommand<string>(status => { SearchedActionsHealers = new List<XivDbItem>(ActionDb.Where(r => r.Name.ToLower().Contains(status.ToLower()))); });

        public ICommand AddTanks => new DelegateCommand<XivDbItem>(action =>
        {
            if (action == null)
                return;

            if (ActionListTanks == null)
            {
                ActionListTanks = new List<XivDbItem>();
            }

            if (ActionListTanks.Contains(action))
                return;

            ActionListTanks?.Add(action);
        });

        public ICommand AddHealers => new DelegateCommand<XivDbItem>(action =>
        {
            if (action == null)
                return;

            if (ActionListHealers == null)
            {
                ActionListHealers = new List<XivDbItem>();
            }

            if (ActionListHealers.Contains(action))
                return;

            ActionListHealers?.Add(action);
        });

        public ICommand RemoveTanks => new DelegateCommand<XivDbItem>(action =>
        {
            if (action == null)
                return;

            ActionListTanks.Remove(action);
        });

        public ICommand RemoveHealers => new DelegateCommand<XivDbItem>(action =>
        {
            if (action == null)
                return;

            ActionListHealers.Remove(action);
        });

        public void Save()
        {
            var data = JsonConvert.SerializeObject(ActionListHealers, Formatting.Indented);
            var file = new FileInfo(_tankBustersHealersFile);
            file.Directory?.Create();
            File.WriteAllText(_tankBustersHealersFile, data);

            var dataTanks = JsonConvert.SerializeObject(ActionListTanks, Formatting.Indented);
            var fileTanks = new FileInfo(_tankBustersTanksFile);
            fileTanks.Directory?.Create();
            File.WriteAllText(_tankBustersTanksFile, dataTanks);
        }
    }
}*/
