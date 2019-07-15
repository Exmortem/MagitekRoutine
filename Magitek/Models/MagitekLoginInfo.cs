using PropertyChanged;

namespace Magitek.Models
{
    [ImplementPropertyChanged]
    public class MagitekLoginInfo
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
