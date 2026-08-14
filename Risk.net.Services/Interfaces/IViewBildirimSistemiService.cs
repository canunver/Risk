using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// ViewBildirimSistemi işlemlerinin yapıldığı servisin arayüzü
    /// </summary>
    public interface IViewBildirimSistemiService
    {
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan, BildirimSistemi kriter);

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların sayısını döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<int> BildirimSayisiAsync(KullaniciDto kullanan);

        /// <summary>
        /// Onay işlemleri mail gönderilecek kayıtların listesini döndüren metod
        /// </summary>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeMailGonderilecekKayitlarAsync();
    }
}
