using ff14bot;
using ff14bot.Managers;
using System;
using System.Linq;
using ff14bot.Enums;

namespace Magitek.Utilities
{
    public static class MagitekActionResourceManager
    {
        public class BlackMage
        {
            private static IntPtr? EnochianLocation = null;

            public static bool Enochian
            {
                get
                {
                    if (Core.Me.CurrentJob != ClassJobType.BlackMage)
                        return false;

                    if (EnochianLocation == null)
                        EnochianLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudBLM0").Pointer + 0x2C0;

                    return Core.Memory.Read<bool>(EnochianLocation.Value);
                }
            }
        }

        public static class Arcanist
        {
            public static bool CanTrance => ActionResourceManager.CostTypesStruct.offset_C > 2 && Spells.Trance.Cooldown.TotalMilliseconds == 0;

            public static bool InTrance => ActionResourceManager.CostTypesStruct.offset_A != 0;
            
            public static int Aetherflow =>
                ActionResourceManager.CostTypesStruct.offset_C == 0 ? 0 :
                ActionResourceManager.CostTypesStruct.offset_C == 1 ? 1 :
                ActionResourceManager.CostTypesStruct.offset_C == 2 ? 2 :
                ActionResourceManager.CostTypesStruct.offset_C == 8 ? 0 :
                ActionResourceManager.CostTypesStruct.offset_C == 9 ? 1 :
                ActionResourceManager.CostTypesStruct.offset_C == 10 ? 2 :
                ActionResourceManager.CostTypesStruct.offset_C == 16 ? 0 :
                ActionResourceManager.CostTypesStruct.offset_C == 17 ? 1 :
                ActionResourceManager.CostTypesStruct.offset_C == 18 ? 2 : 0;
        }

        public static class DarkKnight
        {
            private const int BlackBloodOffset = 0x26C;
            private const int DarksideOffset = 0x26C;
            private const int DarkArtsOffset = 0x26A;
            private const int LivingShadowOffset = 0x26C;
            
            
            private static IntPtr? _blackBloodMemoryLocation = null;
            private static IntPtr? _darksideMemoryLocation = null;
            private static IntPtr? _darkArtsMemoryLocation = null;
            private static IntPtr? _livingShadowMemoryLocation = null;

            public static int BlackBlood
            {
                get
                {
                    if (Core.Me.CurrentJob != ff14bot.Enums.ClassJobType.DarkKnight)
                        return 0;

                    if(_blackBloodMemoryLocation == null)
                        _blackBloodMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudDRK0").Pointer + BlackBloodOffset;

                    return Core.Memory.Read<int>(_blackBloodMemoryLocation.Value);
                }
            }
            
            public static TimeSpan Darkside
            {
                get
                {
                    if (Core.Me.CurrentJob != ff14bot.Enums.ClassJobType.DarkKnight)
                        return TimeSpan.Zero;

                    if(_darksideMemoryLocation == null)
                        _darksideMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudDRK1").Pointer + DarksideOffset;
                    
                    return TimeSpan.FromMilliseconds(Core.Memory.Read<int>(_darksideMemoryLocation.Value));
                }
            }
            
            public static bool DarkArts
            {
                get
                {
                    if (Core.Me.CurrentJob != ff14bot.Enums.ClassJobType.DarkKnight)
                        return false;

                    if(_darkArtsMemoryLocation == null)
                        _darkArtsMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudDRK1").Pointer + DarkArtsOffset;

                    return Core.Memory.Read<byte>(_darkArtsMemoryLocation.Value) == 1;
                }
            }

            public static bool OnChangeJob() {
                Logger.Write("Changing MagitekActionResourceManager");
                _blackBloodMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudDRK0").Pointer + 0x26C;
                return true;
            }
        }
    }
}
