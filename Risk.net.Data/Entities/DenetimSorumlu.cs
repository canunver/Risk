using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki DenetimSorumlu tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class DenetimSorumlu : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("Denetim")]
        public string DenetimKod { get; set; } = "";

        [ForeignKey("ViewPersonel")]
        [Column(TypeName = "varchar(40)")]
        public string SorumluKod { get; set; } = "";

        public ViewPersonel Sorumlu { get; set; }
    }
}
