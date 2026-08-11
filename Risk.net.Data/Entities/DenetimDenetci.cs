using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki DenetimDenetci tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
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
