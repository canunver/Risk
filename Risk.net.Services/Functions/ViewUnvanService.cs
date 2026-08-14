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
    /// ViewUnvan işlemlerinin yapıldığı servis
    /// </summary>
    public class ViewUnvanService : IViewUnvanService
    {
        /// <summary>
        /// IUnitOfWork<ViewUnvan> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewUnvan> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.ViewUnvanService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public ViewUnvanService(IUnitOfWork<ViewUnvan> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
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
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(null, o => o.Adi);
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                ViewUnvan aramaObj = Arac.DataTablesAramaNesne<ViewUnvan>(new ViewUnvan(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(aramaObj.Adi));
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue));
            }


            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

    }
}
