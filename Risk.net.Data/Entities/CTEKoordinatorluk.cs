using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki CTEKoordinatorluk view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class CTEKoordinatorluk : IEntity
    {
        public string Kod { get; set; }
        public string Adi { get; set; }
        public string BagliKod { get; set; }
    }
}
