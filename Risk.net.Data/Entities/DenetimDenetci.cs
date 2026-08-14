using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki DenetimDenetci tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class DenetimDenetci : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("Denetim")]
        public string DenetimKod { get; set; } = "";

        [ForeignKey("ViewPersonel")]
        [Column(TypeName = "varchar(40)")]
        public string DenetciKod { get; set; } = "";

        public ViewPersonel Denetci { get; set; }
    }
}
