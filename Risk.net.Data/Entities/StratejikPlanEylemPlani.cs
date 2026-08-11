using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki StratejikPlanEylemPlani tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class StratejikPlanEylemPlani : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("StratejikPlanHedef")]
        public string StratejikPlanHedefKod { get; set; }

        [Column(TypeName = "int")]
        public int EylemNo { get; set; }

        [Column(TypeName = "varchar(750)")]
        public string EylemAdi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BaslangicTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BitisTarihi { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double TahminiButce { get; set; }

        [Column(TypeName = "int")]
        public int TamamlanmaDurumu { get; set; }

        [Column(TypeName = "varchar(2500)")]
        public string Notlar { get; set; }
    }
}
