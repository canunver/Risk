using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// AnahtarRiskGostergesiDonem iþlemlerinin yapýldýðý servis
    /// </summary>
    public class AnahtarRiskGostergesiDonemService : IAnahtarRiskGostergesiDonemService
    {
        /// <summary>
        /// IUnitOfWork<AnahtarRiskGostergesiDonem> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        private readonly IUnitOfWork<AnahtarRiskGostergesiDonem> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.AnahtarRiskGostergesiDonemService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        public AnahtarRiskGostergesiDonemService(IUnitOfWork<AnahtarRiskGostergesiDonem> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "");

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
        /// <param name="anahtarRiskGostergesiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string anahtarRiskGostergesiKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.AnahtarRiskGostergesiKod == anahtarRiskGostergesiKod);
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                AnahtarRiskGostergesiDonem aramaObj = Arac.DataTablesAramaNesne<AnahtarRiskGostergesiDonem>(new AnahtarRiskGostergesiDonem(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.AnahtarRiskGostergesiKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesiKod.Contains(aramaObj.AnahtarRiskGostergesiKod));


            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesiKod.Contains(dataTablesParam.searchValue));
            }

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, AnahtarRiskGostergesiDonem gelenNesne)
        {
            AnahtarRiskGostergesiDonem islemYapilan = new AnahtarRiskGostergesiDonem();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.AnahtarRiskGostergesiKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AnahtarRiskGostergesiKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    eskiKayit.AnahtarRiskGostergesiKod = gelenNesne.AnahtarRiskGostergesiKod;
                    eskiKayit.Donem = gelenNesne.Donem;
                    eskiKayit.GerceklesenDeger = gelenNesne.GerceklesenDeger;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydý silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, AnahtarRiskGostergesiDonem gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod) && string.IsNullOrWhiteSpace(gelenNesne.AnahtarRiskGostergesiKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                if (!string.IsNullOrWhiteSpace(gelenNesne.Kod))
                    await _unitOfWork.SilAsync(d => d.Kod == gelenNesne.Kod);
                else if (!string.IsNullOrWhiteSpace(gelenNesne.AnahtarRiskGostergesiKod))
                    await _unitOfWork.SilAsync(d => d.AnahtarRiskGostergesiKod == gelenNesne.AnahtarRiskGostergesiKod);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

    }
}
