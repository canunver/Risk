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
    /// StratejikPlanHedef iþlemlerinin yapýldýðý servis
    /// </summary>
    public class StratejikPlanHedefService : IStratejikPlanHedefService
    {
        /// <summary>
        /// IUnitOfWork<StratejikPlanHedef> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<StratejikPlanHedef> _unitOfWork;
        /// <summary>
        /// IStratejikPlanHedefGostergeService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanHedefGostergeService _serviceGosterge;
        /// <summary>
        /// IUnitOfWork<StratejikPlan> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<StratejikPlan> _unitOfWorkStratejikPlan;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;
        /// <summary>
        /// IStratejikPlanIsbirligiBirimService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanIsbirligiBirimService _serviceIsbirligiBirim;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.StratejikPlanHedefService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceGosterge"></param>
        /// <param name="serviceStratejikPlan"></param>
        /// <param name="unitOfWorkGosterge"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanHedefService(IUnitOfWork<StratejikPlanHedef> unitOfWork,
                                        IStratejikPlanHedefGostergeService serviceGosterge,
                                        IUnitOfWork<StratejikPlan> unitOfWorkStratejikPlan,
                                        IUnitOfWork<StratejikPlanHedefGosterge> unitOfWorkGosterge,
                                        IStratejikPlanIsbirligiBirimService serviceIsbirligiBirim,
                                        IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceGosterge = serviceGosterge;
            _serviceIsbirligiBirim = serviceIsbirligiBirim;
            _unitOfWorkStratejikPlan = unitOfWorkStratejikPlan;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "Gostergeler,Koordinatorluk,Birim,IsbirligiBirimler,IsbirligiBirimler.Koordinatorluk,IsbirligiBirimler.Birim,Gostergeler.Birim");

                //Göstergeleri sýrala
                kayit.Gostergeler = kayit.Gostergeler.OrderBy(o => o.GostergeNo).ToList();

                List<StratejikPlanHedefGosterge> gostergeler = new List<StratejikPlanHedefGosterge>();
                foreach (var item in kayit.Gostergeler)
                {
                    if (item.Durum == 99) continue;
                    gostergeler.Add(item);
                }
                kayit.Gostergeler = gostergeler;

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
        /// <param name="kullanan"></param>
        /// <param name="stratejikPlanKod"></param>
        /// <param name="durumKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string stratejikPlanKod, int durumKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.StratejikPlanKod == stratejikPlanKod && a.Durum == durumKod, o => o.HedefNo);
            if (kayitlar.Count > -1)
            {
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                StratejikPlanHedef aramaObj = Arac.DataTablesAramaNesne<StratejikPlanHedef>(new StratejikPlanHedef(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.StratejikPlanKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlanKod.Contains(aramaObj.StratejikPlanKod));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, StratejikPlanHedef gelenNesne)
        {
            StratejikPlanHedef islemYapilan = new StratejikPlanHedef();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.StratejikPlanKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.StratejikPlanKodAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.Adi))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                List<StratejikPlanIsbirligiBirim> isbirligiBirimler = gelenNesne.IsbirligiBirimler;
                gelenNesne.IsbirligiBirimler = null;

                List<StratejikPlanHedefGosterge> gostergeler = gelenNesne.Gostergeler;
                gelenNesne.Gostergeler = null;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod) || gelenNesne.Kod.IndexOf("dtabloYeni_") > -1)
                {
                    if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                        gelenNesne.Kod = Arac.GetGuid();
                    else
                        gelenNesne.Kod = gelenNesne.Kod.Replace("dtabloYeni_", "");

                    //Numara alma iþleminde Stratejik Plan Numarasýný aldýðý için
                    //*****************************************************************
                    string amacNo = gelenNesne.SorguAmacNo;
                    if (string.IsNullOrWhiteSpace(amacNo))
                    {
                        var strPlanKaydi = await _unitOfWorkStratejikPlan.KayitGetirAsync(a => a.Kod == gelenNesne.StratejikPlanKod);
                        if (!string.IsNullOrWhiteSpace(strPlanKaydi.Kod))
                        {
                            amacNo = strPlanKaydi.AmacNo;
                        }
                    }
                    amacNo = amacNo.Replace("A.", "");

                    int kayitSayisi = await _unitOfWork.KayitSayisiAsync(a => a.StratejikPlanKod == gelenNesne.StratejikPlanKod);
                    gelenNesne.HedefNo = "H." + amacNo + "." + (kayitSayisi + 1);
                    //*****************************************************************

                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    eskiKayit.StratejikPlanKod = gelenNesne.StratejikPlanKod;
                    eskiKayit.Adi = gelenNesne.Adi;
                    eskiKayit.Durum = gelenNesne.Durum;
                    eskiKayit.KoordinatorlukKod = gelenNesne.KoordinatorlukKod;
                    eskiKayit.BirimKod = gelenNesne.BirimKod;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                if (isbirligiBirimler != null)
                {
                    await _serviceIsbirligiBirim.SilAsync(kullanan, gelenNesne.Kod);

                    if (isbirligiBirimler != null)
                    {
                        foreach (var item in isbirligiBirimler)
                        {
                            StratejikPlanIsbirligiBirim giden = new StratejikPlanIsbirligiBirim();
                            item.StratejikPlanHedefKod = gelenNesne.Kod;

                            await _serviceIsbirligiBirim.KaydetAsync(kullanan, item);
                        }
                    }
                }

                if (gostergeler != null)
                {
                    foreach (var item in gostergeler)
                    {
                        StratejikPlanHedefGosterge giden = new StratejikPlanHedefGosterge();
                        item.StratejikPlanHedefKod = islemYapilan.Kod;
                        item.Durum = (int)ENUMDurum.Aktif;
                        item.SorguHedefNo = islemYapilan.HedefNo;

                        await _serviceGosterge.KaydetAsync(kullanan, item);
                    }
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
        /// Istemciden parametere ile gönderilen kaydý silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="stratejikPlanKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string stratejikPlanKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(stratejikPlanKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.StratejikPlanKod == stratejikPlanKod);
                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydýn durumunu deðiþtiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, StratejikPlanHedef gelenNesne)
        {
            StratejikPlanHedef islemYapilan = new StratejikPlanHedef();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            var hedefGostergeler = await _serviceGosterge.ListeleAsync(kullanan, gelenNesne.Kod, (int)ENUMDurum.Aktif);
            if (hedefGostergeler.Liste.Count > 0)
                hata = "<li>Silmek istediðiniz Hedefe baðlý Anahtar Performans Göstergeleri bulunmaktadýr. Ýlk önce Göstergeleri silin.</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Durum deðiþikliðine uygun mu?
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

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetBirimAsync(KullaniciDto kullanan, StratejikPlanHedef gelenNesne)
        {
            StratejikPlanHedef islemYapilan = new StratejikPlanHedef();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                List<StratejikPlanIsbirligiBirim> isbirligiBirimler = gelenNesne.IsbirligiBirimler;
                gelenNesne.IsbirligiBirimler = null;

                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                eskiKayit.KoordinatorlukKod = gelenNesne.KoordinatorlukKod;
                eskiKayit.BirimKod = gelenNesne.BirimKod;
                eskiKayit.Durum = gelenNesne.Durum;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _serviceIsbirligiBirim.SilAsync(kullanan, gelenNesne.Kod);
                if (isbirligiBirimler != null)
                {
                    if (isbirligiBirimler != null)
                    {
                        foreach (var item in isbirligiBirimler)
                        {
                            StratejikPlanIsbirligiBirim giden = new StratejikPlanIsbirligiBirim();
                            item.StratejikPlanHedefKod = gelenNesne.Kod;

                            await _serviceIsbirligiBirim.KaydetAsync(kullanan, item);
                        }
                    }
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }
    }
}
