using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RiskEvreniIlIrtibatOfisi tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RiskEvreniIlIrtibatOfisi : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("RiskEvreni")]
        public string RiskEvreniKod { get; set; } = "";

        [ForeignKey("TanimIlIrtibatOfisi")]
        public string IlIrtibatOfisiKod { get; set; } = "";

        public TanimIlIrtibatOfisi IlIrtibatOfisi { get; set; }
    }
}
