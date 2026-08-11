using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RaporRiskSahipleriRisk view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RaporRiskSahipleriRisk : RaporEntityBase, IEntity
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
        public string RiskSahibi { get; set; }
        public int TehditFirsat { get; set; }
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
        public int MevcutDurum { get; set; }
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

        public virtual string StratejikPlanDonemAdi
        {
            get
            {
                if (BaslamaYil > 0)
                    return BaslamaYil + " - " + BitisYil;

                return "";
            }
        }
    }
}
