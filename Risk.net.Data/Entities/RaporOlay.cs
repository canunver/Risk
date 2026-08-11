using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RaporOlay view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RaporOlay : RaporEntityBase, IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public DateTime? OlayTarihi { get; set; }
        public string OlayYeri { get; set; }
        public string OlayTanimi { get; set; }
        public string OlayKategorisi { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? Tutari { get; set; }
        public string Riskler { get; set; }

    }


}
