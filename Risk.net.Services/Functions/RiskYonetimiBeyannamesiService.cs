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
using Microsoft.VisualBasic;
using System;
using System.Text.RegularExpressions;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// RiskYonetimiBeyannamesi iþlemlerinin yapýldýðý servis
    /// </summary>
    public class RiskYonetimiBeyannamesiService : IRiskYonetimiBeyannamesiService
    {
        /// <summary>
        /// IUnitOfWork<RiskYonetimiBeyannamesi> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskYonetimiBeyannamesi> _unitOfWork;
        /// <summary>
        /// ITanimGenelService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITanimGenelService _serviceTanimGenel;
        /// <summary>
        /// IBildirimSistemiService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBildirimSistemiService _serviceBildirimSistemi;
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
        /// <see cref="Risk.net.Services.Functions.RiskYonetimiBeyannamesiService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskYonetimiBeyannamesiService(IUnitOfWork<RiskYonetimiBeyannamesi> unitOfWork,
            ITanimGenelService serviceTanimGenel,
            IBildirimSistemiService serviceBildirimSistemi,
            ITarihceService serviceTarihce,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceTanimGenel = serviceTanimGenel;
            _serviceBildirimSistemi = serviceBildirimSistemi;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "Koordinatorluk,IslemYapan");

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
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, RiskYonetimiBeyannamesi kriter)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Koordinatorluk", a => a.Koordinatorluk);

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Yil == kriter.Yil);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kriter.KoordinatorlukKod);


            var kayitlar = selectData.ToList();

            if (kayitlar.Count > -1)
            {
                foreach (RiskYonetimiBeyannamesi item in kayitlar)
                {
                    if (item.Durum <= 1)
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
                }

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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan,Koordinatorluk", a => a.Koordinatorluk);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                var aramaObj = Arac.DataTablesAramaNesne<RiskYonetimiBeyannamesi>(new RiskYonetimiBeyannamesi(), aramaDegeri);

                if (aramaObj.Yil > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Yil == aramaObj.Yil);

                if (aramaObj.KoordinatorlukKod == "-42")
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Koordinatorluk.Tur == 40); //42 il koordinatörlüðü
                else if (aramaObj.KoordinatorlukKod == "-1")
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Koordinatorluk.Tur < 1000); //Tüm Koordinatörlükler
                else if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);


                //if (!string.IsNullOrWhiteSpace(aramaObj.IslemYapanKod))
                //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.IslemYapan.Adi.Contains(aramaObj.IslemYapanKod) || a.IslemYapan.Soyadi.Contains(aramaObj.IslemYapanKod));

                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dataTablesParam.searchValue))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                                     || a.IslemYapan.Adi.Contains(dataTablesParam.searchValue));

                //selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Yil == DateAndTime.Now.Year);
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, RiskYonetimiBeyannamesi gelenNesne)
        {
            RiskYonetimiBeyannamesi islemYapilan = new RiskYonetimiBeyannamesi();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.IslemYapanKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.RiskYonetimiBeyannamesiIslemYapanKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }


            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }


        /// <summary>
        /// Onaylý kayýtlarýn onaylarýný kaldýran metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> OnayKaldirAsync(KullaniciDto kullanan)
        {
            try
            {
                var kayitlar = await _unitOfWork.ListeleAsync(k => k.Durum == (int)ENUMDurum.Onayli);

                foreach (RiskYonetimiBeyannamesi kayit in kayitlar)
                {
                    kayit.Durum = (int)ENUMDurum.Pasif;
                    await _unitOfWork.GuncelleAsync(kayit);
                }

                await _unitOfWork.KaydetAsync();


                //Ýmza süreci yeniden baþladýðýnda 1 ay sonra hatýrlatma maili göndermek için, sürecin baþladýðý tarih kayýt altýna alýnýyor.
                TanimGenel formGenel = new TanimGenel();

                formGenel.Durum = (int)ENUMDurum.Aktif;
                formGenel.Tur = "RISKYONETIMIBEYANNAMESI";
                formGenel.Adi = string.Format("{0:yyyy-MM-dd 00:00:00.000}", DateTime.Now);
                formGenel.SiraNo = 0;

                var listeTanim = await _serviceTanimGenel.ListeleAsync(kullanan, "RISKYONETIMIBEYANNAMESI");
                foreach (TanimGenel item in listeTanim.Liste)
                    formGenel.SiraNo = item.SiraNo;

                formGenel.SiraNo += 1;

                Sonuc sonuc = await _serviceTanimGenel.KaydetAsync(kullanan, formGenel);

                if (sonuc.IslemSonuc)
                {
                    //Mail Gönder: Ýmza süreci yeniden baþlatýldýðýnda imza sürecindeki kiþilere (MERKEZKOORDINATOR,ILKOORDINATOR) mail gönderilecek. 
                    var formBildirim = new BildirimSistemi()
                    {
                        Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                        BelgeKod = sonuc.AnahtarAlan,
                        BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimiBeyannameImza,
                        OnaylayacakYetki = "MERKEZKOORDINATOR,ILKOORDINATOR"
                    };

                    //TEST iþleminde mail atmasýn kapatýldý. Yazýlým çalýþmaya baþladýðýnda açýlacak Melih 29.09.2023
                    //Talep üzerinde tekrar açýldý. Hüseyin 01.03.2024 
                    Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);

                    //Mail Gönder Bitiþ
                }

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }


            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.YeniImzaSureciBaslatBasarili"]);
        }

        /// <summary>
        /// Hatýrlatma maili gönderilmesi iþlemini saðlayan metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> HatirlatmaMailiGonder(KullaniciDto kullanan)
        {
            try
            {
                //Risk Yönetimi Taahhütnamesi imzalamayanlara hatýrlatma e-postasý gönder
                var sql = @"SELECT '' Kod, 0 Yil, 0 Durum, null IslemTarihi, '' IslemYapanKod, '' IslemYapanRol, 
                            STUFF((SELECT ';' +  KoordinatorlukKod
                            FROM   ViewYetki
                            WHERE (Rol = 'MERKEZKOORDINATOR' OR
                            Rol = 'ILKOORDINATOR') AND (NOT (KoordinatorlukKod IN
                                (SELECT KoordinatorlukKod
                                FROM    RiskYonetimiBeyannamesi
                                WHERE (Durum = 10))))
				                GROUP BY KoordinatorlukKod
                                        FOR XML PATH('')), 1, 1, '') AS KoordinatorlukKod";

                var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);


                if (kayitlar.Count > 0)
                {
                    var formBildirim = new BildirimSistemi()
                    {
                        Islem = EnumBildirimSistemiIslem.Hatirlatma,
                        KoordinatorlukKod = kayitlar[0].KoordinatorlukKod,
                        BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimiBeyannameImzaHatirlat,
                        OnaylayacakYetki = "MERKEZKOORDINATOR,ILKOORDINATOR"
                    };

                    Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);


                    Tarihce tarihce = new Tarihce();

                    //Tarihçe Baþlangýç
                    tarihce.IlgiKod = "RiskYonetimiBeyannamesiMailGonder";
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskYonetimiBeyannamesiMailGonder;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = 19; // Bilgilendirme

                    var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiþ


                }
                else
                    return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + _sharedResource["Kontrol.Duzenle.HatirlatmaGonderilecekKoordinatorlukBulunamadi"] + "</li></small>");


            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }


            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.HatirlatmaMailiGonderBasarili"]);
        }


    }

}
