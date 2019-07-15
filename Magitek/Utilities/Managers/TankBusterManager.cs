using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Enums;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.WebResources;
using Magitek.ViewModels;

namespace Magitek.Utilities.Managers
{
    internal static class TankBusterManager
    {
        public static readonly Dictionary<uint, uint> SuccorList = new Dictionary<uint, uint>();
        public static readonly Dictionary<uint, uint> AdloquiumList = new Dictionary<uint, uint>();
        public static readonly Dictionary<uint, uint> ExcogitationList = new Dictionary<uint, uint>();
        public static readonly HashSet<uint> Cure2List = new HashSet<uint>();
        public static readonly HashSet<uint> MedicaList = new HashSet<uint>();
        public static readonly HashSet<uint> Medica2List = new HashSet<uint>();
        public static readonly HashSet<uint> DivineBenison = new HashSet<uint>();
        public static readonly HashSet<uint> AfflatusRaptureList = new HashSet<uint>();
        public static readonly HashSet<uint> Benefic2List = new HashSet<uint>();
        public static readonly HashSet<uint> HeliosList = new HashSet<uint>();
        public static readonly HashSet<uint> AspectedHeliosList = new HashSet<uint>();
        public static readonly HashSet<uint> AspectedBeneficList = new HashSet<uint>();
        public static readonly HashSet<uint> PalisadeList = new HashSet<uint>();

        public static void ResetHealers()
        {
            if (!Core.Me.IsHealer() && Core.Me.CurrentJob != ClassJobType.Bard)
                return;

            if (Core.Me.CurrentJob == ClassJobType.Scholar || Core.Me.CurrentJob == ClassJobType.Arcanist)
            {
                SuccorList.Clear();
                AdloquiumList.Clear();
                ExcogitationList.Clear();

                if (TankBusters.Instance.ActionListHealers != null && TankBusters.Instance.ActionListHealers.Any())
                {
                    foreach (var action in TankBusters.Instance.ActionListHealers)
                    {
                        if (action.ScholarTbStrategy == ScholarTbStrategies.Succor) { SuccorList.Add(action.Id, action.TankBusterTimeIntoCast); }
                        if (action.ScholarTbStrategy == ScholarTbStrategies.Adloquium) { AdloquiumList.Add(action.Id, action.TankBusterTimeIntoCast); }
                        if (action.ScholarTbStrategy == ScholarTbStrategies.Excogitation) { ExcogitationList.Add(action.Id, action.TankBusterTimeIntoCast); }
                    }
                }

                Logger.Write("Reset Scholar Tank Busters");
            }

            if (Core.Me.CurrentJob == ClassJobType.WhiteMage || Core.Me.CurrentJob == ClassJobType.Conjurer)
            {
                Cure2List.Clear();
                MedicaList.Clear();
                Medica2List.Clear();
                DivineBenison.Clear();

                if (TankBusters.Instance.ActionListHealers != null && TankBusters.Instance.ActionListHealers.Any())
                {
                    foreach (var action in TankBusters.Instance.ActionListHealers)
                    {
                        if (action.WhiteMageTbStrategy == WhiteMageTbStrategies.Cure2) { Cure2List.Add(action.Id); }
                        if (action.WhiteMageTbStrategy == WhiteMageTbStrategies.Medica) { MedicaList.Add(action.Id); }
                        if (action.WhiteMageTbStrategy == WhiteMageTbStrategies.Medica2) { Medica2List.Add(action.Id); }
                        if (action.WhiteMageTbStrategy == WhiteMageTbStrategies.AfflatusRapture) { AfflatusRaptureList.Add(action.Id); }
                        if (action.WhiteMageTbStrategy == WhiteMageTbStrategies.DivineBenison) { DivineBenison.Add(action.Id); }
                    }
                }

                Logger.Write("Reset White Mage Tank Busters");
            }
            
            if (Core.Me.CurrentJob == ClassJobType.Astrologian)
            {
                Benefic2List.Clear();
                HeliosList.Clear();
                AspectedHeliosList.Clear();
                AspectedBeneficList.Clear();

                if (TankBusters.Instance.ActionListHealers != null && TankBusters.Instance.ActionListHealers.Any())
                {

                    foreach (var action in TankBusters.Instance.ActionListHealers)
                    {
                        Logger.WriteInfo($@"ResetHealers: E5 {action.AstrologianTbStrategy}");
                        if (action.AstrologianTbStrategy == AstrologianTbStrategies.Benefic2) { Benefic2List.Add(action.Id); }
                        if (action.AstrologianTbStrategy == AstrologianTbStrategies.Helios) { HeliosList.Add(action.Id); }
                        if (action.AstrologianTbStrategy == AstrologianTbStrategies.AspectedHelios) { AspectedHeliosList.Add(action.Id); 
                        } if (action.AstrologianTbStrategy == AstrologianTbStrategies.AspectedBenefic) { AspectedBeneficList.Add(action.Id); }
                    }
                }

                Logger.Write("Reset Astrologian Tank Busters");
            }
        }
        
        public static readonly Dictionary<uint, XivDbItem> PaladinTankBusters = new Dictionary<uint, XivDbItem>();
        public static readonly Dictionary<uint, XivDbItem> WarriorTankBusters = new Dictionary<uint, XivDbItem>();
        public static readonly Dictionary<uint, XivDbItem> DarkKnightTankBusters = new Dictionary<uint, XivDbItem>();
        public static readonly Dictionary<uint, XivDbItem> GunbreakerTankBusters = new Dictionary<uint, XivDbItem>();
        
        public static void ResetTanks()
        {
            if (!Core.Me.IsTank())
                return;
            
            PaladinTankBusters.Clear();
            DarkKnightTankBusters.Clear();
            WarriorTankBusters.Clear();
            GunbreakerTankBusters.Clear();

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (RotationManager.CurrentRotation)
            {
                    case ClassJobType.Paladin:

                        if (TankBusters.Instance.ActionListTanks != null && TankBusters.Instance.ActionListTanks.Any())
                        {
                            foreach (var action in TankBusters.Instance.ActionListTanks)
                            {
                                if (!action.PaladinTankBuster)
                                    continue;

                                PaladinTankBusters.Add(action.Id, action);
                            }
                        }

                        Logger.WriteInfo("Reset Paladin Tank Busters");

                    break;
                        
                    case ClassJobType.Warrior:

                        if (TankBusters.Instance.ActionListTanks != null && TankBusters.Instance.ActionListTanks.Any())
                        {
                            foreach (var action in TankBusters.Instance.ActionListTanks)
                            {
                                if (!action.WarriorTankBuster)
                                    continue;

                                WarriorTankBusters.Add(action.Id, action);
                            }}
                        

                        Logger.WriteInfo("Reset Warrior Tank Busters");
                    break;

                    case ClassJobType.DarkKnight:

                        if (TankBusters.Instance.ActionListTanks != null && TankBusters.Instance.ActionListTanks.Any()){
                            
                            foreach (var action in TankBusters.Instance.ActionListTanks)
                            {
                                if (!action.DarkKnightTankBuster)
                                    continue;
    
                                DarkKnightTankBusters.Add(action.Id, action);
                            }}

                        Logger.WriteInfo("Reset Dark Knight Busters");
                    break;

                    case ClassJobType.Gunbreaker:

                        if (TankBusters.Instance.ActionListTanks != null && TankBusters.Instance.ActionListTanks.Any())
                        {

                            foreach (var action in TankBusters.Instance.ActionListTanks)
                            {
                                if (!action.GunbreakerTankBuster)
                                    continue;

                                GunbreakerTankBusters.Add(action.Id, action);
                            }
                        }

                        Logger.WriteInfo("Reset Gunbreaker Busters");
                        break;
            }
                
        }
    }
}
