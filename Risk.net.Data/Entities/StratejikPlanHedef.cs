using Risk.net.Data.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki StratejikPlanHedef tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class StratejikPlanHedef : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("StratejikPlan")]
        public string StratejikPlanKod { get; set; } = "";

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; } = "";

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; } = "";

        [Column(TypeName = "varchar(150)")]
        public string Adi { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string HedefNo { get; set; } = "";

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public List<StratejikPlanHedefGosterge> Gostergeler { get; set; }

        [NotMapped]
        public List<StratejikPlanEylemPlani> Eylemler { get; set; }

        [NotMapped]
        public string SorguAmacNo { get; set; }

        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
        public List<StratejikPlanIsbirligiBirim> IsbirligiBirimler { get; set; }

        public virtual string IsbirligiBirimAdlari
        {
            get
            {
                string adlar = "";
                if (IsbirligiBirimler == null) return "";

                int sayac = 0;
                foreach (StratejikPlanIsbirligiBirim item in IsbirligiBirimler)
                {
                    if (item.Koordinatorluk?.Adi.IndexOf(" İl ") > -1)
                        sayac++;
                }

                if (sayac == 42)
                    adlar = "Bütün İl Koordinatörlükleri";

                foreach (StratejikPlanIsbirligiBirim item in IsbirligiBirimler)
                {
                    if (sayac == 42 && item.Koordinatorluk?.Adi.IndexOf(" İl ") > -1)
                        continue;

                    if (adlar != "") adlar += ", ";
                    adlar += item.Koordinatorluk?.Adi + "-" + item.Birim?.Adi;
                }


                return adlar;
            }
        }

    }
}
