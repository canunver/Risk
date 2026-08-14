using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RiskAzaltmaPlaniNot tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RiskAzaltmaPlaniNot : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("RiskAzaltmaPlani")]
        public string RiskAzaltmaPlaniKod { get; set; }

        [Column(TypeName = "varchar(4000)")]
        public string Aciklama { get; set; }
        public DateTime? Tarih { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

    }
}
