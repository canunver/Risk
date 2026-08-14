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
using System.Linq.Dynamic.Core;
using System.Security.Policy;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// AnahtarRiskGostergesi işlemlerinin yapıldığı servis
    /// </summary>
    public class AnahtarRiskGostergesiService : IAnahtarRiskGostergesiService
    {
        /// <summary>
        /// IUnitOfWork<RiskEvreni> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskEvreni> _unitOfWorkRiskEvreni;
        /// <summary>
        /// IUnitOfWork<AnahtarRiskGostergesi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<AnahtarRiskGostergesi> _unitOfWorkAnahtar;
        /// <summary>
        /// IAnahtarRiskGostergesiDonemService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IAnahtarRiskGostergesiDonemService _serviceDonem;
        /// <summary>
        /// IBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBildirimSistemiService _serviceBildirimSistemi;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;
        /// <summary>
        /// ITarihceService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.AnahtarRiskGostergesiService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWorkRiskEvreni"></param>
        /// <param name="unitOfWorkAnahtar"></param>
        /// <param name="serviceDonem"></param>
        /// <param name="serviceCTE"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public AnahtarRiskGostergesiService(IUnitOfWork<RiskEvreni> unitOfWorkRiskEvreni,
                                            IUnitOfWork<AnahtarRiskGostergesi> unitOfWorkAnahtar,
                                            IAnahtarRiskGostergesiDonemService serviceDonem,
                                            IBildirimSistemiService serviceBildirimSistemi,
                                            ICTEKoordinatorlukService serviceCTE,
                                            ITarihceService serviceTarihce,
                                            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWorkRiskEvreni = unitOfWorkRiskEvreni;
            _unitOfWorkAnahtar = unitOfWorkAnahtar;
            _serviceDonem = serviceDonem;
            _serviceBildirimSistemi = serviceBildirimSistemi;
            _serviceCTE = serviceCTE;
            _serviceTarihce = serviceTarihce;
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
                var kayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kod, "Koordinatorluk,Birim,Surec,AltSurec,RiskKategoriler.RiskKategori, AnahtarRiskGostergesi, AnahtarRiskGostergesi.Donemler");

                if (kayit != null)
                {
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
        /// Istemciden parametre ile talep edilen kaydın kod bilgisini döndüren metod
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
                var kayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == kod);

                if (kayit != null)
                    return new Sonuc(ENUMIslemDurum.Basarili, "", kayit.RiskEvreniKod);
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
            var kayitlar = await _unitOfWorkRiskEvreni.ListeleAsync(null, o => o.Kod, k => k.AnahtarRiskGostergesi);
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
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleOneriAsync(KullaniciDto kullanan, RiskEvreni kriter)
        {
            if (kriter.AnahtarRiskGostergesi == null)
                kriter.AnahtarRiskGostergesi = new AnahtarRiskGostergesi();

            var selectData = await _unitOfWorkRiskEvreni.SorguHazirlaAsync(null, "Koordinatorluk,Birim,Surec,AltSurec,RiskKategoriler.RiskKategori, AnahtarRiskGostergesi, AnahtarRiskGostergesi.Donemler");

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kriter.KoordinatorlukKod);
            if (!string.IsNullOrWhiteSpace(kriter.BirimKod))
                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.BirimKod == kriter.BirimKod);
            if (!string.IsNullOrWhiteSpace(kriter.AnahtarRiskGostergesi.Adi))
                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Adi == kriter.AnahtarRiskGostergesi.Adi);

            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);
            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => (a.JenerikRisk == 1 && (a.AnahtarRiskGostergesi != null || !string.IsNullOrWhiteSpace(a.AnahtarRiskGostergesiAdi)))
            || (a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli && a.AnahtarRiskGostergesi.RiskEvreniKod != null && a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Aktif));
            //selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli);
            //selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.RiskEvreniKod != null);

            selectData = selectData.OrderBy("AnahtarRiskGostergesi.Adi");

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
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWorkRiskEvreni.SorguHazirlaAsync(null, "Koordinatorluk,Birim,Surec,AltSurec,RiskKategoriler.RiskKategori, AnahtarRiskGostergesi, AnahtarRiskGostergesi.Donemler");

            //P2-234-(Risk Kaydı/Anahtar Risk Göstergesi/Risklerin Değerlendirilmesi/Risklerin Yönetilmesi) Uzman tüm riskleri görebilir. 05.04.2023 HÖ
            //selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskSahibiKod == kullanan.PersonelKod);

            selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);
            //selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesiAdi != "");

            //P2-234-(Risk Kaydı/Anahtar Risk Göstergesi/Risklerin Değerlendirilmesi/Risklerin Yönetilmesi) Uzman tüm riskleri görebilir. 05.04.2023 HÖ
            /*
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

                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, predicate);
                //****************************************************************************************
            */

            aramaDegeri = aramaDegeri.Replace("AnahtarRiskGostergesiAdiSon", "AnahtarRiskGostergesiAdi");

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                RiskEvreni aramaObj = Arac.DataTablesAramaNesne<RiskEvreni>(new RiskEvreni(), aramaDegeri);

                if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli);

                //Anahtar Risk Göstergesi boş olan kayıtları aranmak için
                if (aramaObj.SorguAnahtarRiskGostergesi == "1")
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => (a.AnahtarRiskGostergesiAdi == null || a.AnahtarRiskGostergesiAdi == "") && (a.AnahtarRiskGostergesi.Adi == null || a.AnahtarRiskGostergesi.Adi == ""));
                else if (aramaObj.SorguAnahtarRiskGostergesi == "2")
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => (a.AnahtarRiskGostergesiAdi != null && a.AnahtarRiskGostergesiAdi != "") || (a.AnahtarRiskGostergesi.Adi != null && a.AnahtarRiskGostergesi.Adi != ""));


                if (!string.IsNullOrWhiteSpace(aramaObj.AnahtarRiskGostergesiAdiSon))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesiAdi.Contains(aramaObj.AnahtarRiskGostergesiAdiSon) || a.AnahtarRiskGostergesi.Adi.Contains(aramaObj.AnahtarRiskGostergesiAdiSon));
                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.RiskNo))
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskNo == aramaObj.RiskNo);
                //if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(aramaObj.Adi));
                //if (aramaObj.Durum > 0)
                //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);

                //if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
                //    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Aktif);
                //else
                //    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Aktif || a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Pasif);

                if (Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
                {
                    if (aramaObj.Durum > 0)
                        selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Durum == aramaObj.Durum);
                }
                else
                    selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Aktif);
            }
            else
            {
                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.RiskYonetimi.Durum == (int)ENUMDurum.Onayli);

                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.Surec.Adi.Contains(dataTablesParam.searchValue)
                                                               || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                               || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                               || a.RiskAdi.Contains(dataTablesParam.searchValue)
                                                               || a.RiskTanimi.Contains(dataTablesParam.searchValue)
                                                               || a.AnahtarRiskGostergesiAdi.Contains(dataTablesParam.searchValue)
                                                               || a.RiskNo.Contains(dataTablesParam.searchValue));

                //selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesiAdi.Contains(dataTablesParam.searchValue));

                selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Aktif);
                //    if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
                //        selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Aktif);
                //    else
                //        selectData = await _unitOfWorkRiskEvreni.KosulEkleAsync(selectData, a => a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Aktif || a.AnahtarRiskGostergesi.Durum == (int)ENUMDurum.Pasif);
            }

            if (dataTablesParam.sortColumn == "AnahtarRiskGostergesiAdiSon")
                dataTablesParam.sortColumn = "AnahtarRiskGostergesiAdi";

            var jsonData = Arac.DataTablesJsonData(selectData, dataTablesParam);

            try
            {
                if (kullanan.AktifRolKod != "RISKSEKRETARYASI")
                {
                    dynamic data = jsonData.GetType().GetProperty("data").GetValue(jsonData, null);

                    foreach (RiskEvreni item in data)
                    {
                        item.KontrolPasifYapGosterme = 1;
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, AnahtarRiskGostergesi gelenNesne)
        {
            AnahtarRiskGostergesi islemYapilan = new AnahtarRiskGostergesi();

            string hata = "";

            if (string.IsNullOrEmpty(gelenNesne.RiskEvreniKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskEvreniKodAlaniBos"] + "</li>";
            if (gelenNesne.YesilDeger == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.YesilDegerAlaniBos"] + "</li>";
            if (gelenNesne.KirmiziDeger == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.KirmiziDegerAlaniBos"] + "</li>";

            if (!string.IsNullOrEmpty(gelenNesne.RiskEvreniKod))
            {
                var eskiKayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod);

                if (eskiKayit.RiskSahibiKod != kullanan.PersonelKod)
                    hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";
            }

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var durum = gelenNesne.DurumDeger;

                var donemler = gelenNesne.Donemler;
                gelenNesne.Donemler = null;

                Tarihce tarihce = new Tarihce();

                var bildirimDurum = 0;
                var bildirimAciklama = "";

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    gelenNesne.Durum = (int)ENUMDurum.Aktif;

                    tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                    tarihce.Durum = (int)ENUMDurum.Aktif;

                    islemYapilan = await _unitOfWorkAnahtar.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod, "Donemler");

                    var kontrolKayit = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod, "RiskYonetimi");
                    //Kayıtlı bir bilgi ancak sahibi tarafından değiştirilebilir
                    if (kontrolKayit.RiskSahibiKod != kullanan.PersonelKod)
                    {
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.AnahtarRiskGostergesiYetkinizYok"] + "</li>";
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);
                    }

                    //ARG değeri kırmızı değerin üzerine çıkarsa ilgili merkez/il koordinatörüne bildirim düşer. (Detay açıklamada)
                    if (kontrolKayit?.RiskYonetimi?.Durum == (int)ENUMDurum.Onayli && durum == "KIRMIZI" && eskiKayit.DurumDeger != "KIRMIZI") //Kırmızı renge dönüştü
                    {
                        bildirimDurum = (int)ENUMBildirimSistemiDurum.ARGDegeriKirmiziDegerinUzerineCikti;
                        bildirimAciklama = "{RiskNo} numaralı riskin anahtar risk göstergesi durumu " + durum + " değere çıkmıştır.";
                    }
                    //----------------------------------------------------------------------------------------------------

                    eskiKayit.Donemler = null;


                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                    eskiKayit.YesilDeger = gelenNesne.YesilDeger;
                    eskiKayit.KirmiziDeger = gelenNesne.KirmiziDeger;
                    eskiKayit.HedefDeger = gelenNesne.HedefDeger;
                    eskiKayit.VeriDayanagi = gelenNesne.VeriDayanagi;
                    eskiKayit.Adi = gelenNesne.Adi;

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    tarihce.Durum = (int)ENUMDurum.Aktif;

                    islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);
                }

                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.AnahtarRiskGostergesi;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);


                if (donemler != null)
                {
                    await _serviceDonem.SilAsync(kullanan, new AnahtarRiskGostergesiDonem() { AnahtarRiskGostergesiKod = gelenNesne.Kod });

                    foreach (var donem in donemler)
                    {
                        var giden = new AnahtarRiskGostergesiDonem();
                        donem.AnahtarRiskGostergesiKod = gelenNesne.Kod;

                        await _serviceDonem.KaydetAsync(kullanan, donem);
                    }
                }

                await _unitOfWorkAnahtar.KaydetAsync();

                //Riks Evreni AnahtarRiskGostergesi alanı güncellensin. https://app.clickup.com/t/86932nx0k
                var riskEvreniArg = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod);
                if (riskEvreniArg != null)
                {
                    riskEvreniArg.AnahtarRiskGostergesiAdi = gelenNesne.Adi;
                    await _unitOfWorkRiskEvreni.GuncelleAsync(riskEvreniArg);
                    await _unitOfWorkRiskEvreni.KaydetAsync();
                }
                //--------------------------------------------------------

                if (bildirimDurum > 0)
                {
                    var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == gelenNesne.RiskEvreniKod, "Koordinatorluk");

                    var bs = new BildirimSistemi
                    {
                        BelgeTipi = (int)EnumTarihceIslemTur.AnahtarRiskGostergesi,
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
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, AnahtarRiskGostergesi gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
                hata = "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();

                var eskiKayit = await _unitOfWorkAnahtar.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Durum değişikliğine uygun mu?
                hata = Arac.DurumDegisikligiUygunMu(kullanan, _sharedResource, eskiKayit, gelenNesne);

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                tarihce.EskiDeger = "";

                eskiKayit.Durum = gelenNesne.Durum;

                var islemYapilan = await _unitOfWorkAnahtar.GuncelleAsync(eskiKayit);

                //Tarihçe Başlangıç
                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.AnahtarRiskGostergesi;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = gelenNesne.Durum;
                if (gelenNesne.Tarihce != null)
                    tarihce.Aciklama = gelenNesne.Tarihce.Aciklama;

                await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                //Tarihçe Bitiş

                await _unitOfWorkAnahtar.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }
    }
}
