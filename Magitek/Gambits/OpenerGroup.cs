using System.Collections.ObjectModel;
using ff14bot.Enums;
using Magitek.Gambits.Conditions;
using PropertyChanged;

namespace Magitek.Gambits
{
    [AddINotifyPropertyChangedInterface]
    public class OpenerGroup
    {
        public string Name { get; set; }   
        public int Id { get; set; } = 1; 
        public int ZoneId { get; set; }  
        public string ZoneName { get; set; }
        public ClassJobType Job { get; set; }
        public string Description { get; set; }
        public int MinimumLevel { get; set; }
        public bool OnlyUseOncePerCombat { get; set; }
        public ObservableCollection<Gambit> Gambits { get; set; } = new ObservableCollection<Gambit>();
        public ObservableCollection<IGambitCondition> StartOpenerConditions { get; set; } = new ObservableCollection<IGambitCondition>();
    }
}
