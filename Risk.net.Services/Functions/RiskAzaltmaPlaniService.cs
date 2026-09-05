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
using Risk.net.Data.Functions;
using Newtonsoft.Json.Linq;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// RiskAzaltmaPlani işlemlerinin yapıldığı servis
    /// </summary>
    public class RiskAzaltmaPlaniService : IRiskAzaltmaPlaniService
    {
        /// <summary>
        /// IUnitOfWork<RiskEvreni> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskEvreni> _unitOfWorkRiskEvreni;
        /// <summary>
        /// IUnitOfWork<RiskYonetimi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskYonetimi> _unitOfWorkRiskYonetimi;
        /// <summary>
        /// IUnitOfWork<RiskAzaltmaPlani> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskAzaltmaPlani> _unitOfWorkAnahtar;
        /// <summary>
        /// IUnitOfWork<IRiskAzaltmaPlaniRiskService> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniRiskService _serviceRisk;
        /// <summary>
        /// IUnitOfWork<IRiskAzaltmaPlaniNotService> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniNotService _serviceNot;
        /// <summary>
        /// IUnitOfWork<IRiskAzaltmaPlaniIliskiService> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniIliskiService _serviceIliski;
        /// <summary>
        /// IUnitOfWork<IRiskAzaltmaPlaniIsbirligiBirimService> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IRiskAzaltmaPlaniIsbirligiBirimService _serviceIsbirligiBirim;
        /// <summary>
        /// IUnitOfWork<IBildirimSistemiService> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBildirimSistemiService _serviceBildirimSistemi;
        /// <summary>
        /// IUnitOfWork<IViewBildirimSistemiService> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBildirimSistemiService _serviceViewBildirim;
        /// <summary>
        /// IViewBirimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBirimService _serviceViewBirim;
        /// <summary>
        /// IUnitOfWork<ITarihceService> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IRiskAzaltmaPlaniService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.RiskAzaltmaPlaniService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWorkRiskEvreni"></param>
        /// <param name="unitOfWorkRiskYonetimi"></param>
        /// <param name="unitOfWorkAnahtar"></param>
        /// <param name="serviceRisk"></param>
        /// <param name="serviceIliski"></param>
        /// <param name="serviceIsbirligiBirim"></param>
        /// <param name="serviceBildirimSistemi"></param>
        /// <param name="serviceViewBildirim"></param>
        /// <param name="serviceViewBirim"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <param name="serviceCTE"></param>
        /// <remarks></remarks>
        public RiskAzaltmaPlaniService(IUnitOfWork<RiskEvreni> unitOfWorkRiskEvreni
            , IUnitOfWork<RiskYonetimi> unitOfWorkRiskYonetimi
            , IUnitOfWork<RiskAzaltmaPlani> unitOfWorkAnahtar
            , IRiskAzaltmaPlaniRiskService serviceRisk
            , IRiskAzaltmaPlaniNotService serviceNot
            , IRiskAzaltmaPlaniIliskiService serviceIliski
            , IRiskAzaltmaPlaniIsbirligiBirimService serviceIsbirligiBirim
            , IBildirimSistemiService serviceBildirimSistemi
            , IViewBildirimSistemiService serviceViewBildirim
            , ICTEKoordinatorlukService serviceCTE
            , IViewBirimService serviceViewBirim
            , ITarihceService serviceTarihce
            , IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWorkRiskEvreni = unitOfWorkRiskEvreni;
            _unitOfWorkRiskYonetimi = unitOfWorkRiskYonetimi;
            _unitOfWorkAnahtar = unitOfWorkAnahtar;
            _serviceRisk = serviceRisk;
            _serviceNot = serviceNot;
            _serviceIliski = serviceIliski;
            _serviceIsbirligiBirim = serviceIsbirligiBirim;
            _serviceBildirimSistemi = serviceBildirimSistemi;
            _serviceViewBildirim = serviceViewBildirim;
            _serviceViewBirim = serviceViewBirim;
            _serviceTarihce = serviceTarihce;
            _serviceCTE = serviceCTE;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="riskAzaltmaPlaniKod"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string riskAzaltmaPlaniKod, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(riskAzaltmaPlaniKod) && string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                RiskEvreni kayit = null;

                if (!string.IsNullOrWhiteSpace(riskAzaltmaPlaniKod))
                    kayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.RiskAzaltmaPlani.Kod == riskAzaltmaPlaniKod, "Koordinatorluk, Birim, RiskSahibi, RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi,RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.Riskler,RiskYonetimi.RiskAzaltmaPlani.Notlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler, RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Birim, RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusu");
                else
                    kayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kod, "Koordinatorluk, Birim, RiskSahibi, RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi,RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.Riskler, RiskYonetimi.RiskAzaltmaPlani.Notlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler, RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Birim, RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusu");

                if (kayit != null)
                {
                    if (kayit.RiskYonetimi != null && kayit.RiskYonetimi.RiskAzaltmaPlani != null)
                        kayit.RiskYonetimi.RiskAzaltmaPlani.Tarihce = await _serviceTarihce.SonAciklamaGetirAsync(kullanan, kayit.RiskYonetimi.RiskAzaltmaPlani.Kod);

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
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan)
        {
            var selectData = await _unitOfWorkRiskEvreni.SorguHazirlaAsync(null, "Koordinatorluk, Birim, RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi, RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk");
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli);
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.Onayli);

            var kayitlar = selectData.ToList();

            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

            //var kayitlar = await _unitOfWorkAnahtar.ListeleAsync(null, o => o.Kod);
            //if (kayitlar.Count > -1)
            //{
            //    for (int i = 0; i < kayitlar.Count; i++)
            //    {
            //        if (kayitlar[i].Durum != (int)ENUMDurum.Onayli)
            //        {
            //            kayitlar.RemoveAt(i);
            //            i--;
            //        }
            //    }
            //    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            //}
            //return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
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
                BildirimSistemi kriter = new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani };

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
            var selectData = await _unitOfWorkRiskEvreni.SorguHazirlaAsync(null, "Koordinatorluk, Birim, Amac, Hedef, RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi, RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk,RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusu,RiskSahibi");

            //P2-234-(Risk Kaydı/Anahtar Risk Göstergesi/Risklerin Değerlendirilmesi/Risklerin Yönetilmesi) Uzman tüm riskleri görebilir. 05.04.2023 HÖ
            //selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskSahibiKod == kullanan.PersonelKod);

            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli);
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskeVerilecekCevap == EnumRiskYonetimiRiskeVerilecekCevap.Azalt);
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.JenerikRisk == 0);


            //Uzman, Birim Amiri/Ünite Sorumlusu ve Koordinatörler Risk Kaydı ekranında kendi koordinatörlüklerindeki tüm riskleri görebilsin.


            if (!onay)
            {
                //****************************************************************************************
                //Kullanici yetkisine göre koşul
                //****************************************************************************************
                var predicate = PredicateBuilder.True<RiskEvreni>();

                string[] birimKosul = await OrtakService.ListeleBirimKosulAsync(kullanan, _serviceCTE, "RISKYONETIMI", "", "");
                //Koordinatörlük
                var koordinatorlukKosul = birimKosul[0].Split(",");//Genel koord birden fazla koordinatörlüğe sahip olduğu için
                if (!string.IsNullOrWhiteSpace(koordinatorlukKosul[0]))
                    predicate = predicate.And(a => koordinatorlukKosul.ToArray().Contains(a.KoordinatorlukKod));
                else
                    predicate = predicate.And(a => a.KoordinatorlukKod != "");//Tüm koordinatörlükler


                if (Arac.YetkisiVarmi("ICDENETIMUZMANI,UZMAN,BIRIMAMIRI,ILKOORDINATOR,MERKEZKOORDINATOR,ICDENETIMKOORDINATOR,BIRIMAMIRI,GENELKOORDINATOR", kullanan))
                {
                    predicate = predicate.Or(a => a.RiskSahibiKod == kullanan.PersonelKod);
                    predicate = predicate.Or(a => a.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod == kullanan.PersonelKod);
                }
                else
                {
                    //Birim
                    if (!string.IsNullOrWhiteSpace(birimKosul[1]))
                        predicate = predicate.And(a => a.BirimKod == birimKosul[1]);
                }

                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, predicate);
                //****************************************************************************************
            }

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                var aramaObj = Arac.DataTablesAramaNesne<RiskEvreni>(new RiskEvreni(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.SorumluKoordinatorlukKod == aramaObj.KoordinatorlukKod
                    || (a.RiskYonetimi.RiskAzaltmaPlani.SorumluKoordinatorlukKod == null && a.KoordinatorlukKod == aramaObj.KoordinatorlukKod)
                    );
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.SorumluBirimKod == aramaObj.BirimKod
                    || (a.RiskYonetimi.RiskAzaltmaPlani.SorumluBirimKod == null && a.BirimKod == aramaObj.BirimKod)
                    );
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
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguRiskSahibiKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.SorguRiskSahibiKod == aramaObj.SorguRiskSahibiKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguAzaltmaPlaniSorumlusuKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod == aramaObj.SorguAzaltmaPlaniSorumlusuKod);

                if (!string.IsNullOrWhiteSpace(aramaObj.RiskNo))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskNo.Contains(aramaObj.RiskNo));
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskAdi))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskAdi.Contains(aramaObj.RiskAdi));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguAzaltmaPlaniNo))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo.Contains(aramaObj.SorguAzaltmaPlaniNo));
                if (aramaObj.BitisTarihi1.HasValue)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi >= aramaObj.BitisTarihi1);
                if (aramaObj.BitisTarihi2.HasValue)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi <= aramaObj.BitisTarihi2);
                if (aramaObj.SorguArtikRiskSeviyesi > 0)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.ArtikRiskSeviyesi == aramaObj.SorguArtikRiskSeviyesi);
                if (aramaObj.KayitTarihi1.HasValue)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.KayitTarihi >= aramaObj.KayitTarihi1);
                if (aramaObj.KayitTarihi2.HasValue)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.KayitTarihi <= aramaObj.KayitTarihi2);

                if (!string.IsNullOrWhiteSpace(aramaObj.SorguIsbirligiKoordinatorlukKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Any(rk => rk.KoordinatorlukKod == aramaObj.SorguIsbirligiKoordinatorlukKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguIsbirligiBirimKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Any(rk => rk.BirimKod == aramaObj.SorguIsbirligiBirimKod));

                if (aramaObj.KontrolEdildi == 1)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.KontrolEdildi == aramaObj.KontrolEdildi);
                else if (aramaObj.KontrolEdildi == 2)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.KontrolEdildi == 0);

                if (onay)
                {
                    if (!yonetici)
                        selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => belgeNolar.ToArray().Contains(a.RiskYonetimi.RiskAzaltmaPlani.Kod));
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.OnayaGonderdi);
                }
                else if (aramaObj.Durum > 0)
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.Aktif
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.GeriGonderildi
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.Reddedildi
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.Onayli
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.OnayaGonderdi
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == null);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dataTablesParam.searchValue))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskTanimi.Contains(dataTablesParam.searchValue)
                                                                                          || a.RiskNo.Contains(dataTablesParam.searchValue)
                                                                                          || a.RiskNo.Contains(dataTablesParam.searchValue)
                                                                                          || a.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani.Contains(dataTablesParam.searchValue)
                                                                                          || a.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo.Contains(dataTablesParam.searchValue)
                                                                                          || a.RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Adi.Contains(dataTablesParam.searchValue)
                                                                                          || a.RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                                          || (a.RiskYonetimi.RiskAzaltmaPlani.SorumluKoordinatorlukKod == null && a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue))
                                                                                          );

                if (onay)
                {
                    if (!yonetici)
                        selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => belgeNolar.ToArray().Contains(a.RiskYonetimi.RiskAzaltmaPlani.Kod));
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.OnayaGonderdi);
                }
                else
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.Aktif
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.GeriGonderildi
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.Reddedildi
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.Onayli
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == (int)ENUMDurum.OnayaGonderdi
                                                                    || a.RiskYonetimi.RiskAzaltmaPlani.Durum == null);
            }

            if (dataTablesParam.sortColumn == "RiskAzaltmaPlaniSorumluKoordinatorluk.Adi")
                dataTablesParam.sortColumn = "RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Koordinatorluk.Adi";
            else if (dataTablesParam.sortColumn == "RiskAzaltmaPlaniSorumluBirim.Adi")
                dataTablesParam.sortColumn = "RiskYonetimi.RiskAzaltmaPlani.SorumluBirim.Adi";
            else if (dataTablesParam.sortColumn == "RiskSahibi.AdiSoyadi")
                dataTablesParam.sortColumn = "RiskSahibi.Adi";
            else if (dataTablesParam.sortColumn == "RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusu.AdiSoyadi")
                dataTablesParam.sortColumn = "RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniSorumlusu.Adi";
            else if (dataTablesParam.sortColumn == "RiskYonetimi.RiskAzaltmaPlani.BitisTarihi2")
                dataTablesParam.sortColumn = "RiskYonetimi.RiskAzaltmaPlani.BitisTarihi";


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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne)
        {
            RiskAzaltmaPlani islemYapilan = new RiskAzaltmaPlani();

            string hata = "";

            if (gelenNesne.RiskYonetimiKod == "")
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiKodAlaniBos"] + "</li>";

            if (gelenNesne.BaslangicTarihi == null || gelenNesne.BaslangicTarihi.Value.Year < 2000)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.BaslangicTarihiAlaniBos"] + "</li>";
            if (gelenNesne.BitisTarihi == null || gelenNesne.BitisTarihi.Value.Year < 2000)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.BitisTarihiAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                bool onayli = false;

                var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.Kod == gelenNesne.RiskYonetimiKod, "Koordinatorluk");
                if (riskEvreni.KoordinatorlukKod != gelenNesne.SorumluKoordinatorlukKod && string.IsNullOrWhiteSpace(gelenNesne.SorumluBirimKod))
                {
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskAzaltmaPlaniSorumluBirimBos"] + "</li>";
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                }

                Tarihce tarihce = new Tarihce();
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                List<RiskAzaltmaPlaniRisk> riskler = gelenNesne.Riskler;
                gelenNesne.Riskler = null;

                List<RiskAzaltmaPlaniNot> notlar = gelenNesne.Notlar;
                gelenNesne.Notlar = null;

                List<RiskAzaltmaPlaniIliski> iliskiliPlanlar = gelenNesne.IliskiliPlanlar;
                gelenNesne.IliskiliPlanlar = null;

                List<RiskAzaltmaPlaniIsbirligiBirim> isbirligiBirimler = gelenNesne.IsbirligiBirimler;
                gelenNesne.IsbirligiBirimler = null;


                //if (string.IsNullOrWhiteSpace(gelenNesne.AzaltmaPlaniSorumlusuKod))
                //    gelenNesne.AzaltmaPlaniSorumlusuKod = kullanan.PersonelKod;

                bool yeniKayit = string.IsNullOrWhiteSpace(gelenNesne.Kod);

                if (yeniKayit)
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    gelenNesne.TamamlamaYuzdesi = 0;
                    gelenNesne.MevcutDurum = EnumRiskAzaltmaPlaniMevcutDurum.DevamEdiyor;

                    int kayitSayisi = await _unitOfWorkAnahtar.KayitSayisiAsync();
                    gelenNesne.AzaltmaPlaniNo = "A" + (kayitSayisi + 1).ToString("00000");

                    tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                    tarihce.Durum = gelenNesne.Durum.Value;

                    islemYapilan = await _unitOfWorkAnahtar.KayitEkleAsync(gelenNesne);

                }
                else
                {
                    var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod, "SorumluBirim,SorumluBirim.Koordinatorluk");


                    //Kayıtlı bir bilgi ancak sahibi tarafından değiştirilebilir
                    if (!(eskiKayit.Durum == (int)ENUMDurum.Onayli && kullanan.AktifRolKod == "RISKSEKRETARYASI") && !string.IsNullOrWhiteSpace(eskiKayit.AzaltmaPlaniSorumlusuKod) && eskiKayit.AzaltmaPlaniSorumlusuKod != kullanan.PersonelKod)
                    {
                        hata += "<li>" + _sharedResource["Kontrol.Duzenle.SorumluDegisiklikYapabilir"] + "</li>";
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);
                    }

                    //Onaya gönderilen kayıt değiştirilemez
                    if (eskiKayit.Durum == (int)ENUMDurum.OnayaGonderdi)
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";
                    else if (eskiKayit.Durum == (int)ENUMDurum.Reddedildi)         //İptal edilen kayıt değiştirilemez
                        hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";
                    else if (eskiKayit.Durum == (int)ENUMDurum.GeriGonderildi)
                    {
                        var bs = new BildirimSistemi
                        {
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani,
                            BelgeKod = eskiKayit.Kod,
                            KoordinatorlukKod = eskiKayit.SorumluKoordinatorlukKod,
                            BirimKod = eskiKayit.SorumluBirimKod,
                            IslemYapanKod = kullanan.PersonelKod,
                            IslemTarihi = DateTime.Now,
                        };

                        var sonYetki = await _serviceBildirimSistemi.SonYetkiVerAsync(kullanan, bs);

                        if (sonYetki.Durum == (int)ENUMDurum.GeriGonderildi)
                        {
                            if (((sonYetki.OnaylayacakYetki != "" || sonYetki.OnaylayacakYetki != "-") && sonYetki.OnaylayacakYetki != kullanan.AktifRolKod)
                                || (kullanan.KoordinatorlukKod != "" && sonYetki.KoordinatorlukKod != kullanan.KoordinatorlukKod)
                                || (kullanan.BirimKod != "" && sonYetki.BirimKod != kullanan.BirimKod))
                                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>"; //Geri gönderilen kayıt gönderilen yetki tarafından düzenlenebilir.
                        }
                    }


                    if (hata != "")
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);

                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);


                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    {
                        onayli = true;
                        //Onaylı kayıt üzerinde belirli alanlar değiştirilebilir. Durum bilgisi akfif yapılmayacak. P2-38 nolu iş
                        //Azaltma Planı Sorumlusu, Tamamlanma Yüzdesi, Mevcut Durum, Azaltma Planı Detayları/Notlar, Ertelenmiş/Değiştirilmiş Zaman Planı, Zaman Planındaki Erteleme/Değişiklik Nedeni alanlarında değişiklik yapabilir

                        //Sekretarya başlangıç ve bitiş tarihlerini, ertelenmiş/değiştirilmiş zaman planını değiştirebilsin istiyoruz.
                        if (kullanan.AktifRolKod == "RISKSEKRETARYASI")
                        {
                            if (gelenNesne.BaslangicTarihi.HasValue)
                                eskiKayit.BaslangicTarihi = gelenNesne.BaslangicTarihi;
                            if (gelenNesne.BitisTarihi.HasValue)
                                eskiKayit.BitisTarihi = gelenNesne.BitisTarihi;
                        }

                        //eskiKayit.AzaltmaPlani = gelenNesne.AzaltmaPlani;
                        eskiKayit.MevcutDurum = gelenNesne.MevcutDurum;
                        eskiKayit.TamamlamaYuzdesi = gelenNesne.TamamlamaYuzdesi;
                        eskiKayit.AzaltmaPlaniSorumlusuKod = gelenNesne.AzaltmaPlaniSorumlusuKod;
                        eskiKayit.AzaltmaPlaniNotlar = gelenNesne.AzaltmaPlaniNotlar;
                        eskiKayit.ErtelemeBaslangicTarihi = gelenNesne.ErtelemeBaslangicTarihi;
                        eskiKayit.ErtelemeBitisTarihi = gelenNesne.ErtelemeBitisTarihi;
                        eskiKayit.ErtelemeNot = gelenNesne.ErtelemeNot;
                        //eskiKayit.SorumluKoordinatorlukKod = gelenNesne.SorumluKoordinatorlukKod;
                        //eskiKayit.SorumluBirimKod = gelenNesne.SorumluBirimKod;
                    }
                    else
                    {
                        eskiKayit.Durum = gelenNesne.Durum;

                        if (gelenNesne.BaslangicTarihi.HasValue)
                            eskiKayit.BaslangicTarihi = gelenNesne.BaslangicTarihi;
                        if (gelenNesne.BitisTarihi.HasValue)
                            eskiKayit.BitisTarihi = gelenNesne.BitisTarihi;

                        eskiKayit.AzaltmaPlani = gelenNesne.AzaltmaPlani;
                        eskiKayit.MevcutDurum = gelenNesne.MevcutDurum;
                        eskiKayit.TamamlamaYuzdesi = gelenNesne.TamamlamaYuzdesi;
                        eskiKayit.AzaltmaPlaniSorumlusuKod = gelenNesne.AzaltmaPlaniSorumlusuKod;
                        eskiKayit.AzaltmaPlaniNotlar = gelenNesne.AzaltmaPlaniNotlar;
                        eskiKayit.ErtelemeBaslangicTarihi = gelenNesne.ErtelemeBaslangicTarihi;
                        eskiKayit.ErtelemeBitisTarihi = gelenNesne.ErtelemeBitisTarihi;
                        eskiKayit.ErtelemeNot = gelenNesne.ErtelemeNot;
                        eskiKayit.SorumluKoordinatorlukKod = gelenNesne.SorumluKoordinatorlukKod;
                        eskiKayit.SorumluBirimKod = gelenNesne.SorumluBirimKod;
                    }

                    eskiKayit.KontrolEdildi = gelenNesne.KontrolEdildi;

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    tarihce.Durum = (int)gelenNesne.Durum;

                    islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);
                }

                if (!onayli)
                {
                    await _serviceRisk.SilAsync(kullanan, gelenNesne.Kod);

                    if (riskler != null)
                    {
                        foreach (var item in riskler)
                        {
                            item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                            await _serviceRisk.KaydetAsync(kullanan, item);
                        }
                    }

                    await _serviceIsbirligiBirim.SilAsync(kullanan, gelenNesne.Kod);

                    if (isbirligiBirimler != null)
                    {
                        foreach (var item in isbirligiBirimler)
                        {
                            item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                            await _serviceIsbirligiBirim.KaydetAsync(kullanan, item);
                        }
                    }

                    await _serviceIliski.SilAsync(kullanan, gelenNesne.Kod);

                    if (iliskiliPlanlar != null)
                    {
                        foreach (var item in iliskiliPlanlar)
                        {
                            item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                            await _serviceIliski.KaydetAsync(kullanan, item);
                        }
                    }
                }




                //var eskiKayitNotlar = await _serviceNot.ListeleAsync(kullanan, gelenNesne.Kod);
                //var eskiKayitNotlarS = Arac.JSONSerialize(eskiKayitNotlar.Liste);
                //tarihce.EskiDeger = tarihce.EskiDeger.Replace("\"Notlar\":null", "\"Notlar\":" + eskiKayitNotlarS);

                await _serviceNot.SilAsync(kullanan, gelenNesne.Kod);

                if (notlar != null)
                {
                    foreach (var item in notlar)
                    {
                        item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                        if (string.IsNullOrWhiteSpace(item.Kod) || item.Kod.StartsWith("dtabloYeni_"))
                        {
                            item.Tarih = DateTime.Now;
                            item.Durum = (int)gelenNesne.MevcutDurum;
                        }

                        item.Kod = "";

                        await _serviceNot.KaydetAsync(kullanan, item);
                    }
                }

                //var yeniKayitNotlarS = Arac.JSONSerialize(notlar);
                //tarihce.YeniDeger = tarihce.YeniDeger.Replace("\"Notlar\":null", "\"Notlar\":" + yeniKayitNotlarS);





                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.RiskAzaltmaPlani;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _unitOfWorkAnahtar.KaydetAsync();

                // Mail şablonu azaltma planını veritabanından okuduğu için bildirim kayıt tamamlandıktan sonra gönderilir.
                if (yeniKayit && isbirligiBirimler != null && isbirligiBirimler.Count > 0)
                {
                    var formBildirim = new BildirimSistemi() { Liste = new List<BildirimSistemi>() };

                    foreach (var item in isbirligiBirimler)
                    {
                        var b = new BildirimSistemi
                        {
                            Islem = EnumBildirimSistemiIslem.Yeni,
                            BelgeKod = gelenNesne.Kod,
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani,
                            KoordinatorlukKod = item.KoordinatorlukKod,
                            Birimler = new List<string>()
                        };

                        if (!string.IsNullOrWhiteSpace(item.BirimKod) && item.BirimKod != "undefined")
                            b.Birimler.Add(item.BirimKod);

                        formBildirim.Liste.Add(b);
                    }

                    await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                }

                if (!onayli)
                {
                    //Bildirim sisteminden kaydı sil
                    var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani, BelgeKod = gelenNesne.Kod, });
                }
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın durumunu değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne)
        {

            string hata = "";

            bool onayTamamlandi = false;

            bool riskYonetimiKodIleOku = false;

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
            {
                if (!string.IsNullOrEmpty(gelenNesne.RiskYonetimiKod))
                    riskYonetimiKodIleOku = true;
                else
                    hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";
            }

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();

                List<RiskAzaltmaPlaniRisk> riskler = gelenNesne.Riskler;
                gelenNesne.Riskler = null;

                List<RiskAzaltmaPlaniNot> notlar = gelenNesne.Notlar;
                gelenNesne.Notlar = null;

                List<RiskAzaltmaPlaniIliski> iliskiliPlanlar = gelenNesne.IliskiliPlanlar;
                gelenNesne.IliskiliPlanlar = null;

                List<RiskAzaltmaPlaniIsbirligiBirim> isbirligiBirimler = gelenNesne.IsbirligiBirimler;
                gelenNesne.IsbirligiBirimler = null;

                var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod, "SorumluBirim,SorumluBirim.Koordinatorluk");
                if (riskYonetimiKodIleOku)
                    eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.RiskYonetimiKod == gelenNesne.RiskYonetimiKod);

                //tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                //Durum değişikliğine uygun mu?
                hata = Arac.DurumDegisikligiUygunMu(kullanan, _sharedResource, eskiKayit, gelenNesne);

                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                    eskiKayit.KayitTarihi = DateTime.Now.Date;

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

                    //Tarihçe Başlangıç
                    tarihce.IlgiKod = gelenNesne.Kod;
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskAzaltmaPlani;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = gelenNesne.Durum.Value;
                    if (gelenNesne.Tarihce != null)
                        tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;

                    await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiş

                    var sonuc = await _unitOfWorkAnahtar.KaydetAsync();


                    // Bildirim sisteminden kaydı sil
                    var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani, BelgeKod = eskiKayit.Kod, });

                }
                else
                {


                    if (!string.IsNullOrWhiteSpace(gelenNesne.RiskYonetimiKod) && (gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Onayli))
                    {
                        if (!gelenNesne.BaslangicTarihi.HasValue)
                            eskiKayit.BaslangicTarihi = gelenNesne.BaslangicTarihi;
                        if (!gelenNesne.BitisTarihi.HasValue)
                            eskiKayit.BitisTarihi = gelenNesne.BitisTarihi;

                        if (!string.IsNullOrWhiteSpace(gelenNesne.AzaltmaPlani))
                            eskiKayit.AzaltmaPlani = gelenNesne.AzaltmaPlani;

                        if (gelenNesne.MevcutDurum > 0)
                            eskiKayit.MevcutDurum = gelenNesne.MevcutDurum;

                        eskiKayit.TamamlamaYuzdesi = gelenNesne.TamamlamaYuzdesi;
                        if ((gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Onayli) && !string.IsNullOrWhiteSpace(gelenNesne.AzaltmaPlaniSorumlusuKod))
                            eskiKayit.AzaltmaPlaniSorumlusuKod = gelenNesne.AzaltmaPlaniSorumlusuKod;

                        eskiKayit.AzaltmaPlaniNotlar = gelenNesne.AzaltmaPlaniNotlar;
                        eskiKayit.ErtelemeBaslangicTarihi = gelenNesne.ErtelemeBaslangicTarihi;
                        eskiKayit.ErtelemeBitisTarihi = gelenNesne.ErtelemeBitisTarihi;
                        eskiKayit.ErtelemeNot = gelenNesne.ErtelemeNot;

                        eskiKayit.SorumluKoordinatorlukKod = gelenNesne.SorumluKoordinatorlukKod;
                        eskiKayit.SorumluBirimKod = gelenNesne.SorumluBirimKod;

                        tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    }
                    else
                        tarihce.EskiDeger = "";

                    eskiKayit.Durum = gelenNesne.Durum;


                    //BildirimSistemi Başlangıç
                    if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi || gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Reddedildi)
                    {
                        riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.RiskAzaltmaPlani.Kod == gelenNesne.Kod, "Koordinatorluk,Birim");

                        var bs = new BildirimSistemi
                        {
                            BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani,
                            BelgeKod = eskiKayit.Kod,
                            KoordinatorlukKod = riskEvreni.KoordinatorlukKod,
                            BirimKod = riskEvreni.BirimKod,
                            IslemYapanKod = kullanan.PersonelKod,
                            IslemTarihi = DateTime.Now,
                        };


                        //var sonYetki = await _serviceBildirimSistemi.SonYetkiVerAsync(kullanan, bs);

                        //if (sonYetki.Durum == (int)ENUMDurum.GeriGonderildi)
                        //{
                        //    if (sonYetki.OnaylayacakYetki == kullanan.AktifRolKod
                        //        && (kullanan.KoordinatorlukKod == "" || sonYetki.KoordinatorlukKod == kullanan.KoordinatorlukKod)
                        //        && (kullanan.BirimKod == "" || sonYetki.BirimKod == kullanan.BirimKod))
                        //        gelenNesne.Durum = (int)ENUMDurum.Onayli;
                        //}


                        if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                        {
                            //bs.OnaylayacakYetki = Arac.UstYetkiVer(kullanan, (EnumKoordinatorlukTur)riskEvreni.Koordinatorluk.Tur);
                            bs.Durum = (int)ENUMDurum.OnayaGonderdi;

                            if (!string.IsNullOrWhiteSpace(riskEvreni.BirimKod))
                                bs.OnaylayacakYetki = Arac.UstYetkiVer(kullanan.AktifRolKod, (EnumKoordinatorlukTur)riskEvreni.Koordinatorluk.Tur);
                            else
                            {
                                //Birimi olmayan koordinatörlük ile işlem yapıldığında
                                bs.OnaylayacakYetki = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(kullanan, riskEvreni.Koordinatorluk.Kod, riskEvreni.Koordinatorluk.Tur, true);
                            }


                            if (string.IsNullOrWhiteSpace(bs.OnaylayacakYetki))
                                hata += "<li>" + _sharedResource["Kontrol.DurumDegistir.OnaylayacakYetkiBulunamadi"] + "</li>";
                            else if (bs.OnaylayacakYetki == kullanan.AktifRolKod || bs.OnaylayacakYetki == "BASKAN*")
                            {
                                //Eğer üst yetki aynı kişi ise onaya gönderilmeden onaylansın
                                bs.Durum = (int)ENUMDurum.Onayli;
                                gelenNesne.Durum = (int)ENUMDurum.Onayli;
                                eskiKayit.Durum = (int)ENUMDurum.Onayli;
                            }
                        }
                        else if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                        {
                            bs.OnaylayacakYetki = kullanan.AktifRolKod;
                            bs.Durum = (int)ENUMDurum.Onayli;

                            var onaylayacakYetki = await _serviceBildirimSistemi.OnaylayacakYetkiVerAsync(kullanan, bs);
                            if (!string.IsNullOrWhiteSpace(onaylayacakYetki))
                            {
                                eskiKayit.Durum = (int)ENUMDurum.OnayaGonderdi;
                                bs.OnaylayacakUstYetki = onaylayacakYetki;

                                //Eğer üst yetki aynı kişi ise onaya gönderilmeden onaylansın
                                if (onaylayacakYetki == kullanan.AktifRolKod)
                                    bs.OnaylayacakUstYetki = "";
                            }

                            if (string.IsNullOrWhiteSpace(onaylayacakYetki) && string.IsNullOrWhiteSpace(bs.OnaylayacakUstYetki))
                            {
                                if (bs.OnaylayacakYetki != "BIRIMAMIRI")
                                {
                                    if (eskiKayit.SorumluBirim.Koordinatorluk.Kod != riskEvreni.KoordinatorlukKod || eskiKayit.SorumluBirim.Kod != riskEvreni.BirimKod)
                                    {
                                        //Sorumlu birim değiş ise sorumlu birim amirine onaya gönder
                                        eskiKayit.Durum = (int)ENUMDurum.OnayaGonderdi;
                                        bs.OnaylayacakUstYetki = "BIRIMAMIRI";
                                        bs.KoordinatorlukKod = eskiKayit.SorumluBirim.Koordinatorluk.Kod;
                                        bs.BirimKod = eskiKayit.SorumluBirim.Kod;
                                    }
                                }
                                else
                                {
                                    //Onay Tamamlandı. Azaltma planı sorumlusu kontrol ediliyor.
                                    if (string.IsNullOrWhiteSpace(eskiKayit.AzaltmaPlaniSorumlusuKod))
                                    {
                                        eskiKayit.AzaltmaPlaniSorumlusuKod = kullanan.PersonelKod;
                                        //hata += "<li>" + _sharedResource["Kontrol.DurumDegistir.AzaltmaPlaniSorumlusuBulunamadi"] + "</li>";
                                    }
                                }
                            }

                        }
                        else if (gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                        {
                            eskiKayit.Durum = (int)ENUMDurum.GeriGonderildi; //Geri gönderildi
                            bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kişi yok
                            bs.OnaylayacakYetki = "-";

                            //eskiKayit.AzaltmaPlaniSorumlusuKod = "";

                            ////Geri gönderilen azaltma planı ilk hazırlayana düşecek. Birim amiri onaylamışsa bilgilendirme mail gönderilecek.
                            //var geriGonderilecekYetki = await _serviceBildirimSistemi.GeriGonderilecekYetkiVerAsync(kullanan, bs);
                            //if (geriGonderilecekYetki != "")
                            //{
                            //    var mailGonderilecekKisi = "";

                            //    var ilgiliTarihce = await _serviceTarihce.ListeleAsync(eskiKayit.Kod);
                            //    if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                            //    {
                            //        foreach (Tarihce t in ilgiliTarihce.Liste)
                            //        {
                            //            if (t.Durum == (int)ENUMDurum.Onayli)
                            //            {
                            //                mailGonderilecekKisi = t.IslemYapanKod;
                            //                break;
                            //            }
                            //        }
                            //    }

                            //    //Mail Gönder: Onay verilmiş bir adım varsa. Azaltma planının hazırlyan kişiye geri gönderildiği mail olarak bildirilir. 
                            //    if (!string.IsNullOrWhiteSpace(mailGonderilecekKisi))
                            //    {
                            //        var formBildirim = new BildirimSistemi()
                            //        {
                            //            Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                            //            BelgeKod = gelenNesne.Kod,
                            //            BelgeTipi = (int)EnumTarihceIslemTur.RisklerinYonetilmesiDurumDegisti,
                            //            MailGonderilecekKisi = mailGonderilecekKisi
                            //        };

                            //        Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                            //    }
                            //    //Mail Gönder Bitiş
                            //}



                            ////Koordinatör birim amirine geri gönderirse birim amirine bilgilendirme mail'i gidecek. Azaltma Planı sorumlusuna risk geri gönderilecek
                            //if (geriGonderilecekYetki == "BIRIMAMIRI")
                            //{
                            //    var ilgiliTarihce = await _serviceTarihce.ListeleAsync(eskiKayit.Kod);
                            //    if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                            //    {
                            //        foreach (Tarihce t in ilgiliTarihce.Liste)
                            //        {
                            //            //if (t.Durum == (int)ENUMDurum.OnayaGonderdi && t.IlgiliRol.Contains("KOORDINATOR"))
                            //            bs.OnaylayacakYetki = t.IslemYapanRol;
                            //            break;
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //    bs.Durum = (int)ENUMDurum.GeriGonderildi;
                            //    bs.OnaylayacakYetki = geriGonderilecekYetki;
                            //}

                            //var geriGonderilecekYetki = await _serviceBildirimSistemi.GeriGonderilecekYetkiVerAsync(kullanan, bs);
                            //if (!string.IsNullOrWhiteSpace(geriGonderilecekYetki))
                            //{
                            //    eskiKayit.Durum = (int)ENUMDurum.GeriGonderildi; //Bir önceki onaylaya geri gönderildi
                            //    bs.Durum = (int)ENUMDurum.GeriGonderildi;
                            //    bs.OnaylayacakYetki = geriGonderilecekYetki;
                            //}
                            //else
                            //{
                            //    bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kişi yok
                            //    bs.OnaylayacakYetki = "-";
                            //}
                        }

                        if (hata != "")
                            return new Sonuc(ENUMIslemDurum.Uyari, hata);

                        var sonucBildirimSistemi = await _serviceBildirimSistemi.KaydetAsync(kullanan, bs);

                        if (!sonucBildirimSistemi.IslemSonuc)
                            return new Sonuc(ENUMIslemDurum.Hata, sonucBildirimSistemi.Mesaj);

                        string mailGonderilecekYetki = bs.Durum == (int)ENUMDurum.OnayaGonderdi
                            ? bs.OnaylayacakYetki
                            : bs.OnaylayacakUstYetki;

                        if (!string.IsNullOrWhiteSpace(mailGonderilecekYetki)
                            && mailGonderilecekYetki != "-"
                            && mailGonderilecekYetki != "BASKAN*"
                            && mailGonderilecekYetki != kullanan.AktifRolKod)
                        {
                            await _serviceBildirimSistemi.MailGonderAsync(kullanan, new BildirimSistemi
                            {
                                Islem = EnumBildirimSistemiIslem.OnayBekliyor,
                                BelgeKod = eskiKayit.Kod,
                                BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani,
                                KoordinatorlukKod = bs.KoordinatorlukKod,
                                BirimKod = bs.BirimKod,
                                OnaylayacakYetki = mailGonderilecekYetki
                            });
                        }


                        if (bs.OnaylayacakYetki != kullanan.AktifRolKod && bs.OnaylayacakYetki != "-")
                            tarihce.IlgiliRol = bs.OnaylayacakYetki;
                        else if (bs.OnaylayacakUstYetki != kullanan.AktifRolKod)
                            tarihce.IlgiliRol = bs.OnaylayacakUstYetki;

                    }
                    //BildirimSistemi Bitiş

                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
                        onayTamamlandi = true;

                    RiskAzaltmaPlani islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);

                    tarihce.IlgiKod = gelenNesne.Kod;
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskAzaltmaPlani;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = gelenNesne.Durum.Value;
                    if (gelenNesne.Tarihce != null)
                        tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;

                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                    {

                        if (riskler != null)
                        {
                            var sonucRiskler = await _serviceRisk.SilAsync(kullanan, gelenNesne.Kod);
                            if (sonucRiskler.IslemSonuc)
                                foreach (var item in riskler)
                                {
                                    RiskAzaltmaPlaniRisk giden = new RiskAzaltmaPlaniRisk();
                                    item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                                    await _serviceRisk.KaydetAsync(kullanan, item);
                                }
                        }

                        if (notlar != null)
                        {
                            var sonucNotlar = await _serviceNot.SilAsync(kullanan, gelenNesne.Kod);
                            if (sonucNotlar.IslemSonuc)
                                foreach (var item in notlar)
                                {
                                    item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                                    if (string.IsNullOrWhiteSpace(item.Kod) || item.Kod.StartsWith("dtabloYeni_"))
                                    {
                                        item.Tarih = DateTime.Now;
                                        item.Durum = (int)gelenNesne.MevcutDurum;
                                    }
                                    item.Kod = "";
                                    await _serviceNot.KaydetAsync(kullanan, item);
                                }
                        }

                        if (isbirligiBirimler != null)
                        {
                            var sonucIsbirligiBirimler = await _serviceIsbirligiBirim.SilAsync(kullanan, gelenNesne.Kod);
                            if (sonucIsbirligiBirimler.IslemSonuc)
                                foreach (var item in isbirligiBirimler)
                                {
                                    RiskAzaltmaPlaniIsbirligiBirim giden = new RiskAzaltmaPlaniIsbirligiBirim();
                                    item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                                    await _serviceIsbirligiBirim.KaydetAsync(kullanan, item);
                                }
                        }

                        if (iliskiliPlanlar != null)
                        {
                            var sonucIliskiliPlanlar = await _serviceIliski.SilAsync(kullanan, gelenNesne.Kod);
                            if (sonucIliskiliPlanlar.IslemSonuc)
                                foreach (var item in iliskiliPlanlar)
                                {
                                    RiskAzaltmaPlaniIliski giden = new RiskAzaltmaPlaniIliski();
                                    item.RiskAzaltmaPlaniKod = gelenNesne.Kod;

                                    await _serviceIliski.KaydetAsync(kullanan, item);
                                }
                        }

                    }

                    var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                    await _unitOfWorkAnahtar.KaydetAsync();

                    if (onayTamamlandi)
                    {
                        var onaylananPlan = await _unitOfWorkAnahtar.KayitGetirAsync(
                            c => c.Kod == eskiKayit.Kod,
                            "IsbirligiBirimler");

                        var alicilar = new List<BildirimSistemi>();

                        if (!string.IsNullOrWhiteSpace(onaylananPlan.SorumluKoordinatorlukKod)
                            && !string.IsNullOrWhiteSpace(onaylananPlan.SorumluBirimKod))
                        {
                            alicilar.Add(new BildirimSistemi
                            {
                                KoordinatorlukKod = onaylananPlan.SorumluKoordinatorlukKod,
                                BirimKod = onaylananPlan.SorumluBirimKod,
                                OnaylayacakYetki = "BIRIMAMIRI"
                            });
                        }

                        if (!string.IsNullOrWhiteSpace(onaylananPlan.AzaltmaPlaniSorumlusuKod))
                        {
                            alicilar.Add(new BildirimSistemi
                            {
                                MailGonderilecekKisi = onaylananPlan.AzaltmaPlaniSorumlusuKod
                            });
                        }

                        if (onaylananPlan.IsbirligiBirimler != null)
                        {
                            foreach (var isbirligiBirimi in onaylananPlan.IsbirligiBirimler)
                            {
                                var onaylayacakYetki = !string.IsNullOrWhiteSpace(isbirligiBirimi.BirimKod)
                                    ? "BIRIMAMIRI"
                                    : await _serviceViewBirim.KoordinatorlukYetkiTipiBul(
                                        kullanan,
                                        isbirligiBirimi.KoordinatorlukKod,
                                        0,
                                        false);

                                if (!string.IsNullOrWhiteSpace(onaylayacakYetki))
                                {
                                    alicilar.Add(new BildirimSistemi
                                    {
                                        KoordinatorlukKod = isbirligiBirimi.KoordinatorlukKod,
                                        BirimKod = isbirligiBirimi.BirimKod,
                                        OnaylayacakYetki = onaylayacakYetki
                                    });
                                }
                            }
                        }

                        foreach (var alici in alicilar)
                        {
                            alici.Islem = EnumBildirimSistemiIslem.Bilgilendirme;
                            alici.BelgeKod = eskiKayit.Kod;
                            alici.BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlaniOnaylandi;
                            alici.Aciklama = Arac.DurumAdGetir(oncekiDurum.Value) + " >> " + Arac.DurumAdGetir(eskiKayit.Durum.Value);
                        }

                        if (alicilar.Count > 0)
                        {
                            await _serviceBildirimSistemi.MailGonderAsync(kullanan, new BildirimSistemi
                            {
                                Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                                BelgeKod = eskiKayit.Kod,
                                BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlaniOnaylandi,
                                Aciklama = Arac.DurumAdGetir(oncekiDurum.Value) + " >> " + Arac.DurumAdGetir(eskiKayit.Durum.Value),
                                Liste = alicilar
                            });
                        }
                    }


                    if (gelenNesne.Durum == (int)ENUMDurum.Onayli && !string.IsNullOrEmpty(tarihce.DegisenDeger) && riskEvreni != null)
                    {
                        //Mail Gönder: Onaycı kişi kayıtta değişiklik yapıp onaylarsa, değişiklik risk sahibine mail olarak bildirilir. 
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
                        //Mail Gönder Bitiş
                    }
                }

                //Mail Gönder: Tüm ekranlar için kaydın durum bilgisi değiştiğinde bilgilendirme maili gidebilir mi? (reddedildi, onaya gönderildi, geri gönderildi vs.). 
                if (oncekiDurum != eskiKayit.Durum && !onayTamamlandi)
                {
                    if (riskEvreni == null)
                        riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.RiskAzaltmaPlani.Kod == gelenNesne.Kod, "Koordinatorluk,Birim");

                    var formBildirim = new BildirimSistemi()
                    {
                        Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                        BelgeKod = gelenNesne.Kod,
                        BelgeTipi = (int)EnumTarihceIslemTur.RisklerinYonetilmesiDurumDegisti,
                        MailGonderilecekKisi = riskEvreni.RiskSahibiKod,
                        Aciklama = Arac.DurumAdGetir(oncekiDurum.Value) + " >> " + Arac.DurumAdGetir(eskiKayit.Durum.Value)
                    };

                    Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);
                }
                //Mail Gönder Bitiş
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, onayTamamlandi ? _sharedResource["Bildirim.RiskYonetimiOnaylandi"] : _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilerden güncel değerleri değişen alanları döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// string türünde değişen alanları döndürür
        /// </returns>
        public async Task<string> DegisenAlanlariGetirAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne)
        {
            var degisenAlanlar = "";
            var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

            if (gelenNesne.BaslangicTarihi.HasValue && eskiKayit.BaslangicTarihi != gelenNesne.BaslangicTarihi)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.BaslangicTarihi"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            if (gelenNesne.BitisTarihi.HasValue && eskiKayit.BitisTarihi != gelenNesne.BitisTarihi)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.BitisTarihi"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.AzaltmaPlani != gelenNesne.AzaltmaPlani)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.AzaltmaPlani"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";

            if (eskiKayit.SorumluKoordinatorlukKod != gelenNesne.SorumluKoordinatorlukKod)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.SorumluKoordinatorluk"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.SorumluBirimKod != gelenNesne.SorumluBirimKod)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.SorumluBirim"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";

            if (eskiKayit.IsbirligiBirimler != null || gelenNesne.IsbirligiBirimler != null)
            {
                var eski = "";
                if (eskiKayit.IsbirligiBirimler != null)
                    foreach (var item in eskiKayit.IsbirligiBirimler)
                        eski += item.Kod;

                var yeni = "";
                if (gelenNesne.IsbirligiBirimler != null)
                    foreach (var item in gelenNesne.IsbirligiBirimler)
                        yeni += item.Kod;

                if (eski != yeni)
                    degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.BirimlerIsbirligi"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            }


            if (eskiKayit.MevcutDurum != gelenNesne.MevcutDurum)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.MevcutDurum"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.TamamlamaYuzdesi != gelenNesne.TamamlamaYuzdesi)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.TamamlanmaYuzdesi"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.AzaltmaPlaniNotlar != gelenNesne.AzaltmaPlaniNotlar)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.AzaltmaPlaniNotlar"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.ErtelemeBaslangicTarihi != gelenNesne.ErtelemeBaslangicTarihi || eskiKayit.ErtelemeBitisTarihi != gelenNesne.ErtelemeBitisTarihi)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.ErtelenmisDegistirilmisZamanPlani"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";
            if (eskiKayit.ErtelemeNot != gelenNesne.ErtelemeNot)
                degisenAlanlar += "<li>" + _sharedResource["RiskAzaltmaPlani.Alan.ErtelemeNot"] + " " + _sharedResource["RiskAzaltmaPlani.AlanindaGuncellemeYapildi"] + " </li>";

            return degisenAlanlar;
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="riskAzaltmaPlani"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, RiskAzaltmaPlani riskAzaltmaPlani)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(riskAzaltmaPlani?.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                //Bildirim sisteminden kaydı sil
                var sonucBildirimSistemi = await _serviceBildirimSistemi.SilAsync(kullanan, new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlani, BelgeKod = riskAzaltmaPlani.Kod, });

                await _serviceRisk.SilAsync(kullanan, riskAzaltmaPlani.Kod);
                await _serviceIsbirligiBirim.SilAsync(kullanan, riskAzaltmaPlani.Kod);
                await _serviceIliski.SilAsync(kullanan, riskAzaltmaPlani.Kod);

                await _unitOfWorkAnahtar.SilAsync(d => d.Kod == riskAzaltmaPlani.Kod);
                await _unitOfWorkAnahtar.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen azaltma planı sorunlusunu değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> AzaltmaPlaniSorumlusuDegistirAsync(KullaniciDto kullanan, RiskAzaltmaPlani gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.AzaltmaPlaniSorumlusuKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.AzaltmaPlaniSorumlusuBos"] + "</li>";

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

                var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod, "SorumluBirim,SorumluBirim.Koordinatorluk");

                if (kullanan.AktifRolKod == "BIRIMAMIRI")
                {
                    if (eskiKayit.SorumluKoordinatorlukKod != kullanan.KoordinatorlukKod)
                    {
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + " / " + eskiKayit.SorumluBirim.Koordinatorluk.Adi + "</li>";
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);
                    }
                    else if (eskiKayit.SorumluBirimKod != kullanan.BirimKod)
                    {
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + " / " + eskiKayit.SorumluBirim.Adi + "</li>";
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);
                    }
                }


                Tarihce tarihce = new Tarihce();

                tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                eskiKayit.AzaltmaPlaniSorumlusuKod = gelenNesne.AzaltmaPlaniSorumlusuKod;

                var islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);

                //Tarihçe Başlangıç
                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.RiskAzaltmaPlani;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = (int)ENUMDurum.AzaltmaPlaniSorumlusuDegisti;

                tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);

                await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                //Tarihçe Bitiş

                var sonuc = await _unitOfWorkAnahtar.KaydetAsync();

                await _serviceBildirimSistemi.MailGonderAsync(kullanan, new BildirimSistemi
                {
                    Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                    BelgeKod = eskiKayit.Kod,
                    BelgeTipi = (int)EnumTarihceIslemTur.RiskAzaltmaPlaniSorumlusuDegisti,
                    MailGonderilecekKisi = eskiKayit.AzaltmaPlaniSorumlusuKod
                });

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }
    }
}
