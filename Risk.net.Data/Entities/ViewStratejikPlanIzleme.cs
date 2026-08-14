using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki ViewStratejikPlanIzleme tablosu ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class ViewStratejikPlanIzleme : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("StratejikPlan")]
        public string AmacKod { get; set; }

        [ForeignKey("StratejikPlanHedef")]
        public string HedefKod { get; set; }

        [Column(TypeName = "int")]

        public int Durum { get; set; }
        [Column(TypeName = "date")]

        public DateTime? KayitTarihi { get; set; }

        public StratejikPlan StratejikPlan { get; set; }

        public StratejikPlanHedef Hedef { get; set; }

        [NotMapped]
        public string SorguKoordinatorlukKod { get; set; }

        [NotMapped]
        public string SorguStratejikPlanDonemKod { get; set; }

        public virtual int Yil
        {
            get
            {
                if (KayitTarihi != null)
                    return KayitTarihi.Value.Year;

                return 0;
            }
        }
    }

}
