using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RiskEvreni tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RiskEvreni : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; }

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; }

        [ForeignKey("StratejikPlan")]
        public string AmacKod { get; set; }

        [ForeignKey("StratejikPlanHedef")]
        public string HedefKod { get; set; }

        [ForeignKey("Surec")]
        public string SurecKod { get; set; }

        [ForeignKey("AltSurec")]
        public string AltSurecKod { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string RiskNo { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string AnahtarRiskGostergesiAdi { get; set; }

        [ForeignKey("TanimGenel")]
        public string IlIrtibatOfisiKod { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string RiskAdi { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string RiskTanimi { get; set; }

        [ForeignKey("ViewPersonel")]
        public string RiskSahibiKod { get; set; }

        [Column(TypeName = "int")]
        public EnumRiskEvreniRiskTuru RiskTuru { get; set; }

        [Column(TypeName = "date")]
        public DateTime? KayitTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RiskBaslangicTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RiskBitisTarihi { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }
        public TanimGenel IlIrtibatOfisi { get; set; }

        [Column(TypeName = "varchar(4000)")]
        public string Aciklama { get; set; }

        [Column(TypeName = "int")]
        public int JenerikRisk { get; set; }
        [Column(TypeName = "int")]
        public int KontrolEdildi { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
        public StratejikPlan Amac { get; set; }
        public StratejikPlanHedef Hedef { get; set; }
        public Surec Surec { get; set; }
        public AltSurec AltSurec { get; set; }
        public ViewPersonel RiskSahibi { get; set; }

        public AnahtarRiskGostergesi AnahtarRiskGostergesi { get; set; }

        public RiskYonetimi RiskYonetimi { get; set; }

        public List<RiskEvreniRiskKategori> RiskKategoriler { get; set; }
        //public List<RiskEvreniOlayKategori> OlayKategoriler { get; set; }
        public List<RiskEvreniOlayRaporlama> OlayRaporlar { get; set; }

        public List<RiskEvreniIlIrtibatOfisi> IlIrtibatOfisler { get; set; }

        public virtual string RiskKategoriAdlari
        {
            get
            {
                string adlar = "";
                if (RiskKategoriler == null) return "";

                foreach (RiskEvreniRiskKategori item in RiskKategoriler)
                {
                    if (adlar != "") adlar += ", ";
                    adlar += item?.RiskKategori?.Adi;
                }
                return adlar;

            }
        }

        public virtual string IlIrtibatOfisleriAdlari
        {
            get
            {
                string adlar = "";
                if (IlIrtibatOfisler == null) return "";

                foreach (RiskEvreniIlIrtibatOfisi item in IlIrtibatOfisler)
                {
                    if (adlar != "") adlar += ", ";
                    adlar += item?.IlIrtibatOfisi?.Adi;
                }
                return adlar;

            }
        }

        [NotMapped]
        public virtual string SonAciklama { get; set; }

        [NotMapped]
        public virtual string RiskGecerlilikTarihi
        {
            get
            {
                string tarih;
                if (RiskBitisTarihi.HasValue && RiskBitisTarihi.Value.Year == 9999)
                    tarih = "Belirsiz";
                else if (RiskBaslangicTarihi.HasValue)
                {
                    tarih = RiskBaslangicTarihi.Value.ToString("dd.MM.yyyy");
                    if (RiskBitisTarihi.HasValue && RiskBitisTarihi.Value.Year != 9999)
                        tarih += "-" + RiskBitisTarihi.Value.ToString("dd.MM.yyyy");
                    else
                        tarih += "-";
                }
                else if (RiskBitisTarihi.HasValue && RiskBitisTarihi.Value.Year != 9999)
                    tarih = "-" + RiskBaslangicTarihi.Value.ToString("dd.MM.yyyy");
                else
                    tarih = "Belirsiz";

                return tarih;
            }
        }

        [NotMapped]
        public DateTime? KayitTarihi1 { get; set; }
        [NotMapped]
        public DateTime? KayitTarihi2 { get; set; }
        [NotMapped]
        public string SorguRiskKategoriKod { get; set; }
        [NotMapped]
        public string SorguIlIrtibatOfisiKod { get; set; }
        [NotMapped]
        public string SorguRiskSahibiKod { get; set; }
        [NotMapped]
        public string SorguAzaltmaPlaniSorumlusuKod { get; set; }
        [NotMapped]
        public int SorguArtikRiskSeviyesi { get; set; }
        [NotMapped]
        public string SorguAzaltmaPlaniNo { get; set; }
        [NotMapped]
        public DateTime? BitisTarihi1 { get; set; }
        [NotMapped]
        public DateTime? BitisTarihi2 { get; set; }

        [NotMapped]
        public DateTime? SorguGecerlilikTarihi1 { get; set; }
        [NotMapped]
        public DateTime? SorguGecerlilikTarihi2 { get; set; }

        [NotMapped]
        public string SorguAnahtarRiskGostergesi { get; set; }

        [NotMapped]
        public string SorguIsbirligiKoordinatorlukKod { get; set; }

        [NotMapped]
        public string SorguIsbirligiBirimKod { get; set; }
        [NotMapped]
        public int SorguBaskasiAdinaRiskDurumu { get; set; }

        [NotMapped]
        public string KopyaKod { get; set; }

        public virtual int KayitTarihiGecenGun
        {
            get
            {
                return KayitTarihi.HasValue ? (DateTime.Now - KayitTarihi.Value).Days : 0;
            }
        }

        public virtual string AnahtarRiskGostergesiAdiSon
        {
            get
            {
                return AnahtarRiskGostergesi != null && !string.IsNullOrWhiteSpace(AnahtarRiskGostergesi.Adi) ? AnahtarRiskGostergesi.Adi : AnahtarRiskGostergesiAdi;
            }
        }

        public virtual ViewKoordinatorluk RiskAzaltmaPlaniSorumluKoordinatorluk
        {
            get
            {
                //return !string.IsNullOrWhiteSpace(RiskYonetimi?.RiskAzaltmaPlani?.SorumluBirim?.Koordinatorluk?.Kod) ? RiskYonetimi.RiskAzaltmaPlani.SorumluBirim?.Koordinatorluk : Koordinatorluk;
                //Listeleme (TabloDoldur kafa karıştırıyor. Orada sorumlu koordinatörlük gösteriyor (Azaltma varsa). Eğer null ise bu durumda koordinatörlük gösteriliyorki bu hatalı Melih 25.09.2023 (Risklerin Yönetilmesi - Listeleme ekranında Sorumlu Koordinatörlük ve Birim sütunlarına da sıralama (aşağı yukarı ok) koyabilir miyiz? Clickup maddesi)
                return !string.IsNullOrWhiteSpace(RiskYonetimi?.RiskAzaltmaPlani?.SorumluBirim?.Koordinatorluk?.Kod) ? RiskYonetimi.RiskAzaltmaPlani.SorumluBirim?.Koordinatorluk : new ViewKoordinatorluk();
            }
        }

        public virtual ViewBirim RiskAzaltmaPlaniSorumluBirim
        {
            get
            {
                //return !string.IsNullOrWhiteSpace(RiskYonetimi?.RiskAzaltmaPlani?.SorumluBirim?.Kod) ? RiskYonetimi.RiskAzaltmaPlani.SorumluBirim : Birim;

                //Listeleme (TabloDoldur kafa karıştırıyor. Orada sorumlu birim gösteriyor (Azaltma varsa). Eğer null ise bu durumda birim gösteriliyorki bu hatalı Melih 25.09.2023 (Risklerin Yönetilmesi - Listeleme ekranında Sorumlu Koordinatörlük ve Birim sütunlarına da sıralama (aşağı yukarı ok) koyabilir miyiz? Clickup maddesi)
                return !string.IsNullOrWhiteSpace(RiskYonetimi?.RiskAzaltmaPlani?.SorumluBirim?.Kod) ? RiskYonetimi.RiskAzaltmaPlani.SorumluBirim : new ViewBirim();

            }
        }

        public virtual string OlayRaporlarAdlari
        {
            get
            {
                string adlar = "";
                if (OlayRaporlar == null) return "";

                foreach (RiskEvreniOlayRaporlama item in OlayRaporlar)
                {
                    if (adlar != "") adlar += ", ";
                    adlar += item?.OlayRaporlama?.OlayTanimi;
                }
                return adlar;

            }
        }
    }
}
