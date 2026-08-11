using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki PersonelAktifRol tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class PersonelAktifRol : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")] 
        public string PersonelKod { get; set; } = "";

        [Column(TypeName = "varchar(150)")]
        public string Rol { get; set; }
        [Column(TypeName = "varchar(40)")]
        public string KoordinatorlukKod { get; set; }
        [Column(TypeName = "varchar(40)")] 
        public string BirimKod { get; set; }

    }
}
