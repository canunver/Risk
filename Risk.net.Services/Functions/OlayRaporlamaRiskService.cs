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
    /// OlayRaporlamaRisk işlemlerinin yapıldığı servis
    /// </summary>
    public class OlayRaporlamaRiskService : IOlayRaporlamaRiskService
    {
        /// <summary>
        /// IUnitOfWork<OlayRaporlamaRisk> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<OlayRaporlamaRisk> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.OlayRaporlamaRiskService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public OlayRaporlamaRiskService(IUnitOfWork<OlayRaporlamaRisk> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
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
        /// <param name="olayRaporlamaKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string olayRaporlamaKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.OlayRaporlamaKod == olayRaporlamaKod, o => o.Kod);
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
                OlayRaporlamaRisk aramaObj = Arac.DataTablesAramaNesne<OlayRaporlamaRisk>(new OlayRaporlamaRisk(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.OlayRaporlamaKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OlayRaporlamaKod.Contains(aramaObj.OlayRaporlamaKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskEvreniKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskEvreniKod.Contains(aramaObj.RiskEvreniKod));
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OlayRaporlamaKod.Contains(dataTablesParam.searchValue));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, OlayRaporlamaRisk gelenNesne)
        {
            OlayRaporlamaRisk islemYapilan = new OlayRaporlamaRisk();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.OlayRaporlamaKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.OlayRaporlamaKodAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.RiskEvreniKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskEvreniKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                gelenNesne.Kod = Arac.GetGuid();
                islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="olayRaporlamaKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string olayRaporlamaKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(olayRaporlamaKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.OlayRaporlamaKod == olayRaporlamaKod);
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
