using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// StratejikPlanHedefGosterge işlemlerinin yapıldığı servis
    /// </summary>
    public class StratejikPlanHedefGostergeService : IStratejikPlanHedefGostergeService
    {
        /// <summary>
        /// IUnitOfWork<StratejikPlanHedefGosterge> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<StratejikPlanHedefGosterge> _unitOfWork;
        /// <summary>
        /// IUnitOfWork<StratejikPlanHedef> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<StratejikPlanHedef> _unitOfWorkStratejikPlanHedef;
        /// <summary>
        /// IStratejikPlanIzlemeDonemService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanIzlemeDonemService _serviceIzlemeDonem;
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
        /// <see cref="Risk.net.Services.Functions.StratejikPlanHedefGostergeService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unitOfWorkStratejikPlanHedef"></param>
        /// <param name="serviceIzlemeDonem"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanHedefGostergeService(IUnitOfWork<StratejikPlanHedefGosterge> unitOfWork,
                                                IUnitOfWork<StratejikPlanHedef> unitOfWorkStratejikPlanHedef,
                                                IStratejikPlanIzlemeDonemService serviceIzlemeDonem,
                                                ITarihceService serviceTarihce,
                                                IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkStratejikPlanHedef = unitOfWorkStratejikPlanHedef;
            _serviceIzlemeDonem = serviceIzlemeDonem;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "Donemler");

                //Donemleri sırala
                kayit.Donemler = kayit.Donemler.OrderBy(o => o.Donem).ToList();


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
        /// <param name="stratejikPlanHedefKod"></param>
        /// <param name="durumKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string stratejikPlanHedefKod, int durumKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.StratejikPlanHedefKod == stratejikPlanHedefKod && a.Durum == durumKod, o => o.GostergeNo, a => a.Donemler, a => a.Birim);
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                StratejikPlanHedefGosterge aramaObj = Arac.DataTablesAramaNesne<StratejikPlanHedefGosterge>(new StratejikPlanHedefGosterge(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(aramaObj.Adi));
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, StratejikPlanHedefGosterge gelenNesne)
        {
            StratejikPlanHedefGosterge islemYapilan = new StratejikPlanHedefGosterge();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.StratejikPlanHedefKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.StratejikPlanHedefKodAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.Adi))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";

            hata += YetkisiVarmi(kullanan, "KAYDET");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                //Etki değeri 100 den büyük kontrolü
                var strPlanHedefEski = await _unitOfWorkStratejikPlanHedef.KayitGetirAsync(a => a.Kod == gelenNesne.StratejikPlanHedefKod, "Gostergeler");

                double toplamEtki = 0;

                if (strPlanHedefEski == null)
                    toplamEtki += gelenNesne.Etki;
                else if (strPlanHedefEski.Gostergeler != null)
                {
                    foreach (var item in strPlanHedefEski.Gostergeler)
                    {
                        if (item.Durum != 1)
                            continue;

                        if (gelenNesne.Kod == item.Kod)
                            toplamEtki += gelenNesne.Etki;
                        else
                            toplamEtki += item.Etki;
                    }

                    if (string.IsNullOrWhiteSpace(gelenNesne.Kod) || gelenNesne.Kod.IndexOf("dtabloYeni_") > -1)
                        toplamEtki += gelenNesne.Etki;
                }

                if (toplamEtki > 100)
                    hata += "<li>" + _sharedResource["Kontrol.Duzenle.StratejikPlanHedefGostergeEtkiHatali"] + "</li>";
                //----------------------------------------------------------------------------

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);



                if (string.IsNullOrWhiteSpace(gelenNesne.Kod) || gelenNesne.Kod.IndexOf("dtabloYeni_") > -1)
                {
                    if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                        gelenNesne.Kod = Arac.GetGuid();
                    else
                        gelenNesne.Kod = gelenNesne.Kod.Replace("dtabloYeni_", "");

                    //Numara alma işleminde Hedef Numarasını aldığı için
                    //*****************************************************************
                    string hedefNo = gelenNesne.SorguHedefNo;
                    if (string.IsNullOrWhiteSpace(hedefNo))
                    {
                        var strPlanHedefKaydi = await _unitOfWorkStratejikPlanHedef.KayitGetirAsync(a => a.Kod == gelenNesne.StratejikPlanHedefKod);
                        if (!string.IsNullOrWhiteSpace(strPlanHedefKaydi.Kod))
                        {
                            hedefNo = strPlanHedefKaydi.HedefNo;
                        }
                    }
                    hedefNo = hedefNo.Replace("H.", "");

                    int kayitSayisi = await _unitOfWork.KayitSayisiAsync(a => a.StratejikPlanHedefKod == gelenNesne.StratejikPlanHedefKod);
                    gelenNesne.GostergeNo = "PG." + hedefNo + "." + (kayitSayisi + 1);
                    //*****************************************************************

                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    eskiKayit.StratejikPlanHedefKod = gelenNesne.StratejikPlanHedefKod;
                    eskiKayit.Adi = gelenNesne.Adi;
                    eskiKayit.Etki = gelenNesne.Etki;
                    eskiKayit.Aciklama = gelenNesne.Aciklama;
                    eskiKayit.RevizyonNedeni = gelenNesne.RevizyonNedeni;
                    eskiKayit.RevizyonTarihi = gelenNesne.RevizyonTarihi;
                    eskiKayit.Durum = gelenNesne.Durum;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen izleme bilgileri kaydeden metod (Tarih, başlangış değeri)
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetIzlemeAsync(KullaniciDto kullanan, StratejikPlanHedefGosterge gelenNesne)
        {
            StratejikPlanHedefGosterge islemYapilan = new StratejikPlanHedefGosterge();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.StratejikPlanHedefKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.StratejikPlanHedefKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                eskiKayit.BaslangicDegeri = gelenNesne.BaslangicDegeri;
                eskiKayit.BaslangicTarihi = gelenNesne.BaslangicTarihi;
                eskiKayit.BitisTarihi = gelenNesne.BitisTarihi;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();

                //Dönem bilgisinin kayıt edilmesi
                //Eskiden kayıt edilen varsa silinsin
                StratejikPlanIzlemeDonem kriter = new StratejikPlanIzlemeDonem { StratejikPlanHedefGostergeKod = gelenNesne.Kod };
                await _serviceIzlemeDonem.SilAsync(kullanan, kriter);

                //Gelen veriler kayıt edilsin
                if (gelenNesne.Donemler != null)
                {
                    foreach (var donem in gelenNesne.Donemler)
                    {
                        await _serviceIzlemeDonem.KaydetAsync(kullanan, donem);
                    }
                }

                //Tarihçe kaydı
                Tarihce tarihce = new Tarihce();
                tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                tarihce.IlgiKod = gelenNesne.StratejikPlanHedefKod + "iz";//Tarihçe izleme kod kaydına göre tutulduğu için
                tarihce.IlgiTur = EnumTarihceIslemTur.StratejikPlanIzleme;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = (int)ENUMDurum.Aktif;
                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="hedefKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string hedefKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(hedefKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.StratejikPlanHedefKod == hedefKod);
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
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, StratejikPlanHedefGosterge gelenNesne)
        {
            StratejikPlanHedefGosterge islemYapilan = new StratejikPlanHedefGosterge();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Durum değişikliğine uygun mu?
                if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum == (int)ENUMDurum.Pasif)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.PasifKayitOnaylanamaz"] + "</li>";
                if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.ZatenOnayli"] + "</li>";

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                eskiKayit.Durum = gelenNesne.Durum;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.IslemBasarili"]);
        }

        private string YetkisiVarmi(KullaniciDto kullanan, string tur)
        {
            bool yetki = Arac.YetkisiVarmi("PLANLAMAUNITESI,BIRIMAMIRI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }
    }
}
