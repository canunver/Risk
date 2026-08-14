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

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// RiskIzleme işlemlerinin yapıldığı servis
    /// </summary>
    public class RiskIzlemeService : IRiskIzlemeService
    {
        /// <summary>
        /// IUnitOfWork<RiskEvreni> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskEvreni> _unitOfWork;

        /// <summary>
        /// IViewBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBildirimSistemiService _serviceViewBildirim;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;
        private readonly IStringLocalizer<CustomResource> _sharedResource;


        /// <summary>
        /// <see cref="Risk.net.Services.Functions.RiskIzlemeService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceViewBildirim"></param>
        /// <param name="serviceCTE"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskIzlemeService(IUnitOfWork<RiskEvreni> unitOfWork,
            IViewBildirimSistemiService serviceViewBildirim,
            ICTEKoordinatorlukService serviceCTE,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceViewBildirim = serviceViewBildirim;
            _serviceCTE = serviceCTE;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "Koordinatorluk,Birim,Amac,Hedef,Surec,AltSurec,AnahtarRiskGostergesi,AnahtarRiskGostergesi.Donemler,RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi,RiskYonetimi,RiskYonetimi.Kontroller,RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.SorumluBirim, RiskYonetimi.RiskAzaltmaPlani.Riskler, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler, RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Birim", c => c.RiskKategoriler, c => c.IlIrtibatOfisler, c => c.OlayRaporlar);

                //if (kayit != null)
                //{
                //    kayit.Tarihce = await _serviceTarihce.SonAciklamaGetirAsync(kullanan, kod);

                //    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
                //}
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
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, RiskEvreni kriter)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "RiskKategoriler.RiskKategori,IlIrtibatOfisler.IlIrtibatOfisi", a => a.Birim, a => a.Koordinatorluk);

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kriter.KoordinatorlukKod);
            if (!string.IsNullOrWhiteSpace(kriter.BirimKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kriter.BirimKod);
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
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <param name="onay"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var onay = false;

            List<string> belgeNolar = new List<string>();
            if (onay)
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Amac,Amac.StratejikPlanDonem,RiskKategoriler,RiskKategoriler.RiskKategori,IlIrtibatOfisler,IlIrtibatOfisler.IlIrtibatOfisi", a => a.Birim, a => a.Koordinatorluk, a => a.Amac, a => a.Amac.StratejikPlanDonem, a => a.Hedef, a => a.Surec, a => a.AltSurec);

            //****************************************************************************************
            //Kullanici yetkisine göre koşul
            //****************************************************************************************
            var predicate = PredicateBuilder.True<RiskEvreni>();

            ////Birim koşulunu uygula
            //****************************************************************************************
            string[] birimKosul = await OrtakService.ListeleBirimKosulAsync(kullanan, _serviceCTE, "RISKYONETIMI", "", "");

            //Koordinatörlük
            var koordinatorlukKosul = birimKosul[0].Split(",");//Genel koord birden fazla koordinatörlüğe sahip olduğu için
            if (!string.IsNullOrWhiteSpace(koordinatorlukKosul[0]))
                predicate = predicate.And(a => koordinatorlukKosul.ToArray().Contains(a.KoordinatorlukKod));
            else
                predicate = predicate.And(a => a.KoordinatorlukKod != "");//Tüm koordinatörlükler

            //Birim
            if (!string.IsNullOrWhiteSpace(birimKosul[1]))
                predicate = predicate.And(a => a.BirimKod == birimKosul[1]);

            //Sadece Kendi kayıtları
            predicate = predicate.Or(a => a.RiskSahibiKod == kullanan.PersonelKod);

            selectData = await _unitOfWork.KosulEkleAsync(selectData, predicate);
            //****************************************************************************************

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                RiskEvreni aramaObj = Arac.DataTablesAramaNesne<RiskEvreni>(new RiskEvreni(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskAdi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskAdi.Contains(aramaObj.RiskAdi));
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
                if (aramaObj.SorguGecerlilikTarihi1.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskBaslangicTarihi >= aramaObj.SorguGecerlilikTarihi1);
                if (aramaObj.SorguGecerlilikTarihi2.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.RiskBitisTarihi <= aramaObj.SorguGecerlilikTarihi2);
                if (aramaObj.KayitTarihi1.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitTarihi >= aramaObj.KayitTarihi1);
                if (aramaObj.KayitTarihi2.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitTarihi <= aramaObj.KayitTarihi2);


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
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Surec.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.RiskAdi.Contains(dataTablesParam.searchValue)
                                                                || a.RiskTanimi.Contains(dataTablesParam.searchValue)
                                                                || a.AnahtarRiskGostergesiAdi.Contains(dataTablesParam.searchValue)
                                                                || a.RiskNo.Contains(dataTablesParam.searchValue));

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

    }
}
