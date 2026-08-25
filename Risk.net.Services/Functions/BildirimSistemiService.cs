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
using Risk.net.Data.Migrations;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Quartz.Util;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Newtonsoft.Json;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// BildirimSistemi işlemlerinin yapıldığı servis
    /// </summary>
    public class BildirimSistemiService : IBildirimSistemiService
    {
        /// <summary>
        /// IUnitOfWork<BildirimSistemi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<BildirimSistemi> _unitOfWork;
        /// <summary>
        /// IViewYetkiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewYetkiService _serviceYetki;
        /// <summary>
        /// IViewPersonelService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewPersonelService _servicePersonel;
        /// <summary>
        /// IViewBirimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBirimService _serviceViewBirim;
        /// <summary>
        /// ITarihceService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IMailTarihceService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IMailTarihceService _serviceMailTarihce;
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
        /// IUnitOfWork<OlayRaporlama> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<OlayRaporlama> _unitOfWorkOlayRaporlama;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// IHttpContextAccessor servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.BildirimSistemiService" />yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceYetki"></param>
        /// <param name="servicePersonel"></param>
        /// <param name="serviceViewBirim"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="serviceMailTarihce"></param>
        /// <param name="unitOfWorkRiskEvreni"></param>
        /// <param name="unitOfWorkRiskYonetimi"></param>
        /// <param name="sharedResource"></param>
        /// <param name="httpContextAccessor"></param>
        /// <remarks></remarks>
        public BildirimSistemiService(IUnitOfWork<BildirimSistemi> unitOfWork,
            IViewYetkiService serviceYetki,
            IViewPersonelService servicePersonel,
            IViewBirimService serviceViewBirim,
            ITarihceService serviceTarihce,
            IMailTarihceService serviceMailTarihce,
            IUnitOfWork<RiskEvreni> unitOfWorkRiskEvreni,
            IUnitOfWork<RiskYonetimi> unitOfWorkRiskYonetimi,
            IUnitOfWork<OlayRaporlama> unitOfWorkOlayRaporlama,
            IStringLocalizer<CustomResource> sharedResource,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _serviceYetki = serviceYetki;
            _servicePersonel = servicePersonel;
            _serviceViewBirim = serviceViewBirim;
            _serviceTarihce = serviceTarihce;
            _serviceMailTarihce = serviceMailTarihce;
            _unitOfWorkRiskEvreni = unitOfWorkRiskEvreni;
            _unitOfWorkRiskYonetimi = unitOfWorkRiskYonetimi;
            _unitOfWorkOlayRaporlama = unitOfWorkOlayRaporlama;
            _sharedResource = sharedResource;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, int kod)
        {
            string hata = "";

            if (kod == 0)
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod);

                if (kayit != null)
                {
                    kayit.Tarihce = await _serviceTarihce.SonKaydiGetirAsync(kullanan, kod.ToString());

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
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan", a => a.Koordinatorluk);

            //if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            //{
            //    BildirimSistemi aramaObj = Arac.DataTablesAramaNesne<BildirimSistemi>(new BildirimSistemi(), aramaDegeri);

            //    if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
            //        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod==aramaObj.KoordinatorlukKod);
            //}
            //else
            //{
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue));
            //}

            if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kullanan.KoordinatorlukKod);

                if (!string.IsNullOrWhiteSpace(kullanan.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kullanan.BirimKod);
            }
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OnaylayacakYetki == kullanan.AktifRolKod);

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMBildirimSistemiDurum.OnayaGonderdi);

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, BildirimSistemi gelenNesne)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan", a => a.Koordinatorluk);

            if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kullanan.KoordinatorlukKod);

                if (!string.IsNullOrWhiteSpace(kullanan.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kullanan.BirimKod);
            }

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OnaylayacakYetki == kullanan.AktifRolKod);

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMBildirimSistemiDurum.OnayaGonderdi);

            var kayitlar = selectData.ToList();

            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, BildirimSistemi gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.KoordinatorlukKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.KoordinatorlukAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);

                string mailGonderilecekYetki = gelenNesne.OnaylayacakYetki;

                if (gelenNesne.Durum == (int)ENUMBildirimSistemiDurum.Onayli && !string.IsNullOrWhiteSpace(gelenNesne.OnaylayacakUstYetki))
                {
                    var bsYeni = new BildirimSistemi
                    {
                        BelgeTipi = gelenNesne.BelgeTipi,
                        BelgeKod = gelenNesne.BelgeKod,
                        KoordinatorlukKod = gelenNesne.KoordinatorlukKod,
                        BirimKod = gelenNesne.BirimKod,
                        IslemYapanKod = gelenNesne.IslemYapanKod,
                        IslemTarihi = DateTime.Now,
                        OnaylayacakYetki = gelenNesne.OnaylayacakUstYetki,
                        Durum = (int)ENUMBildirimSistemiDurum.OnayaGonderdi
                    };

                    await _unitOfWork.KayitEkleAsync(bsYeni);

                    mailGonderilecekYetki = gelenNesne.OnaylayacakUstYetki;
                }

                //var sonucMailAdres = await _serviceYetki.ListeleMailAsync(kullanan, new ViewYetki() { Rol = mailGonderilecekYetki });

                //if (sonucMailAdres.IslemSonuc && !string.IsNullOrWhiteSpace(sonucMailAdres.AnahtarAlan))
                //{
                //    string mail = sonucMailAdres.Nesne + "";
                //    string konu = "";
                //    string mesaj = "";

                //    if (gelenNesne.BelgeTipi == (int)EnumTarihceIslemTur.RiskEvreni)
                //    {
                //        konu = "Risk onayı";
                //        mesaj = "*** nolu risk onay bekliyor.";
                //    }
                //    else if (gelenNesne.BelgeTipi == (int)EnumTarihceIslemTur.RiskYonetimi)
                //    {
                //        konu = "Risk'e verilen cevap onayı";
                //        mesaj = "*** nolu risk'e verilen cevap onay bekliyor.";
                //    }
                //    else if (gelenNesne.BelgeTipi == (int)EnumTarihceIslemTur.RiskAzaltmaPlani)
                //    {
                //        konu = "Azaltma planı onayı";
                //        mesaj = "*** nolu risk için oluşturulan azaltma planı onay bekliyor.";
                //    }
                //    else if (gelenNesne.BelgeTipi == (int)EnumTarihceIslemTur.OlayRaporlama)
                //    {
                //        konu = "Olay paroru onayı";
                //        mesaj = "*** tanımlı olay onay bekliyor.";
                //    }

                //    Mail.MailAt("", mail, konu, mesaj);
                //}

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li><li>" + ex.InnerException + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile talep edilen bilgilere göre onaylayacak yetki biglisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns></returns>
        public async Task<string> OnaylayacakYetkiVerAsync(KullaniciDto kullanan, BildirimSistemi kriter)
        {
            string onaylayacakYetki = "";

            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan,Koordinatorluk", a => a.Koordinatorluk);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BelgeTipi == kriter.BelgeTipi);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BelgeKod == kriter.BelgeKod);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum < (int)ENUMBildirimSistemiDurum.OnayDisiBildirimler);

            int onaySayisi = 0;

            var kayitlar = selectData.ToList();

            if (kayitlar.Count > 0)
            {
                var sonOnayaGonderilenIndex = 0;
                var index = 0;
                foreach (var bs in kayitlar)
                {
                    if (bs.Durum == (int)ENUMBildirimSistemiDurum.OnayaGonderdi || bs.Durum == (int)ENUMBildirimSistemiDurum.GeriGonderildi)
                        sonOnayaGonderilenIndex = index;
                    index++;
                }


                index = 0;
                foreach (var bs in kayitlar)
                {
                    index++;
                    if (index < sonOnayaGonderilenIndex)
                        continue;

                    if (bs.Durum == (int)ENUMBildirimSistemiDurum.OnayaGonderdi || bs.Durum == (int)ENUMBildirimSistemiDurum.GeriGonderildi)
                    {
                        onaylayacakYetki = Arac.UstYetkiVer(kullanan, (EnumKoordinatorlukTur)bs.Koordinatorluk.Tur);
                        if (bs.OnaylayacakYetki == onaylayacakYetki)
                            onaylayacakYetki = "";
                    }

                    if (kriter.Durum == (int)ENUMBildirimSistemiDurum.Onayli)
                    {
                        if (bs.Durum == (int)ENUMBildirimSistemiDurum.Onayli)
                        {
                            onaySayisi++;
                        }
                        else if (bs.Durum == (int)ENUMBildirimSistemiDurum.Pasif)
                        {
                            onaySayisi--;
                            onaylayacakYetki = "";
                        }
                    }

                }

                if (onaySayisi > 0)
                    onaylayacakYetki = "";
            }

            return onaylayacakYetki;
        }

        /// <summary>
        /// Istemciden parametere ile talep edilen bilgilere göre geri gönderilecek yetki biglisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns></returns>
        public async Task<string> GeriGonderilecekYetkiVerAsync(KullaniciDto kullanan, BildirimSistemi kriter)
        {
            string onaylayacakYetki = "";

            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan", a => a.Koordinatorluk);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BelgeTipi == kriter.BelgeTipi);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BelgeKod == kriter.BelgeKod);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum < (int)ENUMBildirimSistemiDurum.OnayDisiBildirimler);

            var kayitlar = selectData.ToList();

            if (kayitlar.Count > 0)
            {
                foreach (var bs in kayitlar)
                {
                    if (bs.Durum == (int)ENUMBildirimSistemiDurum.Onayli)
                        onaylayacakYetki = bs.OnaylayacakYetki;
                    else if (bs.Durum == (int)ENUMBildirimSistemiDurum.Pasif)
                        onaylayacakYetki = "";
                }
            }

            return onaylayacakYetki;
        }

        /// <summary>
        /// Istemciden parametere ile talep edilen bilgilere göre onaylayacak son yetki biglisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns></returns>
        public async Task<BildirimSistemi> SonYetkiVerAsync(KullaniciDto kullanan, BildirimSistemi kriter)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan,Koordinatorluk", a => a.Koordinatorluk);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BelgeTipi == kriter.BelgeTipi);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BelgeKod == kriter.BelgeKod);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum < (int)ENUMBildirimSistemiDurum.OnayDisiBildirimler);

            var kayitlar = selectData.ToList();

            return kayitlar.Count > 0 ? kayitlar[kayitlar.Count - 1] : new BildirimSistemi();
        }


        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere göre mail gönderen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public async Task<Sonuc> MailGonderAsync(KullaniciDto kullanan, BildirimSistemi kriter)
        {
            try
            {
                var mailListe = new List<string>();

                if (kriter.Liste == null)
                    kriter.Liste = new List<BildirimSistemi>();

                if (kriter.Liste.Count == 0)
                    kriter.Liste.Add(kriter);

                foreach (var b in kriter.Liste)
                {
                    string hata = "";


                    if (b.BelgeTipi < (int)EnumTarihceIslemTur.IcKontrolZayifliklariRaporunuHatirlat)
                    {
                        if (b.BelgeTipi <= 0)
                            hata += "<li>" + _sharedResource["Kontrol.Duzenle.BelgeTipiAlaniBos"] + "</li>";
                        if (string.IsNullOrWhiteSpace(b.BelgeKod))
                            hata += "<li>" + _sharedResource["Kontrol.Duzenle.BelgeKodAlaniBos"] + "</li>";
                        if (string.IsNullOrWhiteSpace(b.MailGonderilecekKisi) && string.IsNullOrWhiteSpace(b.KoordinatorlukKod))
                            hata += "<li>" + _sharedResource["Kontrol.Duzenle.KoordinatorlukAlaniBos"] + "</li>";
                    }


                    if (hata != "")
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);

                    if (!string.IsNullOrWhiteSpace(b.MailGonderilecekKisi))
                    {
                        var sonucPersonel = await _servicePersonel.KayitGetirAsync(kullanan, b.MailGonderilecekKisi);
                        if (sonucPersonel.IslemSonuc)
                        {
                            string ePostaPersonel = ((ViewPersonel)sonucPersonel.Nesne).EPosta;
                            if (!string.IsNullOrWhiteSpace(ePostaPersonel))
                                mailListe.Add(ePostaPersonel);
                        }
                    }
                    else
                    {
                        if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskEvreniDurumPasif)
                        {
                            var yetki = new ViewYetki { Rol = "RISKSEKRETARYASI" };

                            var sonucMailAdres = await _serviceYetki.ListeleMailAsync(kullanan, yetki);
                            if (sonucMailAdres.IslemSonuc && !string.IsNullOrWhiteSpace(sonucMailAdres.AnahtarAlan))
                                mailListe.AddRange(sonucMailAdres.AnahtarAlan.Split(';'));
                        }
                        else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskYonetimiBeyannameImza || kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskYonetimiBeyannameImzaHatirlat)
                        {
                            var onaylayacakYetkiler = b.OnaylayacakYetki.Split(',');

                            foreach (var onaylayacakYetki in onaylayacakYetkiler)
                            {
                                if (string.IsNullOrWhiteSpace(onaylayacakYetki))
                                    continue;

                                var yetki = new ViewYetki { Rol = onaylayacakYetki };

                                if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                                    yetki.KoordinatorlukKod = kriter.KoordinatorlukKod;

                                var sonucMailAdres = await _serviceYetki.ListeleMailAsync(kullanan, yetki);
                                if (sonucMailAdres.IslemSonuc && !string.IsNullOrWhiteSpace(sonucMailAdres.AnahtarAlan))
                                    mailListe.AddRange(sonucMailAdres.AnahtarAlan.Split(';'));
                            }

                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(b.OnaylayacakYetki))
                            {
                                if (b.Birimler != null && b.Birimler.Count > 0)
                                    b.OnaylayacakYetki = "BIRIMAMIRI";
                                else
                                    b.OnaylayacakYetki = await _serviceViewBirim.KoordinatorlukYetkiTipiBul(kullanan, b.KoordinatorlukKod, 0, false);
                            }

                            if (!string.IsNullOrWhiteSpace(b.OnaylayacakYetki) && b.OnaylayacakYetki != "BASKAN*")
                            {
                                var onaylayacakYetkiler = b.OnaylayacakYetki.Split(',');

                                foreach (var onaylayacakYetki in onaylayacakYetkiler)
                                {
                                    if (string.IsNullOrWhiteSpace(onaylayacakYetki))
                                        continue;

                                    var yetki = new ViewYetki { Rol = onaylayacakYetki };

                                    if (!string.IsNullOrWhiteSpace(b.KoordinatorlukKod))
                                        yetki.KoordinatorlukKod = b.KoordinatorlukKod;
                                    if (b.Birimler != null && b.Birimler.Count > 0)
                                        yetki.BirimKod = string.Join(";", b.Birimler.ToArray());
                                    else if (!string.IsNullOrWhiteSpace(b.BirimKod))
                                        yetki.BirimKod = b.BirimKod;

                                    var sonucMailAdres = await _serviceYetki.ListeleMailAsync(kullanan, yetki);
                                    if (sonucMailAdres.IslemSonuc && !string.IsNullOrWhiteSpace(sonucMailAdres.AnahtarAlan))
                                        mailListe.AddRange(sonucMailAdres.AnahtarAlan.Split(';'));
                                }

                                //var yetki = new ViewYetki();
                                //yetki.KoordinatorlukKod = b.KoordinatorlukKod;
                                //if (b.Birimler != null && b.Birimler.Count > 0)
                                //    yetki.BirimKod = string.Join(";", b.Birimler.ToArray());
                                //yetki.Rol = b.OnaylayacakYetki;

                                //var sonucMailAdres = await _serviceYetki.ListeleMailAsync(kullanan, yetki);
                                //if (sonucMailAdres.IslemSonuc && !string.IsNullOrWhiteSpace(sonucMailAdres.AnahtarAlan))
                                //    mailListe.AddRange(sonucMailAdres.AnahtarAlan.Split(';'));
                            }
                        }

                    }

                    if (string.IsNullOrWhiteSpace(kriter.BelgeKod))
                    {
                        kriter.BelgeKod = b.BelgeKod;
                        kriter.BelgeTipi = b.BelgeTipi;
                        kriter.Islem = b.Islem;
                    }
                }

                if (mailListe.Count == 0)
                {
                    var sonucKullanicilar = await _servicePersonel.ListeleAsync(kullanan, null, null);
                    if (sonucKullanicilar.IslemSonuc && sonucKullanicilar.Nesne != null)
                    {
                        var kullanicilar = sonucKullanicilar.Nesne as List<object>;
                        if (kullanicilar != null)
                        {
                            foreach (var item in kullanicilar)
                            {
                                var personel = item as ViewPersonel;
                                if (personel != null && !string.IsNullOrWhiteSpace(personel.EPosta) && Arac.EPostaDogrula(personel.EPosta))
                                    mailListe.Add(personel.EPosta);
                            }
                        }
                    }
                }

                if (mailListe.Count > 0)
                {
                    Tarihce tarihce = new Tarihce();

                    //Tarihçe Başlangıç
                    tarihce.IlgiKod = kriter.BelgeKod;
                    tarihce.IlgiTur = (EnumTarihceIslemTur)kriter.BelgeTipi;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = 15; // Bilgilendirme

                    var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiş

                    string mail = string.Join(";", mailListe.Distinct().ToArray());
                    string konu = "";
                    string mesaj = "";

                    string mesajSablon = @"<div style='padding:0 0 10px 0;'>{BASLIK}</div>
                                           <div style='display: table;'>
                                             {ALANLAR}
                                           </div>";
                    string mesajSablonAlan = @"<div style='display: flex;'><span style='width:120px;font-weight:bold;'>{ALAN}</span> : {ACIKLAMA}</div>";
                    string mesajSablonAlanUrl = @"<div style='display: flex;'><span style='width:120px;font-weight:bold;'>{ALAN}</span> : <a href='{ACIKLAMA}' target='_blank'> Görüntüle</a></div>";

                    //string url = _httpContextAccessor.HttpContext.Request.Scheme + "://" + _httpContextAccessor.HttpContext.Request.Host;
                    string url = Arac.ConfigOku("Genel:YazilimURL");

                    if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskEvreni)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kriter.BelgeKod);

                        if (kriter.Islem == EnumBildirimSistemiIslem.Yeni)
                        {
                            konu = "Yeni Risk Kaydı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Yeni Risk kaydı yapıldı.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RiskKaydi?r=" + riskEvreni.Kod);
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                        else
                        {
                            konu = "Risk Onayı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Risk kaydı onay bekliyor.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RiskKaydiOnay");
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskYonetimi)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.Kod == kriter.BelgeKod, "Surec,AltSurec,Amac,Hedef,AnahtarRiskGostergesi,AnahtarRiskGostergesi.Donemler,RiskKategoriler.RiskKategori,RiskYonetimi,RiskYonetimi.Kontroller");

                        if (kriter.Islem == EnumBildirimSistemiIslem.Yeni)
                        {
                            konu = "Yeni Risk Yönetimi Kaydı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Yeni Risk Yönetimi kaydı yapıldı.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Yapısal Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.YapisalRiskPuani.ToString());
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Artık Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.ArtikRiskPuani.ToString());
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinDegerlendirilmesi?r=" + riskEvreni.Kod);
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                        else if (kriter.Islem == EnumBildirimSistemiIslem.AzaltmaPlaniOlustur)
                        {
                            konu = "Risk Azaltma Planı Oluştur";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Risk Azaltma Planı oluşturmak için Risk Azaltma Planı ekranına gidiniz.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Yapısal Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.YapisalRiskPuani.ToString());
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Artık Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.ArtikRiskPuani.ToString());
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinYonetilmesi?r=" + riskEvreni.Kod);
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                        else if (kriter.Islem == EnumBildirimSistemiIslem.Uyari)
                        {
                            //Azaltma Planı Bitiş Tarihi Hatırlatma E-posta (konfigürasyon ekranında) hem risk sahibine hem de azaltma planı sorumlusuna gönderilmeli.
                            var sonucRiskSahibi = await _servicePersonel.KayitGetirAsync(kullanan, riskEvreni.RiskSahibiKod);
                            if (sonucRiskSahibi.IslemSonuc)
                            {
                                string ePostaRiskSahibi = ((ViewPersonel)sonucRiskSahibi.Nesne).EPosta;
                                if (!string.IsNullOrWhiteSpace(ePostaRiskSahibi) && !mail.Contains(ePostaRiskSahibi))
                                    mail += ";" + ePostaRiskSahibi;
                            }

                            konu = "Risk Azaltma Planı Oluşturulmadı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Risk Azaltma Planı oluşturulmadı.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Yapısal Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.YapisalRiskPuani.ToString());
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Artık Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.ArtikRiskPuani.ToString());
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinDegerlendirilmesi?r=" + riskEvreni.Kod);
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                        else
                        {
                            konu = "Risk Yönetimi Onayı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Risk Yönetimi kaydı onay bekliyor.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Yapısal Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.YapisalRiskPuani.ToString());
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Artık Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.ArtikRiskPuani.ToString());
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinDegerlendirilmesiOnay");
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskAzaltmaPlani)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.RiskAzaltmaPlani.Kod == kriter.BelgeKod, "Koordinatorluk, Birim, RiskKategoriler.RiskKategori,RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.Riskler, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler, RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Birim");

                        if (kriter.Islem == EnumBildirimSistemiIslem.Yeni)
                        {
                            konu = "Yeni Risk Azaltma Planı Kaydı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Yeni Azaltma Planı kaydı yapıldı.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı No").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Bitiş Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(riskEvreni.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi));
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinYonetilmesi?r=" + riskEvreni.Kod);
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);

                        }
                        else if (kriter.Islem == EnumBildirimSistemiIslem.Yaklasiyor)
                        {
                            mail = "";
                            var sonucPersonel = await _servicePersonel.KayitGetirAsync(kullanan, riskEvreni.RiskSahibiKod);
                            if (sonucPersonel.IslemSonuc)
                            {
                                string ePostaPersonel = ((ViewPersonel)sonucPersonel.Nesne).EPosta;
                                if (!string.IsNullOrWhiteSpace(ePostaPersonel))
                                    mail = ePostaPersonel;
                            }

                            konu = "Risk Azaltma Planının Tarihi Yaklaşıyor";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Risk Azaltma Planının tarihi yaklaşıyor.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı No").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Bitiş Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(riskEvreni.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi));
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinYonetilmesiOnay");
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                        else if (kriter.Islem == EnumBildirimSistemiIslem.SureGecti)
                        {
                            konu = "Risk Azaltma Planının Süresi Geçti";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Süresi geçen Risk Azaltma Planını var.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı No").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Bitiş Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(riskEvreni.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi));
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinYonetilmesiOnay");
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                        else
                        {
                            konu = "Risk Azaltma Planı Onayı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Risk Azaltma Planı kaydı onay bekliyor.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı No").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Bitiş Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(riskEvreni.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi));
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinYonetilmesiOnay");
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.OlayRaporlama)
                    {
                        var olayRaporlama = await _unitOfWorkOlayRaporlama.KayitGetirAsync(c => c.Kod == kriter.BelgeKod, "Koordinatorluk,Birim,OlayKategori,Riskler.RiskEvreni,Riskler.RiskEvreni.RiskKategoriler.RiskKategori");

                        if (kriter.Islem == EnumBildirimSistemiIslem.Yeni)
                        {
                            konu = "Yeni Olay Raporu Kaydı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Olay Raporu kaydı yapıldı.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", olayRaporlama.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", olayRaporlama.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Olay Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(olayRaporlama.OlayTarihi));
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Olayın Kategorisi").Replace("{ACIKLAMA}", olayRaporlama.OlayKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Olay Tanımı").Replace("{ACIKLAMA}", olayRaporlama.OlayTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Tutarı").Replace("{ACIKLAMA}", olayRaporlama.Tutari.ToString("#.###,00"));
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/OlayRaporlamaOnay");
                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                        else
                        {
                            konu = "Olay Raporu Onayı";

                            mesaj = mesajSablon;
                            mesaj = mesaj.Replace("{BASLIK}", "Olay Raporu kaydı onay bekliyor.");
                            string alanlar = "";
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", olayRaporlama.Koordinatorluk.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", olayRaporlama.Birim.Adi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Olay Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(olayRaporlama.OlayTarihi));
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Olayın Kategorisi").Replace("{ACIKLAMA}", olayRaporlama.OlayKategoriAdlari);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Olay Tanımı").Replace("{ACIKLAMA}", olayRaporlama.OlayTanimi);
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Tutarı").Replace("{ACIKLAMA}", olayRaporlama.Tutari.ToString("#.###,00"));
                            alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/OlayRaporlamaOnay");

                            mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                        }
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.IcKontrolZayifliklariRaporunuHatirlat)
                    {
                        konu = "Hatırlatma";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "İç Kontrol Zayıflıkları Raporunu UYG'ye bildiriniz.");
                        string alanlar = "";
                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskVeAzaltmaPlaniListesiniHatirlat)
                    {
                        konu = "Hatırlatma";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Onaylanmış tüm riskler ve ilgili azaltma planlarının listesini UYG'ye bildiriniz.");
                        string alanlar = "";
                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.YuksekVeOrtaRiskleriHatirlat)
                    {
                        konu = "Hatırlatma";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Yüksek ve orta seviyedeki aktif riskleri ve azaltma planlarını merkez ve il koordinatörlüklerine bildiriniz.");
                        string alanlar = "";
                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskYonetimiProsedurunuHatirlat)
                    {
                        konu = "Hatırlatma";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk Yönetimi Prosedürünü gözden geçiriniz.");
                        string alanlar = "";
                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskDegerlendirmeyiHatirlat)
                    {
                        konu = "Hatırlatma";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk değerlendirmesini gözden geçiriniz.");
                        string alanlar = "";
                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskYonetimiBeyannameImza)
                    {
                        konu = "Yıllık Risk Yönetimi Taahhütnamesi İmzala";

                        mesaj = mesajSablon = @"<div style='padding:0 0 10px 0;'>
                                                Kurumumuz Risk Yönetimi Prosedürü gereği risk yönetimi uygulamalarının yıl boyunca biriminizde uygun şekilde yürürlükte olduğunu ve uygulandığını beyan etmeniz amacıyla hazırlanan ?{YIL} yılı Risk Yönetimi Taahhütnamesi? <a href='{URL}' target='_blank'>{URL}</a> adresinde onaylarınıza sunulmaktadır. 
                                                </div>
                                              ";
                        mesaj = mesaj.Replace("{YIL}", DateTime.Now.AddYears(-1).Year.ToString());
                        mesaj = mesaj.Replace("{URL}", url + "/RiskYonetimiTaahhutnamesi");
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskYonetimiBeyannameImzaHatirlat)
                    {
                        konu = "Yıllık Risk Yönetimi Taahhütnamesi Hatırlatma";

                        mesaj = mesajSablon = @"<div style='padding:0 0 10px 0;'>
                                                Kurumumuz Risk Yönetimi Prosedürü gereği risk yönetimi uygulamalarının yıl boyunca biriminizde uygun şekilde yürürlükte olduğunu ve uygulandığını beyan etmeniz amacıyla hazırlanan ?{YIL} yılı Risk Yönetimi Taahhütnamesi? <a href='{URL}' target='_blank'>{URL}</a> adresinde onaylarınıza sunulmaktadır. 
                                                </div>
                                              ";
                        mesaj = mesaj.Replace("{YIL}", DateTime.Now.AddYears(-1).Year.ToString());
                        mesaj = mesaj.Replace("{URL}", url + "/RiskYonetimiTaahhutnamesi");
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskEvreniDurumPasif)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kriter.BelgeKod, "Koordinatorluk,Birim");

                        konu = "Risk Kaydı Pasif Yapıldı";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk kaydı pasif yapıldı.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RiskKaydi?r=" + riskEvreni.Kod);
                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskEvreniRiskSahibiDegisti)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kriter.BelgeKod, "Koordinatorluk,Birim");

                        konu = "Risk Sahibi Değişti";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk sahibi değişti.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskEvreniOnaydaDegisiklik)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kriter.BelgeKod, "Koordinatorluk,Birim");

                        konu = "Risk Kaydı Onay İşleminde Değişiklik Yapıldı";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk kaydı onay işleminde bazı alanlarda değişiklik yapıldı.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RiskKaydi?r=" + riskEvreni.Kod);

                        var degisenDeger = "";
                        var ilgiliTarihce = await _serviceTarihce.ListeleAsync(riskEvreni.Kod);
                        if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                        {
                            foreach (Tarihce t in ilgiliTarihce.Liste)
                            {
                                if (t.Durum == (int)ENUMDurum.Onayli)
                                    degisenDeger = t.DegisenDeger;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(degisenDeger))
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Değişen Değerler").Replace("{ACIKLAMA}", degisenDeger);

                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RisklerinDegerlendirmesiOnaydaDegisiklik)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.Kod == kriter.BelgeKod, "Surec,AltSurec,Amac,Hedef,AnahtarRiskGostergesi,AnahtarRiskGostergesi.Donemler,RiskKategoriler.RiskKategori,RiskYonetimi,RiskYonetimi.Kontroller");

                        konu = "Risklerin Değerlendirilmesi Onay İşleminde Değişiklik Yapıldı";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk değerlendirilmesi onay işleminde bazı alanlarda değişiklik yapıldı.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Yapısal Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.YapisalRiskPuani.ToString());
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Artık Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.ArtikRiskPuani.ToString());
                        alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinDegerlendirilmesi?r=" + riskEvreni.Kod);

                        var degisenDeger = "";
                        var ilgiliTarihce = await _serviceTarihce.ListeleAsync(kriter.BelgeKod);
                        if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                        {
                            foreach (Tarihce t in ilgiliTarihce.Liste)
                            {
                                if (t.Durum == (int)ENUMDurum.Onayli)
                                    degisenDeger = t.DegisenDeger;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(degisenDeger))
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Değişen Değerler").Replace("{ACIKLAMA}", degisenDeger);

                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RisklerinYonetilmesiOnaydaDegisiklik)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.RiskAzaltmaPlani.Kod == kriter.BelgeKod, "Koordinatorluk, Birim, RiskKategoriler.RiskKategori,RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.Riskler, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler, RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Birim");

                        konu = "Risklerin Yönetilmesi Onay İşleminde Değişiklik Yapıldı";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk yönetilmesi onay işleminde bazı alanlarda değişiklik yapıldı.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı No").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Bitiş Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(riskEvreni.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi));
                        alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinYonetilmesi?r=" + riskEvreni.Kod);

                        var degisenDeger = "";
                        var ilgiliTarihce = await _serviceTarihce.ListeleAsync(kriter.BelgeKod);
                        if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                        {
                            foreach (Tarihce t in ilgiliTarihce.Liste)
                            {
                                if (t.Durum == (int)ENUMDurum.Onayli)
                                    degisenDeger = t.DegisenDeger;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(degisenDeger))
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Değişen Değerler").Replace("{ACIKLAMA}", degisenDeger);

                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RiskEvreniDurumDegisti)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.Kod == kriter.BelgeKod, "Koordinatorluk,Birim");

                        konu = "Risk Kaydı Durumu Değişti";

                        mesaj = mesajSablon;
                        bool riskKaydiOnaylandi = riskEvreni.Durum == (int)ENUMDurum.Onayli;
                        mesaj = mesaj.Replace("{BASLIK}", riskKaydiOnaylandi
                            ? "Risk kaydınız onaylandı. Riskinizi değerlendirmek için Risklerin Değerlendirilmesi ekranına gidiniz."
                            : "Risk kaydı durumu değişti.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Durum Bilgisi").Replace("{ACIKLAMA}", kriter.Aciklama);
                        alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", riskKaydiOnaylandi
                            ? url + "/RisklerinDegerlendirilmesi?r=" + riskEvreni.Kod
                            : url + "/RiskKaydi?r=" + riskEvreni.Kod);

                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RisklerinDegerlendirmesiDurumDegisti)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.Kod == kriter.BelgeKod, "Surec,AltSurec,Amac,Hedef,AnahtarRiskGostergesi,AnahtarRiskGostergesi.Donemler,RiskKategoriler.RiskKategori,RiskYonetimi,RiskYonetimi.Kontroller");

                        konu = "Risklerin Değerlendirilmesi Durumu Değişti";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk değerlendirilmesi durumu değişti.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Yapısal Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.YapisalRiskPuani.ToString());
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Artık Risk Puanı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.ArtikRiskPuani.ToString());
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Durum Bilgisi").Replace("{ACIKLAMA}", kriter.Aciklama);
                        alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinDegerlendirilmesi?r=" + riskEvreni.Kod);

                        var degisenDeger = "";
                        var ilgiliTarihce = await _serviceTarihce.ListeleAsync(kriter.BelgeKod);
                        if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                        {
                            foreach (Tarihce t in ilgiliTarihce.Liste)
                            {
                                if (t.Durum == (int)ENUMDurum.Onayli)
                                    degisenDeger = t.DegisenDeger;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(degisenDeger))
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Değişen Değerler").Replace("{ACIKLAMA}", degisenDeger);

                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }
                    else if (kriter.BelgeTipi == (int)EnumTarihceIslemTur.RisklerinYonetilmesiDurumDegisti)
                    {
                        var riskEvreni = await _unitOfWorkRiskEvreni.KayitGetirAsync(c => c.RiskYonetimi.RiskAzaltmaPlani.Kod == kriter.BelgeKod, "Koordinatorluk, Birim, RiskKategoriler.RiskKategori,RiskYonetimi, RiskYonetimi.Kontroller, RiskYonetimi.RiskAzaltmaPlani, RiskYonetimi.RiskAzaltmaPlani.Riskler, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler, RiskYonetimi.RiskAzaltmaPlani.IliskiliPlanlar, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Koordinatorluk, RiskYonetimi.RiskAzaltmaPlani.IsbirligiBirimler.Birim");

                        konu = "Risklerin Yönetilmesi Durumu Değişti";

                        mesaj = mesajSablon;
                        mesaj = mesaj.Replace("{BASLIK}", "Risk yönetilmesi durumu değişti.");
                        string alanlar = "";
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Koordinatürlük").Replace("{ACIKLAMA}", riskEvreni.Koordinatorluk.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Birim/Ünite").Replace("{ACIKLAMA}", riskEvreni.Birim.Adi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk No").Replace("{ACIKLAMA}", riskEvreni.RiskNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Riskin Kök Nedeni").Replace("{ACIKLAMA}", riskEvreni.RiskTanimi);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Risk Kategorisi").Replace("{ACIKLAMA}", riskEvreni.RiskKategoriAdlari);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı No").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlaniNo);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Azaltma Planı").Replace("{ACIKLAMA}", riskEvreni.RiskYonetimi.RiskAzaltmaPlani.AzaltmaPlani);
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Bitiş Tarihi").Replace("{ACIKLAMA}", Arac.DateTimeToDDMMYYYY(riskEvreni.RiskYonetimi.RiskAzaltmaPlani.BitisTarihi));
                        alanlar += mesajSablonAlan.Replace("{ALAN}", "Durum Bilgisi").Replace("{ACIKLAMA}", kriter.Aciklama);
                        alanlar += mesajSablonAlanUrl.Replace("{ALAN}", "Erişim Adresi").Replace("{ACIKLAMA}", url + "/RisklerinYonetilmesi?r=" + riskEvreni.Kod);

                        var degisenDeger = "";
                        var ilgiliTarihce = await _serviceTarihce.ListeleAsync(kriter.BelgeKod);
                        if (ilgiliTarihce.IslemSonuc && ilgiliTarihce.Liste.Count > 0)
                        {
                            foreach (Tarihce t in ilgiliTarihce.Liste)
                            {
                                if (t.Durum == (int)ENUMDurum.Onayli)
                                    degisenDeger = t.DegisenDeger;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(degisenDeger))
                            alanlar += mesajSablonAlan.Replace("{ALAN}", "Değişen Değerler").Replace("{ACIKLAMA}", degisenDeger);

                        mesaj = mesaj.Replace("{ALANLAR}", alanlar);
                    }

                    Mail.MailAt("", mail, konu, mesaj, true, false, null);


                    var mailTarihce = new MailTarihce();
                    mailTarihce.IlgiKod = kriter.Kod == 0 ? Arac.GetGuid() : kriter.Kod.ToString();
                    mailTarihce.IlgiTur = "BILDIRIMSISTEMI";
                    mailTarihce.IslemTarihi = DateTime.Now;
                    mailTarihce.MailAdres = mail;
                    mailTarihce.MailIcerik = konu + ">>" + mesaj;

                    var sonucMailTarihce = await _serviceMailTarihce.KaydetAsync(kullanan, mailTarihce);

                }

            }
            catch (System.Exception ex)
            {
                Arac.HataStrYaz("E-Posta gönderilirken hata oluştu:" + ex.Message);

                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li><li>" + ex.InnerException + "</li></small>");
            }
            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, BildirimSistemi kriter)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kriter.BelgeKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.BelgeKod == kriter.BelgeKod);
                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }
    }
}
