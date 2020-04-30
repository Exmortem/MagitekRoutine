using ff14bot;
using GreyMagic;
using Magitek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Magitek.Utilities.Managers
{
    [StructLayout(LayoutKind.Explicit, Size = 0x48)]
    internal struct EnmityObj
    {
        [FieldOffset(0x40)]
        internal uint ObjectId;
        [FieldOffset(0x44)]
        internal uint Enmity;
    }

    internal static class EnmityManager
    {
        private static IntPtr EnmityBasePtr;
        private static IntPtr EnmityCountPtr;

        static EnmityManager()
        {
            var pf = new PatternFinder(Core.Memory);
            EnmityBasePtr = pf.Find("Search 4C 8D 3D ? ? ? ? 66 90 8B 03 48 8D 0D ? ? ? ? 41 89 46 FC 45 33 C0 8B 43 04 Add 3 TraceRelative");
            EnmityCountPtr = pf.Find("Search 89 05 ? ? ? ? 48 8B FA 40 38 32 76 78 Add 2 TraceRelative");
        }

        public static int EnmityCount => Core.Memory.Read<short>(EnmityCountPtr);

        public static IEnumerable<Enmity> EnmityList
        {
            get
            {
                var enmityList = Core.Memory.ReadArray<EnmityObj>(EnmityBasePtr, Math.Min(EnmityCount, 32));
                var totalEnmity = enmityList.Sum(r => r.Enmity);
                return enmityList.Select(r => new Enmity(r, (int)totalEnmity));
            }
        }
    }
}
