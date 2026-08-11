using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RaporIcKontrolZayifliklari view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RaporIcKontrolZayifliklari : RaporEntityBase, IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string IlIrtibatOfisiAdi { get; set; }
        public string KontrolTanimi { get; set; }
        public string KontrolEtkinligi { get; set; }
        public string AzaltmaPlani { get; set; }
        public string AzaltmaPlaniSorumlusu { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int MevcutDurum { get; set; }
        public DateTime? ErtelemeBaslangicTarihi { get; set; }
        public DateTime? ErtelemeBitisTarihi { get; set; }
        public string ErtlenmisZamanPlaniNedeni { get; set; }
        public string AzaltmaPlaniNotlar { get; set; }

        public virtual string ErtlenmisZamanPlani
        {
            get
            {
                string deger = "";

                if (ErtelemeBaslangicTarihi.HasValue)
                    deger += ErtelemeBaslangicTarihi.Value.ToString("dd.MM.yyyy");

                deger += "-";

                if (ErtelemeBitisTarihi.HasValue)
                    deger += ErtelemeBitisTarihi.Value.ToString("dd.MM.yyyy");

                return deger;
            }
        }
    }
}
