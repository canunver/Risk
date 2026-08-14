using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki Denetim tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class Denetim : EntityBase, IEntity
    {
        public Denetim()
        {
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        public int Kaynak { get; set; }//1-iç-2-dış

        [Column(TypeName = "varchar(10)")]
        public string DenetimNo { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string DenetimAdi { get; set; }

        [Column(TypeName = "int")]
        public int Yil { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BaslangicTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BitisTarihi { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string ReferansNo { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string RaporNo { get; set; }

        //[ForeignKey("ViewKoordinatorluk")]
        //public string KoordinatorlukKod { get; set; }

        //[ForeignKey("ViewBirim")]
        //public string BirimKod { get; set; }

        //[ForeignKey("Surec")]
        //public string SurecKod { get; set; }

        //[ForeignKey("AltSurec")]
        //public string AltSurecKod { get; set; }

        [ForeignKey("TanimGenel")]
        public string DenetimYapanKurumKod { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public List<DenetimBirim> Birimler { get; set; }

        public List<DenetimDenetci> Denetciler { get; set; }
        public List<DenetimSorumlu> Sorumlular { get; set; }

        public List<DenetimGorevlendirme> Gorevlendirme { get; set; }

        public TanimGenel DenetimYapanKurum { get; set; }

        [ForeignKey("DenetimKod")]
        public List<ViewBulguYonetimi> Bulgular { get; set; }


        [NotMapped]
        public Dosya Dosya { get; set; }
        [NotMapped]
        public DateTime? BitisTarihi1 { get; set; }
        [NotMapped]
        public DateTime? BitisTarihi2 { get; set; }
        [NotMapped]
        public string SorguDenetciKod { get; set; }
        [NotMapped]
        public string SorguSorumluKod { get; set; }
        [NotMapped]
        public string SorguKoordinatorlukKod { get; set; }
        [NotMapped]
        public string SorguBirimKod { get; set; }
        [NotMapped]
        public bool KaydetmeYetkisi { get; set; }
    }

}
