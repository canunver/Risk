using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki StratejikPlanIsbirligiBirim tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class StratejikPlanIsbirligiBirim : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("StratejikPlanHedef")]
        public string StratejikPlanHedefKod { get; set; } = "";

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; } = "";

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; } = "";

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
    }
}
