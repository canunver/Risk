using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// Tarihce iþlemlerinin yapýldýðý servis
    /// </summary>
    public class TarihceService : ITarihceService
    {
        /// <summary>
        /// IUnitOfWork<Tarihce> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<Tarihce> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.TarihceService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TarihceService(IUnitOfWork<Tarihce> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod);

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
        /// <param name="ilgiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(string ilgiKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.IlgiKod == ilgiKod, o => o.IslemTarihi, c => c.IslemYapan);
            if (kayitlar.Count > -1)
            {
                bool ilkKayit = true;
                foreach (Tarihce item in kayitlar)
                {
                    if (item.IlgiTur == EnumTarihceIslemTur.Konfigurasyon || item.Durum == (int)ENUMDurum.Kopyalandý || item.Durum == (int)ENUMDurum.HatirlatmaMail)
                        ilkKayit = false;

                    if (ilkKayit)
                        item.DurumAdi = "Ýlk kayýt";
                    else if (item.Durum <= 1)
                        item.DurumAdi = "Güncelleme";
                    else if (item.Durum == 2)
                        item.DurumAdi = "Geri Gönderildi";
                    else if (item.Durum == 3)
                        item.DurumAdi = "Onaya Gönderildi";
                    else if (item.Durum == 10)
                        item.DurumAdi = "Onaylandý";
                    else if (item.Durum == 98)
                        item.DurumAdi = "Reddedildi";
                    else if (item.Durum == 99)
                        item.DurumAdi = "Pasif yapýldý";
                    else if (item.Durum == 15)
                        item.DurumAdi = "Bilgilendirme Maili Gönderildi";
                    else if (item.Durum == 16)
                        item.DurumAdi = "Risk Sahibi Deðiþti";
                    else if (item.Durum == 17)
                        item.DurumAdi = "Kopyalandý";
                    else if (item.Durum == 18)
                        item.DurumAdi = "Azaltma Planý Sorumlusu Deðiþti";
                    else if (item.Durum == 19)
                        item.DurumAdi = "Hatýrlatma Maili Gönderildi";
                    else if (item.Durum == 999)
                        item.DurumAdi = "Silindi";
                    ilkKayit = false;
                }

                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="ilgiTur"></param>
        /// <param name="siraNo"></param>
        /// <param name="islemYapanKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(EnumTarihceIslemTur ilgiTur, int siraNo, string islemYapanKod)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan");
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.IlgiTur == ilgiTur);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.SiraNo == siraNo);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.IslemYapanKod == islemYapanKod);

            var kayitlar = selectData.ToList();

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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "", a => a.Kod);

            //if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            //{
            //    RiskEvreni aramaObj = Arac.DataTablesAramaNesne<RiskEvreni>(new RiskEvreni(), aramaDegeri);

            //    if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
            //        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod==aramaObj.KoordinatorlukKod);
            //    if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
            //        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod==aramaObj.BirimKod);
            //    //if (!string.IsNullOrWhiteSpace(aramaObj.RiskAdi))
            //    //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskAdi.Contains(aramaObj.RiskAdi));
            //    if (aramaObj.RiskBaslangicTarihi.Value.Year > 1950)
            //        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskBaslangicTarihi >= aramaObj.RiskBaslangicTarihi);
            //    if (aramaObj.RiskBitisTarihi.Value.Year > 1950)
            //        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskBitisTarihi <= aramaObj.RiskBitisTarihi);
            //    if (aramaObj.Durum > 0)
            //        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
            //}
            //else
            //{
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Surec.Adi.Contains(dataTablesParam.searchValue)
            //                                                    || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
            //                                                    || a.Birim.Adi.Contains(dataTablesParam.searchValue)
            //                                                    || a.RiskAdi.Contains(dataTablesParam.searchValue)
            //                                                    || a.RiskTanimi.Contains(dataTablesParam.searchValue)
            //                                                    || a.AnahtarRiskGostergesi.Contains(dataTablesParam.searchValue)
            //                                                    || a.RiskNo.Contains(dataTablesParam.searchValue));
            //}

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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Tarihce gelenNesne)
        {
            Tarihce islemYapilan = new Tarihce();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.IlgiKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.TarihceIlgiAlaniBos"] + "</li>";
            if (gelenNesne.IlgiTur == EnumTarihceIslemTur.Tanimsiz)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.TarihceIlgiTurAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            if (string.IsNullOrWhiteSpace(gelenNesne.YeniDeger)) gelenNesne.YeniDeger = "{}";
            if (string.IsNullOrWhiteSpace(gelenNesne.EskiDeger)) gelenNesne.EskiDeger = "{}";

            try
            {
                gelenNesne.Kod = Arac.GetGuid();
                int kayitSayisi = await _unitOfWork.KayitSayisiAsync(d => d.IlgiKod == gelenNesne.IlgiKod);
                gelenNesne.SiraNo = kayitSayisi + 1;
                gelenNesne.IslemTarihi = System.DateTime.Now;
                //gelenNesne.IslemYapanKod = kullanan.PersonelKod;
                gelenNesne.IslemYapanRol = kullanan.AktifRolKod;

                if (gelenNesne.EskiDeger == gelenNesne.YeniDeger)
                {
                    gelenNesne.EskiDeger = "";
                    gelenNesne.YeniDeger = "";
                    gelenNesne.DegisenDeger = "";
                }
                else
                {
                    var yeni = JToken.Parse(gelenNesne.YeniDeger);
                    var eski = JToken.Parse(gelenNesne.EskiDeger);

                    var degisen = Arac.DegisenleriBul(yeni, eski);

                    gelenNesne.DegisenDeger = degisen.ToString(Newtonsoft.Json.Formatting.None);
                }

                islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);

                await _unitOfWork.KaydetAsync();

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li><li>" + ex.InnerException + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile tapep edilen bilgilere göre son kaydýn bütün bilgileri döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="ilgiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Tarihce> SonKaydiGetirAsync(KullaniciDto kullanan, string ilgiKod)
        {
            var tarihce = new Tarihce();

            var kayitlar = await _unitOfWork.ListeleAsync(k => k.IlgiKod == ilgiKod);
            if (kayitlar.Count > -1)
            {
                foreach (var item in kayitlar)
                {
                    if (item.SiraNo > tarihce.SiraNo)
                        tarihce = item;
                }
            }

            return tarihce;
        }

        /// <summary>
        /// Istemciden parametere ile tapep edilen bilgilere göre son açýklama bilgisi kaydedilen kaydýn bütün bilgileri döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="ilgiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Tarihce> SonAciklamaGetirAsync(KullaniciDto kullanan, string ilgiKod)
        {
            var tarihce = new Tarihce();

            var kayitlar = await _unitOfWork.ListeleAsync(k => k.IlgiKod == ilgiKod);
            if (kayitlar.Count > -1)
            {
                foreach (var item in kayitlar)
                {
                    if (item.SiraNo > tarihce.SiraNo && !string.IsNullOrWhiteSpace(item.Aciklama))
                        tarihce = item;
                }
            }

            return tarihce;
        }
    }

}
