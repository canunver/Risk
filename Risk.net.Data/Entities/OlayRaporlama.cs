using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki OlayRaporlama tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class OlayRaporlama : EntityBase, IEntity
    {
        public OlayRaporlama()
        {
            OlayTarihi = (DateTime?)null;
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("ViewKoordinatorluk")]
        [Column(TypeName = "varchar(40)")]
        public string KoordinatorlukKod { get; set; } = "";

        [ForeignKey("ViewBirim")]
        [Column(TypeName = "varchar(40)")]
        public string BirimKod { get; set; } = "";

        [ForeignKey("TanimOlayKategori")]
        [Column(TypeName = "varchar(40)")]
        public string OlayKategoriKod { get; set; } = "";

        [Column(TypeName = "date")]
        public DateTime? OlayTarihi { get; set; }

        [Column(TypeName = "varchar(350)")]
        public string OlayYeri { get; set; }

        [Column(TypeName = "varchar(1500)")]
        public string OlayTanimi { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double Tutari { get; set; }

        [Column(TypeName = "varchar(2100)")]
        public string KokNedeni { get; set; }

        [Column(TypeName = "varchar(2100)")]
        public string Aciklama { get; set; }

        //[ForeignKey("ViewPersonel")]
        //[Column(TypeName = "varchar(40)")]
        //public string KayitEdenPersonelKod { get; set; } = "";

        //[ForeignKey("ViewPersonel")]
        //[Column(TypeName = "varchar(40)")]
        //public string OnaylayanPersonelKod { get; set; } = "";

        //[Column(TypeName = "varchar(40)")]
        //public string OnaylayacakYetkiKod { get; set; } = "";

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? KayitTarihi { get; set; }

        [ForeignKey("ViewPersonel")]
        public string OlaySahibiKod { get; set; }



        [NotMapped]
        public DateTime SorguTarihi1 { get; set; }
        [NotMapped]
        public DateTime SorguTarihi2 { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
        //public ViewPersonel KayitEdenPersonel { get; set; }
        //public ViewPersonel OnaylayanPersonel { get; set; }

        public ViewPersonel OlaySahibi { get; set; }

        public List<OlayRaporlamaRisk> Riskler { get; set; }

        public List<OlayRaporlamaKategori> OlayKategoriler { get; set; }

        public virtual string OlayKategoriAdlari
        {
            get
            {
                string adlar = "";
                if (OlayKategoriler == null) return "";

                foreach (OlayRaporlamaKategori item in OlayKategoriler)
                {
                    if (adlar != "") adlar += ", ";
                    adlar += item?.OlayKategori?.Adi;
                }
                return adlar;

            }
        }
    }

}
