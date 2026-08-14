using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RaporSurecAltSurec view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RaporSurecAltSurec : RaporEntityBase, IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string SurecKod { get; set; }
        public string SurecAdi { get; set; }
        public string AltSurecAdi { get; set; }
        public string SurecSahibi { get; set; }
        public string SurecSahibiUnvan { get; set; }

    }
}
