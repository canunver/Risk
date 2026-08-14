using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki OlayRaporlamaKategori tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class OlayRaporlamaKategori : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("OlayRaporlama")]
        public string OlayRaporlamaKod { get; set; } = "";

        [ForeignKey("TanimOlayKategori")]
        public string OlayKategoriKod { get; set; } = "";

        public TanimOlayKategori OlayKategori { get; set; }
    }
}
