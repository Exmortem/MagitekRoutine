using System.Windows.Media;
using ff14bot.Helpers;

namespace Magitek.Utilities
{
    internal static class Logger
    {
        public static void Error(string err)
        {
            Logging.Write(Colors.IndianRed, $@"[Magitek] Error: {err}");
        }

        public static void Write(string msg)
        {
            Logging.Write(Colors.CornflowerBlue, $@"[Magitek] {msg}");
        }

        public static void WriteInfo(string msg)
        {
            Logging.Write(Colors.Gold, $@"[Magitek] {msg}");
        }

        public static void WriteCast(string msg)
        {
            Logging.Write(Colors.DodgerBlue, $@"[Magitek] {msg}");
        }
    }
}
