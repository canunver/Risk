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
    /// DenetimGorevlendirme işlemlerinin yapıldığı servis
    /// </summary>
    public class DenetimGorevlendirmeService : IDenetimGorevlendirmeService
    {
        /// <summary>
        /// IUnitOfWork<DenetimGorevlendirme> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<DenetimGorevlendirme> _unitOfWork;
        /// <summary>
        /// IUnitOfWork<DenetimGorevlendirme> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimService _serviceDenetim;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.DenetimGorevlendirmeService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public DenetimGorevlendirmeService(IUnitOfWork<DenetimGorevlendirme> unitOfWork,
                                            IDenetimService serviceDenetim,
                                            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceDenetim = serviceDenetim;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, DenetimGorevlendirme gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.DenetimKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DenetimKodAlaniBos"] + "</li>";
            if (gelenNesne.Tip == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DenetimGorevlendirmeTipAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWork.KayitGetirAsync(d => d.DenetimKod == gelenNesne.DenetimKod && d.Tip == gelenNesne.Tip, "");

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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, DenetimGorevlendirme gelenNesne)
        {
            DenetimGorevlendirme islemYapilan = new DenetimGorevlendirme();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.DenetimKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DenetimKodAlaniBos"] + "</li>";
            if (gelenNesne.Tip == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DenetimGorevlendirmeTipAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                if (!Arac.YetkisiVarmi("ICDENETIMKOORDINATOR", kullanan))
                {
                    Sonuc sonucDenetim = await _serviceDenetim.KayitGetirAsync(kullanan, gelenNesne.DenetimKod);
                    if (!sonucDenetim.IslemSonuc || sonucDenetim.Nesne == null) return new Sonuc(ENUMIslemDurum.Uyari, "Denetim bulunamadı");
                    Denetim denetimKaydi = (Denetim)sonucDenetim.Nesne;
                    bool bulundu = false;
                    foreach (var item in denetimKaydi.Sorumlular)
                    {
                        if (item.SorumluKod == kullanan.PersonelKod)
                            bulundu = true;
                    }
                    foreach (var item in denetimKaydi.Denetciler)
                    {
                        if (item.DenetciKod == kullanan.PersonelKod)
                            bulundu = true;
                    }

                    if (!bulundu)
                        return new Sonuc(ENUMIslemDurum.Hata, "Kaydetme için yetkiniz yok. Kayıt işlemini, ilgili denetimde görevli kişiler yapabilir.");
                }

                await SilAsync(kullanan, gelenNesne);

                gelenNesne.Kod = Arac.GetGuid();
                islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, DenetimGorevlendirme gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.DenetimKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DenetimKodAlaniBos"] + "</li>";
            if (gelenNesne.Tip == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DenetimGorevlendirmeTipAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.DenetimKod == gelenNesne.DenetimKod && d.Tip == gelenNesne.Tip);
                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

    }
}
