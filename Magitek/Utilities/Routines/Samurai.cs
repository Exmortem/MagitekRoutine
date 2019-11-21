using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Gambits.Conditions;
using Magitek.Logic;
using Magitek.Models.QueueSpell;
using Magitek.Models.Samurai;
using static ff14bot.Managers.ActionResourceManager.Samurai;

namespace Magitek.Utilities.Routines
{
    internal static class Samurai
    {
        public static bool OnGcd => Spells.Hakaze.Cooldown > TimeSpan.FromMilliseconds(SamuraiSettings.Instance.UseOffGCDAbilitiesWithMoreThanXMSLeft);

        public static int SenCount
        {
            get
            {
                var senCount = 0;
                if (Sen.HasFlag(Iaijutsu.Getsu)) senCount++;
                if (Sen.HasFlag(Iaijutsu.Ka)) senCount++;
                if (Sen.HasFlag(Iaijutsu.Setsu)) senCount++;
                return senCount;
            }
        }

        public static Queue<SpellData> CastDuringMeikyo = new Queue<SpellData>();

        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(10);
            AoeEnemies5Yards = Combat.Enemies.Count(x => x.WithinSpellRange(5));
            AoeEnemies8Yards = Combat.Enemies.Count(x => x.WithinSpellRange(8));
        }

        public static bool QueueKaitenFollowUp(SpellData spell, SpellData tsubame = null)
        {
            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.CancelSpellQueue = () => false;

            Logger.WriteInfo($@"Queueing: {spell.LocalizedName}");

            SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell
            {
                Spell = spell,
                TargetSelf = false,
                Wait = new QueueSpellWait() { Name = "Wait for Kaiten", Check = () => Core.Me.HasAura(Auras.Kaiten), WaitTime = 2000, EndQueueIfWaitFailed = true },
                Checks = new[]
                {
                    new QueueSpellCheck { Check = () => Core.Me.HasAura(Auras.Kaiten), Name = "Has Kaiten"}
                }
            });

            if (tsubame != null)
            {
                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell
                {
                    Spell = tsubame,
                    TargetSelf = false,
                    Wait = new QueueSpellWait()
                        {Name = "Wait for 0 Sen (used Iaijutsu)", Check = () => SenCount == 0, WaitTime = 2000, EndQueueIfWaitFailed = true },
                    Checks = new[]
                    {
                        new QueueSpellCheck {Check = () =>  Casting.LastSpell.Id == spell.Id, Name = "Last spell was Iaijutsu"}
                    }
                });
            }

            return true;
        }

    }
}
