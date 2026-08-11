using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RiskAzaltmaPlaniNot tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RiskAzaltmaPlaniNot : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("RiskAzaltmaPlani")]
        public string RiskAzaltmaPlaniKod { get; set; }

        [Column(TypeName = "varchar(4000)")]
        public string Aciklama { get; set; }
        public DateTime? Tarih { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

    }
}
