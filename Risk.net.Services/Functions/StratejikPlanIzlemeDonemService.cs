using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Linq;
using System.Threading.Tasks;
using Quartz.Util;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// StratejikPlanIzlemeDonem iþlemlerinin yapýldýðý servis
    /// </summary>
    public class StratejikPlanIzlemeDonemService : IStratejikPlanIzlemeDonemService
    {
        /// <summary>
        /// IUnitOfWork<StratejikPlanIzlemeDonem> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<StratejikPlanIzlemeDonem> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.StratejikPlanIzlemeDonemService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanIzlemeDonemService(IUnitOfWork<StratejikPlanIzlemeDonem> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
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
        /// <param name="stratejikPlanIzlemeGostergeKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string stratejikPlanIzlemeGostergeKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.StratejikPlanHedefGostergeKod == stratejikPlanIzlemeGostergeKod);
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
                StratejikPlanIzlemeDonem aramaObj = Arac.DataTablesAramaNesne<StratejikPlanIzlemeDonem>(new StratejikPlanIzlemeDonem(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.StratejikPlanHedefGostergeKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlanHedefGostergeKod.Contains(aramaObj.StratejikPlanHedefGostergeKod));


            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlanHedefGostergeKod.Contains(dataTablesParam.searchValue));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, StratejikPlanIzlemeDonem gelenNesne)
        {
            StratejikPlanIzlemeDonem islemYapilan = new StratejikPlanIzlemeDonem();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.StratejikPlanHedefGostergeKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.StratejikPlanKodAlaniBos"] + "</li>";
            if (gelenNesne.PlanlananDeger <= 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.StratejikPlanPlanlananDegerAlaniBos"] + "</li>";

            hata += YetkisiVarmi(kullanan, "KAYDET");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                if (string.IsNullOrWhiteSpace(gelenNesne.SapmaNedeni)) gelenNesne.SapmaNedeni = "";

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    eskiKayit.StratejikPlanHedefGostergeKod = gelenNesne.StratejikPlanHedefGostergeKod;
                    eskiKayit.Donem = gelenNesne.Donem;
                    eskiKayit.DonemAdi = gelenNesne.DonemAdi;
                    eskiKayit.PlanlananDeger = gelenNesne.PlanlananDeger;
                    eskiKayit.GerceklesenDeger = gelenNesne.GerceklesenDeger;
                    eskiKayit.GerceklesenDegerYilSonu = gelenNesne.GerceklesenDegerYilSonu;
                    eskiKayit.SapmaNedeni = gelenNesne.SapmaNedeni;
                    eskiKayit.SapmaOrani = gelenNesne.SapmaOrani;
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
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, StratejikPlanIzlemeDonem gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod) && string.IsNullOrWhiteSpace(gelenNesne.StratejikPlanHedefGostergeKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata += YetkisiVarmi(kullanan, "SILME");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                if (!string.IsNullOrWhiteSpace(gelenNesne.Kod))
                    await _unitOfWork.SilAsync(d => d.Kod == gelenNesne.Kod);
                else if (!string.IsNullOrWhiteSpace(gelenNesne.StratejikPlanHedefGostergeKod))
                    await _unitOfWork.SilAsync(d => d.StratejikPlanHedefGostergeKod == gelenNesne.StratejikPlanHedefGostergeKod);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        private string YetkisiVarmi(KullaniciDto kullanan, string tur)
        {
            bool yetki = Arac.YetkisiVarmi("PLANLAMAUNITESI,BIRIMAMIRI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }
    }
}
