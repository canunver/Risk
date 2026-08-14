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
    /// Denetim işlemlerinin yapıldığı servis
    /// </summary>
    public class DenetimService : IDenetimService
    {
        /// <summary>
        /// IUnitOfWork<Denetim> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<Denetim> _unitOfWork;
        /// <summary>
        /// IDenetimDenetciService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimDenetciService _serviceDenetimDenetci;
        /// <summary>
        /// IDenetimSorumluService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimSorumluService _serviceDenetimSorumlu;
        /// <summary>
        /// IDenetimBirimService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDenetimBirimService _serviceDenetimBirim;
        /// <summary>
        /// IUnitOfWork<BulguYonetimi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<BulguYonetimi> _unitOfWorkBulguYonetimi;
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
        /// <param name="serviceDenetimDenetci"></param>
        /// <param name="serviceDenetimSorumlu"></param>
        /// <param name="serviceDenetimBirim"></param>
        /// <param name="unitOfWorkBulguYonetimi"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public DenetimService(IUnitOfWork<Denetim> unitOfWork,
                                IDenetimDenetciService serviceDenetimDenetci,
                                IDenetimSorumluService serviceDenetimSorumlu,
                                IDenetimBirimService serviceDenetimBirim,
                                IUnitOfWork<BulguYonetimi> unitOfWorkBulguYonetimi,
                                ITarihceService serviceTarihce,
                                IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceDenetimDenetci = serviceDenetimDenetci;
            _serviceDenetimSorumlu = serviceDenetimSorumlu;
            _serviceDenetimBirim = serviceDenetimBirim;
            _unitOfWorkBulguYonetimi = unitOfWorkBulguYonetimi;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "Denetciler,Denetciler.Denetci,Sorumlular,Sorumlular.Sorumlu,DenetimYapanKurum,Birimler,Birimler.Koordinatorluk, Birimler.Birim, Birimler.Surec, Birimler.AltSurec,Gorevlendirme");

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
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, Denetim gelenNesne)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.Kaynak == gelenNesne.Kaynak, o => o.DenetimNo, a => a.DenetimYapanKurum);//, a => a.Denetciler.Select(c => c.Denetci), a => a.Sorumlular.Select(c => c.Sorumlu)
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
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, int denetimKaynak, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Bulgular,Denetciler,Sorumlular,Denetciler.Denetci,Sorumlular.Sorumlu,Birimler,Birimler.Koordinatorluk, Birimler.Birim");

            selectData = selectData.OrderBy(a => a.Durum).ThenBy(a => a.Kaynak).ThenBy(a => a.DenetimNo);

            if (denetimKaynak > 0)
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Kaynak == denetimKaynak);

            //bool yetki = Arac.YetkisiVarmi("ICDENETIMKOORDINATOR,ICDENETIMUZMANI", kullanan);
            //if (!yetki)
            //{
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Sorumlular.Any(a => a.SorumluKod == kullanan.PersonelKod) || a.Denetciler.Any(a => a.DenetciKod == kullanan.PersonelKod));
            //}

            if (dataTablesParam.pageName == "EylemPlani" || dataTablesParam.pageName == "IzlemeTakip")
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Bulgular.Count > 0);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                Denetim aramaObj = Arac.DataTablesAramaNesne<Denetim>(new Denetim(), aramaDegeri);

                if (aramaObj.Yil > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Yil == aramaObj.Yil);
                if (!string.IsNullOrWhiteSpace(aramaObj.DenetimAdi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.DenetimAdi.Contains(aramaObj.DenetimAdi));
                if (!string.IsNullOrWhiteSpace(aramaObj.DenetimNo))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.DenetimNo == aramaObj.DenetimNo);
                if (!string.IsNullOrWhiteSpace(aramaObj.ReferansNo))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.ReferansNo.Contains(aramaObj.ReferansNo));
                if (!string.IsNullOrWhiteSpace(aramaObj.DenetimYapanKurumKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.DenetimYapanKurumKod.Contains(aramaObj.DenetimYapanKurumKod));
                if (aramaObj.BitisTarihi1.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BaslangicTarihi >= aramaObj.BitisTarihi1);
                if (aramaObj.BitisTarihi2.HasValue)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BitisTarihi <= aramaObj.BitisTarihi2);
                if (aramaObj.Kaynak > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Kaynak == aramaObj.Kaynak);

                if (!string.IsNullOrWhiteSpace(aramaObj.SorguSorumluKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Sorumlular.Any(a => a.SorumluKod == aramaObj.SorguSorumluKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguDenetciKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Denetciler.Any(a => a.DenetciKod == aramaObj.SorguDenetciKod));

                if (!string.IsNullOrWhiteSpace(aramaObj.SorguKoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Birimler.Any(a => a.KoordinatorlukKod == aramaObj.SorguKoordinatorlukKod));
                if (!string.IsNullOrWhiteSpace(aramaObj.SorguBirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Birimler.Any(a => a.BirimKod == aramaObj.SorguBirimKod));

                //if (aramaObj.Yil > 1950)
                //    selectData = await _unitOfWork.KosulEkle(selectData, a => a.OlayTarihi <= aramaObj.SorguTarihi2);
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.DenetimAdi.Contains(dataTablesParam.searchValue)
                                                                || a.DenetimNo.Contains(dataTablesParam.searchValue)
                                                                || a.DenetimYapanKurum.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.ReferansNo.Contains(dataTablesParam.searchValue));

                //Durum Pasif olanlar hariç hepsi listelensin 24.10.2023
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum != (int)ENUMDurum.Pasif);
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Denetim gelenNesne)
        {
            Denetim islemYapilan = new Denetim();

            string hata = "";

            hata = YetkisiVarmi(kullanan, "KAYDET");

            if (gelenNesne.Kaynak == 0)
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.DenetimKaynakAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();

                List<DenetimDenetci> denetciler = gelenNesne.Denetciler;
                gelenNesne.Denetciler = null;
                List<DenetimSorumlu> sorumlular = gelenNesne.Sorumlular;
                gelenNesne.Sorumlular = null;
                List<DenetimBirim> birimler = gelenNesne.Birimler;
                gelenNesne.Birimler = null;

                gelenNesne.Gorevlendirme = null;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    int kayitSayisi = await _unitOfWork.KayitSayisiAsync(a => a.Kaynak == gelenNesne.Kaynak);
                    gelenNesne.DenetimNo = (kayitSayisi + 1).ToString("00000");
                    gelenNesne.Durum = (int)ENUMDurum.Aktif;

                    tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                    tarihce.Durum = gelenNesne.Durum;
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                    if (eskiKayit.Durum == (int)ENUMDurum.GeriGonderildi)//Denetim Tamamlandı
                    {
                        hata += "<li>Denetim Tamamlandığı için değişiklik yapılamaz";
                        return new Sonuc(ENUMIslemDurum.Uyari, hata);
                    }


                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                    eskiKayit.DenetimAdi = gelenNesne.DenetimAdi;
                    eskiKayit.BaslangicTarihi = gelenNesne.BaslangicTarihi;
                    eskiKayit.BitisTarihi = gelenNesne.BitisTarihi;
                    eskiKayit.ReferansNo = gelenNesne.ReferansNo;
                    eskiKayit.Yil = gelenNesne.Yil;
                    eskiKayit.DenetimYapanKurumKod = gelenNesne.DenetimYapanKurumKod;
                    eskiKayit.Durum = (int)ENUMDurum.Aktif;

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    tarihce.Durum = eskiKayit.Durum;

                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.Denetim;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _serviceDenetimDenetci.SilAsync(kullanan, gelenNesne.Kod);

                if (denetciler != null)
                {
                    foreach (var item in denetciler)
                    {
                        DenetimDenetci giden = new DenetimDenetci();
                        item.DenetimKod = gelenNesne.Kod;

                        await _serviceDenetimDenetci.KaydetAsync(kullanan, item);
                    }
                }

                await _serviceDenetimSorumlu.SilAsync(kullanan, gelenNesne.Kod);

                if (sorumlular != null)
                {
                    foreach (var item in sorumlular)
                    {
                        DenetimSorumlu giden = new DenetimSorumlu();
                        item.DenetimKod = gelenNesne.Kod;

                        await _serviceDenetimSorumlu.KaydetAsync(kullanan, item);
                    }
                }

                await _serviceDenetimBirim.SilAsync(kullanan, gelenNesne.Kod);

                if (birimler != null)
                {
                    foreach (var item in birimler)
                    {
                        DenetimBirim giden = new DenetimBirim();
                        item.DenetimKod = gelenNesne.Kod;

                        await _serviceDenetimBirim.KaydetAsync(kullanan, item);
                    }
                }

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
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, Denetim gelenNesne)
        {
            Denetim islemYapilan = new Denetim();

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
                tarihce.IlgiTur = EnumTarihceIslemTur.Denetim;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _unitOfWork.KaydetAsync();
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
        /// string türünde yetki bilgisi döndürür
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
        /// Istemciden parametere ile gönderilen kaydın rapor no bilgisini değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RaporNoKaydetAsync(KullaniciDto kullanan, Denetim gelenNesne)
        {
            Denetim islemYapilan = new Denetim();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "RAPORNO");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                eskiKayit.RaporNo = gelenNesne.RaporNo;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın durum bilgisini değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> TamamlaAsync(KullaniciDto kullanan, Denetim gelenNesne)
        {
            Denetim islemYapilan = new Denetim();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "TAMAMLA");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                if (eskiKayit != null)
                {
                    //Bağlı bulgular bulunup kapandı yapılacak
                    //Eğer denetimin tüm bulguları kapandı ise denetimi de kapat, açık ise denetimi de aç
                    var bulguKayitlar = await _unitOfWorkBulguYonetimi.ListeleAsync(a => a.DenetimKod == gelenNesne.Kod && a.Durum < 10);
                    foreach (var bulgu in bulguKayitlar)
                    {
                        bulgu.Durum = 12;
                        await _unitOfWorkBulguYonetimi.GuncelleAsync(bulgu);
                    }
                    await _unitOfWorkBulguYonetimi.KaydetAsync();

                    //Denetimi tamamlandı yap
                    Denetim denetimNesne = new Denetim();
                    denetimNesne.Kod = gelenNesne.Kod;
                    denetimNesne.Durum = (int)ENUMDurum.GeriGonderildi;//Denetim Kapalı
                    await DurumDegistirAsync(kullanan, denetimNesne);
                }
                else
                    return new Sonuc(ENUMIslemDurum.Hata, "<li>Denetim bulunamadı</li>");
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }
    }
}