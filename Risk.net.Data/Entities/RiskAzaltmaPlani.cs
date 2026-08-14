using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RiskAzaltmaPlani tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RiskAzaltmaPlani : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("RiskYonetimi")]
        public string RiskYonetimiKod { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string AzaltmaPlaniNo { get; set; }

        [Column(TypeName = "int")]
        public EnumRiskAzaltmaPlaniMevcutDurum MevcutDurum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BaslangicTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BitisTarihi { get; set; }

        [Column(TypeName = "int")]
        public int TamamlamaYuzdesi { get; set; }

        [Column(TypeName = "varchar(250)")]
        public string AzaltmaPlani { get; set; }

        [ForeignKey("ViewPersonel")]
        public string AzaltmaPlaniSorumlusuKod { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string AzaltmaPlaniNotlar { get; set; }

        [Column(TypeName = "int")]
        public int? Durum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? KayitTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ErtelemeBaslangicTarihi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ErtelemeBitisTarihi { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string ErtelemeNot { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        public string SorumluKoordinatorlukKod { get; set; }

        [ForeignKey("ViewBirim")]
        public string SorumluBirimKod { get; set; }
        [Column(TypeName = "int")]
        public int KontrolEdildi { get; set; }

        public List<RiskAzaltmaPlaniIliski> IliskiliPlanlar { get; set; }
        public List<RiskAzaltmaPlaniIsbirligiBirim> IsbirligiBirimler { get; set; }
        public List<RiskAzaltmaPlaniRisk> Riskler { get; set; }

        public List<RiskAzaltmaPlaniNot> Notlar { get; set; }


        public ViewBirim SorumluBirim { get; set; }
        public ViewPersonel AzaltmaPlaniSorumlusu { get; set; }


        public virtual int KayitTarihiGecenGun
        {
            get
            {
                return KayitTarihi.HasValue ? (DateTime.Now - KayitTarihi.Value).Days : 0;
            }
        }

        public virtual DateTime? BitisTarihi2
        {
            get
            {
                return ErtelemeBitisTarihi != null && BitisTarihi != null && ErtelemeBitisTarihi.Value != BitisTarihi.Value ? ErtelemeBitisTarihi : BitisTarihi;
            }
        }

    }
}
