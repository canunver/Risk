using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// ViewPersonel işlemlerinin yapıldığı servisin arayüzü
    /// </summary>
    public interface IViewPersonelService
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
        /// <param name="koordinatorlukKod"></param>
        /// <param name="birimKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string koordinatorlukKod, string birimKod);
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
        /// Istemciden parametre ile talep edilen bilgiye ait resim bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ResimGetirAsync(string kod);

    }
}
