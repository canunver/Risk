using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki GrafikIzlemeMatrisi view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class GrafikIzlemeMatrisi : IEntity
    {
        public string RiskNo { get; set; }
        public int Etki { get; set; }
        public int Olasilik { get; set; }
        public int YapisalRiskSeviyesi { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal KontrolKriteriAgirligi { get; set; }

    }
}
