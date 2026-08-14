using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RaporTrend view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RaporTrend : RaporEntityBase, IEntity
    {
        public int? Yil { get; set; }
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string RiskNo { get; set; }
        public string RiskKategorisiAdi { get; set; }
        public string RiskAdi { get; set; }
        public string RiskTanimi { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? ArtikRiskPuaniOncekiDeger { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? ArtikRiskPuaniGuncelDeger { get; set; }
        public int? ArtikRiskSeviyesiOncekiDeger { get; set; }
        public int? ArtikRiskSeviyesiGuncelDeger { get; set; }
        public DateTime? ArtikRiskSeviyesiGirisTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public string AnahtarRiskGostergesiAdi { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public double? KirmiziDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double? YesilDeger { get; set; }

        public string AnahtarRiskGostergesiDonem { get; set; }

        public virtual string TrendPuan
        {
            get
            {
                if (GuncellemeTarihi.HasValue)
                {
                    var SonDeger2 = ArtikRiskPuaniGuncelDeger;
                    var SonDeger1 = ArtikRiskPuaniOncekiDeger;

                    if (SonDeger2 > SonDeger1) return "ARTAN";
                    if (SonDeger2 < SonDeger1) return "AZALAN";
                    else return "SABİT";
                }

                return "";
            }
        }

        public virtual string TrendSeviye
        {
            get
            {
                if (GuncellemeTarihi.HasValue)
                {
                    var SonDeger2 = ArtikRiskSeviyesiGuncelDeger;
                    var SonDeger1 = ArtikRiskSeviyesiOncekiDeger;

                    if (SonDeger2 > SonDeger1) return "ARTAN";
                    if (SonDeger2 < SonDeger1) return "AZALAN";
                    else return "SABİT";
                }

                return "";
            }
        }

        public virtual string AnahtarRiskGostergesiDurum
        {
            get
            {
                double sonDonemDeger = 0;

                if (!string.IsNullOrWhiteSpace(AnahtarRiskGostergesiDonem))
                {
                    string[] d = AnahtarRiskGostergesiDonem.Replace(".", ",").Split(';');
                    sonDonemDeger = Convert.ToDouble(d[d.Length - 1]);
                }

                if (sonDonemDeger > 0)
                {
                    //YD:belirsiz, YDSON:40
                    //KD:75,       KDSSON:belirsiz
                    //SonDonemDeger:60 ise Sarı
                    //SonDonemDeger:15 ise Yeşil (40 isede)
                    //SonDonemDeger:85 ise Kırmızı (75 isede)

                    if (sonDonemDeger >= KirmiziDeger) return "KIRMIZI";
                    else if (sonDonemDeger <= YesilDeger) return "YEŞİL";
                    else return "SARI";
                }

                return "";
            }
        }

        public virtual string AnahtarRiskGostergesiTrend
        {
            get
            {
                double sonDeger2 = 0.0;
                double sonDeger1 = 0.0;

                if (!string.IsNullOrWhiteSpace(AnahtarRiskGostergesiDonem))
                {
                    string[] d = AnahtarRiskGostergesiDonem.Replace(".", ",").Split(';');

                    if (d.Length > 1)
                    {
                        sonDeger2 = Convert.ToDouble(d[d.Length - 1]);
                        sonDeger1 = Convert.ToDouble(d[d.Length - 2]);

                        if (sonDeger1 + sonDeger2 > 0)
                        {
                            if (sonDeger2 > sonDeger1) return "ARTAN";
                            if (sonDeger2 < sonDeger1) return "AZALAN";
                            else return "SABİT";
                        }
                    }
                }

                return "";
            }
        }
    }
}
