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
using System;
using System.Linq.Expressions;
using Risk.net.Data.Functions;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// RiskYonetimi iþlemlerinin yapýldýðý servis
    /// </summary>
    public class RiskYonetimiService : IRiskYonetimiService
    {
        /// <summary>
        /// IUnitOfWork<RiskEvreni> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskEvreni> _unitOfWorkRiskEvreni;
        /// <summary>
        /// IUnitOfWork<RiskYonetimi> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskYonetimi> _unitOfWorkAnahtar;
        /// <summary>
        /// IRiskYonetimiKontrolService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskYonetimiKontrolService _serviceKontrol;
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
        /// <summary>
        /// IViewBirimService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBirimService _serviceViewBirim;
        /// <summary>
        /// ITarihceService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IViewYetkiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniService _serviceRiskAzaltmaPlani;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewYetkiService _serviceYetki;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.RiskYonetimiService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWorkRiskEvreni"></param>
        /// <param name="unitOfWorkAnahtar"></param>
        /// <param name="serviceKontrol"></param>
        /// <param name="serviceBildirimSistemi"></param>
        /// <param name="serviceViewBildirim"></param>
        /// <param name="serviceViewBirim"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="serviceRiskAzaltmaPlani"></param>
        /// <param name="serviceYetki"></param>
        /// <param name="serviceCTE"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskYonetimiService(IUnitOfWork<RiskEvreni> unitOfWorkRiskEvreni
            , IUnitOfWork<RiskYonetimi> unitOfWorkAnahtar
            , IRiskYonetimiKontrolService serviceKontrol
            , IBildirimSistemiService serviceBildirimSistemi
            , IViewBildirimSistemiService serviceViewBildirim
            , IViewBirimService serviceViewBirim
            , ITarihceService serviceTarihce
            , IRiskAzaltmaPlaniService serviceRiskAzaltmaPlani
            , IViewYetkiService serviceYetki
            , ICTEKoordinatorlukService serviceCTE
            , IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWorkRiskEvreni = unitOfWorkRiskEvreni;
            _unitOfWorkAnahtar = unitOfWorkAnahtar;
            _serviceKontrol = serviceKontrol;
            _serviceBildirimSistemi = serviceBildirimSistemi;
            _serviceViewBildirim = serviceViewBildirim;
            _serviceViewBirim = serviceViewBirim;
            _serviceTarihce = serviceTarihce;
            _serviceRiskAzaltmaPlani = serviceRiskAzaltmaPlani;
            _serviceYetki = serviceYetki;
            _serviceCTE = serviceCTE;
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
                var kayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kod, "Koordinatorluk,Birim,Surec,AltSurec,Amac,Hedef,AnahtarRiskGostergesi,AnahtarRiskGostergesi.Donemler,RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi,RiskSahibi,RiskYonetimi,RiskYonetimi.Kontroller,RiskYonetimi.RiskAzaltmaPlani");

                if (kayit != null && kayit.RiskYonetimi != null)
                {
                    kayit.RiskYonetimi.Tarihce = await _serviceTarihce.SonAciklamaGetirAsync(kullanan, kayit.RiskYonetimi.Kod);

                    if (kayit.RiskYonetimi.Kontroller != null)
                    {
                        for (int i = 0; i < kayit.RiskYonetimi.Kontroller.Count; i++)
                        {
                            if (kayit.RiskYonetimi.Kontroller[i].Durum == (int)ENUMDurum.Pasif)
                            {
                                kayit.RiskYonetimi.Kontroller.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
                }
                else if (kayit != null)
                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
            }
            catch (System.Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }

            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn kod bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskNoGetirAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.Kod == kod, "");

                if (kayit != null)
                    return new Sonuc(ENUMIslemDurum.Basarili, "", kayit.Kod);
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
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan)
        {
            var kayitlar = await _unitOfWorkRiskEvreni.ListeleAsync(null, o => o.Kod, k => k.RiskYonetimi);
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
            bool yonetici = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI", kullanan);

            List<string> belgeNolar = new List<string>();
            if (onay && !yonetici)
            {
                BildirimSistemi kriter = new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimi };

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
            var selectData = await _unitOfWorkRiskEvreni.SorguHazirlaAsync(null, "Koordinatorluk, Birim, Amac, Hedef, Surec, RiskKategoriler.RiskKategori, IlIrtibatOfisler.IlIrtibatOfisi, RiskYonetimi, RiskYonetimi.Kontroller, RiskSahibi");

            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.JenerikRisk == 0);

            //Uzman, Birim Amiri/Ünite Sorumlusu ve Koordinatörler Risk Kaydý ekranýnda kendi koordinatörlüklerindeki tüm riskleri görebilsin.

            //****************************************************************************************
            //Kullanici yetkisine göre koþul
            //****************************************************************************************
            var predicate = PredicateBuilder.True<RiskEvreni>();

            string[] birimKosul = await OrtakService.ListeleBirimKosulAsync(kullanan, _serviceCTE, "RISKYONETIMI", "", "");
            //Koordinatörlük
            var koordinatorlukKosul = birimKosul[0].Split(",");//Genel koord birden fazla koordinatörlüðe sahip olduðu için
            if (!string.IsNullOrWhiteSpace(koordinatorlukKosul[0]))
                predicate = predicate.And(a => koordinatorlukKosul.ToArray().Contains(a.KoordinatorlukKod));
            else
                predicate = predicate.And(a => a.KoordinatorlukKod != "");//Tüm koordinatörlükler


            if (Arac.YetkisiVarmi("ICDENETIMUZMANI,UZMAN,BIRIMAMIRI,ILKOORDINATOR,MERKEZKOORDINATOR,ICDENETIMKOORDINATOR,BIRIMAMIRI,GENELKOORDINATOR", kullanan))
                predicate = predicate.Or(a => a.RiskSahibiKod == kullanan.PersonelKod);
            else
            {
                //Birim
                if (!string.IsNullOrWhiteSpace(birimKosul[1]))
                    predicate = predicate.And(a => a.BirimKod == birimKosul[1]);
            }

            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, predicate);
            //****************************************************************************************


            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                var aramaObj = Arac.DataTablesAramaNesne<RiskEvreni>(new RiskEvreni(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.AmacKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AmacKod == aramaObj.AmacKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.HedefKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.HedefKod == aramaObj.HedefKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.SurecKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.SurecKod == aramaObj.SurecKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguRiskKategoriKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskKategoriler.Any(rk => rk.RiskKategoriKod == aramaObj.SorguRiskKategoriKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguIlIrtibatOfisiKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.IlIrtibatOfisler.Any(rk => rk.IlIrtibatOfisiKod == aramaObj.SorguIlIrtibatOfisiKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskNo))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskNo.Contains(aramaObj.RiskNo));
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskAdi))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskAdi.Contains(aramaObj.RiskAdi));
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskTanimi))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskTanimi.Contains(aramaObj.RiskTanimi));
                if (aramaObj.KontrolEdildi == 1)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.KontrolEdildi == aramaObj.KontrolEdildi);
                else if (aramaObj.KontrolEdildi == 2)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.KontrolEdildi == 0);

                if (onay)
                {
                    if (!yonetici)
                        selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => belgeNolar.ToArray().Contains(a.RiskYonetimi.Kod));
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.OnayaGonderdi);
                }
                else if (aramaObj.Durum > 0)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.Aktif // [r0].[Durum] IN (1, 2, 98, 10) // 
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.GeriGonderildi
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.Reddedildi
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.OnayaGonderdi
                                                                    || a.RiskYonetimi.Durum == null);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dataTablesParam.searchValue))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskNo.Contains(dataTablesParam.searchValue)
                                                                                     || a.RiskTanimi.Contains(dataTablesParam.searchValue)
                                                                                     || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                                     || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue));

                if (onay)
                {
                    if (!yonetici)
                        selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => belgeNolar.ToArray().Contains(a.RiskYonetimi.Kod));
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.OnayaGonderdi);
                }
                else
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.Aktif
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.GeriGonderildi
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.Reddedildi
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli
                                                                    || a.RiskYonetimi.Durum == (int)ENUMDurum.OnayaGonderdi
                                                                    || a.RiskYonetimi.Durum == null);
            }

            if (dataTablesParam.sortColumn == "RiskSahibi.AdiSoyadi")
                dataTablesParam.sortColumn = "RiskSahibi.Adi";

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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, RiskYonetimi gelenNesne)
        {
            RiskYonetimi islemYapilan = new RiskYonetimi();

            string hata = "";

            if (gelenNesne.RiskEvreniKod == "")
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiKodAlaniBos"] + "</li>";
            if (gelenNesne.Olasilik == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiOlasilikAlaniBos"] + "</li>";
            if (gelenNesne.Etki == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiEtkiAlaniBos"] + "</li>";
            if (gelenNesne.RiskeVerilecekCevap > 0 && gelenNesne.RiskeVerilecekCevap != EnumRiskYonetimiRiskeVerilecekCevap.Azalt && string.IsNullOrWhiteSpace(gelenNesne.Aciklama))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiAciklamaAlaniBos"] + "</li>";

            if (!string.IsNullOrEmpty(gelenNesne.RiskEvreniKod))
            {
                var eskiKayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod);

                if (eskiKayit.RiskSahibiKod != kullanan.PersonelKod && kullanan.AktifRolKod != "RISKSEKRETARYASI")
                    hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";
            }

            if (gelenNesne.Kontroller != null)
            {
                double toplamOnemDuzeyi = 0;
                foreach (var item in gelenNesne.Kontroller)
                {
                    toplamOnemDuzeyi += item.OnemDuzeyi;
                    if (item.OnemDuzeyi <= 0)
                        hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiOnemDuzeyiSifir"] + "</li>";
                }

                if (toplamOnemDuzeyi != 100)
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiOnemDuzeyiHatali"] + "</li>";
            }

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                //Risk Azaltma Planý verilerini silmek için RiskAzaltmaPlani bilgileri gönderiliyor. Daha sonra Tarhiçe karþýlaþtýrmasý için null deðeri veriliyor.
                var riskAzaltmaPlani = gelenNesne.RiskAzaltmaPlani;
                gelenNesne.RiskAzaltmaPlani = null;

                Tarihce tarihce = new Tarihce();
                gelenNesne.Durum = (int)ENUMDurum.Aktif;
                gelenNesne.KontrolEtkinlikAgirligi = gelenNesne.KontrolEtkinlikAgirligiGetir;
                gelenNesne.ArtikRiskSeviyesi = gelenNesne.ArtikRiskSeviyesiGetir;

                //sureckaydet iþleminde altsurecler dolu olursa hata veriyor EF den dolayý
                List<RiskYonetimiKontrol> kontroller = gelenNesne.Kontroller;
                gelenNesne.Kontroller = null;

                bool onayli = false;
                var bildirimDurum = 0;
                var bildirimAciklama = "";
                var azalmaPlaniSil = false;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();

                    tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                    tarihce.Durum = gelenNesne.Durum.Value;

                    islemYapilan = await _unitOfWorkAnahtar.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                    //Kayýtlý bir bilgi ancak sahibi tarafýndan deðiþtirilebilir
                    var kontrolKayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod);
                    if (kontrolKayit.RiskSahibiKod != kullanan.PersonelKod && (eskiKayit.Durum == (int)ENUMDurum.Onayli && kullanan.AktifRolKod != "RISKSEKRETARYASI"))
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";

                    //Onaya gönderilen kayýt deðiþtirilemez
                    if (eskiKayit.Durum == (int)ENUMDurum.OnayaGonderdi)
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";
                    else if (eskiKayit.Durum == (int)ENUMDurum.Reddedildi)         //Ýptal edilen kayýt deðiþtirilemez
                        hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";

                    if (hata != "")
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);

                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    {
                        if (gelenNesne.RiskeVerilecekCevap != EnumRiskYonetimiRiskeVerilecekCevap.Azalt)
                            azalmaPlaniSil = true;
                    }

                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    {
                        //Yapýsal risk seviyesi Yüksek/Çok Yüksek risk seviyesinden Çok Düþük risk seviyesine çekilen riskler (veya tam tersi) için ilgili merkez/il koordinatörüne bildirim düþer. (Detayý açýklamada)
                        if (gelenNesne.YapisalRiskPuani <= 5 && eskiKayit.YapisalRiskPuani >= 15) //Yüksek/Çok Yüksek > Çok Düþük 
                        {
                            bildirimDurum = (int)ENUMBildirimSistemiDurum.YapisalRiskSeviyesiDustu;
                            bildirimAciklama = "{RiskNo} numaralý riskin yapýsal risk seviyesi " + (eskiKayit.YapisalRiskPuani >= 20 ? "çok " : "") + "yüksekten çok düþüðe çekilmiþtir.";
                        }
                        else if (gelenNesne.YapisalRiskPuani >= 15 && eskiKayit.YapisalRiskPuani <= 5) //Çok Düþük > Yüksek/Çok Yüksek
                        {
                            bildirimDurum = (int)ENUMBildirimSistemiDurum.YapisalRiskSeviyesiYukseldi;
                            bildirimAciklama = "{RiskNo} numaralý riskin yapýsal risk seviyesi çok düþükten " + (eskiKayit.YapisalRiskPuani >= 20 ? "çok " : "") + "yükseðe çýkarýlmýþtýr.";
                        }

                        //Artýk risk seviyesi Yüksek/Çok Yüksek risk seviyesinden Çok Düþük risk seviyesine çekilen riskler (veya tam tersi) için ilgili merkez/il koordinatörüne bildirim düþer. (Detayý açýklamada)
                        else if (gelenNesne.ArtikRiskSeviyesi <= 1 && eskiKayit.ArtikRiskSeviyesi >= 4) //Yüksek/Çok Yüksek > Çok Düþük 
                        {
                            bildirimDurum = (int)ENUMBildirimSistemiDurum.ArtikRiskSeviyesiDustu;
                            bildirimAciklama = "{RiskNo} numaralý riskin artýk risk seviyesi " + (eskiKayit.ArtikRiskSeviyesi >= 5 ? "çok " : "") + "yüksekten çok düþüðe çekilmiþtir.";
                        }
                        else if (gelenNesne.ArtikRiskSeviyesi >= 4 && eskiKayit.ArtikRiskSeviyesi <= 1) //Çok Düþük > Yüksek/Çok Yüksek
                        {
                            bildirimDurum = (int)ENUMBildirimSistemiDurum.ArtikRiskSeviyesiYukseldi;
                            bildirimAciklama = "{RiskNo} numaralý riskin artýk risk seviyesi çok düþükten " + (eskiKayit.ArtikRiskSeviyesi >= 5 ? "çok " : "") + "yükseðe çýkarýlmýþtýr.";
                        }
                        else if (gelenNesne.YapisalRiskPuani != eskiKayit.YapisalRiskPuani) //YapisalRiskPuani 
                        {
                            bildirimDurum = (int)ENUMBildirimSistemiDurum.YapisalRiskPuaniDegisiti;
                            bildirimAciklama = "{RiskNo} numaralý riskin yapýsal risk puaný" + eskiKayit.YapisalRiskPuani + " iken " + gelenNesne.YapisalRiskPuani + " olarak deðiþtirildi.";
                        }
                        else if (gelenNesne.ArtikRiskPuani != eskiKayit.ArtikRiskPuani) //ArtikRiskPuani 
                        {
                            bildirimDurum = (int)ENUMBildirimSistemiDurum.ArtikRiskPuaniDegisiti;
                            bildirimAciklama = "{RiskNo} numaralý riskin artýk risk puaný" + eskiKayit.ArtikRiskPuani + " iken " + gelenNesne.ArtikRiskPuani + " olarak deðiþtirildi.";
                        }
                        //----------------------------------------------------------------------------------------------------
                    }

                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    {
                        onayli = true;
                        eskiKayit.ArtikRiskPuaniOncekiDeger = eskiKayit.ArtikRiskPuani;
                        eskiKayit.ArtikRiskSeviyesiOncekiDeger = eskiKayit.ArtikRiskSeviyesi;
                        eskiKayit.GuncellemeTarihi = DateTime.Now;
                    }
                    else
                        eskiKayit.Durum = gelenNesne.Durum;

                    eskiKayit.Olasilik = gelenNesne.Olasilik;
                    eskiKayit.Etki = gelenNesne.Etki;
                    eskiKayit.RiskeVerilecekCevap = gelenNesne.RiskeVerilecekCevap;
                    eskiKayit.Aciklama = gelenNesne.Aciklama;
                    eskiKayit.KontrolEtkinlikAgirligi = gelenNesne.KontrolEtkinlikAgirligi;
                    eskiKayit.ArtikRiskSeviyesi = gelenNesne.ArtikRiskSeviyesi;
                    eskiKayit.KontrolEdildi = gelenNesne.KontrolEdildi;

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    tarihce.Durum = (int)gelenNesne.Durum;

                    islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);
                }

                //Mevcut kontrol bulunmamaktadýr. Yapýsý eklendi. O nedenle önce kayýtlar siliniyor. Sonra varsa ekleniyor. Zorunluluk kontrolü kaldýrýldý. HÖ - 29.04.2024
                await _serviceKontrol.SilAsync(kullanan, gelenNesne.Kod);

                if (kontroller != null)
                {
                    foreach (var item in kontroller)
                    {
                        RiskYonetimiKontrol giden = new RiskYonetimiKontrol();
                        item.RiskYonetimiKod = gelenNesne.Kod;
                        item.Durum = (int)ENUMDurum.Aktif;

                        await _serviceKontrol.KaydetAsync(kullanan, item);
                    }
                }


                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.RiskYonetimi;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _unitOfWorkAnahtar.KaydetAsync();


                if (!onayli || bildirimDurum > 0)
                {
                    //Bildirim sisteminden kaydý sil
                    var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimi, BelgeKod = gelenNesne.Kod, });

                    if (!string.IsNullOrWhiteSpace(riskAzaltmaPlani?.Kod))
                    {
                        var sonucRiskAzaltmaPlani = await _serviceRiskAzaltmaPlani.SilAsync(kullanan, riskAzaltmaPlani);
                    }

                    if (bildirimDurum > 0)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod, "Koordinatorluk");

                        var bs = new BildirimSistemi
                        {
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimi,
                            BelgeKod = gelenNesne.Kod,
                            KoordinatorlukKod = riskEvreni.KoordinatorlukKod,
                            BirimKod = riskEvreni.BirimKod,
                            IslemYapanKod = kullanan.PersonelKod,
                            IslemTarihi = DateTime.Now,
                        };

                        bs.Durum = bildirimDurum;
                        bs.Aciklama = bildirimAciklama.Replace("{RiskNo}", riskEvreni.RiskNo);

                        var kTur = (EnumKoordinatorlukTur)riskEvreni.Koordinatorluk.Tur;

                        if (kTur == EnumKoordinatorlukTur.Il)
                            bs.OnaylayacakYetki = "ILKOORDINATOR";
                        else if (kTur == EnumKoordinatorlukTur.Hukuk)
                            bs.OnaylayacakYetki = "HUKUKKOORDINATORU";
                        else
                            bs.OnaylayacakYetki = "MERKEZKOORDINATOR";


                        var sonucBildirimSistemi2 = await _serviceBildirimSistemi.KaydetAsync(kullanan, bs);

                        if (!sonucBildirimSistemi2.IslemSonuc)
                            return new Sonuc(ENUMIslemDurum.Hata, sonucBildirimSistemi2.Mesaj);
                    }
                }
                else if (azalmaPlaniSil)
                {
                    if (!string.IsNullOrWhiteSpace(riskAzaltmaPlani?.Kod))
                    {
                        var sonucRiskAzaltmaPlani = await _serviceRiskAzaltmaPlani.SilAsync(kullanan, riskAzaltmaPlani);
                    }
                }

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydýn durumunu deðiþtiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, RiskYonetimi gelenNesne)
        {
            string hata = "";

            bool sistemTarafindanOnaylandi = false;

            bool riskEvreniKodIleOku = false;

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
            {
                if (!string.IsNullOrEmpty(gelenNesne.RiskEvreniKod))
                    riskEvreniKodIleOku = true;
                else
                    hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";
            }

            if (gelenNesne.Kontroller != null)
            {
                double toplamOnemDuzeyi = 0;
                foreach (var item in gelenNesne.Kontroller)
                {
                    toplamOnemDuzeyi += item.OnemDuzeyi;
                    if (item.OnemDuzeyi <= 0)
                        hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiOnemDuzeyiSifir"] + "</li>";
                }

                if (toplamOnemDuzeyi != 100)
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiOnemDuzeyiHatali"] + "</li>";
            }

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();

                gelenNesne.KontrolEtkinlikAgirligi = gelenNesne.KontrolEtkinlikAgirligiGetir;
                gelenNesne.ArtikRiskSeviyesi = gelenNesne.ArtikRiskSeviyesiGetir;

                List<RiskYonetimiKontrol> kontroller = gelenNesne.Kontroller;
                gelenNesne.Kontroller = null;

                var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                if (riskEvreniKodIleOku)
                {
                    eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.RiskEvreniKod == gelenNesne.RiskEvreniKod);

                    var sonucRiskAzaltmaPlani = await _serviceRiskAzaltmaPlani.DurumDegistirAsync(kullanan, new RiskAzaltmaPlani { RiskYonetimiKod = gelenNesne.RiskEvreniKod, Durum = (int)ENUMDurum.Aktif });
                }
                //tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                //Durum deðiþikliðine uygun mu?
                hata = Arac.DurumDegisikligiUygunMu(kullanan, _sharedResource, eskiKayit, gelenNesne);

                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                {
                    eskiKayit.KayitTarihi = DateTime.Now.Date;
                    if (eskiKayit.RiskeVerilecekCevap == 0 || eskiKayit.ArtikRiskPuani == 0 || eskiKayit.YapisalRiskPuani == 0)
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiRiskeVerilecekCevapBos"] + "</li>";
                    else if (eskiKayit.ArtikRiskPuani == 0 || eskiKayit.YapisalRiskPuani == 0)
                        hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";
                }

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                var oncekiDurum = eskiKayit.Durum;
                RiskEvreni riskEvreni = null;

                tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                if (gelenNesne.Durum == (int)ENUMDurum.Pasif)
                {
                    eskiKayit.Durum = gelenNesne.Durum;

                    var islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);

                    //Tarihçe Baþlangýç
                    tarihce.IlgiKod = gelenNesne.Kod;
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskYonetimi;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = gelenNesne.Durum.Value;
                    if (gelenNesne.Tarihce != null)
                        tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;

                    await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiþ

                    var sonuc = await _unitOfWorkAnahtar.KaydetAsync();


                    // Bildirim sisteminden kaydý sil
                    var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimi, BelgeKod = gelenNesne.Kod, });

                    var riskAzaltmaPlani = gelenNesne.RiskAzaltmaPlani;

                    if (!string.IsNullOrWhiteSpace(riskAzaltmaPlani?.Kod))
                    {
                        var sonucRiskAzaltmaPlani = await _serviceRiskAzaltmaPlani.SilAsync(kullanan, riskAzaltmaPlani);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(gelenNesne.RiskEvreniKod) && (gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Onayli))
                    {
                        //eskiKayit.Durum = gelenNesne.Durum;
                        eskiKayit.Olasilik = gelenNesne.Olasilik;
                        eskiKayit.Etki = gelenNesne.Etki;
                        eskiKayit.RiskeVerilecekCevap = gelenNesne.RiskeVerilecekCevap;
                        eskiKayit.Aciklama = gelenNesne.Aciklama;
                        eskiKayit.KontrolEtkinlikAgirligi = gelenNesne.KontrolEtkinlikAgirligi;
                        eskiKayit.ArtikRiskSeviyesi = gelenNesne.ArtikRiskSeviyesi;

                        tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    }
                    else
                        tarihce.EskiDeger = "";

                    eskiKayit.Durum = gelenNesne.Durum;

                    //BildirimSistemi Baþlangýç
                    if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi || gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Reddedildi)
                    {
                        riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == eskiKayit.RiskEvreniKod, "Koordinatorluk");

                        var bs = new BildirimSistemi
                        {
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimi,
                            BelgeKod = eskiKayit.Kod,
                            KoordinatorlukKod = riskEvreni.KoordinatorlukKod,
                            BirimKod = riskEvreni.BirimKod,
                            IslemYapanKod = kullanan.PersonelKod,
                            IslemTarihi = DateTime.Now,
                        };

                        //Transfer Et veya Reddet seçeneklerini seçili ve RISKSEKRETARYASI onaylarsa 
                        var ustYoneticiyeOnayaGonder = kullanan.AktifRolKod == "RISKSEKRETARYASI" && gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.ArtikRiskSeviyesi > 2 && (eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.TransferEt || eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.Reddet);

                        if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi || ustYoneticiyeOnayaGonder)
                        {
                            //bs.OnaylayacakYetki = Arac.UstYetkiVer(kullanan, (EnumKoordinatorlukTur)riskEvreni.Koordinatorluk.Tur);
                            bs.Durum = (int)ENUMDurum.OnayaGonderdi;

                            //Transfer Et veya Reddet seçeneklerini seçerse, kayýt bir üst yöneticinin onayýndan önce risk sekretaryasý onayýna gitsin.
                            if (eskiKayit.ArtikRiskSeviyesi > 2 && (eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.TransferEt || eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.Reddet))
                            {
                                if (!ustYoneticiyeOnayaGonder)
                                    bs.OnaylayacakYetki = "RISKSEKRETARYASI";
                                else
                                {
                                    eskiKayit.Durum = (int)ENUMDurum.OnayaGonderdi;
                                    var eskiIslemYapanRol = "";

                                    //RISKSEKRETARYASI öncesinde iþlem yapan rol bilgisini bulmak için tarihçe bilgisine bakmamýz gerekiyor. 

                                    var ilgiliTarihce = await _serviceTarihce.ListeleAsync(eskiKayit.Kod);
                                    if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                                    {
                                        foreach (Tarihce t in ilgiliTarihce.Liste)
                                        {
                                            if (t.Durum == (int)ENUMDurum.OnayaGonderdi && t.IlgiliRol == "RISKSEKRETARYASI")
                                                eskiIslemYapanRol = t.IslemYapanRol;
                                        }
                                    }

                                    if (!string.IsNullOrWhiteSpace(eskiIslemYapanRol))
                                    {
                                        var sanalKullanici = new KullaniciDto();
                                        sanalKullanici.AktifRolKod = eskiIslemYapanRol;
                                        sanalKullanici.Roller.Add(new KullaniciRolDto() { Adi = eskiIslemYapanRol });

                                        if (!string.IsNullOrWhiteSpace(riskEvreni.BirimKod))
                                            bs.OnaylayacakYetki = Arac.UstYetkiVer(sanalKullanici, (EnumKoordinatorlukTur)riskEvreni.Koordinatorluk.Tur);
                                        else
                                        {
                                            //Birimi olmayan koordinatörlük ile iþlem yapýldýðýnda
                                            bs.OnaylayacakYetki = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(sanalKullanici, riskEvreni.KoordinatorlukKod, riskEvreni.Koordinatorluk.Tur, true);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(riskEvreni.BirimKod))
                                    bs.OnaylayacakYetki = Arac.UstYetkiVer(kullanan, (EnumKoordinatorlukTur)riskEvreni.Koordinatorluk.Tur);
                                else
                                {
                                    //Birimi olmayan koordinatörlük ile iþlem yapýldýðýnda
                                    bs.OnaylayacakYetki = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(kullanan, riskEvreni.KoordinatorlukKod, riskEvreni.Koordinatorluk.Tur, true);
                                }
                            }

                            if (string.IsNullOrWhiteSpace(bs.OnaylayacakYetki))
                                hata += "<li>" + _sharedResource["Kontrol.DurumDegistir.OnaylayacakYetkiBulunamadi"] + "</li>";
                            else if (bs.OnaylayacakYetki == kullanan.AktifRolKod || bs.OnaylayacakYetki == "BASKAN*")
                            {
                                //Eðer üst yetki ayný kiþi ise onaya gönderilmeden onaylansýn
                                bs.Durum = (int)ENUMDurum.Onayli;
                                gelenNesne.Durum = (int)ENUMDurum.Onayli;
                                eskiKayit.Durum = (int)ENUMDurum.Onayli;
                                sistemTarafindanOnaylandi = true;
                            }

                        }
                        else if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                        {
                            bs.OnaylayacakYetki = kullanan.AktifRolKod;
                            bs.Durum = (int)ENUMDurum.Onayli;

                            //Risk sahibinin koordinatorlük veya birim bilgisi farklý ise birim amirine onaya gönder
                            if (eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.TransferEt ||
                                eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.KabulEt ||
                                eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.Reddet)
                            {

                                //Risk sahibinin koordinatorlük veya birim bilgisi farklý ise birim amirine onaya gönder
                                var kriter = new ViewYetki();
                                kriter.PersonelKod = riskEvreni.RiskSahibiKod;
                                kriter.KoordinatorlukKod = riskEvreni.KoordinatorlukKod;
                                kriter.BirimKod = riskEvreni.BirimKod;

                                var sonucYetki = await _serviceYetki.ListeleAsync(kullanan, kriter);

                                if (sonucYetki.IslemSonuc && sonucYetki.Liste.Count <= 0)
                                {
                                    var onaylayacakYetki = "";
                                    if (!string.IsNullOrWhiteSpace(riskEvreni.BirimKod))
                                        onaylayacakYetki = await _serviceBildirimSistemi.OnaylayacakYetkiVerAsync(kullanan, bs);
                                    else
                                    {
                                        //Birimi olmayan koordinatörlük ile iþlem yapýldýðýnda
                                        onaylayacakYetki = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(kullanan, riskEvreni.KoordinatorlukKod, riskEvreni.Koordinatorluk.Tur, true);
                                    }

                                    //var onaylayacakYetki = await _serviceBildirimSistemi.OnaylayacakYetkiVerAsync(kullanan, bs);
                                    if (!string.IsNullOrWhiteSpace(onaylayacakYetki))
                                    {
                                        eskiKayit.Durum = (int)ENUMDurum.OnayaGonderdi;
                                        bs.OnaylayacakUstYetki = onaylayacakYetki;

                                        //Eðer üst yetki ayný kiþi ise onaya gönderilmeden onaylansýn
                                        if (onaylayacakYetki == kullanan.AktifRolKod || onaylayacakYetki == "BASKAN*")
                                        {
                                            bs.OnaylayacakUstYetki = "";
                                            sistemTarafindanOnaylandi = true;
                                        }
                                    }
                                }
                            }
                        }
                        else if (gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                        {
                            //Geri gönderilen risk deðerlendirmesi ilk hazýrlayana düþecek. Birim amiri onaylamýþsa bilgilendirme mail gönderilecek.
                            eskiKayit.Durum = (int)ENUMDurum.GeriGonderildi; //Geri gönderildi
                            bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kiþi yok
                            bs.OnaylayacakYetki = "-";

                            //var geriGonderilecekYetki = await _serviceBildirimSistemi.GeriGonderilecekYetkiVerAsync(kullanan, bs);
                            //if (!string.IsNullOrWhiteSpace(geriGonderilecekYetki))
                            //{
                            //    eskiKayit.Durum = (int)ENUMDurum.GeriGonderildi;
                            //    bs.Durum = (int)ENUMDurum.GeriGonderildi;
                            //    bs.OnaylayacakYetki = geriGonderilecekYetki;
                            //}
                            //else
                            //{
                            //    bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kiþi yok
                            //    bs.OnaylayacakYetki = "-";
                            //}
                        }


                        if (hata != "")
                            return new Sonuc(ENUMIslemDurum.Uyari, hata);

                        var sonucBildirimSistemi = await _serviceBildirimSistemi.KaydetAsync(kullanan, bs);

                        if (!sonucBildirimSistemi.IslemSonuc)
                            return new Sonuc(ENUMIslemDurum.Hata, sonucBildirimSistemi.Mesaj);

                        if (bs.OnaylayacakYetki != kullanan.AktifRolKod && bs.OnaylayacakYetki != "-")
                            tarihce.IlgiliRol = bs.OnaylayacakYetki;
                        else if (bs.OnaylayacakUstYetki != kullanan.AktifRolKod)
                            tarihce.IlgiliRol = bs.OnaylayacakUstYetki;
                    }
                    //BildirimSistemi Bitiþ

                    RiskYonetimi islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);

                    //Tarihçe Baþlangýç
                    tarihce.IlgiKod = gelenNesne.Kod;
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskYonetimi;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = gelenNesne.Durum.Value;
                    if (gelenNesne.Tarihce != null)
                        tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;

                    if (tarihce.Durum == (int)ENUMDurum.Onayli && sistemTarafindanOnaylandi)
                        tarihce.Aciklama = "Sistem";


                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                    {
                        if (kontroller != null)
                        {
                            var sonucKontrol = await _serviceKontrol.SilAsync(kullanan, gelenNesne.Kod);

                            if (sonucKontrol.IslemSonuc)
                            {
                                foreach (var item in kontroller)
                                {
                                    RiskYonetimiKontrol giden = new RiskYonetimiKontrol();
                                    item.RiskYonetimiKod = gelenNesne.Kod;
                                    item.Durum = (int)ENUMDurum.Aktif;

                                    await _serviceKontrol.KaydetAsync(kullanan, item);
                                }
                            }
                        }
                    }


                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                    {
                        eskiKayit.GirisTarihi = DateTime.Now;
                    }

                    var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiþ

                    //Mail Gönder: Risk Azaltma Planý oluþturmak için Risk Azaltma Planý ekranýna gidiniz. 
                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli && eskiKayit.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.Azalt)
                    {
                        var formBildirim = new BildirimSistemi()
                        {
                            Islem = EnumBildirimSistemiIslem.AzaltmaPlaniOlustur,
                            BelgeKod = gelenNesne.Kod,
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimi,
                            MailGonderilecekKisi = riskEvreni.RiskSahibiKod, //Risk azaltma planý oluþturma maili riskin sahibine gitmeli
                        };

                        Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                    }
                    //Mail Gönder Bitiþ

                    await _unitOfWorkAnahtar.KaydetAsync();


                    if (kontroller != null)
                    {
                        await _serviceKontrol.SilAsync(kullanan, gelenNesne.Kod);

                        foreach (var item in kontroller)
                        {
                            RiskYonetimiKontrol giden = new RiskYonetimiKontrol();
                            item.RiskYonetimiKod = gelenNesne.Kod;
                            item.Durum = (int)ENUMDurum.Aktif;

                            await _serviceKontrol.KaydetAsync(kullanan, item);
                        }
                    }

                    //Risk Sahibini Deðiþtir #Baþlangýç
                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli && !string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod))
                    {
                        var eskiKayitRiskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod, "");

                        if (eskiKayitRiskEvreni != null && eskiKayitRiskEvreni.RiskYonetimi != null && eskiKayitRiskEvreni.RiskSahibiKod != gelenNesne.RiskSahibiKod)
                        {
                            tarihce = new Tarihce();

                            tarihce.EskiDeger = Arac.JSONSerialize(eskiKayitRiskEvreni);

                            eskiKayitRiskEvreni.RiskSahibiKod = gelenNesne.RiskSahibiKod;

                            await _unitOfWorkRiskEvreni.GuncelleAsync(eskiKayitRiskEvreni);

                            //Tarihçe Baþlangýç
                            tarihce.IlgiKod = gelenNesne.Kod;
                            tarihce.IlgiTur = EnumTarihceIslemTur.RiskYonetimi;
                            tarihce.IslemYapanKod = kullanan.PersonelKod;
                            tarihce.Durum = (int)ENUMDurum.RiskSahibiDegisti;

                            await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                            //Tarihçe Bitiþ

                            await _unitOfWorkRiskEvreni.KaydetAsync();
                        }
                    }
                    //Risk Sahibini Deðiþtir #Bitiþ

                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli && !string.IsNullOrEmpty(tarihce.DegisenDeger) && riskEvreni != null)
                    {
                        //Mail Gönder: Onaycý kiþi kayýtta deðiþiklik yapýp onaylarsa, deðiþiklik risk sahibine mail olarak bildirilir. 
                        if (!string.IsNullOrWhiteSpace(riskEvreni.RiskSahibiKod))
                        {
                            var formBildirim = new BildirimSistemi()
                            {
                                Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                                BelgeKod = gelenNesne.Kod,
                                BelgeTipi = (int)EnumTarihceIslemTur.RisklerinDegerlendirmesiOnaydaDegisiklik,
                                MailGonderilecekKisi = riskEvreni.RiskSahibiKod
                            };

                            Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                        }
                        //Mail Gönder Bitiþ
                    }
                }


                //Mail Gönder: Tüm ekranlar için kaydýn durum bilgisi deðiþtiðinde bilgilendirme maili gidebilir mi? (reddedildi, onaya gönderildi, geri gönderildi vs.). 
                if (oncekiDurum != eskiKayit.Durum)
                {
                    if (riskEvreni == null)
                        riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == eskiKayit.RiskEvreniKod, "Koordinatorluk");

                    var formBildirim = new BildirimSistemi()
                    {
                        Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                        BelgeKod = gelenNesne.Kod,
                        BelgeTipi = (int)EnumTarihceIslemTur.RisklerinDegerlendirmesiDurumDegisti,
                        MailGonderilecekKisi = riskEvreni.RiskSahibiKod,
                        Aciklama = Arac.DurumAdGetir(oncekiDurum.Value) + " >> " + Arac.DurumAdGetir(eskiKayit.Durum.Value)
                    };

                    Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                }
                //Mail Gönder Bitiþ

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilerden güncel deðerleri deðiþen alanlarý döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// string türünde deðiþen alanlarý döndürür
        /// </returns>
        public async Task<string> DegisenAlanlariGetirAsync(KullaniciDto kullanan, RiskYonetimi gelenNesne)
        {
            var degisenAlanlar = "";
            var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

            gelenNesne.KontrolEtkinlikAgirligi = gelenNesne.KontrolEtkinlikAgirligiGetir;
            gelenNesne.ArtikRiskSeviyesi = gelenNesne.ArtikRiskSeviyesiGetir;

            if (eskiKayit.Olasilik != gelenNesne.Olasilik)
                degisenAlanlar += "<li>" + _sharedResource["RiskYonetimi.Alan.Olasilik"] + " " + _sharedResource["RiskYonetimi.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.Etki != gelenNesne.Etki)
                degisenAlanlar += "<li>" + _sharedResource["RiskYonetimi.Alan.Etki"] + " " + _sharedResource["RiskYonetimi.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.RiskeVerilecekCevap != gelenNesne.RiskeVerilecekCevap)
                degisenAlanlar += "<li>" + _sharedResource["RiskYonetimi.Alan.RiskeVerilecekCevap"] + " " + _sharedResource["RiskYonetimi.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.Aciklama != gelenNesne.Aciklama)
                degisenAlanlar += "<li>" + _sharedResource["RiskYonetimi.Alan.Aciklama"] + " " + _sharedResource["RiskYonetimi.AlanindaGuncellemeYapildi"] + " </li>";

            return degisenAlanlar;
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydý silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="riskYonetimi"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, RiskYonetimi riskYonetimi)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(riskYonetimi?.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                if (!string.IsNullOrWhiteSpace(riskYonetimi?.RiskAzaltmaPlani?.Kod))
                {
                    var sonucRiskAzaltmaPlani = await _serviceRiskAzaltmaPlani.SilAsync(kullanan, riskYonetimi.RiskAzaltmaPlani);
                }

                //Bildirim sisteminden kaydý sil
                var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimi, BelgeKod = riskYonetimi.Kod, });

                await _serviceKontrol.SilAsync(kullanan, riskYonetimi.Kod);
                await _unitOfWorkAnahtar.SilAsync(d => d.Kod == riskYonetimi.Kod);
                await _unitOfWorkAnahtar.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }
    }
}
