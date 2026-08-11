using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki StratejikPlanHedefGosterge tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class StratejikPlanHedefGosterge : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("StratejikPlanHedef")]
        public string StratejikPlanHedefKod { get; set; } = "";

        [Column(TypeName = "varchar(10)")]
        public string GostergeNo { get; set; } = "";

        [Column(TypeName = "varchar(150)")]
        public string Adi { get; set; }

        [Column(TypeName = "int")]
        public int Etki { get; set; }

        [Column(TypeName = "varchar(1200)")]
        public string Aciklama { get; set; }
        [Column(TypeName = "varchar(250)")]
        public string RevizyonNedeni { get; set; } = "";

        [Column(TypeName = "date")]
        public DateTime? RevizyonTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BaslangicTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BitisTarihi { get; set; }

        [Column(TypeName = "int")]
        public int BaslangicDegeri { get; set; }

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; } = "";

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [NotMapped]
        public string SorguHedefNo { get; set; }

        [NotMapped]
        public double YilDegeri1 { get; set; }
        [NotMapped]
        public double YilDegeri2 { get; set; }
        [NotMapped]
        public double YilDegeri3 { get; set; }
        [NotMapped]
        public double YilDegeri4 { get; set; }
        [NotMapped]
        public double YilDegeri5 { get; set; }

        public ViewBirim Birim { get; set; }
                    
        public List<StratejikPlanIzlemeDonem> Donemler { get; set; }
    }
}
