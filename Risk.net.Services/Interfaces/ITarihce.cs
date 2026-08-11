using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// Tarihce iþlemlerinin yapýldýðý servisin arayüzü
    /// </summary>
    public interface ITarihceService
    {
        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string kod);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="ilgiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(string ilgiKod);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="ilgiTur"></param>
        /// <param name="siraNo"></param>
        /// <param name="islemYapanKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(EnumTarihceIslemTur ilgiTur, int siraNo, string islemYapanKod);
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
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Tarihce gelenNesne);
        /// <summary>
        /// Istemciden parametere ile tapep edilen bilgilere göre son kaydýn bütün bilgileri döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="ilgiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Tarihce> SonKaydiGetirAsync(KullaniciDto kullanan, string ilgiKod);
        /// <summary>
        /// Istemciden parametere ile tapep edilen bilgilere göre son açýklama bilgisi kaydedilen kaydýn bütün bilgileri döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="ilgiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Tarihce> SonAciklamaGetirAsync(KullaniciDto kullanan, string ilgiKod);
    }
}
