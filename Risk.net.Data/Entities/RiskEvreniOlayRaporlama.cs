using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RiskEvreniOlayRaporlama tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RiskEvreniOlayRaporlama : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("RiskEvreni")]
        public string RiskEvreniKod { get; set; } = "";

        [ForeignKey("OlayRaporlama")]
        public string OlayRaporlamaKod { get; set; } = "";

        public OlayRaporlama OlayRaporlama { get; set; }

    }
}
