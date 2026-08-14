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

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// BulguYonetimiCevap işlemlerinin yapıldığı servis
    /// </summary>
    public class BulguYonetimiCevapService : IBulguYonetimiCevapService
    {
        /// <summary>
        /// IUnitOfWork<BulguYonetimiCevap> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<BulguYonetimiCevap> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;
        /// <summary>
        /// IBulguYonetimiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiService _serviceBulguYonetimi;
        /// <summary>
        /// IDenetimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimService _serviceDenetim;
        /// <summary>
        /// IViewPersonelService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewPersonelService _servicePersonel;
        /// <summary>
        /// IUnitOfWork<ViewKoordinatorluk> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewKoordinatorluk> _unitOfWorkViewKoordinatorluk;
        /// <summary>
        /// IBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBildirimSistemiService _serviceBildirimSistemi;
        /// <summary>
        /// IViewBildirimSistemiService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IViewBildirimSistemiService _serviceViewBildirim;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.BulguYonetimiCevapService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceBulguYonetimi"></param>
        /// <param name="serviceDenetim"></param>
        /// <param name="servicePersonel"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public BulguYonetimiCevapService(IUnitOfWork<BulguYonetimiCevap> unitOfWork,
                                        IBulguYonetimiService serviceBulguYonetimi,
                                        IDenetimService serviceDenetim,
                                        IViewPersonelService servicePersonel,
                                        IUnitOfWork<ViewKoordinatorluk> unitOfWorkViewKoordinatorluk,
                                        IBildirimSistemiService serviceBildirimSistemi,
                                        IViewBildirimSistemiService serviceViewBildirim,
                                        IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceBulguYonetimi = serviceBulguYonetimi;
            _serviceDenetim = serviceDenetim;
            _servicePersonel = servicePersonel;
            _unitOfWorkViewKoordinatorluk = unitOfWorkViewKoordinatorluk;
            _serviceBildirimSistemi = serviceBildirimSistemi;
            _serviceViewBildirim = serviceViewBildirim;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "IlgiliPersonel,BulguYonetimi.Denetim.DenetimYapanKurum,BulguYonetimi.Denetim,BulguYonetimi,BulguYonetimi.OnemDuzeyi,BulguYonetimi.Birimler,BulguYonetimi.Birimler.Koordinatorluk,BulguYonetimi.Birimler.Birim,OnemDuzeyiGorusu,OneriGorusu,BulguGorusu");

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
        /// <param name="kullanan"></param>
        /// <param name="bulguYonetimiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, BulguYonetimiCevap kriter)
        {
            var predicate = PredicateBuilder.True<BulguYonetimiCevap>();

            predicate = predicate.And(a => a.Durum != (int)ENUMDurum.Pasif);

            if (!string.IsNullOrWhiteSpace(kriter.SorguDenetimKod))
                predicate = predicate.And(a => a.BulguYonetimi.DenetimKod == kriter.SorguDenetimKod);

            if (!string.IsNullOrWhiteSpace(kriter.BulguYonetimiKod))
                predicate = predicate.And(a => a.BulguYonetimiKod == kriter.BulguYonetimiKod);

            if (kriter.Tur > 0)
                predicate = predicate.And(a => a.Tur == kriter.Tur);

            var kayitlar = await _unitOfWork.ListeleAsync(predicate, o => o.IlgiliPersonel.Adi, b => b.IlgiliPersonel, b => b.BulguYonetimi);
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
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam, bool onay)
        {
            List<string> belgeNolar = new List<string>();
            if (onay)
            {
                BildirimSistemi kriter = new BildirimSistemi { BelgeTipi = (int)EnumTarihceIslemTur.BulguYonetimiCevap };

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
            var selectData = await _unitOfWork.SorguHazirlaAsync(k => k.Durum != (int)ENUMDurum.Pasif, "", a => a.BulguYonetimi.Denetim.DenetimYapanKurum, a => a.BulguYonetimi.Denetim, a => a.BulguYonetimi, a => a.IlgiliPersonel, a => a.BulguYonetimi.OnemDuzeyi, a => a.OnemDuzeyiGorusu, a => a.OneriGorusu, a => a.BulguGorusu);
            if (dataTablesParam.pageName == "EylemPlani")
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Onayli);
            }
            else if (!onay)
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.IlgiliPersonelKod == kullanan.PersonelKod);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                BulguYonetimi aramaObj = Arac.DataTablesAramaNesne<BulguYonetimi>(new BulguYonetimi(), aramaDegeri);

                if (aramaObj.SorguKaynak > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BulguYonetimi.Denetim.Kaynak == aramaObj.SorguKaynak);
                if (!string.IsNullOrWhiteSpace(aramaObj.BulguNo))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BulguYonetimi.BulguNo == aramaObj.BulguNo);

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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, BulguYonetimiCevap gelenNesne)
        {
            BulguYonetimiCevap islemYapilan = new BulguYonetimiCevap();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod) && string.IsNullOrWhiteSpace(gelenNesne.BulguYonetimiKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.BulguYonetimiKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            var eskiKayit = new BulguYonetimiCevap();
            if (!string.IsNullOrWhiteSpace(gelenNesne.Kod))//Sadece kod geldiyse
                eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

            //Cevap isteği bulgu başına tekken bu oluyordu. Bir bulgu için aynı kişiye birden fazla kez gönderim yapılabilir isteği geldi.
            //else if (!string.IsNullOrWhiteSpace(gelenNesne.BulguYonetimiKod) && !string.IsNullOrWhiteSpace(gelenNesne.IlgiliPersonelKod))//BulguKod ve Ilgili kişi kod geldiyse
            //    eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.BulguYonetimiKod == gelenNesne.BulguYonetimiKod && c.IlgiliPersonelKod == gelenNesne.IlgiliPersonelKod);

            if (!string.IsNullOrWhiteSpace(eskiKayit?.Kod))
                gelenNesne.Kod = eskiKayit.Kod;

            try
            {
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    int kayitSayisi = await _unitOfWork.KayitSayisiAsync();
                    gelenNesne.AtamaTarihi = System.DateTime.Now;

                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    //Onaya gönderilen kayıt değiştirilemez
                    if (eskiKayit.Durum == (int)ENUMDurum.OnayaGonderdi || eskiKayit.Durum == (int)ENUMDurum.Onayli)
                        hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";

                    if (hata != "")
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);

                    if (gelenNesne.CevapTarihi.HasValue)
                    {
                        //eskiKayit.Durum = (int)ENUMDurum.Onayli;//Cevaplandi   //Onaya gönderme eklendi
                        eskiKayit.Aciklama = gelenNesne.Aciklama;
                        eskiKayit.BulguGorusuKod = gelenNesne.BulguGorusuKod;
                        eskiKayit.CevapTarihi = gelenNesne.CevapTarihi;
                        eskiKayit.Eylem = gelenNesne.Eylem;
                        eskiKayit.OnemDuzeyiGorusuKod = gelenNesne.OnemDuzeyiGorusuKod;
                        eskiKayit.OneriGorusuKod = gelenNesne.OneriGorusuKod;
                        eskiKayit.Sorumlusu = gelenNesne.Sorumlusu;
                        eskiKayit.TamamlamaTarihi = gelenNesne.TamamlamaTarihi;
                        eskiKayit.NedenBulguKatilmiyor = gelenNesne.NedenBulguKatilmiyor;
                        eskiKayit.NedenOnemKatilmiyor = gelenNesne.NedenOnemKatilmiyor;
                        eskiKayit.NedenOneriKatilmiyor = gelenNesne.NedenOneriKatilmiyor;
                        eskiKayit.SonDurum = gelenNesne.SonDurum;
                    }
                    else
                    {
                        gelenNesne.AtamaTarihi = System.DateTime.Now;
                        gelenNesne.Durum = (int)ENUMDurum.Aktif;
                    }

                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                await _unitOfWork.KaydetAsync();

                //Onay verildiğinde mail gönderilecek
                //if (gelenNesne.CevapTarihi.HasValue)
                //    await CevapVerildiEPostaGonderAsync(kullanan, gelenNesne.Kod);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            Sonuc kayit = await KayitGetirAsync(kullanan, islemYapilan.Kod);

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], kayit.Nesne, islemYapilan.Kod);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, BulguYonetimiCevap gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.BulguYonetimiKod) || string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var kayit = await _unitOfWork.KayitSayisiAsync(d => d.BulguYonetimiKod == gelenNesne.BulguYonetimiKod && d.Kod == gelenNesne.Kod && d.CevapTarihi != null);
                if (kayit > 0)
                    return new Sonuc(ENUMIslemDurum.Uyari, _sharedResource["Kontrol.Sil.Cevaplandi"]);


                await _unitOfWork.SilAsync(d => d.BulguYonetimiKod == gelenNesne.BulguYonetimiKod && d.Kod == gelenNesne.Kod);
                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın durumunu değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, BulguYonetimiCevap gelenNesne)
        {
            BulguYonetimiCevap islemYapilan = new BulguYonetimiCevap();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Durum değişikliğine uygun mu?
                hata = Arac.DurumDegisikligiUygunMu(kullanan, _sharedResource, eskiKayit, gelenNesne);

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                eskiKayit.Durum = gelenNesne.Durum;

                //BildirimSistemi Başlangıç
                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi || gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Reddedildi)
                {
                    var bs = new BildirimSistemi
                    {
                        BelgeTipi = (int)EnumTarihceIslemTur.BulguYonetimiCevap,
                        BelgeKod = eskiKayit.Kod,
                        KoordinatorlukKod = kullanan.KoordinatorlukKod,
                        BirimKod = kullanan.BirimKod,
                        IslemYapanKod = kullanan.PersonelKod,
                        IslemTarihi = DateTime.Now,
                    };

                    if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                    {
                        var koordinatorluk = _unitOfWorkViewKoordinatorluk.KayitGetirAsync(k => k.Kod == kullanan.KoordinatorlukKod);

                        bs.OnaylayacakYetki = Arac.UstYetkiVer(kullanan, (EnumKoordinatorlukTur)koordinatorluk.Result.Tur);
                        if (bs.OnaylayacakYetki == "BIRIMAMIRI")
                            bs.OnaylayacakYetki = Arac.UstYetkiVer("BIRIMAMIRI", (EnumKoordinatorlukTur)koordinatorluk.Result.Tur);

                        //Eğer onaylayan kişi koordinatör ise kendisinin onaylayacak demektir. Otomatik onay yapılsın Melih 31.07.2023 
                        if (Arac.YetkisiVarmi("ILKOORDINATOR,MERKEZKOORDINATOR,GENELKOORDINATOR,ICDENETIMKOORDINATOR", kullanan))
                        {
                            bs.OnaylayacakYetki = kullanan.AktifRolKod;
                        }

                        bs.Durum = (int)ENUMDurum.OnayaGonderdi;

                        if (string.IsNullOrWhiteSpace(bs.OnaylayacakYetki))
                            hata += "<li>" + _sharedResource["Kontrol.DurumDegistir.OnaylayacakYetkiBulunamadi"] + "</li>";
                        else if (bs.OnaylayacakYetki == kullanan.AktifRolKod)
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
                    }
                    else if (gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi)
                    {
                        bs.Durum = (int)ENUMDurum.Pasif; //Geri Gönderilecek kişi yok
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
                }
                //BildirimSistemi Bitiş

                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();

                if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                    await CevapVerildiEPostaGonderAsync(kullanan, gelenNesne.Kod);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Kişilere atanan bulguların atanan kişiye haber verilmesi için eposta gönderiminin yapıldığı metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="bulgu"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> BilgiEPostaGonderAsync(KullaniciDto kullanan, string bulguKod, List<string> gelenNesne)
        {
            List<ViewPersonel> gidecekler = new List<ViewPersonel>();

            //Görevlendirme yapılan bulgu bilgisini almak için
            Sonuc sonucBulgu = await _serviceBulguYonetimi.KayitGetirAsync(kullanan, bulguKod);
            if (!sonucBulgu.IslemSonuc || sonucBulgu.Nesne == null) return new Sonuc(ENUMIslemDurum.Uyari, "Bulgu bulunamadı");
            BulguYonetimi gidecekBulgu = (BulguYonetimi)sonucBulgu.Nesne;

            //Görevlendirilen personelin eposta adreslerini almak için
            foreach (var personelKod in gelenNesne)
            {
                var sonucPersonel = await _servicePersonel.KayitGetirAsync(kullanan, personelKod);
                if (sonucPersonel.IslemSonuc && sonucPersonel.Nesne != null)
                {
                    ViewPersonel p = (ViewPersonel)sonucPersonel.Nesne;
                    string ePostaPersonel = p.EPosta;
                    if (!string.IsNullOrWhiteSpace(ePostaPersonel))
                        gidecekler.Add(p);
                }
            }

            try
            {
                foreach (var item in gidecekler)
                {
                    string to = item.EPosta;
                    string konu = "Bulguyu cevaplamak üzere görevlendirildiniz";
                    string denetimAdi = gidecekBulgu.Denetim.DenetimAdi;
                    if (gidecekBulgu.Denetim.Kaynak == 2)
                        denetimAdi = gidecekBulgu.Denetim.DenetimYapanKurum.Adi;

                    string mesaj = $"Sayın {item.AdiSoyadi};<br>" +
                                   $"{denetimAdi} / {gidecekBulgu.Denetim.DenetimNo} nolu Denetim kapsamında " +
                                   $"'{gidecekBulgu.BulguTanimi}' bulgusu ile ilgili, sorumlusu olarak belirlendiğiniz Eylem için " +
                                   $"Risk Uygulamasından giriş yaparak ilgili kısımlara {Arac.DateTimeToDDMMYYYY(gidecekBulgu.SonCevapTarihi)} tarihine kadar cevap vermeniz beklenmektedir.<br><br>" +
                                   $"Bilgilerinize önemle rica olunur.<br><br><br>" +
                                   $"Saygılarımızla,<br>" +
                                   $"İç Denetim Koordinatörlüğü";

                    /*
                “Sayın ……,
                . …Denetim Adı…. Denetimi kapsamında …Bulgu… bulgusu ile ilgili olarak,  sorumlusu olarak belirlendiğiniz Eylem için …. uygulamasından giriş yaparak ilgili kısımlara ….. tarihine kadar cevap vermeniz beklenmektedir.
                Bilgilerinize önemle rica olunur.”
                 */

                    Mail.MailAt("", to, konu, mesaj, true, false, null);
                }



            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.IslemBasarili"]);
        }

        /// <summary>
        /// Kişilerin bulgulara verdiği cevap bilgisini ilgili denetcilere haber verilmesi için eposta gönderiminin yapıldığı metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="bulguCevapKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> CevapVerildiEPostaGonderAsync(KullaniciDto kullanan, string bulguCevapKod)
        {
            //Cevap verilen kayıt bilgisini almak için
            Sonuc sonucBulguCevap = await KayitGetirAsync(kullanan, bulguCevapKod);
            if (!sonucBulguCevap.IslemSonuc || sonucBulguCevap.Nesne == null) return new Sonuc(ENUMIslemDurum.Uyari, "Bulgu Cevap bulunamadı");
            BulguYonetimiCevap gidecekBulguCevap = (BulguYonetimiCevap)sonucBulguCevap.Nesne;

            //Görevlendirme yapılan bulgu bilgisini almak için
            Sonuc sonucBulgu = await _serviceBulguYonetimi.KayitGetirAsync(kullanan, gidecekBulguCevap.BulguYonetimiKod);
            if (!sonucBulgu.IslemSonuc || sonucBulgu.Nesne == null) return new Sonuc(ENUMIslemDurum.Uyari, "Bulgu bulunamadı");
            BulguYonetimi gidecekBulgu = (BulguYonetimi)sonucBulgu.Nesne;

            Sonuc sonucDenetim = await _serviceDenetim.KayitGetirAsync(kullanan, gidecekBulgu.DenetimKod);
            if (!sonucDenetim.IslemSonuc || sonucDenetim.Nesne == null) return new Sonuc(ENUMIslemDurum.Uyari, "Denetim bulunamadı");
            Denetim gidecekDenetim = (Denetim)sonucDenetim.Nesne;

            string denetimKisiPostalari = "";
            //Denetim görevlilerinin eposta bilgisini bul
            foreach (var denetci in gidecekDenetim.Denetciler)
            {
                var sonucPersonel = await _servicePersonel.KayitGetirAsync(kullanan, denetci.DenetciKod);
                if (sonucPersonel.IslemSonuc && sonucPersonel.Nesne != null)
                {
                    ViewPersonel p = (ViewPersonel)sonucPersonel.Nesne;
                    string ePostaPersonel = p.EPosta;
                    if (!string.IsNullOrWhiteSpace(ePostaPersonel))
                    {
                        if (denetimKisiPostalari != "") denetimKisiPostalari += ";";
                        denetimKisiPostalari += ePostaPersonel;
                    }
                }
            }

            try
            {
                string to = denetimKisiPostalari;
                string konu = "Bulguya cevap verildi";

                string mesaj = $"'{gidecekBulgu.Denetim.DenetimNo}' nolu Denetim kapsamında " +
                               $"'{gidecekBulgu.BulguTanimi}' bulgusu {gidecekBulguCevap.IlgiliPersonel.AdiSoyadi} tarafından cevaplanmıştır.";

                Mail.MailAt("", to, konu, mesaj, true, false, null);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kullanıcının yetkisinin olup olmadığını döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="tur"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        private string YetkisiVarmi(KullaniciDto kullanan, string tur)
        {
            bool yetki = false;

            if (tur == "SILME")
                yetki = Arac.YetkisiVarmi("ICDENETIMKOORDINATOR", kullanan);
            else
                yetki = Arac.YetkisiVarmi("ICDENETIMKOORDINATOR,ICDENETIMUZMANI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }
    }
}
