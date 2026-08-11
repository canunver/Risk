using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace Risk.net.Services.Functions
{
    public class TanimBirimKoordinatorlukService : ITanimBirimKoordinatorlukService
    {
        private readonly IUnitOfWork<TanimBirimKoordinatorluk> _unitOfWork;
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        public TanimBirimKoordinatorlukService(IUnitOfWork<TanimBirimKoordinatorluk> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _sharedResource = sharedResource;
        }

        public async Task<Sonuc> GetAll(string birimKod)
        {
            //var kayitlar = await _unitOfWork.GetAllAsync(f => f.BirimKod == birimKod, a => a.Koordinatorluk);
            //if (kayitlar.Count > -1)
            //{
            //    return new Sonuc(IslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            //}
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        public async Task<Sonuc> Save(string birimKod, string koordinatorlukKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(birimKod))
                hata += "<li>" + "Birim kod boþ olamaz" + "</li>";//_sharedResource["Kontrol.Duzenle.KullaniciKodAlaniBos"]
            if (string.IsNullOrWhiteSpace(koordinatorlukKod))
                hata += "<li>" + "Koordinatörlük kod boþ olamaz" + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                //Kayýt edilmek istenen rol önceden kayýtlý mý?
                var eskiKayit = await _unitOfWork.GetAsync(c => c.BirimKod == birimKod && c.KoordinatorlukKod == koordinatorlukKod);
                if (eskiKayit != null)
                {
                    //Eðer kayýt bulunursa çýkýþ yap
                    return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
                }

                TanimBirimKoordinatorluk yeni = new TanimBirimKoordinatorluk();

                yeni.Kod = Arac.GetGuid();
                yeni.BirimKod = birimKod;
                yeni.KoordinatorlukKod = koordinatorlukKod;

                await _unitOfWork.AddAsync(yeni);
                await _unitOfWork.SaveAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }

        public async Task<Sonuc> Delete(string birimKod, string koordinatorlukKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(birimKod))
                hata += "<li>" + "Birim kod boþ olamaz" + "</li>";//_sharedResource["Kontrol.Duzenle.KullaniciKodAlaniBos"]
            if (string.IsNullOrWhiteSpace(koordinatorlukKod))
                hata += "<li>" + "Koordinatörlük kod boþ olamaz" + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                await _unitOfWork.DeleteAsync(d => d.BirimKod == birimKod && d.KoordinatorlukKod == koordinatorlukKod);
                await _unitOfWork.SaveAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }
            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }
    }
}
