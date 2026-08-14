using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki Yardim tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class Yardim : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string SayfaAdi { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string Baslik { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string Icerik { get; set; }
    }
}
