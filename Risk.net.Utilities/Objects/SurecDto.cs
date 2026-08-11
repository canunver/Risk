
using System.Collections.Generic;

namespace Risk.net.Utilities.Objects
{
    public class SurecDto
    {
        public string Kod { get; set; } = "";
        public string KoordinatorlukKod { get; set; } = "";
        public string BirimKod { get; set; } = "";
        public string Adi { get; set; }
        public int Durum { get; set; }

        public List<AltSurecDto> AltSurecler { get; set; }
    }
}
