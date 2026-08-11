using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;
using System.IO;
using System;
using System.Web;
using MimeKit;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// OrtakService iþlemlerinin yapýldýðý servis
    /// </summary>
    public class OrtakService
    {

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait birim koþulunu döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="serviceCTE"></param>
        /// <param name="tur"></param>
        /// <param name="koordinatorlukKod"></param>
        /// <param name="birimKod"></param>
        /// <returns>
        /// string türünde koþul bilgisi döndürür
        /// </returns>
        public static async Task<string[]> ListeleBirimKosulAsync(KullaniciDto kullanan, ICTEKoordinatorlukService serviceCTE, string tur, string koordinatorlukKod, string birimKod)
        {
            string[] kosul = { "", "" };//[0] koordinatörlük, [1] birim

            if (Arac.YetkisiVarmi("RISKSEKRETARYASI,PLANLAMAUNITESI,YETKILIRISKGOREVLISI,BASKAN,SISTEMYONETICISI", kullanan))
            {
                if (!string.IsNullOrWhiteSpace(koordinatorlukKod))
                {
                    kosul[0] = koordinatorlukKod;
                    if (!string.IsNullOrWhiteSpace(birimKod))
                        kosul[1] = birimKod;
                }
            }
            else if (Arac.YetkisiVarmi("GENELKOORDINATOR", kullanan))
            {

                List<string> gorebilecekleri = new List<string>();
                gorebilecekleri.Add(kullanan.KoordinatorlukKod);//Yetkisindeki koordinatorluk

                CTEKoordinatorluk qKriter = new CTEKoordinatorluk();
                qKriter.BagliKod = kullanan.KoordinatorlukKod;
                Sonuc sonucListe = await serviceCTE.ListeleAsync(kullanan, qKriter);
                foreach (CTEKoordinatorluk item in sonucListe.Liste)
                {
                    //Kriter olarak verilen koordinatörlük görmeye yetkili deðilse koþula ekleme
                    if (!string.IsNullOrWhiteSpace(koordinatorlukKod) && item.Kod != koordinatorlukKod) continue;

                    gorebilecekleri.Add(item.Kod);
                }

                foreach (var item in gorebilecekleri)
                {
                    if (kosul[0] != "") kosul[0] += ",";
                    kosul[0] += item;
                }

                if (kosul[0] != "" && koordinatorlukKod != "")
                {
                    if (!string.IsNullOrWhiteSpace(birimKod))
                        kosul[1] = birimKod;
                }
            }
            else if (Arac.YetkisiVarmi("ILKOORDINATOR,MERKEZKOORDINATOR,ICDENETIMKOORDINATOR,BIRIMAMIRI", kullanan))
            {
                kosul[0] = kullanan.KoordinatorlukKod;//kendi Koordinatörlüðü
                if (!string.IsNullOrWhiteSpace(birimKod))
                    kosul[1] = birimKod;
            }
            else if (Arac.YetkisiVarmi("ICDENETIMUZMANI,UZMAN", kullanan))
            {
                kosul[0] = kullanan.KoordinatorlukKod;//kendi Koordinatörlüðü
                kosul[1] = kullanan.BirimKod;//kendi Birimi
            }
            else
            {

            }


            return kosul;
        }

        public static string KosulEkle(string kosul, string baglac, string ekKosul)
        {
            if (string.IsNullOrEmpty(ekKosul)) return kosul;
            if (kosul != "") return kosul + " " + baglac + " " + ekKosul;
            else return ekKosul;
        }

        public static string KosulOl(string pAlanAd, string pKosulIslec, object pAlanDeger)
        {
            string donenDeger = "";

            return donenDeger + pAlanAd + pKosulIslec + DegerYap(pAlanDeger);
        }

        public static string DegerYap(object pAlanDeger)
        {
            string tut = Convert.ToString(pAlanDeger);

            if (pAlanDeger is double ||
                pAlanDeger is decimal)
                tut = tut.Replace(",", ".");

            if (pAlanDeger is DateTime)
                tut = $"CONVERT(DATETIME, '{Arac.DateTimeToYYYYMMDD(((DateTime?)pAlanDeger).Value)}', 102)";

            return tut;
        }
    }
}
