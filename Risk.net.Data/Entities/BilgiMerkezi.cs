using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki BilgiMerkezi tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class BilgiMerkezi : EntityBase, IEntity
    {
        public BilgiMerkezi()
        {
            KayitTarihi = (DateTime?)null;
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [Column(TypeName = "varchar(150)")]
        public string Adi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? KayitTarihi { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string KayitEden { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string SurumNo { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [NotMapped]
        public string RevizeKod { get; set; }

        public Dosya Dosya { get; set; }
    }
}
