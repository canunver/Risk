using Risk.net.Data.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki ViewKoordinatorluk view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class ViewKoordinatorluk : IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string Adi { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string BagliKod { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [Column(TypeName = "int")]
        public int Tur { get; set; }

        /*  Tur : EnumKoordinatorlukTur

            10:Baþkan
            20:Genel Koordinator, 21:Ýç Kontrol, 22:Hukuk
            30:Merkez
            40:Ýl Koordinator
            
            Üst Yetki
            10:10
            20:10
            30:20
            40:10
        */

    }
}

