using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki BulguYonetimiCevap tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class BulguYonetimiCevap : EntityBase, IEntity
    {
        public BulguYonetimiCevap()
        {
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("BulguYonetimi")]
        public string BulguYonetimiKod { get; set; }

        [ForeignKey("ViewPersonel")]
        public string IlgiliPersonelKod { get; set; } = "";

        [Column(TypeName = "date")]
        public DateTime? AtamaTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CevapTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TamamlamaTarihi { get; set; }

        [ForeignKey("TanimGenel")]
        public string BulguGorusuKod { get; set; }

        [ForeignKey("TanimGenel")]
        public string OneriGorusuKod { get; set; }

        [ForeignKey("TanimGenel")]
        public string OnemDuzeyiGorusuKod { get; set; }

        [Column(TypeName = "varchar(5000)")]
        public string Aciklama { get; set; }

        [Column(TypeName = "varchar(5000)")]
        public string Eylem { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string Sorumlusu { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [Column(TypeName = "int")]
        public int Tur { get; set; }//1 Eylem 2 İzleme Takip

        [Column(TypeName = "varchar(5000)")]
        public string NedenBulguKatilmiyor { get; set; }
        [Column(TypeName = "varchar(5000)")]
        public string NedenOneriKatilmiyor { get; set; }
        [Column(TypeName = "varchar(5000)")]
        public string NedenOnemKatilmiyor { get; set; }
        [Column(TypeName = "varchar(5000)")]
        public string SonDurum { get; set; }


        public ViewPersonel IlgiliPersonel { get; set; }
        public TanimGenel BulguGorusu { get; set; }
        public TanimGenel OneriGorusu { get; set; }
        public TanimGenel OnemDuzeyiGorusu { get; set; }

        public BulguYonetimi BulguYonetimi { get; set; }

        [NotMapped]
        public Dosya Dosya { get; set; }
        [NotMapped]
        public string SorguDenetimKod { get; set; }
    }

}
