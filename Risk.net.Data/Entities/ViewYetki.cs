using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki ViewYetki view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class ViewYetki : IEntity
    {
        [Key]
        public string Kod { get; set; }
        public string KullaniciKod { get; set; }
        public string Rol { get; set; }

        [ForeignKey("ViewPersonel")]
        public string PersonelKod { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; }

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
        public ViewPersonel Personel { get; set; }

    }
}
