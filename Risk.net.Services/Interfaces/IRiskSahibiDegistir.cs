using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// RiskSahibiDegistir iþlemlerinin yapýldýðý servisin arayüzü
    /// </summary>
    public interface IRiskSahibiDegistirService
    {
        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilerine yeniKod bilgisini eskiKod bilgisi ile güncelleyen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="eskiKod"></param>
        /// <param name="yeniKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, string eskiKod, string yeniKod);

    }
}
