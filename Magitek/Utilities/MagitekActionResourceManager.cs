using ff14bot.Managers;

namespace Magitek.Utilities
{
    public static class MagitekActionResourceManager
    {
        public static class BlackMage
        {
            public static bool Enochian => ActionResourceManager.CostTypesStruct.offset_F == 1;
        }
        public static class DarkKnight
        {
            public static bool DarkArts => ActionResourceManager.CostTypesStruct.offset_C == 1;
        }
    }
}