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
using System.Text.Json.Serialization;
using System;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// RiskEvreni iþlemlerinin yapýldýðý servis
    /// </summary>
    public class RiskEvreniService : IRiskEvreniService
    {
        /// <summary>
        /// IUnitOfWork<RiskEvreni> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskEvreni> _unitOfWork;
        /// <summary>
        /// IRiskEvreniRiskKategoriService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniRiskKategoriService _serviceRiskKategori;
        /// <summary>
        /// IRiskEvreniIlIrtibatOfisiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniIlIrtibatOfisiService _serviceIIlIrtibatOfisi;
        /// <summary>
        /// IRiskEvreniOlayRaporlamaService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskEvreniOlayRaporlamaService _serviceOlayRaporlama;
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
        /// IViewYetkiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewYetkiService _serviceYetki;
        /// <summary>
        /// ITarihceService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// IRiskYonetimiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskYonetimiService _serviceRiskYonetimi;
        private readonly IStringLocalizer<CustomResource> _sharedResource;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;


        /// <summary>
        /// <see cref="Risk.net.Services.Functions.RiskEvreniService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceRiskKategori"></param>
        /// <param name="serviceIIlIrtibatOfisi"></param>
        /// <param name="serviceOlayRaporlama"></param>
        /// <param name="serviceBildirimSistemi"></param>
        /// <param name="serviceViewBildirim"></param>
        /// <param name="serviceViewBirim"></param>
        /// <param name="serviceYetki"></param>
        /// <param name="serviceCTE"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="serviceRiskYonetimi"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskEvreniService(IUnitOfWork<RiskEvreni> unitOfWork,
            IRiskEvreniRiskKategoriService serviceRiskKategori,
            IRiskEvreniIlIrtibatOfisiService serviceIIlIrtibatOfisi,
            IRiskEvreniOlayRaporlamaService serviceOlayRaporlama,
            IBildirimSistemiService serviceBildirimSistemi,
            IViewBildirimSistemiService serviceViewBildirim,
            IViewBirimService serviceViewBirim,
            IViewYetkiService serviceYetki,
            ICTEKoordinatorlukService serviceCTE,
            ITarihceService serviceTarihce,
            IRiskYonetimiService serviceRiskYonetimi,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceRiskKategori = serviceRiskKategori;
            _serviceIIlIrtibatOfisi = serviceIIlIrtibatOfisi;
            _serviceOlayRaporlama = serviceOlayRaporlama;
            _serviceBildirimSistemi = serviceBildirimSistemi;
            _serviceViewBildirim = serviceViewBildirim;
            _serviceViewBirim = serviceViewBirim;
            _serviceYetki = serviceYetki;
            _serviceTarihce = serviceTarihce;
            _serviceCTE = serviceCTE;
            _sharedResource = sharedResource;
            _serviceRiskYonetimi = serviceRiskYonetimi;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi, Koordinatorluk,Birim,Amac,Amac.StratejikPlanDonem,Hedef,Surec,AltSurec,RiskYonetimi,RiskYonetimi.RiskAzaltmaPlani,IlIrtibatOfisi", c => c.RiskKategoriler, c => c.IlIrtibatOfisler, c => c.OlayRaporlar);

                if (kayit != null)
                {
                    kayit.Tarihce = await _serviceTarihce.SonAciklamaGetirAsync(kullanan, kod);

                    //(Koord ve Birim deðiþtirilirse Risk Sahibi alan boþaltýlsýn.) Risk Sahibi; onaylayan Birim Amiri olur. 
                    if (kullanan.AktifRolKod == "BIRIMAMIRI")
                    {
                        //Risk sahibinin koordinatorlük veya birim bilgisi farklý mý?
                        var kriter = new ViewYetki();
                        kriter.PersonelKod = kayit.RiskSahibiKod;
                        kriter.KoordinatorlukKod = kayit.KoordinatorlukKod;
                        kriter.BirimKod = kayit.BirimKod;
                        kriter.Rol = "BIRIMAMIRI";

                        var sonucYetki = await _serviceYetki.ListeleAsync(kullanan, kriter);

                        if (sonucYetki.IslemSonuc && sonucYetki.Liste.Count <= 0)
                        {
                            kayit.RiskSahibiKod = "";
                        }

                    }

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
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirHepsiAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "Koordinatorluk,Birim,Amac,Hedef,Surec,AltSurec,AnahtarRiskGostergesi,AnahtarRiskGostergesi.Donemler,RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi,OlayRaporlar,OlayRaporlar.OlayRaporlama,RiskYonetimi,RiskYonetimi.Kontroller,RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim, RiskYonetimi.RiskAzaltmaPlani.Riskler,RiskYonetimi.RiskAzaltmaPlani.Notlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler, RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Birim, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim,RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusu", c => c.RiskKategoriler, c => c.IlIrtibatOfisler);

                if (kayit != null)
                {

                    //Risk Azaltma Planý Riskler
                    if (kayit.RiskYonetimi != null && kayit.RiskYonetimi.RiskAzaltmaPlani != null)
                    {
                        var listRiskKod = new List<string>();
                        foreach (var risk in kayit.RiskYonetimi.RiskAzaltmaPlani.Riskler)
                            listRiskKod.Add(risk.RiskEvreniKod);

                        var selectData = await _unitOfWork.SorguHazirlaAsync(null, "");

                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => listRiskKod.ToArray().Contains(a.Kod));

                        var riskler = selectData.ToList();
                        if (riskler != null)
                            foreach (var item in riskler)
                            {
                                foreach (var risk in kayit.RiskYonetimi.RiskAzaltmaPlani.Riskler)
                                {
                                    if (risk.RiskEvreniKod == item.Kod)
                                        risk.RiskEvreniKod = item.RiskTanimi;
                                }
                            }
                    }
                    //-------------------------------------------------------

                    //Risk Azaltma Planý Notlar
                    if (kayit.RiskYonetimi != null && kayit.RiskYonetimi.RiskAzaltmaPlani != null)
                    {
                        var listRiskAzaltmaPlaniKod = new List<string>();
                        foreach (var risk in kayit.RiskYonetimi.RiskAzaltmaPlani.Notlar)
                            listRiskAzaltmaPlaniKod.Add(risk.RiskAzaltmaPlaniKod);

                        var selectData = await _unitOfWork.SorguHazirlaAsync(null, "RiskYonetimi,RiskYonetimi.RiskAzaltmaPlani");

                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => listRiskAzaltmaPlaniKod.ToArray().Contains(a.RiskYonetimi.RiskAzaltmaPlani.Kod));

                        var notlar = selectData.ToList();
                    }
                    //-------------------------------------------------------

                    //Risk Azaltma Ýliþlikili Planlar
                    if (kayit.RiskYonetimi != null && kayit.RiskYonetimi.RiskAzaltmaPlani != null)
                    {
                        var listRiskAzaltmaPlaniKod = new List<string>();
                        foreach (var risk in kayit.RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar)
                            listRiskAzaltmaPlaniKod.Add(risk.IliskiKod);

                        var selectData = await _unitOfWork.SorguHazirlaAsync(null, "RiskYonetimi,RiskYonetimi.RiskAzaltmaPlani");

                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => listRiskAzaltmaPlaniKod.ToArray().Contains(a.RiskYonetimi.RiskAzaltmaPlani.Kod));

                        var riskAzaltmaPlanlari = selectData.ToList();
                        if (riskAzaltmaPlanlari != null)
                            foreach (var item in riskAzaltmaPlanlari)
                            {
                                foreach (var iliski in kayit.RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar)
                                {
                                    if (iliski.IliskiKod == item.RiskYonetimi.RiskAzaltmaPlani.Kod)
                                        iliski.IliskiKod = item.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani;
                                }
                            }
                    }
                    //-------------------------------------------------------

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
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, RiskEvreni kriter)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi", a => a.Birim, a => a.Koordinatorluk);

            selectData = selectData.OrderBy(a => a.RiskNo);

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kriter.KoordinatorlukKod);
            if (!string.IsNullOrWhiteSpace(kriter.BirimKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kriter.BirimKod);
            if (!string.IsNullOrWhiteSpace(kriter.SurecKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.SurecKod == kriter.SurecKod);
            //if (!string.IsNullOrWhiteSpace(kriter.AltSurecKod))
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.AltSurecKod == kriter.AltSurecKod);
            if (kriter.Durum == (int)ENUMDurum.Onayli)
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
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam, bool onay, bool riskIzleme)
        {
            bool yonetici = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI", kullanan);

            List<string> belgeNolar = new List<string>();
            if (onay && !yonetici)
            {
                BildirimSistemi kriter = new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreni };

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
            //var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Amac,Amac.StratejikPlanDonem,RiskKategoriler,RiskKategoriler.RiskKategori,IlIrtibatOfisler,IlIrtibatOfisler.IlIrtibatOfisi,RiskSahibi", a => a.Birim, a => a.Koordinatorluk, a => a.Amac, a => a.Hedef, a => a.Surec, a => a.AltSurec);
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Amac,Amac.StratejikPlanDonem,RiskKategoriler,RiskKategoriler.RiskKategori,IlIrtibatOfisler,IlIrtibatOfisler.IlIrtibatOfisi,RiskSahibi,RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk,RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusu", a => a.Birim, a => a.Koordinatorluk, a => a.Amac, a => a.Hedef, a => a.Surec, a => a.AltSurec);

            //P2-234-(Risk Kaydý/Anahtar Risk Göstergesi/Risklerin Deðerlendirilmesi/Risklerin Yönetilmesi) Uzman tüm riskleri görebilir. 05.04.2023 HÖ
            /*
                //****************************************************************************************
                //Kullanici yetkisine göre koþul
                //****************************************************************************************
                var predicate = PredicateBuilder.True<RiskEvreni>();

                ////Birim koþulunu uygula
                //****************************************************************************************
                string[] birimKosul = await OrtakService.ListeleBirimKosulAsync(kullanan, _serviceCTE, "RISKYONETIMI", "", "");

                //Koordinatörlük
                var koordinatorlukKosul = birimKosul[0].Split(",");//Genel koord birden fazla koordinatörlüðe sahip olduðu için
                if (!string.IsNullOrWhiteSpace(koordinatorlukKosul[0]))
                    predicate = predicate.And(a => koordinatorlukKosul.ToArray().Contains(a.KoordinatorlukKod));
                else
                    predicate = predicate.And(a => a.KoordinatorlukKod != "");//Tüm koordinatörlükler

                //Birim
                if (!string.IsNullOrWhiteSpace(birimKosul[1]))
                    predicate = predicate.And(a => a.BirimKod == birimKosul[1]);

                //Sadece Kendi kayýtlarý
                predicate = predicate.Or(a => a.RiskSahibiKod == kullanan.PersonelKod);

                selectData = await _unitOfWork.KosulEkleAsync(selectData, predicate);
                //****************************************************************************************
            */



            //2- Kullanýcý Risk Evreni ekranýnda Kopyala dediðinde Risk Kaydý ekranýna ilgili kayýt düþecek ve süreç oradan ilerleyecek. Risk Kaydý ekranýndaki yetkiler açýklamadaki gibi olacak.
            /*
                Uzman kendi girdiði riskleri görebilir.
                Birim Amiri/Ünite Sorumlusu kendi ilindeki tüm birimlerin risklerini görebilir. 
                Ýl Koordinatörleri kendi illerindeki tüm riskleri görebilir. 
                Ýlgili Koordinatör/Hukuk Müþaviri kendi Koordinatörlüðündeki riskleri görebilir.
                Risk Sekretaryasý, Yetkili Risk Görevlisi ve Baþkan tüm riskleri görebilir.  
            */


            if (!riskIzleme)
            {
                //****************************************************************************************
                //Kullanici yetkisine göre koþul
                //****************************************************************************************
                var predicate = PredicateBuilder.True<RiskEvreni>();

                predicate = predicate.And(a => a.KoordinatorlukKod != "9999");//Ýl Koordinatörlüðüne sahip riskler sadece riskizleme ekranýnda gösterilecek Melih 09.01.2024

                //Uzman, Birim Amiri/Ünite Sorumlusu ve Koordinatörler Risk Kaydý ekranýnda kendi koordinatörlüklerindeki tüm riskleri görebilsin.

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

                //if (kullanan.AktifRolKod == "UZMAN")
                //{
                //    //Sadece Kendi kayýtlarý
                //    predicate = predicate.And(a => a.RiskSahibiKod == kullanan.PersonelKod);
                //}
                //else
                //{
                //    ////Birim koþulunu uygula
                //    //****************************************************************************************
                //    string[] birimKosul = await OrtakService.ListeleBirimKosulAsync(kullanan, _serviceCTE, "RISKYONETIMI", "", "");
                //    //Koordinatörlük
                //    var koordinatorlukKosul = birimKosul[0].Split(",");//Genel koord birden fazla koordinatörlüðe sahip olduðu için
                //    if (!string.IsNullOrWhiteSpace(koordinatorlukKosul[0]))
                //        predicate = predicate.And(a => koordinatorlukKosul.ToArray().Contains(a.KoordinatorlukKod));
                //    else
                //        predicate = predicate.And(a => a.KoordinatorlukKod != "");//Tüm koordinatörlükler

                //    //Birim
                //    if (!string.IsNullOrWhiteSpace(birimKosul[1]))
                //        predicate = predicate.And(a => a.BirimKod == birimKosul[1]);

                //}

                if (!aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
                    predicate = predicate.And(a => a.JenerikRisk != 1);

                selectData = await _unitOfWork.KosulEkleAsync(selectData, predicate);
                //****************************************************************************************
            }

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                RiskEvreni aramaObj = Arac.DataTablesAramaNesne<RiskEvreni>(new RiskEvreni(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskAdi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskAdi.Contains(aramaObj.RiskAdi));
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskTanimi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskTanimi.Contains(aramaObj.RiskTanimi));
                if (!string.IsNullOrWhiteSpace(aramaObj.SurecKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Surec.Kod == aramaObj.SurecKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.AmacKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Amac.Kod == aramaObj.AmacKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.HedefKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Hedef.Kod == aramaObj.HedefKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguRiskKategoriKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskKategoriler.Any(rk => rk.RiskKategoriKod == aramaObj.SorguRiskKategoriKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguIlIrtibatOfisiKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.IlIrtibatOfisler.Any(rk => rk.IlIrtibatOfisiKod == aramaObj.SorguIlIrtibatOfisiKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguRiskSahibiKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskSahibiKod == aramaObj.SorguRiskSahibiKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguAzaltmaPlaniSorumlusuKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod == aramaObj.SorguAzaltmaPlaniSorumlusuKod);
                if (aramaObj.SorguGecerlilikTarihi1.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskBaslangicTarihi >= aramaObj.SorguGecerlilikTarihi1);
                if (aramaObj.SorguGecerlilikTarihi2.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskBitisTarihi <= aramaObj.SorguGecerlilikTarihi2);
                if (aramaObj.KayitTarihi1.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitTarihi >= aramaObj.KayitTarihi1);
                if (aramaObj.KayitTarihi2.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitTarihi <= aramaObj.KayitTarihi2);
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskNo))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskNo == aramaObj.RiskNo);

                if (riskIzleme)
                {
                    if (aramaObj.KontrolEdildi == 1)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KontrolEdildi == aramaObj.KontrolEdildi && a.RiskYonetimi.KontrolEdildi == aramaObj.KontrolEdildi && a.RiskYonetimi.RiskAzaltmaPlani.KontrolEdildi == aramaObj.KontrolEdildi);
                    else if (aramaObj.KontrolEdildi == 2)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KontrolEdildi == 0 || a.RiskYonetimi.KontrolEdildi == 0 || a.RiskYonetimi.RiskAzaltmaPlani.KontrolEdildi == 0);
                }
                else
                {
                    if (aramaObj.KontrolEdildi == 1)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KontrolEdildi == aramaObj.KontrolEdildi);
                    else if (aramaObj.KontrolEdildi == 2)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KontrolEdildi == 0);
                }


                if (kullanan.AktifRolKod == "RISKSEKRETARYASI" && aramaObj.JenerikRisk > 0)
                {
                    if (aramaObj.JenerikRisk == 2)
                        aramaObj.JenerikRisk = 0;
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.JenerikRisk == aramaObj.JenerikRisk);
                }
                else if (!riskIzleme)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.JenerikRisk == 0);


                if (riskIzleme && aramaObj.SorguBaskasiAdinaRiskDurumu > 0)
                {
                    List<string> belgeNolarPersonelTarihce = new List<string>();
                    Sonuc tarihSonuc = await _serviceTarihce.ListeleAsync(EnumTarihceIslemTur.RiskEvreni, 1, kullanan.PersonelKod);
                    if (tarihSonuc.IslemSonuc)
                    {
                        foreach (Tarihce item in tarihSonuc.Liste)
                        {
                            belgeNolarPersonelTarihce.Add(item.IlgiKod);
                        }
                    }

                    /*
                     * 0:Tümü
                     * 1:Baþkasý adýna kaydettiklerim
                     * 2:Risk sahibi olduklarým
                     * 3:Kaydettigim bütün riskler
                     */
                    if (aramaObj.SorguBaskasiAdinaRiskDurumu == 1)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => belgeNolarPersonelTarihce.ToArray().Contains(a.Kod) && a.RiskSahibiKod != kullanan.PersonelKod);
                    else if (aramaObj.SorguBaskasiAdinaRiskDurumu == 2)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskSahibiKod == kullanan.PersonelKod);
                    else if (aramaObj.SorguBaskasiAdinaRiskDurumu == 3)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => belgeNolarPersonelTarihce.ToArray().Contains(a.Kod) || a.RiskSahibiKod == kullanan.PersonelKod);
                }


                if (onay)
                {
                    if (!yonetici)
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
                if (!string.IsNullOrWhiteSpace(dataTablesParam.searchValue))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Surec.Adi.Contains(dataTablesParam.searchValue)
                                                                    || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                    || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                    || a.RiskAdi.Contains(dataTablesParam.searchValue)
                                                                    || a.RiskTanimi.Contains(dataTablesParam.searchValue)
                                                                    || a.AnahtarRiskGostergesiAdi.Contains(dataTablesParam.searchValue)
                                                                    || a.RiskNo.Contains(dataTablesParam.searchValue));

                if (onay)
                {
                    if (!yonetici)
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

            if (dataTablesParam.sortColumn == "RiskSahibi.AdiSoyadi")
                dataTablesParam.sortColumn = "RiskSahibi.Adi";

            var jsonData = Arac.DataTablesJsonData(selectData, dataTablesParam);

            try
            {
                if (kullanan.AktifRolKod != "RISKSEKRETARYASI")
                {
                    dynamic data = jsonData.GetType().GetProperty("data").GetValue(jsonData, null);

                    foreach (RiskEvreni item in data)
                    {
                        if (item.JenerikRisk == 1)
                        {
                            item.KontrolDuzenlemeGosterme = 1;
                            item.KontrolOnayaGonderGosterme = 1;
                            item.KontrolTarihceGosterme = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return jsonData;

        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, RiskEvreni gelenNesne)
        {
            RiskEvreni islemYapilan = new RiskEvreni();

            string hata = "";

            if (gelenNesne == null)
                hata += "<li>Risk Evreni içeriði boþ geldi</li>";
            else
            {
                if (string.IsNullOrWhiteSpace(gelenNesne.KoordinatorlukKod))
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.KoordinatorlukAlaniBos"] + "</li>";
                if (string.IsNullOrWhiteSpace(gelenNesne.RiskAdi))
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskAdiAlaniBos"] + "</li>";
                if (string.IsNullOrWhiteSpace(gelenNesne.RiskTanimi))
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskTanimiAlaniBos"] + "</li>";

                if (gelenNesne.RiskBitisTarihi != null && gelenNesne.RiskBitisTarihi.Value.Year < 1950)
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskBitisTarihiAlaniBos"] + "</li>";
            }


            //Ýller arasýnda risk atamasý yapýlamaz. Örneðin Aydýn Ýl Koordinatörlüðünden bir kullanýcý, Balýkesir Ýl Koordinatörlüðüne bir risk atayamaz.
            //Fakat Ýl ve Merkez arasýnda veya Ýl Koordinatörlüklerindeki Birimler arasýnda risk atamasý yapýlabilir. 
            bool uzman = Arac.YetkisiVarmi("UZMAN", kullanan);
            if (uzman && kullanan.KoordinatorlukKod != gelenNesne.KoordinatorlukKod)
            {
                var yetkiTip1 = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(kullanan, kullanan.KoordinatorlukKod, 0, true);
                var yetkiTip2 = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(kullanan, gelenNesne.KoordinatorlukKod, 0, true);

                if (yetkiTip1 == "ILKOORDINATOR" && yetkiTip2 == "ILKOORDINATOR")
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.IllerArasiRiskAtamasiYapilamaz"] + "</li>";
            }

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                //Risk Yönetimi ve Risk Azaltma Planý verilerini silmek için RiskYonetimi bilgileri gönderiliyor. Daha sonra Tarhiçe karþýlaþtýrmasý için null deðeri veriliyor.

                var degerlendirmeYonetilmeSifirla = false;
                var riskYonetimi = gelenNesne.RiskYonetimi;
                gelenNesne.RiskYonetimi = null;

                Tarihce tarihce = new Tarihce();

                List<RiskEvreniRiskKategori> riskKategoriler = gelenNesne.RiskKategoriler;
                gelenNesne.RiskKategoriler = null;

                List<RiskEvreniIlIrtibatOfisi> ilIrtibatOfisler = gelenNesne.IlIrtibatOfisler;
                gelenNesne.IlIrtibatOfisler = null;

                List<RiskEvreniOlayRaporlama> olayRaporlar = gelenNesne.OlayRaporlar;
                gelenNesne.OlayRaporlar = null;

                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    int kayitSayisi = await _unitOfWork.KayitSayisiAsync();
                    gelenNesne.RiskNo = "R" + (kayitSayisi + 1).ToString("00000");
                    gelenNesne.RiskSahibiKod = kullanan.PersonelKod;

                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                    tarihce.EskiDeger = Arac.JSONSerialize(new RiskEvreni());
                    tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                    bool yonetici = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", kullanan);
                    if (!yonetici)
                    {
                        //Kayýtlý bir bilgi ancak sahibi tarafýndan deðiþtirilebilir
                        if (eskiKayit.RiskSahibiKod != kullanan.PersonelKod)
                            hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";
                    }

                    //Onaya gönderilen kayýt deðiþtirilemez
                    if (eskiKayit.Durum == (int)ENUMDurum.OnayaGonderdi)
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";
                    else if (eskiKayit.Durum == (int)ENUMDurum.Reddedildi)         //Ýptal edilen kayýt deðiþtirilemez
                        hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";


                    //Risk Kaydý ekranýnda Koord Birim Risk Adý alanlarýný riskin sahibi Onaylý bir kayýtta deðiþtirirse süreç en baþtan baþlayacak ve diðer ekranlar da sýfýrlanacak.
                    //(Uyarý da versin emin misiniz diye diðer ekranlardaki bilgileri sýfýrlayacaðýna dair) Ama kalan alanlarda deðiþiklik yapýlýrsa diðer ekranlar sýfýrlanmayacak.
                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli && (eskiKayit.KoordinatorlukKod != gelenNesne.KoordinatorlukKod || eskiKayit.BirimKod != gelenNesne.BirimKod))
                        degerlendirmeYonetilmeSifirla = true;

                    if (hata != "")
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);

                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                    if (degerlendirmeYonetilmeSifirla || eskiKayit.Durum == (int)ENUMDurum.Pasif || eskiKayit.Durum == (int)ENUMDurum.Sil || eskiKayit.Durum == (int)ENUMDurum.GeriGonderildi)
                        eskiKayit.Durum = gelenNesne.Durum;

                    eskiKayit.KoordinatorlukKod = gelenNesne.KoordinatorlukKod;
                    eskiKayit.BirimKod = gelenNesne.BirimKod;
                    eskiKayit.AmacKod = gelenNesne.AmacKod;
                    eskiKayit.HedefKod = gelenNesne.HedefKod;
                    eskiKayit.SurecKod = gelenNesne.SurecKod;
                    eskiKayit.AltSurecKod = gelenNesne.AltSurecKod;
                    eskiKayit.AnahtarRiskGostergesiAdi = gelenNesne.AnahtarRiskGostergesiAdi;
                    eskiKayit.RiskAdi = gelenNesne.RiskAdi;
                    eskiKayit.RiskTanimi = gelenNesne.RiskTanimi;
                    eskiKayit.RiskTuru = gelenNesne.RiskTuru;
                    eskiKayit.RiskBaslangicTarihi = gelenNesne.RiskBaslangicTarihi;
                    eskiKayit.RiskBitisTarihi = gelenNesne.RiskBitisTarihi;
                    eskiKayit.IlIrtibatOfisiKod = gelenNesne.IlIrtibatOfisiKod;
                    eskiKayit.Aciklama = gelenNesne.Aciklama;
                    eskiKayit.JenerikRisk = gelenNesne.JenerikRisk;
                    eskiKayit.KontrolEdildi = gelenNesne.KontrolEdildi;

                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                }

                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.RiskEvreni;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = gelenNesne.Durum;

                //Kopyalama iþlemiyle risk kaydý yapýlýyorsa. Referans risk'in tarihçesine yeni riskin bilgisini kaydet
                if (!string.IsNullOrWhiteSpace(gelenNesne.KopyaKod) && !string.IsNullOrWhiteSpace(gelenNesne.RiskNo))
                {
                    var kopyaKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.KopyaKod);

                    Tarihce tarihceKopya = new Tarihce();
                    tarihceKopya.EskiDeger = Arac.JSONSerialize(kopyaKayit);
                    tarihceKopya.Durum = (int)ENUMDurum.Kopyalandý;
                    tarihceKopya.IlgiKod = gelenNesne.KopyaKod;
                    tarihceKopya.IlgiTur = EnumTarihceIslemTur.RiskEvreni;
                    tarihceKopya.IslemYapanKod = kullanan.PersonelKod;
                    tarihceKopya.Aciklama = gelenNesne.RiskNo + " nolu risk oluþturuldu.";
                    tarihceKopya.YeniDeger = Arac.JSONSerialize(kopyaKayit);

                    var sonucKopya = await _serviceTarihce.KaydetAsync(kullanan, tarihceKopya);

                    tarihce.Durum = (int)ENUMDurum.Kopyalandý;
                    tarihce.Aciklama = kopyaKayit.RiskNo + " nolu risk kaydýndan kopyalandý.";
                }

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);


                await _serviceRiskKategori.SilAsync(kullanan, gelenNesne.Kod);

                if (riskKategoriler != null)
                {
                    foreach (var item in riskKategoriler)
                    {
                        RiskEvreniRiskKategori giden = new RiskEvreniRiskKategori();
                        item.RiskEvreniKod = gelenNesne.Kod;

                        await _serviceRiskKategori.KaydetAsync(kullanan, item);
                    }
                }

                await _serviceIIlIrtibatOfisi.SilAsync(kullanan, gelenNesne.Kod);

                if (ilIrtibatOfisler != null)
                {
                    foreach (var item in ilIrtibatOfisler)
                    {
                        RiskEvreniIlIrtibatOfisi giden = new RiskEvreniIlIrtibatOfisi();
                        item.RiskEvreniKod = gelenNesne.Kod;

                        await _serviceIIlIrtibatOfisi.KaydetAsync(kullanan, item);
                    }
                }

                await _serviceOlayRaporlama.SilAsync(kullanan, gelenNesne.Kod);

                if (olayRaporlar != null)
                {
                    foreach (var item in olayRaporlar)
                    {
                        RiskEvreniOlayRaporlama giden = new RiskEvreniOlayRaporlama();
                        item.RiskEvreniKod = gelenNesne.Kod;

                        await _serviceOlayRaporlama.KaydetAsync(kullanan, item);
                    }
                }

                await _unitOfWork.KaydetAsync();


                //Bildirim sisteminden kaydý sil
                var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreni, BelgeKod = gelenNesne.Kod, });

                if (!string.IsNullOrWhiteSpace(riskYonetimi?.Kod) && degerlendirmeYonetilmeSifirla)
                {
                    var sonucRiskYonetimi = await _serviceRiskYonetimi.SilAsync(kullanan, riskYonetimi);
                }
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li><li>" + ex.InnerException + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.RiskKayitBasarili"], gelenNesne.Kod);
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
            //string hata = "";

            //if (string.IsNullOrWhiteSpace(kod))
            //    hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            //var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod);

            //if (eskiKayit.RiskSahibiKod != kullanan.PersonelKod)
            //    hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";

            //if (hata != "")
            //    return new Sonuc(ENUMIslemDurum.Uyari, hata);

            //try
            //{
            //    await _unitOfWork.SilAsync(d => d.Kod == kod);
            //    await _unitOfWork.KaydetAsync();
            //}
            //catch (System.Exception ex)
            //{
            //    return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            //}

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
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, RiskEvreni gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod, "Koordinatorluk");

                //Durum deðiþikliðine uygun mu?
                hata = Arac.DurumDegisikligiUygunMu(kullanan, _sharedResource, eskiKayit, gelenNesne);

                if (hata == "" && gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi && eskiKayit.JenerikRisk == 1)
                    hata = "<li>" + _sharedResource["Kontrol.Duzenle.JenerikRiskOnayaGonderilemez"] + "</li>";

                if (hata == "" && (gelenNesne.Durum == (int)ENUMDurum.Pasif || gelenNesne.Durum == (int)ENUMDurum.Sil))
                {
                    if (gelenNesne.Tarihce == null || string.IsNullOrWhiteSpace(gelenNesne.Tarihce.Aciklama))
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.AciklamaBos"] + "</li>";
                }

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                var oncekiDurum = eskiKayit.Durum;

                Tarihce tarihce = new Tarihce();

                List<RiskEvreniRiskKategori> riskKategoriler = gelenNesne.RiskKategoriler;
                gelenNesne.RiskKategoriler = null;

                List<RiskEvreniIlIrtibatOfisi> ilIrtibatOfisler = gelenNesne.IlIrtibatOfisler;
                gelenNesne.IlIrtibatOfisler = null;

                List<RiskEvreniOlayRaporlama> olayRaporlar = gelenNesne.OlayRaporlar;
                gelenNesne.OlayRaporlar = null;

                tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);


                if (gelenNesne.Durum == (int)ENUMDurum.Pasif || gelenNesne.Durum == (int)ENUMDurum.Sil)
                {
                    eskiKayit.Durum = gelenNesne.Durum;

                    var islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                    //Tarihçe Baþlangýç
                    tarihce.IlgiKod = gelenNesne.Kod;
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskEvreni;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = gelenNesne.Durum;
                    if (gelenNesne.Tarihce != null)
                        tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);

                    await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiþ

                    var sonuc = await _unitOfWork.KaydetAsync();


                    var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreni, BelgeKod = gelenNesne.Kod, });


                    var riskYonetimi = gelenNesne.RiskYonetimi;

                    if (!string.IsNullOrWhiteSpace(riskYonetimi?.Kod))
                    {
                        var sonucRiskYonetimi = await _serviceRiskYonetimi.SilAsync(kullanan, riskYonetimi);
                    }

                    //Mail Gönder: Bir risk kaydýnýn durumu Pasif durumuna çekildiðinde Risk Sekreteryasýna mail bildirimi gönder. 
                    if (eskiKayit.Durum == (int)ENUMDurum.Pasif)
                    {
                        var formBildirim = new BildirimSistemi()
                        {
                            Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                            BelgeKod = gelenNesne.Kod,
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreniDurumPasif,
                        };

                        Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                    }
                    //Mail Gönder Bitiþ


                }
                else
                {
                    var riskSahibiEski = "";

                    if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                        eskiKayit.KayitTarihi = DateTime.Now.Date;

                    if ((gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Onayli) && riskKategoriler?.Count > 0)
                    {
                        eskiKayit.AmacKod = gelenNesne.AmacKod;
                        eskiKayit.HedefKod = gelenNesne.HedefKod;
                        eskiKayit.SurecKod = gelenNesne.SurecKod;
                        eskiKayit.AltSurecKod = gelenNesne.AltSurecKod;
                        eskiKayit.AnahtarRiskGostergesiAdi = gelenNesne.AnahtarRiskGostergesiAdi + "";
                        eskiKayit.RiskAdi = gelenNesne.RiskAdi + "";
                        eskiKayit.RiskTanimi = gelenNesne.RiskTanimi + "";

                        if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                        {
                            if (kullanan.AktifRolKod == "RISKSEKRETARYASI" && !string.IsNullOrWhiteSpace(gelenNesne.Aciklama))
                                eskiKayit.Aciklama = gelenNesne.Aciklama;

                            if (!string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod) && eskiKayit.RiskSahibiKod != gelenNesne.RiskSahibiKod)
                            {
                                riskSahibiEski = eskiKayit.RiskSahibiKod;
                                eskiKayit.RiskSahibiKod = gelenNesne.RiskSahibiKod;
                            }
                            else if (string.IsNullOrWhiteSpace(eskiKayit.RiskSahibiKod))
                                eskiKayit.RiskSahibiKod = kullanan.PersonelKod;
                            //else if (string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod))
                            //    eskiKayit.RiskSahibiKod = kullanan.PersonelKod;
                        }

                        eskiKayit.RiskTuru = gelenNesne.RiskTuru;
                        eskiKayit.RiskBaslangicTarihi = gelenNesne.RiskBaslangicTarihi;
                        eskiKayit.RiskBitisTarihi = gelenNesne.RiskBitisTarihi;

                        tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    }
                    else
                        tarihce.EskiDeger = "";


                    eskiKayit.Durum = gelenNesne.Durum;


                    //BildirimSistemi Baþlangýç
                    if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi || gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Reddedildi)
                    {
                        var bs = new BildirimSistemi
                        {
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreni,
                            BelgeKod = eskiKayit.Kod,
                            KoordinatorlukKod = eskiKayit.KoordinatorlukKod,
                            BirimKod = eskiKayit.BirimKod,
                            IslemYapanKod = kullanan.PersonelKod,
                            IslemTarihi = DateTime.Now,
                        };

                        if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                        {
                            bs.OnaylayacakYetki = "RISKSEKRETARYASI";
                            bs.Durum = (int)ENUMDurum.OnayaGonderdi;
                        }
                        else if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                        {
                            bs.OnaylayacakYetki = kullanan.AktifRolKod;
                            bs.Durum = (int)ENUMDurum.Onayli;

                            if (kullanan.AktifRolKod == "RISKSEKRETARYASI")
                            {
                                //Deðiþiklik : Risk sahibinin birimine bakýlmaksýzýn her durumda risk sekretaryasýndan sonra birim amirinin onayýna gönderilsin. Birim yok ise koordinatör yetksinin onayýna gidecek.
                                /*
                                    //Risk sahibinin koordinatorlük veya birim bilgisi farklý ise birim amirine onaya gönder
                                    var kriter = new ViewYetki();
                                    kriter.PersonelKod = eskiKayit.RiskSahibiKod;
                                    kriter.KoordinatorlukKod = eskiKayit.KoordinatorlukKod;
                                    kriter.BirimKod = eskiKayit.BirimKod;

                                    var sonucYetki = await _serviceYetki.ListeleAsync(kullanan, kriter);

                                    if (sonucYetki.IslemSonuc && sonucYetki.Liste.Count <= 0)
                                */




                                bool birimAmirineGonder = true;

                                //Risk Sekretaryasýna onaya gönderen yetkiyi bul
                                var onayaGonderenYetki = "";
                                var ilgiliTarihce = await _serviceTarihce.ListeleAsync(eskiKayit.Kod);
                                if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                                {
                                    foreach (Tarihce t in ilgiliTarihce.Liste)
                                    {
                                        if (t.Durum == (int)ENUMDurum.OnayaGonderdi && t.IlgiliRol == "RISKSEKRETARYASI")
                                            onayaGonderenYetki = t.IslemYapanRol;
                                    }
                                }


                                //Baþkan/Genel Koord. kendi adýna risk kaydý girdi. Risk Sekretaryasý onayladý.
                                //Baþkan/Genel Koord. baþkasý adýna risk kaydý girdi. Risk Sekretaryasý onayladý. Ýlgili Birim Amiri risk sahibini deðiþtirmeden direkt onayladý.

                                if (onayaGonderenYetki == "BASKAN" || onayaGonderenYetki == "GENELKOORDINATOR")
                                {
                                    //Risk sahibinin koordinatorlük veya birim bilgisi farklý mý? //Baþkasý adýna risk kaydý mý?
                                    var kriter = new ViewYetki();
                                    kriter.PersonelKod = eskiKayit.RiskSahibiKod;
                                    kriter.KoordinatorlukKod = eskiKayit.KoordinatorlukKod;
                                    //kriter.BirimKod = eskiKayit.BirimKod;

                                    var sonucYetki = await _serviceYetki.ListeleAsync(kullanan, kriter);

                                    if (sonucYetki.IslemSonuc)
                                    {
                                        foreach (ViewYetki item in sonucYetki.Liste)
                                        {
                                            if (item.KoordinatorlukKod == eskiKayit.KoordinatorlukKod
                                                && string.IsNullOrWhiteSpace(item.BirimKod)
                                                && item.Rol == onayaGonderenYetki)
                                                onayaGonderenYetki = "";
                                        }

                                        if (!string.IsNullOrWhiteSpace(onayaGonderenYetki))
                                            onayaGonderenYetki = "BIRIMAMIRI"; //Birim amirine gönder
                                    }


                                    if (string.IsNullOrWhiteSpace(onayaGonderenYetki))
                                        birimAmirineGonder = false;

                                }


                                if (birimAmirineGonder)
                                {
                                    var onaylayacakYetki = "";
                                    if (!string.IsNullOrWhiteSpace(eskiKayit.BirimKod))
                                        onaylayacakYetki = "BIRIMAMIRI"; //onaylayacakYetki = await _serviceBildirimSistemi.OnaylayacakYetkiVerAsync(kullanan, bs);
                                    else
                                    {
                                        //Birimi olmayan koordinatörlük ile iþlem yapýldýðýnda
                                        onaylayacakYetki = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(kullanan, eskiKayit.KoordinatorlukKod, eskiKayit.Koordinatorluk.Tur, true);
                                    }

                                    if (!string.IsNullOrWhiteSpace(onaylayacakYetki))
                                    {
                                        if (onaylayacakYetki != "BASKAN*")
                                        {
                                            eskiKayit.Durum = (int)ENUMDurum.OnayaGonderdi;
                                            bs.OnaylayacakUstYetki = onaylayacakYetki;
                                        }
                                    }
                                    else
                                        hata += "<li>" + _sharedResource["Kontrol.DurumDegistir.OnaylayacakYetkiBulunamadi"] + "</li>";
                                }
                            }
                            else if (kullanan.AktifRolKod != "RISKSEKRETARYASI")
                            {
                                if (!string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod) && eskiKayit.RiskSahibiKod != gelenNesne.RiskSahibiKod)
                                {
                                    riskSahibiEski = eskiKayit.RiskSahibiKod;
                                    eskiKayit.RiskSahibiKod = gelenNesne.RiskSahibiKod;
                                }
                                else if (string.IsNullOrWhiteSpace(eskiKayit.RiskSahibiKod))
                                    eskiKayit.RiskSahibiKod = kullanan.PersonelKod;
                                //else if (string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod))
                                //    eskiKayit.RiskSahibiKod = kullanan.PersonelKod;


                                //(Koord ve Birim deðiþtirilirse Risk Sahibi alan boþaltýlsýn.) Risk Sahibi; onaylayan Birim Amiri olur. 
                                if (kullanan.AktifRolKod == "BIRIMAMIRI" && string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod))
                                {
                                    //Risk sahibinin koordinatorlük veya birim bilgisi farklý mý? //Baþkasý adýna risk kaydý mý?
                                    var kriter = new ViewYetki();
                                    kriter.PersonelKod = eskiKayit.RiskSahibiKod;
                                    kriter.KoordinatorlukKod = eskiKayit.KoordinatorlukKod;
                                    kriter.BirimKod = eskiKayit.BirimKod;

                                    var sonucYetki = await _serviceYetki.ListeleAsync(kullanan, kriter);

                                    if (sonucYetki.IslemSonuc && sonucYetki.Liste.Count <= 0)
                                    {
                                        eskiKayit.RiskSahibiKod = kullanan.PersonelKod;
                                    }

                                }

                            }

                        }
                        else if (gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                        {
                            eskiKayit.Durum = (int)ENUMDurum.GeriGonderildi; //Geri gönderildi
                            bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kiþi yok
                            bs.OnaylayacakYetki = "-";

                            var ilgiliTarihce = await _serviceTarihce.ListeleAsync(eskiKayit.Kod);
                            if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                            {
                                foreach (Tarihce t in ilgiliTarihce.Liste)
                                {
                                    if (t.Durum == (int)ENUMDurum.OnayaGonderdi && t.IlgiliRol == "RISKSEKRETARYASI")
                                        bs.OnaylayacakYetki = t.IslemYapanRol;
                                }
                            }

                            //if (kullanan.AktifRolKod != "RISKSEKRETARYASI")
                            //{
                            //    eskiKayit.Durum = (int)ENUMDurum.GeriGonderildi; //RiskSekretaryasýna geri gönderildi
                            //    bs.Durum = (int)ENUMDurum.GeriGonderildi;
                            //    bs.OnaylayacakYetki = "RISKSEKRETARYASI";
                            //}
                            //else
                            //{
                            //    bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kiþi yok
                            //    bs.OnaylayacakYetki = "-";
                            //}
                        }
                        else if (gelenNesne.Durum == (int)ENUMDurum.Reddedildi)
                        {
                            bs.Durum = (int)ENUMDurum.Reddedildi;
                            bs.OnaylayacakYetki = "-";
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

                    var islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);


                    //Tarihçe Baþlangýç
                    tarihce.IlgiKod = gelenNesne.Kod;
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskEvreni;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = gelenNesne.Durum;
                    if (gelenNesne.Tarihce != null)
                        tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;


                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                    {

                        if (riskKategoriler != null)
                        {
                            await _serviceRiskKategori.SilAsync(kullanan, gelenNesne.Kod);
                            foreach (var item in riskKategoriler)
                            {
                                RiskEvreniRiskKategori giden = new RiskEvreniRiskKategori();
                                item.RiskEvreniKod = gelenNesne.Kod;

                                await _serviceRiskKategori.KaydetAsync(kullanan, item);
                            }
                        }

                        if (ilIrtibatOfisler != null)
                        {
                            await _serviceIIlIrtibatOfisi.SilAsync(kullanan, gelenNesne.Kod);
                            foreach (var item in ilIrtibatOfisler)
                            {
                                RiskEvreniIlIrtibatOfisi giden = new RiskEvreniIlIrtibatOfisi();
                                item.RiskEvreniKod = gelenNesne.Kod;

                                await _serviceIIlIrtibatOfisi.KaydetAsync(kullanan, item);
                            }
                        }

                        if (olayRaporlar != null)
                        {
                            await _serviceOlayRaporlama.SilAsync(kullanan, gelenNesne.Kod);
                            foreach (var item in olayRaporlar)
                            {
                                RiskEvreniOlayRaporlama giden = new RiskEvreniOlayRaporlama();
                                item.RiskEvreniKod = gelenNesne.Kod;

                                await _serviceOlayRaporlama.KaydetAsync(kullanan, item);
                            }
                        }
                    }

                    await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiþ


                    var sonuc = await _unitOfWork.KaydetAsync();


                    //Mail Gönder: Bir riskin Risk Sahibi deðiþtiðinde riskin ilk sahibine bildirim maili gitmeli. Süreçten çýktýðýna dair eski sahibine mail gönder. 
                    if (!string.IsNullOrWhiteSpace(riskSahibiEski))
                    {
                        var formBildirim = new BildirimSistemi()
                        {
                            Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                            BelgeKod = gelenNesne.Kod,
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreniRiskSahibiDegisti,
                            MailGonderilecekKisi = riskSahibiEski
                        };

                        Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                    }
                    //Mail Gönder Bitiþ

                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli && !string.IsNullOrEmpty(tarihce.DegisenDeger))
                    {
                        //Mail Gönder: Onaycý kiþi kayýtta deðiþiklik yapýp onaylarsa, deðiþiklik risk sahibine mail olarak bildirilir. 
                        if (!string.IsNullOrWhiteSpace(eskiKayit.RiskSahibiKod))
                        {
                            var formBildirim = new BildirimSistemi()
                            {
                                Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                                BelgeKod = gelenNesne.Kod,
                                BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreniOnaydaDegisiklik,
                                MailGonderilecekKisi = eskiKayit.RiskSahibiKod
                            };

                            Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                        }
                        //Mail Gönder Bitiþ
                    }
                }

                //Mail Gönder: Tüm ekranlar için kaydýn durum bilgisi deðiþtiðinde bilgilendirme maili gidebilir mi? (reddedildi, onaya gönderildi, geri gönderildi vs.). 
                if (oncekiDurum != eskiKayit.Durum && !string.IsNullOrWhiteSpace(eskiKayit.RiskSahibiKod))
                {
                    var formBildirim = new BildirimSistemi()
                    {
                        Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                        BelgeKod = gelenNesne.Kod,
                        BelgeTipi = (int)EnumTarihceIslemTur.RiskEvreniDurumDegisti,
                        MailGonderilecekKisi = eskiKayit.RiskSahibiKod,
                        Aciklama = Arac.DurumAdGetir(oncekiDurum) + " >> " + Arac.DurumAdGetir(eskiKayit.Durum)
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
                    RiskEvreni form = new RiskEvreni();
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

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilerden güncel deðerleri deðiþen alanlarý döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// string türünde deðiþen alanlarý döndürür
        /// </returns>
        public async Task<string> DegisenAlanlariGetirAsync(KullaniciDto kullanan, RiskEvreni gelenNesne)
        {
            var degisenAlanlar = "";
            var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

            if (eskiKayit.AnahtarRiskGostergesiAdi + "" != gelenNesne.AnahtarRiskGostergesiAdi + "")
                degisenAlanlar += "<li>" + _sharedResource["RiskEvreni.Alan.AnahtarRiskGostergesi"] + " " + _sharedResource["RiskEvreni.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.RiskAdi + "" != gelenNesne.RiskAdi + "")
                degisenAlanlar += "<li>" + _sharedResource["RiskEvreni.Alan.RiskAdi"] + " " + _sharedResource["RiskEvreni.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.RiskTanimi + "" != gelenNesne.RiskTanimi + "")
                degisenAlanlar += "<li>" + _sharedResource["RiskEvreni.Alan.RiskTanimi"] + " " + _sharedResource["RiskEvreni.AlanindaGuncellemeYapildi"] + " </li>";
            if (!string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod) && eskiKayit.RiskSahibiKod + "" != gelenNesne.RiskSahibiKod + "")
                degisenAlanlar += "<li>" + _sharedResource["RiskEvreni.Alan.RiskSahibi"] + " " + _sharedResource["RiskEvreni.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.RiskTuru != gelenNesne.RiskTuru)
                degisenAlanlar += "<li>" + _sharedResource["RiskEvreni.Alan.RiskTuru"] + " " + _sharedResource["RiskEvreni.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.RiskBaslangicTarihi != gelenNesne.RiskBaslangicTarihi)
                degisenAlanlar += "<li>" + _sharedResource["RiskEvreni.Alan.RiskBaslangicTarihi"] + " " + _sharedResource["RiskEvreni.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.RiskBitisTarihi != gelenNesne.RiskBitisTarihi)
                degisenAlanlar += "<li>" + _sharedResource["RiskEvreni.Alan.RiskBitisTarihi"] + " " + _sharedResource["RiskEvreni.AlanindaGuncellemeYapildi"] + " </li>";

            return degisenAlanlar;
        }


        /// <summary>
        /// Istemciden parametere ile gönderilen risk sahibini deðiþtiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskSahibiDegistirAsync(KullaniciDto kullanan, RiskEvreni gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.RiskSahibiKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.RiskSahibiBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                bool yonetici = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI,BIRIMAMIRI", kullanan);

                if (!yonetici)
                    hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod, "Koordinatorluk,Birim");

                if (kullanan.AktifRolKod == "BIRIMAMIRI")
                {
                    if (eskiKayit.KoordinatorlukKod != kullanan.KoordinatorlukKod)
                    {
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + " / " + eskiKayit.Koordinatorluk.Adi + "</li>";
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);
                    }
                    else if (eskiKayit.BirimKod != kullanan.BirimKod)
                    {
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + " / " + eskiKayit.Birim.Adi + "</li>";
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);
                    }
                }


                Tarihce tarihce = new Tarihce();

                tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                eskiKayit.RiskSahibiKod = gelenNesne.RiskSahibiKod;

                var islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                //Tarihçe Baþlangýç
                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.RiskEvreni;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = (int)ENUMDurum.RiskSahibiDegisti;

                tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);

                await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                //Tarihçe Bitiþ

                var sonuc = await _unitOfWork.KaydetAsync();

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }


    }
}
