using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki Grafik view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class Grafik : IEntity
    {
        public string Aciklama { get; set; }
        public string EkAciklama { get; set; }

        public int Deger1 { get; set; }

        [Column(TypeName = "decimal(13, 2)")]
        public double Deger2 { get; set; }

        [NotMapped]
        public DateTime? sorguTarihi1 { get; set; }
        [NotMapped]
        public DateTime? sorguTarihi2 { get; set; }

        [NotMapped]
        public string KoordinatorlukKod { get; set; }
        [NotMapped]
        public string BirimKod { get; set; }
    }
}
