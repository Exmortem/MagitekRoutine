using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Magitek.Models.WebResources;
using Newtonsoft.Json;

namespace Magitek.Utilities
{
    internal static class XivDataHelper
    {
        static XivDataHelper()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string statusFile = "Magitek.Resources.StatusList.json";

            string statuses;

            using (var stream = assembly.GetManifestResourceStream(statusFile))

            using (var reader = new StreamReader(stream))
            {
                statuses = reader.ReadToEnd();
            }

            XivDbStatuses = new List<XivDbItem>(JsonConvert.DeserializeObject<List<XivDbItem>>(statuses));

            const string actionFile = "Magitek.Resources.ActionList.json";

            string actions;

            using (var stream = assembly.GetManifestResourceStream(actionFile))

            using (var reader = new StreamReader(stream))
            {
                actions = reader.ReadToEnd();
            }

            XivDbActions = new List<XivDbItem>(JsonConvert.DeserializeObject<List<XivDbItem>>(actions));

            const string bossFile = "Magitek.Resources.BossDictionary.json";

            string bosses;

            using (var stream = assembly.GetManifestResourceStream(bossFile))

            using (var reader = new StreamReader(stream))
            {
                bosses = reader.ReadToEnd();
            }

            BossDictionary = new Dictionary<uint, string>(JsonConvert.DeserializeObject<Dictionary<uint, string>>(bosses));
        }


        public static readonly List<XivDbItem> XivDbStatuses;
        public static readonly List<XivDbItem> XivDbActions;
        public static Dictionary<uint, string> BossDictionary;
    }
}
