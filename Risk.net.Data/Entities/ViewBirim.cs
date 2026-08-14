using Risk.net.Data.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki ViewBirim view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class ViewBirim :  IEntity
    {
        [Key]
        public string Kod { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; }

        public string Adi { get; set; }

        public int Durum { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }

    }
}
