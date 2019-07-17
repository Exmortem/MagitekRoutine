using ff14bot;
using ff14bot.Managers;
using System;
using System.Linq;
using ff14bot.Enums;
using static ff14bot.Managers.ActionResourceManager.Samurai;

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

        public class Arcanist
        {
            private static IntPtr? AetherflowLocation = null;

            public static int Aetherflow
            {
                get
                {
                    if (Core.Me.CurrentJob != ClassJobType.Arcanist && Core.Me.CurrentJob != ClassJobType.Scholar && Core.Me.CurrentJob != ClassJobType.Summoner)
                        return 0;

                    if (AetherflowLocation == null)
                        AetherflowLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudACN0").Pointer + 0x278;

                    return Core.Memory.Read<byte>(AetherflowLocation.Value);
                }
            }
        }

        public static class Samurai
        {
            private static IntPtr? KenkiMemoryLocation = null;
            private static IntPtr? SenMemoryLocation = null;

            public static int Kenki
            {
                get
                {
                    if (Core.Me.CurrentJob != ff14bot.Enums.ClassJobType.Samurai)
                        return 0;

                    if(KenkiMemoryLocation == null)
                        KenkiMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudSAM0").Pointer + 0x26C;

                    return Core.Memory.Read<int>(KenkiMemoryLocation.Value);
                }
            }

            public static Iaijutsu Sen
            {
                get
                {
                    if (Core.Me.CurrentJob != ClassJobType.Samurai)
                        return 0;

                    if (SenMemoryLocation == null)
                        SenMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudSAM1").Pointer + 0x268;

                    var Sens = Core.Memory.ReadArray<bool>(SenMemoryLocation.Value, 4);

                    Iaijutsu iaijutsu = 0;
                    if (Sens[1])
                        iaijutsu |= Iaijutsu.Setsu;

                    if (Sens[2])
                        iaijutsu |= Iaijutsu.Getsu;

                    if (Sens[3])
                        iaijutsu |= Iaijutsu.Ka;


                    return iaijutsu;
                }
            }

            public static bool OnChangeJob() {
                Logger.Write("Changing MagitekActionResourceManager");
                KenkiMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudSAM0").Pointer + 0x26C;
                SenMemoryLocation = RaptureAtkUnitManager.GetRawControls.FirstOrDefault(x => x.Name == "JobHudSAM1").Pointer + 0x268;
                return true;
            }
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

        //public class WhiteMage
        //{
        //    public static int Lily
        //    {
        //        get
        //        {
        //            if (Core.Me.CurrentJob != ClassJobType.WhiteMage)
        //                return 0;

        //            return Core.Me.ClassLevel < 74 ? ActionResourceManager.CostTypesStruct.offset_A : ActionResourceManager.CostTypesStruct.offset_C;
        //        }
        //    }

        //    public static int BloodLily
        //    {
        //        get
        //        {
        //            if (Core.Me.CurrentJob != ClassJobType.WhiteMage)
        //                return 0;

        //            return ActionResourceManager.CostTypesStruct.offset_D;
        //        }
        //    }
        //}
    }
}
