using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// BildirimSistemi iþlemlerinin yapýldýðý servisin arayüzü
    /// </summary>
    public interface IBildirimSistemiService
    {
        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, int kod);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan, BildirimSistemi gelenNesne);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, BildirimSistemi gelenNesne);
        /// <summary>
        /// Istemciden parametere ile talep edilen bilgilere göre onaylayacak yetki biglisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns></returns>
        Task<string> OnaylayacakYetkiVerAsync(KullaniciDto kullanan, BildirimSistemi kriter);
        /// <summary>
        /// Istemciden parametere ile talep edilen bilgilere göre geri gönderilecek yetki biglisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns></returns>
        Task<string> GeriGonderilecekYetkiVerAsync(KullaniciDto kullanan, BildirimSistemi kriter);
        /// <summary>
        /// Istemciden parametere ile talep edilen bilgilere göre onaylayacak son yetki biglisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns></returns>
        Task<BildirimSistemi> SonYetkiVerAsync(KullaniciDto kullanan, BildirimSistemi kriter);
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere göre mail gönderen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns></returns>
        Task<Sonuc> MailGonderAsync(KullaniciDto kullanan, BildirimSistemi kriter);

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydý silen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> SilAsync(KullaniciDto kullanan, BildirimSistemi kriter);
    }
}
