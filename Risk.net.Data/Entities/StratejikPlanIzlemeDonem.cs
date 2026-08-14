using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki StratejikPlanIzlemeDonem tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class StratejikPlanIzlemeDonem : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("StratejikPlanHedefGosterge")]
        public string StratejikPlanHedefGostergeKod { get; set; }

        [Column(TypeName = "int")]
        public int Donem { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string DonemAdi { get; set; }

        [Column(TypeName = "int")]
        public int PlanlananDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double GerceklesenDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double GerceklesenDegerYilSonu { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double SapmaOrani { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string SapmaNedeni { get; set; }
    }
}
