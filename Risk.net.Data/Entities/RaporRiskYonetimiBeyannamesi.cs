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
    /// Veritabanýndaki RaporRiskYonetimiBeyannamesi view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RaporRiskYonetimiBeyannamesi : RaporEntityBase, IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public int Yil { get; set; }
        public DateTime? IslemTarihi { get; set; }
        public string IslemYapanAdi { get; set; }
        public string IslemYapanRol { get; set; }
    }


}
