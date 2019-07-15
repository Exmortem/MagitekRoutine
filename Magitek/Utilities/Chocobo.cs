using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using Magitek.Models.Account;

namespace Magitek.Utilities
{
    internal static class Chocobo
    {
        public static async Task<bool> HandleChocobo()
        {
            if (await SummonChocobo()) return true;
            return ManageChocoboStance();            
        }

        private static async Task<bool> SummonChocobo()
        {
            if (!BaseSettings.Instance.SummonChocobo)
                return false;

            if (!ChocoboManager.CanSummon)
                return false;

            if (ChocoboManager.Summoned)
                return false;

            if (MovementManager.IsMoving)
                return false;
      
            if (DutyManager.InInstance)
                return false;
                  
            ChocoboManager.Summon();
            Logger.WriteInfo("Summoning Chocobo");
            await Coroutine.Wait(4000, () => ChocoboManager.Summoned);

            return true;
        }

        private static bool ManageChocoboStance()
        {
            if (!BaseSettings.Instance.SummonChocobo)
                return false;

            if (!ChocoboManager.Summoned)
                return false;

            if (Core.Me.IsMounted)
                return false;

            if (DutyManager.InInstance)
                return false;

            // If we're stance dancing, we just need to switch to healer whenever we need to
            // so we can leave everything else as is

            if (BaseSettings.Instance.ChocoboStanceDance)
            {
                if (ChocoboManager.Stance == CompanionStance.Healer && Core.Player.CurrentHealthPercent < BaseSettings.Instance.ChocoboStanceDanceHealthPercent)
                    return false;

                if (Core.Player.CurrentHealthPercent < BaseSettings.Instance.ChocoboStanceDanceHealthPercent)
                {
                    ChocoboManager.HealerStance();
                    Logger.WriteInfo("[Chocobo] Switching To Healer Stance");
                    return false;
                }
            }

            switch (BaseSettings.Instance.ChocoboStance)
            {
                case CompanionStance.Free:
                    if (ChocoboManager.Stance == CompanionStance.Free)
                        break;

                    ChocoboManager.FreeStance();
                    Logger.WriteInfo("[Chocobo] Switching To Free Stance");
                    break;

                case CompanionStance.Attacker:
                    if (ChocoboManager.Stance == CompanionStance.Attacker)
                        break;

                    ChocoboManager.AttackerStance();
                    Logger.WriteInfo("[Chocobo] Switching To Attacker Stance");
                    break;

                case CompanionStance.Healer:
                    if (ChocoboManager.Stance == CompanionStance.Healer)
                        break;

                    ChocoboManager.HealerStance();
                    Logger.WriteInfo("[Chocobo] Switching To Healer Stance");
                    break;

                case CompanionStance.Defender:
                    if (ChocoboManager.Stance == CompanionStance.Defender)
                        break;

                    ChocoboManager.DefenderStance();
                    Logger.WriteInfo("[Chocobo] Switching To Defender Stance");
                    break;

                case CompanionStance.Follow:
                    break;

                default:
                    break;
            }
            return false;
        }

    }
}
