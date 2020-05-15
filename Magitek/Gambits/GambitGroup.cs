using ff14bot.Enums;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace Magitek.Gambits
{
    [AddINotifyPropertyChangedInterface]
    public class GambitGroup
    {
        public string Name { get; set; }

        public int Id { get; set; } = 1;

        public int ZoneId { get; set; }

        public string ZoneName { get; set; }

        public ClassJobType Job { get; set; }

        public string Description { get; set; }

        public ObservableCollection<Gambit> Gambits { get; set; } = new ObservableCollection<Gambit>();
    }
}
