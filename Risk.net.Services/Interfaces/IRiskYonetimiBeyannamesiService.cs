using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// RiskYonetimiBeyannamesi işlemlerinin yapıldığı servisin arayüzü
    /// </summary>
    public interface IRiskYonetimiBeyannamesiService
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
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan, RiskYonetimiBeyannamesi kriter);
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
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, RiskYonetimiBeyannamesi gelenNesne);

        /// <summary>
        /// Onaylı kayıtların onaylarını kaldıran metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> OnayKaldirAsync(KullaniciDto kullanan);

        /// <summary>
        /// Hatırlatma maili gönderilmesi işlemini sağlayan metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> HatirlatmaMailiGonder(KullaniciDto kullanan);

    }
}
