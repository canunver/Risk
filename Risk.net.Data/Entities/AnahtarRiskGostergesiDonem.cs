using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki AnahtarRiskGostergesiDonem tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class AnahtarRiskGostergesiDonem : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("AnahtarRiskGostergesi")]
        public string AnahtarRiskGostergesiKod { get; set; }

        public int Donem { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double GerceklesenDeger { get; set; }

        [Column(TypeName = "int")]
        public EnumAnahtarRiskGostergesiDonemPeriyot Periyot { get; set; }
    }
}
