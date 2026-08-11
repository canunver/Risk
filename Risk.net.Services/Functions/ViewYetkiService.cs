using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// ViewYetki iþlemlerinin yapýldýðý servis
    /// </summary>
    public class ViewYetkiService : IViewYetkiService
    {
        /// <summary>
        /// IUnitOfWork<ViewYetki> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewYetki> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.ViewYetkiService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public ViewYetkiService(IUnitOfWork<ViewYetki> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.KullaniciKod == kod, "Personel");

                if (kayit != null)
                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
            }
            catch (System.Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }

            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, ViewYetki kriter)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Personel", a => a.Birim, a => a.Koordinatorluk, a => a.Personel);

            if (!string.IsNullOrWhiteSpace(kriter.KullaniciKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KullaniciKod == kriter.KullaniciKod);
            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kriter.KoordinatorlukKod);
            if (!string.IsNullOrWhiteSpace(kriter.BirimKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kriter.BirimKod);
            if (!string.IsNullOrWhiteSpace(kriter.PersonelKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.PersonelKod == kriter.PersonelKod);
            if (!string.IsNullOrWhiteSpace(kriter.Rol))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Rol == kriter.Rol);

            var kayitlar = selectData.OrderBy(a => a.Rol).ThenBy(a => a.KoordinatorlukKod).ToList();

            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "", a => a.Birim, a => a.Koordinatorluk, a => a.Personel);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                ViewYetki aramaObj = Arac.DataTablesAramaNesne<ViewYetki>(new ViewYetki(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.KullaniciKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KullaniciKod == aramaObj.KullaniciKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.Rol))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Rol == aramaObj.Rol);
                if (!string.IsNullOrWhiteSpace(aramaObj.PersonelKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.PersonelKod == aramaObj.PersonelKod);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KullaniciKod == dataTablesParam.searchValue
                                                                         || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                         || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                         || a.Rol.Contains(dataTablesParam.searchValue));
            }

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait mail listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleMailAsync(KullaniciDto kullanan, ViewYetki kriter)
        {
            if (string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                kriter.KoordinatorlukKod = Arac.ConfigOku("Genel:ListeleMailKoordinatorlukKod");

            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Personel", a => a.Personel);

            if (!string.IsNullOrWhiteSpace(kriter.Rol))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Rol == kriter.Rol);
            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
            {
                if (kriter.KoordinatorlukKod.Contains(";"))
                {
                    var koordinatorlukler = kriter.KoordinatorlukKod.Split(';');
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => koordinatorlukler.Contains(a.KoordinatorlukKod));
                }
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kriter.KoordinatorlukKod);

            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimKod))
            {
                if (kriter.BirimKod.Contains(";"))
                {
                    var birimler = kriter.BirimKod.Split(';');
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => birimler.Contains(a.BirimKod));
                }
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kriter.BirimKod);
            }


            var kayitlar = selectData.ToList();

            if (kayitlar.Count > -1)
            {
                string liste = "";
                foreach (var item in kayitlar)
                {
                    if (liste != "") liste += ";";
                    liste += item?.Personel?.EPosta;
                }

                return new Sonuc(ENUMIslemDurum.Basarili, "", liste);
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }
    }
}
