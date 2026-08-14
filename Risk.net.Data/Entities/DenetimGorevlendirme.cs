using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki Denetim Görevlendirme tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class DenetimGorevlendirme : EntityBase, IEntity
    {
        public DenetimGorevlendirme()
        {
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string DenetimKod { get; set; }

        [Column(TypeName = "int")]
        public int Tip { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string GorevYaziSayisi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? GorevYaziTarihi { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string RaporSayisi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RaporTarihi { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string RaporOnaySayisi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RaporOnayTarihi { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string GondermeSayisi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? GondermeTarihi { get; set; }
    }

}
