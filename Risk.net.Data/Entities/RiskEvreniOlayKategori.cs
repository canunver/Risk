using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RiskEvreniOlayKategori tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RiskEvreniOlayKategori : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("RiskEvreni")]
        [Column(TypeName = "varchar(40)")]
        public string RiskEvreniKod { get; set; } = "";

        [ForeignKey("TanimOlayKategori")]
        [Column(TypeName = "varchar(40)")] 
        public string OlayKategoriKod { get; set; } = "";

        public TanimOlayKategori OlayKategori { get; set; }
    }
}
