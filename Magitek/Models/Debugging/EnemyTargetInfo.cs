using PropertyChanged;

namespace Magitek.Models.Debugging
{
    [AddINotifyPropertyChangedInterface]
    public class EnemyTargetInfo
    {
        public EnemyTargetInfo(string name, uint id, ushort zoneid, uint maxHealth, byte level)
        {
            Name = name;
            Id = id;
            ZoneId = ZoneId;
            MaxHealth = maxHealth;
            Level = level;
        }

        public string Name { get; set; }
        public uint Id { get; set; }
        public uint ZoneId { get; set; }
        public uint MaxHealth { get; set; }
        public byte Level { get; set; }
    }
}
