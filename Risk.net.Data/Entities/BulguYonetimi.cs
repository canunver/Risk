using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki BulguYonetimi tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class BulguYonetimi : EntityBase, IEntity
    {
        public BulguYonetimi()
        {
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("Denetim")]
        public string DenetimKod { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string BulguNo { get; set; }

        [Column(TypeName = "varchar(5000)")]
        public string BulguTanimi { get; set; }

        [ForeignKey("TanimGenel")]
        public string OnemDuzeyiKod { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string BulguAciklama { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Nedenler { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Kriterler { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Riskler { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Oneriler { get; set; }

        [Column(TypeName = "date")]
        public DateTime? SonCevapTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DenetimTarihi { get; set; }
        [Column(TypeName = "varchar(5000)")]
        public string NihaiGorus { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string EylemSonDurum { get; set; }

        [Column(TypeName = "int")]
        public int EylemKarsilama { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public TanimGenel OnemDuzeyi { get; set; }
        public Denetim Denetim { get; set; }

        public List<BulguYonetimiBirim> Birimler { get; set; }

        [ForeignKey("BulguYonetimiKod")]
        public List<ViewBulguYonetimiCevapDurum> CevapDurum { get; set; }

        [NotMapped]
        public int SorguKaynak { get; set; }
        [NotMapped]
        public string SorguKoordinatorlukKod { get; set; }
        [NotMapped]
        public string SorguBirimKod { get; set; }
        [NotMapped]
        public Dosya Dosya { get; set; }
        [NotMapped]
        public List<BulguYonetimiCevap> Cevaplar { get; set; }
        [NotMapped]
        public int SorguDurum { get; set; }
        [NotMapped]
        public DateTime? SonCevapTarihi1 { get; set; }
        [NotMapped]
        public DateTime? SonCevapTarihi2 { get; set; }
    }

}
