using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki ViewBulguYonetimi view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class ViewBulguYonetimi : IEntity
    {
        public ViewBulguYonetimi()
        {
        }

        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("Denetim")]
        public string DenetimKod { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string BulguNo { get; set; }

        [Column(TypeName = "varchar(750)")]
        public string BulguTanimi { get; set; }

        [ForeignKey("TanimGenel")]
        public string OnemDuzeyiKod { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public TanimGenel OnemDuzeyi { get; set; }

    }

}
