using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RaporStratejikPlanIzlemeDonem view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RaporStratejikPlanIzlemeDonem : IEntity
    {
        public string StratejikPlanIzlemeGostergeKod { get; set; }

        public int Donem { get; set; }

        public int PlanlananDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double GerceklesenDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double GerceklesenDegerYilSonu { get; set; }

    }
}
