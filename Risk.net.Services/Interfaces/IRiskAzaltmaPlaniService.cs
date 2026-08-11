using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// RiskAzaltmaPlani iþlemlerinin yapýldýðý servisin arayüzü
    /// </summary>
    public interface IRiskAzaltmaPlaniService
    {
        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="riskAzaltmaPlaniKod"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string riskAzaltmaPlaniKod, string kod);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <param name="onay"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam, bool onay);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen kaydýn durumunu deðiþtiren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilerden güncel deðerleri deðiþen alanlarý döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// string türünde deðiþen alanlarý döndürür
        /// </returns>
        Task<string> DegisenAlanlariGetirAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen kaydý silen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="riskAzaltmaPlani"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> SilAsync(KullaniciDto kullanan, RiskAzaltmaPlani riskAzaltmaPlani);
        /// <summary>
        /// Istemciden parametere ile gönderilen azaltma planý sorunlusunu deðiþtiren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> AzaltmaPlaniSorumlusuDegistirAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne);
    }
}
