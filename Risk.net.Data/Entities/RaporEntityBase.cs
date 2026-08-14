using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// RaporEntityBase den türüyen tüm Entity lere ortak alanların eklenmesini sağlayan base sınıftır
    /// </summary>
    public abstract class RaporEntityBase
    {
        [NotMapped]
        public int KriterYil{ get; set; }

        [NotMapped]
        public string KriterKoordinatorlukKod { get; set; }
        
        [NotMapped]
        public string KriterBirimKod { get; set; }

        [NotMapped]
        public string lIlIrtibatOfisiKod { get; set; }
    }
}
