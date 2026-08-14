using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RaporAnahtarRiskGostergesiDonem view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RaporAnahtarRiskGostergesiDonem : IEntity
    {
        public int Donem { get; set; }

        [Column(TypeName = "decimal(13, 4)")] 
        public decimal? Deger { get; set; }

        public int Periyot { get; set; }

    }
}
