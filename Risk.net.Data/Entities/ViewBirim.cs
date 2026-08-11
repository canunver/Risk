using Risk.net.Data.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki ViewBirim view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class ViewBirim :  IEntity
    {
        [Key]
        public string Kod { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; }

        public string Adi { get; set; }

        public int Durum { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }

    }
}
