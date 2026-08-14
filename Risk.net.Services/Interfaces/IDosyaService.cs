using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    /// <summary>
    /// Dosya işlemlerinin yapıldığı servisin arayüzü
    /// </summary>
    public interface IDosyaService
    {
        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string kod, string baglantiKod);
        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string baglantiKod);
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
        Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Dosya gelenNesne);
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
        /// Istemciden parametere ile gönderilen bağlantı kaydını silen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> SilBaglantiKodAsync(KullaniciDto kullanan, string baglantiKod);
        /// <summary>
        /// Istemciden parametre ile talep edilen dosya bilgisini döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Dosya> IndirAsync(KullaniciDto kullanan, string kod, string baglantiKod);
        /// <summary>
        /// Istemciden parametere ile gönderilen bağlatı kod bilgisini güncelleyen metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="eskiKod"></param>
        /// <param name="yeniKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> BaglantiKodGuncelleAsync(KullaniciDto kullanan, string eskiKod, string yeniKod);
        /// <summary>
        /// Istemciden parametere ile kayda ait dosya sayısını döndüren metodun arayüzü
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> SayiVerAsync(KullaniciDto kullanan, string baglantiKod);
        /// <summary>
        /// Istemciden parametere ile kayda ait açıklamayı güncelleyen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        Task<Sonuc> AciklamaKaydetAsync(KullaniciDto kullanan, Dosya gelenNesne);
    }
}
