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

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// BulguYonetimi işlemlerinin yapıldığı servis
    /// </summary>
    public class BulguYonetimiService : IBulguYonetimiService
    {
        /// <summary>
        /// IUnitOfWork<BulguYonetimi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<BulguYonetimi> _unitOfWork;
        /// <summary>
        /// IBulguYonetimiBirimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IBulguYonetimiBirimService _serviceBulguYonetimiBirim;
        /// <summary>
        /// IDenetimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimService _serviceDenetim;
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
        /// <see cref="Risk.net.Services.Functions.AltSurecService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceBulguYonetimiBirim"></param>
        /// <param name="serviceDenetim"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public BulguYonetimiService(IUnitOfWork<BulguYonetimi> unitOfWork,
                                    IBulguYonetimiBirimService serviceBulguYonetimiBirim,
                                    IDenetimService serviceDenetim,
                                    ITarihceService serviceTarihce,
                                    IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceBulguYonetimiBirim = serviceBulguYonetimiBirim;
            _serviceDenetim = serviceDenetim;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "Denetim,Denetim.DenetimYapanKurum,OnemDuzeyi,Birimler,Birimler.Koordinatorluk, Birimler.Birim");

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
        /// <param name="denetimKodu"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string denetimKodu)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.DenetimKod == denetimKodu, o => o.BulguNo);
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
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, BulguYonetimi kriter, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "CevapDurum,Denetim,Birimler,Birimler.Koordinatorluk, Birimler.Birim", a => a.OnemDuzeyi);

            if (!string.IsNullOrWhiteSpace(kriter.DenetimKod))
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.DenetimKod == kriter.DenetimKod);

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum != (int)ENUMDurum.Pasif);

            //bool yetkiKoordinator = Arac.YetkisiVarmi("ICDENETIMKOORDINATOR", kullanan);
            //bool yetkiUzman = Arac.YetkisiVarmi("ICDENETIMUZMANI", kullanan);
            //if (!yetkiKoordinator && yetkiUzman)
            //{
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Denetim.Sorumlular.Any(b => b.SorumluKod == kullanan.PersonelKod) || a.Denetim.Denetciler.Any(b => b.DenetciKod == kullanan.PersonelKod));
            //}
            //if (!yetkiKoordinator && !yetkiUzman)
            //{
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Birimler.Any(a => a.KoordinatorlukKod == kullanan.KoordinatorlukKod));
            //}

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                BulguYonetimi aramaObj = Arac.DataTablesAramaNesne<BulguYonetimi>(new BulguYonetimi(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.SorguKoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Birimler.Any(a => a.KoordinatorlukKod == aramaObj.SorguKoordinatorlukKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguBirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Birimler.Any(a => a.BirimKod == aramaObj.SorguBirimKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.OnemDuzeyiKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OnemDuzeyiKod.Contains(aramaObj.OnemDuzeyiKod));
                if (aramaObj.SorguKaynak > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Denetim.Kaynak == aramaObj.SorguKaynak);
                if (!string.IsNullOrWhiteSpace(aramaObj.DenetimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.DenetimKod == aramaObj.DenetimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BulguNo))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BulguNo == aramaObj.BulguNo);
                if (!string.IsNullOrWhiteSpace(aramaObj.Oneriler))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Oneriler.Contains(aramaObj.Oneriler));
                if (aramaObj.SonCevapTarihi1.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.SonCevapTarihi >= aramaObj.SonCevapTarihi1);
                if (aramaObj.SonCevapTarihi2.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.SonCevapTarihi <= aramaObj.SonCevapTarihi2);
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                //if (!string.IsNullOrWhiteSpace(aramaObj.OnemDuzeyiKod))
                //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OnemDuzeyiKod == aramaObj.OnemDuzeyiKod);
                else if (kriter.SorguDurum > 0)//İzleme takip ekranında sadece açık olanlar listelenecek
                {
                    if (kriter.SorguDurum == (int)ENUMDurum.Aktif)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum < 10);
                    else
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum > 10);
                }

            }
            else
            {
                if (kriter.SorguDurum > 0)
                {
                    if (kriter.SorguDurum == (int)ENUMDurum.Aktif)
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum < 10);
                    else
                        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum > 10);
                }

                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BulguTanimi.Contains(dataTablesParam.searchValue)
                                                                || a.BulguNo.Contains(dataTablesParam.searchValue));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, BulguYonetimi gelenNesne)
        {
            BulguYonetimi islemYapilan = new BulguYonetimi();

            string hata = "";

            if (gelenNesne.SorguKaynak == (int)EnumDenetimKaynak.IcDenetim)
            {
                if (string.IsNullOrWhiteSpace(gelenNesne.BulguTanimi))
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.BulguTanimiAlaniBos"] + "</li>";
            }

            hata = YetkisiVarmi(kullanan, "KAYDET");

            Sonuc denetimSonuc = await _serviceDenetim.KayitGetirAsync(kullanan, gelenNesne.DenetimKod);
            if (denetimSonuc.IslemSonuc)
            {
                if (denetimSonuc.Nesne != null)
                {
                    Denetim d = (Denetim)denetimSonuc.Nesne;
                    if (d.Durum == (int)ENUMDurum.GeriGonderildi)//Denetim Tamamlandı
                    {
                        hata += "<li>Denetim Tamamlandığı için Bulgu kaydında değişiklik yapılamaz";
                    }
                }
            }
            else
                hata += "Denetim kaydına ulaşılamadı";


            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();

                List<BulguYonetimiBirim> birimler = gelenNesne.Birimler;
                gelenNesne.Birimler = null;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    //gelenNesne.Durum = (int)ENUMDurum.Aktif;

                    gelenNesne.Kod = Arac.GetGuid();
                    int kayitSayisi = await _unitOfWork.KayitSayisiAsync(a => a.DenetimKod == gelenNesne.DenetimKod);
                    gelenNesne.BulguNo = "B" + (kayitSayisi + 1).ToString("000");

                    tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                    tarihce.Durum = gelenNesne.Durum;
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                    eskiKayit.BulguTanimi = gelenNesne.BulguTanimi;
                    eskiKayit.OnemDuzeyiKod = gelenNesne.OnemDuzeyiKod;
                    eskiKayit.BulguAciklama = gelenNesne.BulguAciklama;
                    eskiKayit.Riskler = gelenNesne.Riskler;
                    eskiKayit.Nedenler = gelenNesne.Nedenler;
                    eskiKayit.Kriterler = gelenNesne.Kriterler;
                    eskiKayit.Oneriler = gelenNesne.Oneriler;
                    eskiKayit.SonCevapTarihi = gelenNesne.SonCevapTarihi;
                    eskiKayit.Durum = gelenNesne.Durum;

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    tarihce.Durum = eskiKayit.Durum;

                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                await _serviceBulguYonetimiBirim.SilAsync(kullanan, gelenNesne.Kod);

                if (birimler != null)
                {
                    foreach (var item in birimler)
                    {
                        BulguYonetimiBirim giden = new BulguYonetimiBirim();
                        item.BulguYonetimiKod = gelenNesne.Kod;

                        await _serviceBulguYonetimiBirim.KaydetAsync(kullanan, item);
                    }
                }

                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.BulguYonetimi;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan.Kod);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            hata = YetkisiVarmi(kullanan, "SILME");

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

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
        /// Istemciden parametere ile gönderilen kaydın durumunu değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, BulguYonetimi gelenNesne)
        {
            BulguYonetimi islemYapilan = new BulguYonetimi();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "DURUM");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                eskiKayit.Durum = gelenNesne.Durum;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                Tarihce tarihce = new Tarihce();

                tarihce.Durum = gelenNesne.Durum;
                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.BulguYonetimi;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _unitOfWork.KaydetAsync();

                //Eğer denetimin tüm bulguları kapandı ise denetimi de kapat, açık ise denetimi de aç
                var bulguKayitlar = await _unitOfWork.ListeleAsync(a => a.DenetimKod == islemYapilan.DenetimKod && a.Durum < 10);
                int denetimDurum = (int)ENUMDurum.Aktif;
                if (bulguKayitlar.Count == 0)
                {
                    denetimDurum = (int)ENUMDurum.GeriGonderildi;//Denetim Kapalı
                }
                Denetim denetimNesne = new Denetim();
                denetimNesne.Kod = islemYapilan.DenetimKod;
                denetimNesne.Durum = denetimDurum;
                await _serviceDenetim.DurumDegistirAsync(kullanan, denetimNesne);
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

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın İzleme Takip bilgilerini değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> IzlemeTakipKaydetAsync(KullaniciDto kullanan, BulguYonetimi gelenNesne)
        {
            BulguYonetimi islemYapilan = new BulguYonetimi();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "DURUM");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                eskiKayit.DenetimTarihi = gelenNesne.DenetimTarihi;
                eskiKayit.EylemKarsilama = gelenNesne.EylemKarsilama;
                eskiKayit.EylemSonDurum = gelenNesne.EylemSonDurum;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();

                await DurumDegistirAsync(kullanan, gelenNesne);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen nihayi görüş bilgilerini kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> NihaiGorusKaydetAsync(KullaniciDto kullanan, BulguYonetimi gelenNesne)
        {
            BulguYonetimi islemYapilan = new BulguYonetimi();

            string hata = "";

            hata = YetkisiVarmi(kullanan, "KAYDET");

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.BulguYonetimiKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                eskiKayit.NihaiGorus = gelenNesne.NihaiGorus;
                eskiKayit.Durum = gelenNesne.Durum;

                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();

                await DurumDegistirAsync(kullanan, gelenNesne);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }

    }
}