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
    /// ViewPersonel işlemlerinin yapıldığı servis
    /// </summary>
    public class ViewPersonelService : IViewPersonelService
    {
        /// <summary>
        /// IUnitOfWork<ViewPersonel> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewPersonel> _unitOfWork;
        /// <summary>
        /// IUnitOfWork<ViewPersonelResim> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewPersonelResim> _unitOfWorkResim;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.ViewPersonelService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unitOfWorkResim"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public ViewPersonelService(IUnitOfWork<ViewPersonel> unitOfWork, IUnitOfWork<ViewPersonelResim> unitOfWorkResim, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkResim = unitOfWorkResim;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod);

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
        /// <param name="birimKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string koordinatorlukKod, string birimKod)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "", a => a.Birim, a => a.Koordinatorluk);

            if (!string.IsNullOrWhiteSpace(koordinatorlukKod))
            {
                if (koordinatorlukKod.IndexOf(";") > -1)
                {
                    string[] koordinatorlukler = koordinatorlukKod.Split(";");

                    var predicate = PredicateBuilder.False<ViewPersonel>();
                    foreach (string item in koordinatorlukler)
                    {
                        if (string.IsNullOrWhiteSpace(item)) continue;
                        predicate = predicate.Or(a => a.KoordinatorlukKod == item);
                    }

                    selectData = await _unitOfWork.KosulEkleAsync(selectData, predicate);
                }
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == koordinatorlukKod);
            }
            if (!string.IsNullOrWhiteSpace(birimKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == birimKod);

            selectData = selectData.OrderBy(o => o.Adi).ThenBy(o => o.Soyadi);

            var kayitlar = selectData.ToList();

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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "", a => a.Birim, a => a.Koordinatorluk);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                ViewPersonel aramaObj = Arac.DataTablesAramaNesne<ViewPersonel>(new ViewPersonel(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(aramaObj.Adi));
                if (!string.IsNullOrWhiteSpace(aramaObj.Soyadi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Soyadi.Contains(aramaObj.Soyadi));
                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.UnvanKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.UnvanKod == aramaObj.UnvanKod);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue)
                                                                         || a.Soyadi.Contains(dataTablesParam.searchValue)
                                                                         || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                         || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                         || a.UnvanAdi.Contains(dataTablesParam.searchValue));
            }

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgiye ait resim bilgisini döndüren metod
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ResimGetirAsync(string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWorkResim.KayitGetirAsync(c => c.Kod == kod);

                if (kayit != null)
                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
            }
            catch (System.Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }

            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }
    }
}
