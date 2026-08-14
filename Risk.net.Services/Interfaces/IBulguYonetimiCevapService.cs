using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// BulguYonetimiCevap işlemlerinin yapıldığı servisin arayüzü
    /// </summary>
    public interface IBulguYonetimiCevapService
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
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan, BulguYonetimiCevap kriter);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
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
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, BulguYonetimiCevap gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> SilAsync(KullaniciDto kullanan, BulguYonetimiCevap gelenNesne);
        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın durumunu değiştiren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, BulguYonetimiCevap gelenNesne);
        /// <summary>
        /// Kişilere atanan bulguların atanan kişiye haber verilmesi için eposta gönderiminin yapıldığı metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="bulgu"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> BilgiEPostaGonderAsync(KullaniciDto kullanan, string bulguKod, List<string> gelenNesne);
        /// <summary>
        /// Kişilerin bulgulara verdiği cevap bilgisini ilgili denetcilere haber verilmesi için eposta gönderiminin yapıldığı metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="bulguCevapKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> CevapVerildiEPostaGonderAsync(KullaniciDto kullanan, string bulguCevapKod);
    }
}
