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
using Newtonsoft.Json.Linq;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// StratejikPlan iþlemlerinin yapýldýðý servis
    /// </summary>
    public class StratejikPlanService : IStratejikPlanService
    {
        /// <summary>
        /// IUnitOfWork<StratejikPlan> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<StratejikPlan> _unitOfWork;
        /// <summary>
        /// IStratejikPlanHedefService servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStratejikPlanHedefService _serviceHedef;
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
        /// <see cref="Risk.net.Services.Functions.StratejikPlanService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceHedef"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanService(IUnitOfWork<StratejikPlan> unitOfWork, IStratejikPlanHedefService serviceHedef, ITarihceService serviceTarihce, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceHedef = serviceHedef;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod && (c.Hedefler.Any(rk => rk.Durum == (int)ENUMDurum.Aktif) || c.Hedefler.Count == 0), "StratejikPlanDonem, Hedefler.Gostergeler");

                if (kayit != null)
                {
                    kayit.Hedefler = kayit.Hedefler.OrderBy(u => u.HedefNo).ToList();//Hedef Noya göre sýralama
                    foreach (var hedef in kayit.Hedefler)
                    {
                        hedef.Gostergeler = hedef.Gostergeler.OrderBy(u => u.GostergeNo).ToList();
                    }

                    for (int i = 0; i < kayit.Hedefler.Count; i++)
                    {
                        if (kayit.Hedefler[i].Durum == (int)ENUMDurum.Pasif)
                        {
                            kayit.Hedefler.RemoveAt(i);
                            i--;
                        }

                    }

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
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="koordinatorlukKod"></param>
        /// <param name="durumKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string koordinatorlukKod, int durumKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.Durum == durumKod, o => o.AmacNo);//k => k.KoordinatorlukKod == koordinatorlukKod && 
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
        /// <param name="durumKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, int durumKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.Durum == durumKod, o => o.AmacNo);
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
        /// <param name="durumKod"></param>
        /// <param name="baslamaYil"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, int durumKod, int baslamaYil)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Hedefler,StratejikPlanDonem");

            //Baþalama yýlýna göre aratýlýyorsa duruma bakýlmasýn.
            //selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == durumKod);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlanDonem.BaslamaYil == baslamaYil);

            var kayitlar = selectData.ToList();

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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "Hedefler,StratejikPlanDonem");//Koordinatorluk,Birim,

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => (a.Hedefler.Any(rk => rk.Durum == (int)ENUMDurum.Aktif) || a.Hedefler.Count == 0));

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                StratejikPlan aramaObj = Arac.DataTablesAramaNesne<StratejikPlan>(new StratejikPlan(), aramaDegeri);

                //if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                //if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.Amac))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Amac.Contains(aramaObj.Amac));
                if (!string.IsNullOrWhiteSpace(aramaObj.StratejikPlanDonemKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlanDonemKod == aramaObj.StratejikPlanDonemKod);
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif);

                if (!string.IsNullOrWhiteSpace(aramaObj.AmacNo))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.AmacNo == aramaObj.AmacNo);


                if (aramaObj.Yil > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitTarihi.Value.Year == aramaObj.Yil);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif);
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Amac.Contains(dataTablesParam.searchValue)
                                                                //|| a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.AmacNo == dataTablesParam.searchValue
                                                                //|| a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.Hedefler.Any(k => k.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.KayitTarihi.Value.Year == Arac.ConvertToInt(dataTablesParam.searchValue, 0)));
                //|| (a.StratejikPlanDonem.BaslamaYil >= Arac.ConvertToInt(dataTablesParam.searchValue, 0) && a.StratejikPlanDonem.BitisYil <= Arac.ConvertToInt(dataTablesParam.searchValue, 0))));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, StratejikPlan gelenNesne)
        {
            StratejikPlan islemYapilan = new StratejikPlan();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Amac))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";
            //if (string.IsNullOrWhiteSpace(gelenNesne.KoordinatorlukKod))
            //    hata += "<li>" + _sharedResource["Kontrol.Duzenle.KoordinatorlukAlaniBos"] + "</li>";
            //if (string.IsNullOrWhiteSpace(gelenNesne.BirimKod))
            //    hata += "<li>" + _sharedResource["Kontrol.Duzenle.BirimAlaniBos"] + "</li>";
            //if (gelenNesne.Hedefler == null || gelenNesne.Hedefler.Count == 0)
            //    hata += "<li>" + _sharedResource["Kontrol.Duzenle.HedefAlaniBos"] + "</li>";


            //Etki deðeri 100 kontrolü
            var kontrol = string.IsNullOrWhiteSpace(gelenNesne.Kod) ? gelenNesne : await _unitOfWork.KayitGetirAsync(a => a.Kod == gelenNesne.Kod && a.Hedefler.Any(rk => rk.Gostergeler.Any(rk2 => rk2.Durum == 1)), "Hedefler,Hedefler.Gostergeler");

            if (kontrol.Hedefler != null)
            {
                foreach (var hedef in kontrol.Hedefler)
                {
                    double toplamEtki = 0;

                    foreach (var item in hedef.Gostergeler)
                    {
                        if (item.Durum != 1)
                            continue;

                        toplamEtki += item.Etki;
                    }

                    if (toplamEtki != 100)
                        hata += "<li>" + hedef.Adi + "  hedefinin " + _sharedResource["Kontrol.Duzenle.StratejikPlanHedefGostergeEtkiHatali"] + "</li>";
                }
            }
            //-----------------------

            hata += YetkisiVarmi(kullanan, "KAYDET");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                Tarihce tarihce = new Tarihce();
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                //sureckaydet iþleminde altsurecler dolu olursa hata veriyor EF den dolayý
                List<StratejikPlanHedef> hedefler = gelenNesne.Hedefler;
                gelenNesne.Hedefler = null;

                //Kayýt yapýldýðýnda durum aktif deðerini alsýn
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    gelenNesne.KayitTarihi = DateTime.Now.Date;

                    int kayitSayisi = await _unitOfWork.KayitSayisiAsync();
                    gelenNesne.AmacNo = "A." + (kayitSayisi + 1);

                    tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                    tarihce.Durum = gelenNesne.Durum;

                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    tarihce.EskiDeger = Arac.JSONSerialize(eskiKayit);

                    eskiKayit.Amac = gelenNesne.Amac;
                    eskiKayit.StratejikPlanDonemKod = gelenNesne.StratejikPlanDonemKod;
                    eskiKayit.Durum = gelenNesne.Durum;

                    tarihce.YeniDeger = Arac.JSONSerialize(eskiKayit);
                    tarihce.Durum = gelenNesne.Durum;

                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                if (hedefler != null)
                {
                    foreach (var item in hedefler)
                    {
                        item.StratejikPlanKod = gelenNesne.Kod;
                        item.Durum = (int)ENUMDurum.Aktif;
                        item.SorguAmacNo = gelenNesne.AmacNo;

                        await _serviceHedef.KaydetAsync(kullanan, item);
                    }
                }

                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.StratejikPlan;
                tarihce.IslemYapanKod = kullanan.PersonelKod;

                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], gelenNesne.Kod);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydý silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "SIL");

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
        /// Istemciden parametere ile gönderilen kaydýn durumunu deðiþtiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, StratejikPlan gelenNesne)
        {
            StratejikPlan islemYapilan = new StratejikPlan();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "DURUM");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Durum deðiþikliðine uygun mu?
                //if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum == (int)ENUMDurum.Pasif)
                //    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.PasifKayitOnaylanamaz"] + "</li>";
                //if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum == (int)ENUMDurum.Onayli)
                //    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.ZatenOnayli"] + "</li>";

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                eskiKayit.Durum = gelenNesne.Durum;
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                Tarihce tarihce = new Tarihce();

                tarihce.Durum = gelenNesne.Durum;
                tarihce.IlgiKod = gelenNesne.Kod;
                tarihce.IlgiTur = EnumTarihceIslemTur.StratejikPlan;
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
        /// Istemciden parametre ile talep edilen kullanýcýnýn yetkisinin olup olmadýðýný döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="tur"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        private string YetkisiVarmi(KullaniciDto kullanan, string tur)
        {
            bool yetki = Arac.YetkisiVarmi("PLANLAMAUNITESI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }
    }
}