using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Utilities.Managers;

namespace Magitek.Models
{
    public class Enmity
    {
        internal Enmity(EnmityObj obj, int totalEnmity)
        {
            Object = obj.ObjectId == Core.Me.ObjectId ? Core.Me : GameObjectManager.GetObjectByObjectId(obj.ObjectId);
            CurrentEnmity = obj.Enmity;

            if (totalEnmity == 0)
            {
                CurrentEnmityPercent = 0;
                return;
            }

            CurrentEnmityPercent = (int)((CurrentEnmity * 200 + totalEnmity) / (totalEnmity * 2));

        }

        public GameObject Object { get; }
        public uint CurrentEnmity { get; }
        public int CurrentEnmityPercent { get; }
    }
}
