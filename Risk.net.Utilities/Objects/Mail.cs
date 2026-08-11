using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk.net.Utilities.Objects
{
    public class EPostaEki
    {
        public byte[] ek { get; set; }
        public string ad { get; set; }
        public string dosyaAd { get; set; }

        public EPostaEki(byte[] pEk, string pAd)
        {
            ek = pEk;
            ad = pAd;
        }
    }
}
