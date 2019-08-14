using ff14bot.Managers;

namespace Magitek.Utilities
{
    public static class MagitekActionResourceManager
    {
        public static class BlackMage
        {
            public static bool Enochian => ActionResourceManager.CostTypesStruct.offset_F == 1;
        }
    }
}
