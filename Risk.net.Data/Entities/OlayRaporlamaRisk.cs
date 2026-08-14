using Risk.net.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki OlayRaporlamaRisk tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
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
