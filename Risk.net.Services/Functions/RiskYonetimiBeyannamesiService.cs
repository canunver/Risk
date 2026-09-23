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
    /// RiskYonetimiBeyannamesi işlemlerinin yapıldığı servis
    /// </summary>
    public class RiskYonetimiBeyannamesiService : IRiskYonetimiBeyannamesiService
    {
        /// <summary>
        /// IUnitOfWork<RiskYonetimiBeyannamesi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskYonetimiBeyannamesi> _unitOfWork;
        private readonly IUnitOfWork<ViewKoordinatorluk> _unitOfWorkKoordinatorluk;
        /// <summary>
        /// ITanimGenelService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITanimGenelService _serviceTanimGenel;
        /// <summary>
        /// IBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBildirimSistemiService _serviceBildirimSistemi;
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
        /// <see cref="Risk.net.Services.Functions.RiskYonetimiBeyannamesiService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskYonetimiBeyannamesiService(IUnitOfWork<RiskYonetimiBeyannamesi> unitOfWork,
            IUnitOfWork<ViewKoordinatorluk> unitOfWorkKoordinatorluk,
            ITanimGenelService serviceTanimGenel,
            IBildirimSistemiService serviceBildirimSistemi,
            ITarihceService serviceTarihce,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkKoordinatorluk = unitOfWorkKoordinatorluk;
            _serviceTanimGenel = serviceTanimGenel;
            _serviceBildirimSistemi = serviceBildirimSistemi;
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
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
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
                        item.DurumAdi = "Onaylandı";
                    else if (item.Durum == 98)
                        item.DurumAdi = "Reddedildi";
                    else if (item.Durum == 99)
                        item.DurumAdi = "Pasif yapıldı";
                    else if (item.Durum == 15)
                        item.DurumAdi = "Bilgilendirme Maili Gönderildi";
                }

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
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam, int yil, int surecDurumu)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var aramaObj = new RiskYonetimiBeyannamesi();

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
                aramaObj = Arac.DataTablesAramaNesne<RiskYonetimiBeyannamesi>(aramaObj, aramaDegeri);

            // İmza süreci filtresi, gelişmiş arama metninin oluşturulma biçimine bağlı
            // kalmadan istemciden ayrıca gönderilir. Böylece ekran ve rapor aynı değeri kullanır.
            if (surecDurumu > 0)
                aramaObj.SorguSurecDurumu = surecDurumu;

            var beyannameler = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan,Koordinatorluk", a => a.Koordinatorluk);
            var tumBeyannameler = beyannameler.ToList();
            var raporYil = yil > 0
                ? yil
                : (tumBeyannameler.Count > 0 ? tumBeyannameler.Max(a => a.Yil) : DateTime.Now.Year - 1);
            var beyannameListesi = tumBeyannameler.Where(a => a.Yil == raporYil).GroupBy(a => a.KoordinatorlukKod).ToDictionary(a => a.Key, a => a.OrderByDescending(x => x.IslemTarihi).First());
            var koordinatorluklar = await _unitOfWorkKoordinatorluk.SorguHazirlaAsync(a => a.Durum == (int)ENUMDurum.Aktif && a.Tur < 1000, "");

            var liste = koordinatorluklar.ToList().Select(koordinatorluk =>
            {
                beyannameListesi.TryGetValue(koordinatorluk.Kod, out var beyanname);
                if (beyanname == null)
                {
                    beyanname = new RiskYonetimiBeyannamesi
                    {
                        Kod = "SURECBASLAMADI_" + koordinatorluk.Kod,
                        Yil = raporYil,
                        KoordinatorlukKod = koordinatorluk.Kod,
                        Koordinatorluk = koordinatorluk
                    };
                }

                beyanname.SurecDurumu = beyanname.Kod.StartsWith("SURECBASLAMADI_", StringComparison.Ordinal)
                    ? 4
                    : (beyanname.Durum == (int)ENUMDurum.Onayli ? 1 : 3);
                return beyanname;
            });

            if (aramaObj.KoordinatorlukKod == "-42")
                liste = liste.Where(a => a.Koordinatorluk.Tur == 40);
            else if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod) && aramaObj.KoordinatorlukKod != "-1")
                liste = liste.Where(a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);

            if (aramaObj.SorguSurecDurumu == 1)
                liste = liste.Where(a => a.SurecDurumu == 1);
            else if (aramaObj.SorguSurecDurumu == 2)
                liste = liste.Where(a => a.SurecDurumu == 3 || a.SurecDurumu == 4);
            else if (aramaObj.SorguSurecDurumu == 3)
                liste = liste.Where(a => a.SurecDurumu == 3);
            else if (aramaObj.SorguSurecDurumu == 4)
                liste = liste.Where(a => a.SurecDurumu == 4);

            if (!aramaDegeri.StartsWith("GELISMIS_ARAMA:") && !string.IsNullOrWhiteSpace(aramaDegeri))
                liste = liste.Where(a => a.Koordinatorluk.Adi.Contains(aramaDegeri, StringComparison.CurrentCultureIgnoreCase));

            // Bu liste veritabanı sorgusu değil; beyannamesi bulunmayan koordinatörlükler
            // için bellekte üretilen satırları da içeriyor. IQueryable<object> üzerinden
            // dinamik sıralama yapmak özellikle SurecDurumu alanında hata oluşturduğu için
            // DataTables sıralama ve sayfalamasını tip güvenli olarak burada uyguluyoruz.
            var kayitlar = liste.ToList();
            var azalan = string.Equals(dataTablesParam.sortColumnDirection, "desc", StringComparison.OrdinalIgnoreCase);
            IEnumerable<RiskYonetimiBeyannamesi> siraliKayitlar;

            switch (dataTablesParam.sortColumn)
            {
                case "Yil":
                    siraliKayitlar = azalan ? kayitlar.OrderByDescending(a => a.Yil) : kayitlar.OrderBy(a => a.Yil);
                    break;
                case "Koordinatorluk.Adi":
                    siraliKayitlar = azalan
                        ? kayitlar.OrderByDescending(a => a.Koordinatorluk?.Adi ?? "")
                        : kayitlar.OrderBy(a => a.Koordinatorluk?.Adi ?? "");
                    break;
                case "IslemYapan.AdiSoyadi":
                    siraliKayitlar = azalan
                        ? kayitlar.OrderByDescending(a => a.IslemYapan?.AdiSoyadi ?? "")
                        : kayitlar.OrderBy(a => a.IslemYapan?.AdiSoyadi ?? "");
                    break;
                case "IslemTarihi":
                    siraliKayitlar = azalan ? kayitlar.OrderByDescending(a => a.IslemTarihi) : kayitlar.OrderBy(a => a.IslemTarihi);
                    break;
                case "SurecDurumu":
                    siraliKayitlar = azalan ? kayitlar.OrderByDescending(a => a.SurecDurumu) : kayitlar.OrderBy(a => a.SurecDurumu);
                    break;
                default:
                    siraliKayitlar = kayitlar.OrderBy(a => a.Koordinatorluk?.Adi ?? "");
                    break;
            }

            var toplamKayit = kayitlar.Count;
            return new
            {
                draw = dataTablesParam.draw,
                recordsFiltered = toplamKayit,
                recordsTotal = toplamKayit,
                data = siraliKayitlar.Skip(dataTablesParam.skip).Take(dataTablesParam.pageSize).ToList()
            };
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
        /// Onaylı kayıtların onaylarını kaldıran metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> OnayKaldirAsync(KullaniciDto kullanan, int yil)
        {
            if (yil <= 0)
                return new Sonuc(ENUMIslemDurum.Uyari, "<li>İmza sürecinin başlatılacağı yıl seçilmelidir.</li>");

            try
            {
                var kayitlar = await _unitOfWork.ListeleAsync(k => k.Yil == yil && k.Durum == (int)ENUMDurum.Onayli);

                foreach (RiskYonetimiBeyannamesi kayit in kayitlar)
                {
                    kayit.Durum = (int)ENUMDurum.Pasif;
                    await _unitOfWork.GuncelleAsync(kayit);
                }

                await _unitOfWork.KaydetAsync();


                //İmza süreci yeniden başladığında 1 ay sonra hatırlatma maili göndermek için, sürecin başladığı tarih kayıt altına alınıyor.
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
                    //Mail Gönder: İmza süreci yeniden başlatıldığında imza sürecindeki kişilere (MERKEZKOORDINATOR,ILKOORDINATOR) mail gönderilecek. 
                    var formBildirim = new BildirimSistemi()
                    {
                        Islem = EnumBildirimSistemiIslem.Bilgilendirme,
                        BelgeKod = sonuc.AnahtarAlan,
                        BelgeTipi = (int)EnumTarihceIslemTur.RiskYonetimiBeyannameImza,
                        OnaylayacakYetki = "MERKEZKOORDINATOR,ILKOORDINATOR"
                    };

                    //TEST işleminde mail atmasın kapatıldı. Yazılım çalışmaya başladığında açılacak Melih 29.09.2023
                    //Talep üzerinde tekrar açıldı. Hüseyin 01.03.2024 
                    Sonuc sonucMail = await _serviceBildirimSistemi.MailGonderAsync(kullanan, formBildirim);

                    //Mail Gönder Bitiş
                }

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }


            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.YeniImzaSureciBaslatBasarili"]);
        }

        /// <summary>
        /// Hatırlatma maili gönderilmesi işlemini sağlayan metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> HatirlatmaMailiGonder(KullaniciDto kullanan)
        {
            try
            {
                //Risk Yönetimi Taahhütnamesi imzalamayanlara hatırlatma e-postası gönder
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

                    //Tarihçe Başlangıç
                    tarihce.IlgiKod = "RiskYonetimiBeyannamesiMailGonder";
                    tarihce.IlgiTur = EnumTarihceIslemTur.RiskYonetimiBeyannamesiMailGonder;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = 19; // Bilgilendirme

                    var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiş


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
