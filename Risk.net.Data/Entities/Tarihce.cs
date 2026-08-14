using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki Tarihce tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class Tarihce : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string IlgiKod { get; set; }

        [Column(TypeName = "int")]
        public EnumTarihceIslemTur IlgiTur { get; set; }

        [Column(TypeName = "int")]
        public int SiraNo { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? IslemTarihi { get; set; }

        [ForeignKey("ViewPersonel")]
        [Column(TypeName = "varchar(40)")]
        public string IslemYapanKod { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string Aciklama { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string DegisenDeger { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string IslemYapanRol { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string IlgiliRol { get; set; }

        [NotMapped]
        public string EskiDeger { get; set; }

        [NotMapped]
        public string YeniDeger { get; set; }
        [NotMapped]
        public string DurumAdi { get; set; }

        public ViewPersonel IslemYapan { get; set; }
    }
}
