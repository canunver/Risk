using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RiskAzaltmaPlaniIliski tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RiskAzaltmaPlaniIliski : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("RiskAzaltmaPlani")]
        public string RiskAzaltmaPlaniKod { get; set; }

        public string IliskiKod { get; set; } = "";
    }
}
