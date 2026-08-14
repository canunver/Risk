using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RiskYonetimi tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RiskYonetimi : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("RiskEvreni")]
        public string RiskEvreniKod { get; set; }

        [Column(TypeName = "int")]
        public int Olasilik { get; set; }

        [Column(TypeName = "int")]
        public int Etki { get; set; }

        [Column(TypeName = "int")]
        public EnumRiskYonetimiRiskeVerilecekCevap RiskeVerilecekCevap { get; set; }

        [Column(TypeName = "varchar(750)")]
        public string Aciklama { get; set; }

        [Column(TypeName = "int")]
        public int? Durum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? KayitTarihi { get; set; }

        public RiskAzaltmaPlani RiskAzaltmaPlani { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double KontrolEtkinlikAgirligi { get; set; }

        public int ArtikRiskSeviyesi { get; set; }

        public int ArtikRiskSeviyesiOncekiDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double ArtikRiskPuaniOncekiDeger { get; set; }

        [Column(TypeName = "date")]
        public DateTime? GirisTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? GuncellemeTarihi { get; set; }
        [Column(TypeName = "int")]
        public int KontrolEdildi { get; set; }

        public List<RiskYonetimiKontrol> Kontroller { get; set; }

        public virtual int YapisalRiskPuani
        {
            get
            {
                return Olasilik * Etki;
            }
        }

        public virtual double ArtikRiskPuani
        {
            get
            {
                return Math.Round(YapisalRiskPuani * KontrolEtkinlikAgirligi, 2);
            }
        }

        public virtual double KontrolEtkinlikAgirligiGetir
        {
            get
            {
                double agirlik = 0;
                if (Kontroller != null)
                {
                    foreach (var kontrol in Kontroller)
                        if (kontrol.Durum != (int)ENUMDurum.Pasif)
                            agirlik += kontrol.KontrolKriteriAgirligi;
                }
                if (agirlik == 0)
                    agirlik = 1;

                return Math.Round(agirlik, 2);
            }
        }

        public virtual int ArtikRiskSeviyesiGetir
        {
            get
            {
                if (20 <= ArtikRiskPuani && 25 >= ArtikRiskPuani)
                    return 5;
                else if (15 <= ArtikRiskPuani && 20 > ArtikRiskPuani)
                    return 4;
                else if (10 <= ArtikRiskPuani && 15 > ArtikRiskPuani)
                    return 3;
                else if (5 <= ArtikRiskPuani && 10 > ArtikRiskPuani)
                    return 2;
                else if (0.2 <= ArtikRiskPuani && 5 > ArtikRiskPuani)
                    return 1;

                return 0;
            }
        }

        public virtual object YapisalRiskPuaniRenk
        {
            get
            {
                return new { Etki, Olasilik };
            }
        }


        public virtual string ArtikRiskPuaniRenk
        {
            get
            {
                return ArtikRiskPuaniRenkVer(ArtikRiskPuani, (int)ArtikRiskSeviyesiGetir);
            }
        }

        public static string ArtikRiskPuaniRenkVer(double artikRiskPuani, int artikRiskSeviyesi, bool html = true)
        {

            string donenDeger = "";

            if (artikRiskSeviyesi == 1)
                donenDeger = "rgb(147, 209, 76)";
            else if (artikRiskSeviyesi == 2)
                donenDeger = "rgb(0, 178, 78)";
            else if (artikRiskSeviyesi == 3)
                donenDeger = "rgb(255, 192, 1)";
            else if (artikRiskSeviyesi == 4)
                donenDeger = "rgb(253, 0, 3)";
            else if (artikRiskSeviyesi == 5)
                donenDeger = "rgb(192, 0, 2)";


            if (!string.IsNullOrWhiteSpace(donenDeger))
            {
                if (html)
                    donenDeger = "<div class='text-center' style='color:#ffffff;line-height: 2em;background-color: " + donenDeger + " !important;'>" + artikRiskPuani.ToString().Replace(",", ".") + "</div>";
            }
            return donenDeger;
        }

        public virtual string TrendSeviye
        {
            get
            {
                if (GuncellemeTarihi.HasValue)
                {
                    var SonDeger2 = ArtikRiskSeviyesi;
                    var SonDeger1 = ArtikRiskSeviyesiOncekiDeger;

                    if (SonDeger2 > SonDeger1) return "ARTAN";
                    if (SonDeger2 < SonDeger1) return "AZALAN";
                    else return "SABİT";
                }

                return "";
            }
        }

        public virtual string TrendPuan
        {
            get
            {
                if (GuncellemeTarihi.HasValue)
                {
                    var SonDeger2 = ArtikRiskSeviyesi;
                    var SonDeger1 = ArtikRiskSeviyesiOncekiDeger;

                    if (SonDeger2 > SonDeger1) return "ARTAN";
                    if (SonDeger2 < SonDeger1) return "AZALAN";
                    else return "SABİT";
                }

                return "";
            }
        }

        public virtual string MevcutKontroller
        {
            get
            {
                string donenDeger = "";

                if (Kontroller != null && Kontroller.Count > 0)
                {
                    foreach (var kontrol in Kontroller)
                    {
                        if (donenDeger != "")
                            donenDeger += "<br>";
                        donenDeger += kontrol.Tanim + "/" + kontrol.Etkinlik + "/" + kontrol.OnemDuzeyi;
                    }
                }

                return donenDeger;
            }
        }


        [NotMapped]
        public string RiskSahibiKod { get; set; }

    }
}
