using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki DenetimBirim tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class DenetimBirim : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("Denetim")]
        public string DenetimKod { get; set; } = "";

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; } = "";

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; } = "";

        [ForeignKey("Surec")]
        public string SurecKod { get; set; } = "";

        [ForeignKey("AltSurec")]
        public string AltSurecKod { get; set; } = "";

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
        public Surec Surec { get; set; }
        public AltSurec AltSurec { get; set; }
    }
}
