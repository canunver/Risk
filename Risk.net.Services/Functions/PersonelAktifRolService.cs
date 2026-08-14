using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// PersonelAktifRol işlemlerinin yapıldığı servis
    /// </summary>
    public class PersonelAktifRolService : IPersonelAktifRolService
    {
        /// <summary>
        /// IUnitOfWork<PersonelAktifRol> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<PersonelAktifRol> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.PersonelAktifRolService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public PersonelAktifRolService(IUnitOfWork<PersonelAktifRol> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="personelKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string personelKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(personelKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.PersonelKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.PersonelKod == personelKod);

                if (kayit != null)
                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
            }
            catch (System.Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }

            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, PersonelAktifRol gelenNesne)
        {
            PersonelAktifRol islemYapilan = new PersonelAktifRol();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.PersonelKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.PersonelKodAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.Rol))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RolAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                PersonelAktifRol eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.PersonelKod == gelenNesne.PersonelKod);
                if (eskiKayit != null)
                {
                    eskiKayit.PersonelKod = gelenNesne.PersonelKod;
                    eskiKayit.Rol = gelenNesne.Rol;
                    eskiKayit.KoordinatorlukKod = gelenNesne.KoordinatorlukKod;
                    eskiKayit.BirimKod = gelenNesne.BirimKod;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }
                else
                {
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }
    }
}
