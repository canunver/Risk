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
using System.Linq.Expressions;
using System;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// OlayRaporlama iþlemlerinin yapýldýðý servis
    /// </summary>
    public class OlayRaporlamaService : IOlayRaporlamaService
    {
        /// <summary>
        /// IUnitOfWork<OlayRaporlama> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<OlayRaporlama> _unitOfWork;
        /// <summary>
        /// IOlayRaporlamaRiskService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IOlayRaporlamaRiskService _serviceOlayRaporlamaRisk;
        /// <summary>
        /// IOlayRaporlamaOlayKategoriService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IOlayRaporlamaOlayKategoriService _serviceOlayRaporlamaKategori;
        /// <summary>
        /// IBildirimSistemiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBildirimSistemiService _serviceBildirimSistemi;
        /// <summary>
        /// IViewBildirimSistemiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBildirimSistemiService _serviceViewBildirim;
        private readonly IViewBirimService _serviceViewBirim;
        /// <summary>
        /// IUnitOfWork<ITarihceService> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// ITarihceService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.OlayRaporlamaService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceOlayRaporlamaRisk"></param>
        /// <param name="serviceBildirimSistemi"></param>
        /// <param name="serviceViewBildirim"></param>
        /// <param name="serviceViewBirim"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public OlayRaporlamaService(IUnitOfWork<OlayRaporlama> unitOfWork,
            IOlayRaporlamaRiskService serviceOlayRaporlamaRisk,
            IOlayRaporlamaOlayKategoriService serviceOlayRaporlamaKategori,
            IBildirimSistemiService serviceBildirimSistemi,
            IViewBildirimSistemiService serviceViewBildirim,
            IViewBirimService serviceViewBirim,
            ITarihceService serviceTarihce,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceOlayRaporlamaRisk = serviceOlayRaporlamaRisk;
            _serviceOlayRaporlamaKategori = serviceOlayRaporlamaKategori;
            _serviceBildirimSistemi = serviceBildirimSistemi;
            _serviceViewBildirim = serviceViewBildirim;
            _serviceViewBirim = serviceViewBirim;
            _serviceTarihce = serviceTarihce;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "OlayKategoriler,OlayKategoriler.OlayKategori,Riskler.RiskEvreni,Riskler.RiskEvreni.RiskKategoriler.RiskKategori", c => c.OlayKategoriler);

                if (kayit != null)
                {
                    kayit.Tarihce = await _serviceTarihce.SonAciklamaGetirAsync(kullanan, kod);

                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
                }
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
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, OlayRaporlama kriter)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "", a => a.Birim, a => a.Koordinatorluk);

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kriter.KoordinatorlukKod);
            if (!string.IsNullOrWhiteSpace(kriter.BirimKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kriter.BirimKod);

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);

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
        /// <param name="onay"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam, bool onay)
        {
            List<string> belgeNolar = new List<string>();
            if (onay)
            {
                BildirimSistemi kriter = new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.OlayRaporlama };

                Sonuc bildirimSonuc = await _serviceViewBildirim.ListeleAsync(kullanan, kriter);
                if (bildirimSonuc.IslemSonuc)
                {
                    foreach (ViewBildirimSistemi item in bildirimSonuc.Liste)
                    {
                        belgeNolar.Add(item.BelgeKod);
                    }
                }
            }

            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "OlayKategoriler,OlayKategoriler.OlayKategori", a => a.Birim, a => a.Koordinatorluk, a => a.OlayKategoriler);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                OlayRaporlama aramaObj = Arac.DataTablesAramaNesne<OlayRaporlama>(new OlayRaporlama(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                //if (!string.IsNullOrWhiteSpace(aramaObj.OlayKategoriKod))
                //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OlayKategori.Kod.Contains(aramaObj.OlayKategoriKod));
                if (aramaObj.SorguTarihi1.Year > 1950)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OlayTarihi >= aramaObj.SorguTarihi1);
                if (aramaObj.SorguTarihi2.Year > 1950)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OlayTarihi <= aramaObj.SorguTarihi2);
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);

                if (onay)
                {
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => belgeNolar.ToArray().Contains(a.Kod));
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.OnayaGonderdi);
                }
                else if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif
                                                                    || a.Durum == (int)ENUMDurum.GeriGonderildi
                                                                    || a.Durum == (int)ENUMDurum.Reddedildi
                                                                    || a.Durum == (int)ENUMDurum.Onayli
                                                                    || a.Durum == (int)ENUMDurum.OnayaGonderdi);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.OlayYeri.Contains(dataTablesParam.searchValue)
                                                                || a.OlayTanimi.Contains(dataTablesParam.searchValue));

                if (onay)
                {
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => belgeNolar.ToArray().Contains(a.Kod));
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.OnayaGonderdi);
                }
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif
                                                                    || a.Durum == (int)ENUMDurum.GeriGonderildi
                                                                    || a.Durum == (int)ENUMDurum.Reddedildi
                                                                    || a.Durum == (int)ENUMDurum.Onayli
                                                                    || a.Durum == (int)ENUMDurum.OnayaGonderdi);
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, OlayRaporlama gelenNesne)
        {
            OlayRaporlama islemYapilan = new OlayRaporlama();

            string hata = "";

            //if (string.IsNullOrWhiteSpace(gelenNesne.KoordinatorlukKod))
            //    hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";

            ////Bazý koordinatörlüklerin birimleri yok. Bu durumda seçim yapýlamaz. Bu kontrolün yapýlabilmesi için seçilmiþ olan koordinatörlüðün
            ////birimi var mý kontrolünün yapýlmasý gerekli
            ////Client tarafýnda boþ olamaz kontrolü var Melih 22.09.2021
            ////if (string.IsNullOrWhiteSpace(gelenNesne.BirimKod))
            ////    hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";

            if (string.IsNullOrWhiteSpace(gelenNesne.OlayTanimi))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.OlayTanimiAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.OlayYeri))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.OlayYeriAlaniBos"] + "</li>";
            //if (string.IsNullOrWhiteSpace(gelenNesne.OlayKategoriKod))
            //    hata += "<li>" + _sharedResource["Kontrol.Duzenle.OlayKategoriAlaniBos"] + "</li>";
            //if (gelenNesne.OlayTarihi < 1950)
            //    hata += "<li>" + _sharedResource["Kontrol.Duzenle.OlayTarihiAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                //kaydet iþleminde riskler dolu olursa hata veriyor EF den dolayý
                //List<OlayRaporlamaRisk> riskler = gelenNesne.Riskler;
                //gelenNesne.Riskler = null;

                List<OlayRaporlamaKategori> kategoriler = gelenNesne.OlayKategoriler;
                gelenNesne.OlayKategoriler = null;


                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    gelenNesne.OlaySahibiKod = kullanan.PersonelKod;

                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                    tarihce.EskiDeger = Arac.JSONSerialize(new OlayRaporlama());
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                    ////Olay sahibi sonradan eklendi. Boþ olan kayýtlarý ilk güncelleyen kiþiyi kaydet.
                    //if (string.IsNullOrWhiteSpace(eskiKayit.OlaySahibiKod))
                    //    eskiKayit.OlaySahibiKod = kullanan.PersonelKod;
                    ////Kayýtlý bir bilgi ancak sahibi tarafýndan deðiþtirilebilir
                    //else 


                    if (eskiKayit.OlaySahibiKod != kullanan.PersonelKod)
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";

                    //Onaya gönderilen kayýt deðiþtirilemez
                    if (eskiKayit.Durum == (int)ENUMDurum.OnayaGonderdi)
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";

                    if (hata != "")
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);

                    eskiKayit.Durum = gelenNesne.Durum;
                    eskiKayit.KoordinatorlukKod = gelenNesne.KoordinatorlukKod;
                    eskiKayit.BirimKod = gelenNesne.BirimKod;
                    eskiKayit.OlayKategoriKod = gelenNesne.OlayKategoriKod;
                    eskiKayit.OlayTanimi = gelenNesne.OlayTanimi;
                    eskiKayit.OlayTarihi = gelenNesne.OlayTarihi;
                    eskiKayit.OlayYeri = gelenNesne.OlayYeri;
                    eskiKayit.Tutari = gelenNesne.Tutari;
                    eskiKayit.Aciklama = gelenNesne.Aciklama;
                    eskiKayit.KokNedeni = gelenNesne.KokNedeni;

                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                //if (riskler != null)
                //{
                //    await _serviceOlayRaporlamaRisk.SilAsync(kullanan, gelenNesne.Kod);

                //    foreach (var item in riskler)
                //    {
                //        OlayRaporlamaRisk giden = new OlayRaporlamaRisk();
                //        item.OlayRaporlamaKod = gelenNesne.Kod;

                //        await _serviceOlayRaporlamaRisk.KaydetAsync(kullanan, item);
                //    }
                //}

                await _serviceOlayRaporlamaKategori.SilAsync(kullanan, gelenNesne.Kod);

                if (kategoriler != null)
                {
                    foreach (var item in kategoriler)
                    {
                        OlayRaporlamaKategori giden = new OlayRaporlamaKategori();
                        item.OlayRaporlamaKod = gelenNesne.Kod;

                        await _serviceOlayRaporlamaKategori.KaydetAsync(kullanan, item);
                    }
                }

                tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.OlayRaporlama;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = gelenNesne.Durum;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], gelenNesne.Kod);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kullanýcýnýn onaylanacak yetki deðerini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// string türünde yetki bilgisi döndürür
        /// </returns>
        private string OnaylanacakYetkiKoduBul(KullaniciDto kullanan)
        {
            foreach (KullaniciRolDto item in kullanan.Roller)
            {

                if (item.Adi == "ICDENETIMKOORDINATOR" || item.Adi == "ILKOORDINATOR" || item.Adi == "GENELKOORDINATOR")//Eðer Ýl Koordinatörü/Genel Koordinatör veri giriþi yaparsa Baþkan onaylar.
                    return "BASKAN";
                else if (item.Adi == "MERKEZKOORDINATOR")//Eðer Merkez Koordinatörü veri giriþi yaparsa baðlý olduðu Genel Koordinatör onaylar
                    return "GENELKOORDINATOR";
                else if (item.Adi == "BIRIMAMIRI")//Eðer Birim Amiri veri giriþi yaparsa baðlý olduðu Koordinatör onaylar. (Ýl Koordinatörü veya Merkez Koordinatör olabilir.)
                    return "KOORDINATOR";
                else if (item.Adi == "ICDENETIMUZMANI")
                    return "ICDENETIMKOORDINATOR";
            }

            return "BIRIMAMIRI";// Eðer Uzman seviyesinde giriþ yapýldýysa baðlý olduðu Birimin Amiri onaylar.
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydý silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.Kod == kod);
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
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, OlayRaporlama gelenNesne)
        {
            OlayRaporlama islemYapilan = new OlayRaporlama();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();

                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod, "Koordinatorluk");

                //Durum deðiþikliðine uygun mu?
                hata = Arac.DurumDegisikligiUygunMu(kullanan, _sharedResource, eskiKayit, gelenNesne);

                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                    eskiKayit.KayitTarihi = DateTime.Now.Date;

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                tarihce.EskiDeger = "";

                eskiKayit.Durum = gelenNesne.Durum;

                //BildirimSistemi Baþlangýç
                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi || gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Reddedildi)
                {
                    var bs = new BildirimSistemi
                    {
                        BelgeTipi = (int)EnumTarihceIslemTur.OlayRaporlama,
                        BelgeKod = eskiKayit.Kod,
                        KoordinatorlukKod = eskiKayit.KoordinatorlukKod,
                        BirimKod = eskiKayit.BirimKod,
                        IslemYapanKod = kullanan.PersonelKod,
                        IslemTarihi = DateTime.Now,
                    };

                    if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                    {
                        bs.OnaylayacakYetki = Arac.UstYetkiVer(kullanan, (EnumKoordinatorlukTur)eskiKayit.Koordinatorluk.Tur);
                        bs.Durum = (int)ENUMDurum.OnayaGonderdi;

                        if (string.IsNullOrWhiteSpace(bs.OnaylayacakYetki))
                            hata += "<li>" + _sharedResource["Kontrol.DurumDegistir.OnaylayacakYetkiBulunamadi"] + "</li>";
                        else if (bs.OnaylayacakYetki == kullanan.AktifRolKod)
                        {
                            //Eðer üst yetki ayný kiþi ise onaya gönderilmeden onaylansýn
                            bs.Durum = (int)ENUMDurum.Onayli;
                            gelenNesne.Durum = (int)ENUMDurum.Onayli;
                            eskiKayit.Durum = (int)ENUMDurum.Onayli;
                        }
                    }
                    else if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                    {
                        bs.OnaylayacakYetki = kullanan.AktifRolKod;
                        bs.Durum = (int)ENUMDurum.Onayli;
                    }
                    else if (gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                    {
                        bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kiþi yok
                        bs.OnaylayacakYetki = "-";
                    }
                    else if (gelenNesne.Durum == (int)ENUMDurum.Reddedildi)
                    {
                        bs.Durum = (int)ENUMDurum.Reddedildi;
                        bs.OnaylayacakYetki = "-";
                    }

                    var sonucBildirimSistemi = await _serviceBildirimSistemi.KaydetAsync(kullanan, bs);

                    if (!sonucBildirimSistemi.IslemSonuc)
                        return new Sonuc(ENUMIslemDurum.Hata, sonucBildirimSistemi.Mesaj);


                    if (bs.OnaylayacakYetki != kullanan.AktifRolKod && bs.OnaylayacakYetki != "-")
                        tarihce.IlgiliRol = bs.OnaylayacakYetki;
                }
                //BildirimSistemi Bitiþ

                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                //Tarihçe Baþlangýç
                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.OlayRaporlama;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = gelenNesne.Durum;
                if (gelenNesne.Tarihce != null)
                    tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;

                await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                //Tarihçe Bitiþ

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere göre kaydýn durumunu deðiþtiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kodListe"></param>
        /// <param name="durum"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, string[] kodListe, int durum)
        {
            string hata = "";
            if (kodListe == null || kodListe.Length == 0)
                hata = "<li>" + _sharedResource["Kontrol.OnaylanacakKayitlariSeciniz"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                foreach (var kod in kodListe)
                {
                    var form = new OlayRaporlama();
                    form.Kod = kod;
                    form.Durum = (int)ENUMDurum.Onayli;

                    var sonuc = await DurumDegistirAsync(kullanan, form);

                    if (!sonuc.IslemSonuc)
                        hata += "<li>" + sonuc.Mesaj + "</li>";

                }

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }
    }
}
