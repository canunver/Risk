using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// Konfigurasyon işlemlerinin yapıldığı servisin arayüzü
    /// </summary>
    public interface IKonfigurasyonService
    {
        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="durum"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, int durum);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Konfigurasyon gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın durumunu değiştiren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, Konfigurasyon gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere ait etki kriteri adını döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="seviye"></param>
        /// <param name="riskKategorisi"></param>
        /// <returns>
        /// Etki kriter adını göndürür
        /// </returns>
        Task<string> EtkiKriteriAdiVer(KullaniciDto kullanan, int seviye, string riskKategorisi);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere ait yapısal risk seviyesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="etki"></param>
        /// <param name="olasilik"></param>
        /// <returns>
        /// Yapısal risk seviyesini göndürür
        /// </returns>
        Task<object> YapisalRiskSeviyesiVer(KullaniciDto kullanan, int etki, int olasilik);
        /// <summary>
        /// Etki olasılık matrisine ait tüm bilgileri döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// object nesnesi döndürür
        /// </returns>
        Task<object> EtkiOlasilikMatrisiVer(KullaniciDto kullanan);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere ait artık risk seviyesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="artikRiskPuani"></param>
        /// <returns>
        /// object nesnesi döndürür
        /// </returns>
        Task<object> ArtikRiskSeviyesiVer(KullaniciDto kullanan, double artikRiskPuani);
    }
}
