using ff14bot.Managers;
using ff14bot.Objects;
using Newtonsoft.Json;
using PropertyChanged;

namespace Magitek.Models.QueueSpell
{
    [AddINotifyPropertyChangedInterface]
    public class JobSpell
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public uint Id { get; set; }

        [JsonIgnore]
        public SpellData Spell => DataManager.GetSpellData(Id);
    }
}
