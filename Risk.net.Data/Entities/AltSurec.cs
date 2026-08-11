using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki AltSurec tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class AltSurec : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("Surec")]
        [Column(TypeName = "varchar(40)")]
        public string SurecKod { get; set; } = "";

        [Column(TypeName = "varchar(250)")]
        public string Adi { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Numara { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }
    }
}
