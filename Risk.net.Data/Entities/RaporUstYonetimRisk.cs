using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RaporUstYonetimRisk view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RaporUstYonetimRisk : RaporEntityBase, IEntity
    {
        public int? Yil { get; set; }
        public int? Donem { get; set; }
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string RiskNumarasi { get; set; }
        public string RiskKategorisiAdi { get; set; }
        public string IlIrtibatOfisiAdi { get; set; }

        public string RiskAdi { get; set; }
        public string RiskTanimi { get; set; }
        public int? ArtikRiskSeviyesi { get; set; }
        public string AzaltmaPlani { get; set; }
        public string AnahtarRiskGostergesiAdi { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal AnahtarRiskGostergesiDonem { get; set; }
        public List<RiskEvreniRiskKategori> RiskKategoriler { get; set; }

    }
}
