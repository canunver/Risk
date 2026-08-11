
using System.Collections.Generic;

namespace Risk.net.Utilities.Objects
{
    public class TanimKoordinatorlukDto
    {
        public string Kod { get; set; }

        public string Adi { get; set; }

        public int Durum { get; set; }

        public List<TanimBirimDto> Birimler { get; set; }
    }
}
