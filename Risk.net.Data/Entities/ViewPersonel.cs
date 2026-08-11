using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki ViewPersonel view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class ViewPersonel : IEntity
    {
        [Key]
        public string Kod { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string EPosta { get; set; }
        public string UnvanKod { get; set; }
        public string UnvanAdi { get; set; }
        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; }
        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; }
        public string Rol { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }

        public virtual string AdiSoyadi
        {
            get { return Adi + " " + Soyadi; }
        }

        public virtual string AdiSoyadiUnvan
        {
            get { return Adi + " " + Soyadi + " - " + UnvanAdi; }
        }
    }
}
