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
    public class TargetAuraInfo
    {
        public TargetAuraInfo(string name, uint id, string affectedName)
        {
            Name = name;
            Id = id;
            AffectedName = affectedName;
            Icon = this.GetIcon();
        }

        public string Name { get; set; }
        public uint Id { get; set; }
        public string AffectedName { get; set; }
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

        public ICommand AddToDispels => new DelegateCommand<TargetAuraInfo>(info =>
        {
            if (info == null)
                return;

            if (Dispelling.Instance.StatusList.Any(r => r.Id == info.Id))
                return;

            var newXivItem = new XivDbItem()
            {
                Id = info.Id,
                Name = info.Name,
                Icon = info.Icon,
                Scholar = true,
                Bard = true,
                BlueMage = true,
                Astrologian = true
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                Dispelling.Instance.StatusList.Add(newXivItem);
            });
        });
    }
}