using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RaporYillikRiskPlani view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RaporYillikRiskPlani : RaporEntityBase, IEntity
    {
        public int? Yil { get; set; }
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string RiskKategorisiAdi { get; set; }
        public string IlIrtibatOfisiAdi { get; set; }
        public int? BaslamaYil { get; set; }
        public int? BitisYil { get; set; }
        public string Amac { get; set; }
        public string Hedef { get; set; }
        public string Surec { get; set; }
        public string AltSurec { get; set; }
        public string RiskNo { get; set; }
        public string RiskAdi { get; set; }
        public string RiskTanimi { get; set; }
        public int RiskTuru { get; set; }
        public string IliskiliOlay { get; set; }
        public DateTime? RiskBaslangicTarihi { get; set; }
        public DateTime? RiskBitisTarihi { get; set; }
        public string MevcutKontroller { get; set; }

        public string AnahtarRiskGostergesi { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? MevcutDeger { get; set; }
        public int Etki { get; set; }
        public int Olasilik { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal KontrolKriteriAgirligi { get; set; }
        public string EtkiOlasilikMatrisi { get; set; }
        public string RiskKategoriFinansal { get; set; }
        public int RiskeVerilecekCevap { get; set; }
        public string AzaltmaPlani { get; set; }
        public string AzaltmaPlaniSorumlusu { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? MevcutDurum { get; set; }
        public DateTime? ErtelemeBaslangicTarihi { get; set; }
        public DateTime? ErtelemeBitisTarihi { get; set; }
        public string ErtlenmisZamanPlaniNedeni { get; set; }
        public string AzaltmaPlaniNotlar { get; set; }

        public virtual string ErtlenmisZamanPlani
        {
            get
            {
                string deger = "";

                if (ErtelemeBaslangicTarihi.HasValue)
                    deger += ErtelemeBaslangicTarihi.Value.ToString("dd.MM.yyyy");

                deger += "-";

                if (ErtelemeBitisTarihi.HasValue)
                    deger += ErtelemeBitisTarihi.Value.ToString("dd.MM.yyyy");

                return deger;
            }
        }

        public virtual int YapisalRiskPuani
        {
            get
            {
                return Olasilik * Etki;
            }
        }

        [Column(TypeName = "decimal(13, 4)")]
        public virtual decimal ArtikRiskPuani
        {
            get
            {
                return Math.Round(YapisalRiskPuani * KontrolKriteriAgirligi, 2);
            }
        }

        public virtual string StratejikPlanDonemAdi
        {
            get
            {
                if (BaslamaYil > 0)
                    return BaslamaYil + " - " + BitisYil;

                return "";
            }
        }
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
        public virtual string RiskinNiteligi
        {
            get
            {
                if (RiskTuru == (int)EnumRiskEvreniRiskTuru.Tehdit)
                    return "Tehdit";
                else if (RiskTuru == (int)EnumRiskEvreniRiskTuru.Firsat)
                    return "Firsat";
                return "";
            }
        }

    }
}
