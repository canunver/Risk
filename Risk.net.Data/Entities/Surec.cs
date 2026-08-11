using Risk.net.Data.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki Surec tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class Surec : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("ViewKoordinatorluk")]
        [Column(TypeName = "varchar(40)")]
        public string KoordinatorlukKod { get; set; } = "";

        [ForeignKey("ViewBirim")]
        [Column(TypeName = "varchar(40)")]
        public string BirimKod { get; set; } = "";

        [ForeignKey("ViewUnvan")]
        [Column(TypeName = "varchar(40)")]
        public string SurecSahibiUnvanKod { get; set; } = "";

        [ForeignKey("ViewPersonel")]
        [Column(TypeName = "varchar(40)")]
        public string SurecSahibiPersonelKod { get; set; } = "";

        [Column(TypeName = "varchar(300)")]
        public string Adi { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string Numara { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
        public ViewUnvan SurecSahibiUnvan { get; set; }
        public ViewPersonel SurecSahibiPersonel { get; set; }
        public List<AltSurec> AltSurecler { get; set; }

    }
}
