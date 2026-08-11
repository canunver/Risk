
using System.Collections.Generic;

namespace Risk.net.Utilities.Objects
{
    public class StratejikPlanDto
    {
        public string Kod { get; set; } = "";
        public string KoordinatorlukKod { get; set; } = "";
        public string BirimKod { get; set; } = "";
        public string Amac { get; set; }
        public int Durum { get; set; }

        public List<StratejikPlanHedefDto> Hedefler { get; set; }
    }
}
