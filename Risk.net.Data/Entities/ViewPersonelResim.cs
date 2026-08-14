using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki ViewPersonelResim view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class ViewPersonelResim : IEntity
    {
        [Key]
        public string Kod { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public byte[] Resim { get; set; }

        public virtual string AdiSoyadi
        {
            get { return Adi + " " + Soyadi; }
        }

    }
}
