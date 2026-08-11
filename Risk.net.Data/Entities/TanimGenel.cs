using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki TanimGenel tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class TanimGenel : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [Column(TypeName = "varchar(60)")]
        public string Tur { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string Adi { get; set; }

        [Column(TypeName = "int")]
        public int SiraNo { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }
    }
}
