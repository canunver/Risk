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
    /// ViewBirim işlemlerinin yapıldığı servis
    /// </summary>
    public class ViewBirimService : IViewBirimService
    {
        /// <summary>
        /// IUnitOfWork<ViewBirim> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewBirim> _unitOfWork;
        /// <summary>
        /// IUnitOfWork<ViewKoordinatorluk> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewKoordinatorluk> _unitOfWorkKoordinatorluk;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.SurecService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unitOfWorkKoordinatorluk"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public ViewBirimService(IUnitOfWork<ViewBirim> unitOfWork, IUnitOfWork<ViewKoordinatorluk> unitOfWorkKoordinatorluk, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkKoordinatorluk = unitOfWorkKoordinatorluk;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "", b => b.Koordinatorluk);

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
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="koordinatorlukKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string koordinatorlukKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.KoordinatorlukKod == koordinatorlukKod, o => o.Adi);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "", b => b.Koordinatorluk);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                ViewBirim aramaObj = Arac.DataTablesAramaNesne<ViewBirim>(new ViewBirim(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(aramaObj.Adi));
                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue)
                                                                         || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue));
            }

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Koordinatorluk türüne göre yetki döndürüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="koordinatorlukKod"></param>
        /// <param name="koordinatorlukTur"></param>
        /// <param name="birimKontrolu"></param>
        /// <returns>String türünde değer döndürür</returns>
        public async Task<string> KoordinatorlukYetkiTipiBul(KullaniciDto kullanan, string koordinatorlukKod, int koordinatorlukTur, bool birimKontrolu)
        {
            //birimKontrolu : Birimi olmayan koordinatörlük ile işlem yapıldığında

            string yetki = "";

            var kayitlar = await _unitOfWork.ListeleAsync(k => k.KoordinatorlukKod == koordinatorlukKod, o => o.Adi);
            if (birimKontrolu || kayitlar.Count <= 0)
            {
                if (koordinatorlukTur == 0)
                {
                    var koordinatorluk = await _unitOfWorkKoordinatorluk.KayitGetirAsync(k => k.Kod == koordinatorlukKod);
                    if (koordinatorluk != null)
                        koordinatorlukTur = koordinatorluk.Tur;
                }

                yetki = Arac.KoordinatorlukYetkiVer((EnumKoordinatorlukTur)koordinatorlukTur);
            }

            return yetki;
        }
    }
}
