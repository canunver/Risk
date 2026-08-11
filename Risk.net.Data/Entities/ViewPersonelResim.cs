using Risk.net.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki ViewPersonelResim view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
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
