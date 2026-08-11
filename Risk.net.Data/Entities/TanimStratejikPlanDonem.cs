using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki StratejikPlanDonem tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class TanimStratejikPlanDonem : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [Column(TypeName = "int")]
        public int BaslamaYil { get; set; }

        [Column(TypeName = "int")]
        public int BitisYil { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }
    }
}
