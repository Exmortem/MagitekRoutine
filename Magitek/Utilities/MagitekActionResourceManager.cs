using ff14bot.Managers;

namespace Magitek.Utilities
{
    public static class MagitekActionResourceManager
    {
        public static class BlackMage
        {
            public static ushort PolyGlotTimer => ActionResourceManager.CostTypesStruct.timer;
        }
        public static class DarkKnight
        {
            public static bool DarkArts => ActionResourceManager.CostTypesStruct.offset_C == 1;
        }

        public static class Ninja
        {

            public static int NinkiGauge => ActionResourceManager.CostTypesStruct.offset_A;

        }
    }
}