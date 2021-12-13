using Magitek.Commands;
using Magitek.Extensions;
using Magitek.Models.WebResources;
using Magitek.ViewModels;
using PropertyChanged;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Magitek.Models.Debugging
{
    [AddINotifyPropertyChangedInterface]
    public class EnemySpellCastInfo
    {
        public EnemySpellCastInfo(string name, uint id, string castedBy)
        {
            Name = name;
            Id = id;
            CastedBy = castedBy;
            Icon = this.GetIcon();
        }

        public string Name { get; set; }
        public uint Id { get; set; }
        public string CastedBy { get; set; }
        public double Icon { get; set; }

        public string IconUrl
        {
            get
            {
                var folder = (Math.Floor(Icon / 1000) * 1000).ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
                var image = Icon.ToString(CultureInfo.InvariantCulture).Trim().PadLeft(6, '0');
                return $@"https://secure.xivdb.com/img/game/{folder}/{image}.png";
            }
        }

        public ICommand AddToInterruptsAndStuns => new DelegateCommand<EnemySpellCastInfo>(info =>
        {
            if (info == null)
                return;

            if (InterruptsAndStuns.Instance.ActionList.Any(r => r.Id == info.Id))
                return;

            var newXivItem = new XivDbItem()
            {
                Id = info.Id,
                Name = info.Name,
                Icon = info.Icon,
                Stun = true,
                Interrupt = true,
                Scholar = true,
                BlueMage = true,
                Bard = true,
                Paladin = true,
                Warrior = true,
                DarkKnight = true,
                Machinist = true,
                Dragoon = true,
                Monk = true,
                Ninja = true
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                InterruptsAndStuns.Instance.ActionList.Add(newXivItem);
            });
        });

        /*public ICommand AddToTankBusters => new DelegateCommand<EnemySpellCastInfo>(info =>
        {
            if (info == null)
                return;

            var newXivItem = new XivDbItem()
            {
                Id = info.Id,
                Name = info.Name,
                Icon = info.Icon
            };

            if (!TankBusters.Instance.ActionListHealers.Select(r => r.Id).Contains(info.Id))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TankBusters.Instance.ActionListHealers.Add(newXivItem);
                });
            }

            if (!TankBusters.Instance.ActionListTanks.Select(r => r.Id).Contains(info.Id))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TankBusters.Instance.ActionListTanks.Add(newXivItem);
                });
            }

        });*/
    }
}
