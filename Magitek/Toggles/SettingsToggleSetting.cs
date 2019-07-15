using System.Text;
using Newtonsoft.Json;

namespace Magitek.Toggles
{
    public class SettingsToggleSetting
    {
        // Setting name
        public string Name { get; set; }

        // Setting type
        public SettingType Type { get; set; }

        // Checked and unchecked values
        public bool BoolCheckedValue { get; set; }
        public bool BoolUncheckedValue { get; set; }

        public int IntCheckedValue { get; set; }
        public int IntUncheckedValue { get; set; }

        public float FloatCheckedValue { get; set; }
        public float FloatUncheckedValue { get; set; }

        [JsonIgnore]
        public string UiText => AddSpacesToSentence(Name, true);

        public string AddSpacesToSentence(string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                    if ((text[i - 1] != ' ' && !char.IsUpper(text[i - 1])) ||
                        (preserveAcronyms && char.IsUpper(text[i - 1]) &&
                         i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                        newText.Append(' ');
                newText.Append(text[i]);
            }

            return newText.ToString();
        }
    }
}
