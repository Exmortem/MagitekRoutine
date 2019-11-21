using System.Windows.Media;
using ff14bot.Helpers;

namespace Magitek.Utilities
{
    internal static class Logger
    {
        public static void Error(string text, params object[] args)
        {
            Logging.Write(Colors.IndianRed, $@"[Magitek] {text}", args);

        }

        public static void Write(string text, params object[] args)
        {
            Logging.Write(Colors.CornflowerBlue, $@"[Magitek] {text}", args);

        }

        public static void WriteInfo(string text, params object[] args)
        {
            Logging.Write(Colors.Gold, $@"[Magitek] {text}", args);

        }

        public static void WriteCast(string text, params object[] args)
        {
            Logging.Write(Colors.DodgerBlue, $@"[Magitek] {text}", args);

        }
    }
}
