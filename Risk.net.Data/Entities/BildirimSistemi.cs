using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki BildirimSistemi tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class BildirimSistemi : EntityBase, IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Kod { get; set; }

        [Column(TypeName = "int")]
        public int BelgeTipi { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string BelgeKod { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; }

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; }

        [ForeignKey("ViewPersonel")]
        public string IslemYapanKod { get; set; }

        [Column(TypeName = "dateTime")]
        public DateTime? IslemTarihi { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string OnaylayacakYetki { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [Column(TypeName = "varchar(4000)")]
        public string Aciklama { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }

        public ViewPersonel IslemYapan { get; set; }

        [NotMapped]
        public string OnaylayacakUstYetki { get; set; }

        [NotMapped]
        public List<string> Birimler { get; set; }

        [NotMapped]
        public EnumBildirimSistemiIslem? Islem { get; set; }

        [NotMapped]
        public List<BildirimSistemi> Liste { get; set; }

        [NotMapped]
        public string MailGonderilecekKisi { get; set; }
    }
}
