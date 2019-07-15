using System.Collections.Generic;
using System.Configuration;
using ff14bot.Helpers;
using Magitek.Gambits;

namespace Magitek.Models
{
    public class GambitSettings : JsonSettings
    {
        public GambitSettings(string directory) : base(directory)
        {
        }

        [Setting]
        public IEnumerable<Gambit> Gambits { get; set; }
    }
}