using Risk.net.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki OlayRaporlamaRisk tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class OlayRaporlamaRisk : EntityBase, IEntity
    {
        public OlayRaporlamaRisk()
        {
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [Column(TypeName = "varchar(40)")]
        public string OlayRaporlamaKod { get; set; } = "";

        [ForeignKey("RiskEvreni")]
        [Column(TypeName = "varchar(40)")]
        public string RiskEvreniKod { get; set; } = "";

        public RiskEvreni RiskEvreni { get; set; }
    }

}
