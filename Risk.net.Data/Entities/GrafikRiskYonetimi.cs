using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki GrafikRiskYonetimi view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class GrafikRiskYonetimi : IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string RiskNo { get; set; }
        public string RiskAdi { get; set; }

        [NotMapped]
        public string KoordinatorlukKod { get; set; }
        [NotMapped]
        public string BirimKod { get; set; }
        [NotMapped]
        public int YapisalRiskSeviyesi { get; set; }
        [NotMapped]
        public int ArtikRiskSeviyesi { get; set; }

        [NotMapped]
        public string Kategori { get; set; }
        [NotMapped]
        public string Durum { get; set; }

        [NotMapped]
        public DateTime? sorguTarihi1 { get; set; }
        [NotMapped]
        public DateTime? sorguTarihi2 { get; set; }
    }
}
