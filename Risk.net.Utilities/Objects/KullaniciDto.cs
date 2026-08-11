
using System.Collections.Generic;

namespace Risk.net.Utilities.Objects
{
    public class KullaniciDto
    {
        public string KullaniciKod { get; set; }

        public string Unvan { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string EPosta { get; set; }
        public string ResimUrl { get; set; } = "";
        public string PersonelKod { get; set; }
        public string KoordinatorlukKod { get; set; } = "";
        public string BirimKod { get; set; } = "";
        public string AktifRolKod { get; set; } = "";

        public List<KullaniciRolDto> Roller { get; set; } = new List<KullaniciRolDto>();
        //public KullaniciRolDto AktifRol { get; set; } = new KullaniciRolDto();

        public virtual string AdiSoyadi
        {
            get { return Adi + " " + Soyadi; }
        }
    }

    public class KullaniciRolDto
    {
        public string Adi { get; set; }

        public string BirimKod { get; set; }

        public string KoordinatorlukKod { get; set; }

    }
}
