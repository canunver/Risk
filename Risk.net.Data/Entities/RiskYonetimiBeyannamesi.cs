using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RiskYonetimiBeyannamesi tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RiskYonetimiBeyannamesi : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        public int Yil { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        [Column(TypeName = "varchar(40)")]
        public string KoordinatorlukKod { get; set; } = "";

        [Column(TypeName = "datetime")]
        public DateTime? IslemTarihi { get; set; }

        [ForeignKey("ViewPersonel")]
        [Column(TypeName = "varchar(40)")]
        public string IslemYapanKod { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string IslemYapanRol { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public ViewPersonel IslemYapan { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }

        [NotMapped]
        public string DurumAdi { get; set; } = string.Empty;
    }
}
