using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// EntityBase den türüyen tüm Entity lere ortak alanların eklenmesini sağlayan base sınıftır
    /// </summary>
    public abstract class EntityBase
    {
        [NotMapped]
        public Tarihce Tarihce { get; set; }

        [NotMapped]
        public int KontrolDuzenlemeGosterme { get; set; }
        [NotMapped]
        public int KontrolOnayaGonderGosterme { get; set; }
        [NotMapped]
        public int KontrolTarihceGosterme { get; set; }
        [NotMapped]
        public int KontrolPasifYapGosterme { get; set; }
    }
}
