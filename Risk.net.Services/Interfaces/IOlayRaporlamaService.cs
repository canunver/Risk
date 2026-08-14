using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// OlayRaporlama işlemlerinin yapıldığı servisin arayüzü
    /// </summary>
    public interface IOlayRaporlamaService
    {
        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string kod);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan, OlayRaporlama kriter);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metodun arayüzü
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
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, OlayRaporlama gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> SilAsync(KullaniciDto kullanan, string kod);
        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın durumunu değiştiren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, OlayRaporlama gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere göre kaydın durumunu değiştiren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kodListe"></param>
        /// <param name="durum"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, string[] kodListe, int durum);
    }
}
