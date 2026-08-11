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
    /// RiskYonetimiKontrol iþlemlerinin yapýldýðý servis
    /// </summary>
    public class RiskYonetimiKontrolService : IRiskYonetimiKontrolService
    {
        /// <summary>
        /// IUnitOfWork<RiskYonetimiKontrol> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskYonetimiKontrol> _unitOfWork;
        /// <summary>
        /// IUnitOfWork<RiskYonetimi> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskYonetimi> _unitOfWorkRiskYonetimi;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.RiskYonetimiKontrolService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unitOfWorkRiskYonetimi"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskYonetimiKontrolService(IUnitOfWork<RiskYonetimiKontrol> unitOfWork, IUnitOfWork<RiskYonetimi> unitOfWorkRiskYonetimi, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkRiskYonetimi = unitOfWorkRiskYonetimi;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "");

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
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="riskYonetimiKod"></param>
        /// <param name="durumKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string riskYonetimiKod, int durumKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.RiskYonetimiKod == riskYonetimiKod && a.Durum == durumKod, o => o.Tanim);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                RiskYonetimiKontrol aramaObj = Arac.DataTablesAramaNesne<RiskYonetimiKontrol>(new RiskYonetimiKontrol(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.RiskYonetimiKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskYonetimiKod.Contains(aramaObj.RiskYonetimiKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.Tanim))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Tanim.Contains(aramaObj.Tanim));
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Tanim.Contains(dataTablesParam.searchValue));
            }

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, RiskYonetimiKontrol gelenNesne)
        {
            RiskYonetimiKontrol islemYapilan = new RiskYonetimiKontrol();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.RiskYonetimiKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.Tanim))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";
            if (gelenNesne.OnemDuzeyi <= 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.OnemDuzeyiAlaniBos"] + "</li>";


            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                //Durum deðiþikliðine uygun mu?
                var riskYonetimi = await _unitOfWorkRiskYonetimi.KayitGetirAsync(c => c.Kod == gelenNesne.RiskYonetimiKod, "Kontroller");
                if (riskYonetimi.Durum == (int)ENUMDurum.Pasif)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.IptalEdilmis"] + "</li>";
                //else if (riskYonetimi.Durum == (int)ENUMDurum.Onayli)
                //    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.Onaylanmis"] + "</li>";


                if (hata == "" && riskYonetimi.Kontroller != null)
                {
                    double toplamOnemDuzeyi = gelenNesne.OnemDuzeyi;
                    foreach (var item in riskYonetimi.Kontroller)
                    {
                        if (item.Durum == 1)
                            toplamOnemDuzeyi += item.OnemDuzeyi;
                    }

                    if (toplamOnemDuzeyi > 100)
                        hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiOnemDuzeyiHatali"] + "</li>";
                }


                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    gelenNesne.Durum = (int)ENUMDurum.Aktif;
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    eskiKayit.RiskYonetimiKod = gelenNesne.RiskYonetimiKod;
                    eskiKayit.Tanim = gelenNesne.Tanim;
                    eskiKayit.Etkinlik = gelenNesne.Etkinlik;
                    eskiKayit.OnemDuzeyi = gelenNesne.OnemDuzeyi;
                    eskiKayit.Durum = gelenNesne.Durum;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydý silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="riskYonetimiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string riskYonetimiKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(riskYonetimiKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.RiskYonetimiKod == riskYonetimiKod);
                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydýn durumunu deðiþtiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, RiskYonetimiKontrol gelenNesne)
        {
            RiskYonetimiKontrol islemYapilan = new RiskYonetimiKontrol();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Durum deðiþikliðine uygun mu?
                var riskYonetimi = await _unitOfWorkRiskYonetimi.KayitGetirAsync(c => c.Kod == eskiKayit.RiskYonetimiKod);
                if (riskYonetimi.Durum == (int)ENUMDurum.Pasif)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.IptalEdilmis"] + "</li>";
                else if (riskYonetimi.Durum == (int)ENUMDurum.Onayli)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.Onaylanmis"] + "</li>";

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                eskiKayit.Durum = gelenNesne.Durum;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.IslemBasarili"]);
        }
    }
}
