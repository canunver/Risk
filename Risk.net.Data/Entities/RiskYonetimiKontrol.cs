using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RiskYonetimiKontrol tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RiskYonetimiKontrol : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; }

        [ForeignKey("RiskYonetimi")]
        public string RiskYonetimiKod { get; set; }

        [Column(TypeName = "varchar(200)")]
        public string Tanim { get; set; }

        [Column(TypeName = "int")]
        public int Etkinlik { get; set; }

        [Column(TypeName = "money")]
        public double OnemDuzeyi { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public virtual double KontrolKriteriAgirligi
        {
            get
            {
                double agirlik = 0;

                if (Etkinlik == 5)
                    agirlik = 0.2 * OnemDuzeyi / 100;
                else if (Etkinlik == 4)
                    agirlik = 0.4 * OnemDuzeyi / 100;
                else if (Etkinlik == 3)
                    agirlik = 0.6 * OnemDuzeyi / 100;
                else if (Etkinlik == 2)
                    agirlik = 0.8 * OnemDuzeyi / 100;
                else if (Etkinlik == 1)
                    agirlik = 1 * OnemDuzeyi / 100;

                return Math.Round(agirlik, 2);
            }
        }

    }
}
