using ff14bot;
using ff14bot.Enums;
using System.Windows.Controls;

namespace Magitek.Views.UserControls.Lists
{
    public partial class TankBustersTanks : UserControl
    {
        public TankBustersTanks()
        {
            InitializeComponent();

            switch (Core.Me.CurrentJob)
            {
                case ClassJobType.Gladiator:
                    PaladinTab.IsSelected = true;
                    break;

                case ClassJobType.Paladin:
                    PaladinTab.IsSelected = true;
                    break;

                case ClassJobType.Marauder:
                    WarriorTab.IsSelected = true;
                    break;

                case ClassJobType.Warrior:
                    WarriorTab.IsSelected = true;
                    break;

                case ClassJobType.DarkKnight:
                    DarkKnightTab.IsSelected = true;
                    break;
            }
        }
    }
}
