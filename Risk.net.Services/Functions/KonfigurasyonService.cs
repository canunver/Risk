using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// Konfigurasyon iþlemlerinin yapýldýðý servis
    /// </summary>
    public class KonfigurasyonService : IKonfigurasyonService
    {
        /// <summary>
        /// IUnitOfWork<Konfigurasyon> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<Konfigurasyon> _unitOfWork;
        /// <summary>
        /// IUnitOfWork<TanimGenel> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<TanimGenel> _unitOfWorkGenel;
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.KonfigurasyonService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unitOfWorkGenel"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public KonfigurasyonService(IUnitOfWork<Konfigurasyon> unitOfWork,
            IUnitOfWork<TanimGenel> unitOfWorkGenel,
            ITarihceService serviceTarihce,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkGenel = unitOfWorkGenel;
            _serviceTarihce = serviceTarihce;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="durum"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, int durum)
        {
            string hata = "";

            if (durum == 0)
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                Sonuc sonuc = await ListeleAsync(kullanan);
                Konfigurasyon onayli = new Konfigurasyon();
                Konfigurasyon onaysiz = new Konfigurasyon();

                foreach (Konfigurasyon item in sonuc.Liste)
                {
                    if (item.Durum == (int)ENUMDurum.Onayli)
                        onayli = item;
                    else
                        onaysiz = item;
                }
                if (onayli != null && onaysiz != null)
                {
                    onayli.DigerKaydinDurumu = onaysiz.Durum;
                    onaysiz.DigerKaydinDurumu = onayli.Durum;
                }

                var kayit = onayli;

                if (durum != (int)ENUMDurum.Onayli)
                    kayit = onaysiz;
                else if (kayit.Kod == "")//Onaylý kayýt istendi ve yoksa
                    kayit = onaysiz;

                return new Sonuc(ENUMIslemDurum.Basarili, kayit);
            }
            catch (System.Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.Durum != (int)ENUMDurum.Pasif, null);
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Konfigurasyon gelenNesne)
        {
            Konfigurasyon islemYapilan = new Konfigurasyon();

            string hata = "";

            hata = YetkisiVarmi(kullanan, "KAYDET");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            string anahtar = "";

            try
            {
                if (!string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    //Onaylý kayýt kayýt edilmek isteniyorsa revize yapýlýyordur
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
                        gelenNesne.Kod = "";
                }

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                    gelenNesne.Durum = (int)ENUMDurum.Aktif;
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                    eskiKayit.AzaltmaPlaniBitis = gelenNesne.AzaltmaPlaniBitis;
                    eskiKayit.AzaltmaPlaniOlusturulmadi = gelenNesne.AzaltmaPlaniOlusturulmadi;
                    eskiKayit.EtkiOlasilikMatrisi = gelenNesne.EtkiOlasilikMatrisi;
                    eskiKayit.OnayBekleyenAzaltma = gelenNesne.OnayBekleyenAzaltma;
                    eskiKayit.OnayBekleyenRiskler = gelenNesne.OnayBekleyenRiskler;
                    eskiKayit.RiskKategoriFinansal = gelenNesne.RiskKategoriFinansal;

                    eskiKayit.Durum = (int)ENUMDurum.Aktif;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                anahtar = gelenNesne.Kod;

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], anahtar);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydýn durumunu deðiþtiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, Konfigurasyon gelenNesne)
        {
            Konfigurasyon islemYapilan = new Konfigurasyon();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "DURUM");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Onaylý belge, onaya gönderilmez
                if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    hata += "<li>" + _sharedResource["Kontrol.DurumDegistir.ZatenOnayli"] + "</li>";

                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                    hata = YetkisiVarmi(kullanan, "ONAYAGONDERME");
                else if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                    hata = YetkisiVarmi(kullanan, "ONAY");

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);

                if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum != (int)ENUMDurum.Onayli)
                {
                    //Onaylý kayýtlarýn hepsini iptal durumuna getir
                    var oListe = await _unitOfWork.SorguHazirlaAsync(x => x.Durum == (int)ENUMDurum.Onayli);
                    var kayitlar = oListe.Cast<Konfigurasyon>().ToList();
                    kayitlar.ForEach(a => a.Durum = (int)ENUMDurum.Pasif);

                }

                eskiKayit.Durum = gelenNesne.Durum;

                if (gelenNesne.Durum == (int)ENUMDurum.Onayli)
                {

                    //Tarihçe Baþlangýç

                    //EtkiOlasilikMatrisi alaný json olduðu için sadece deðiþen json alanlarýný almak için 
                    var eski = Newtonsoft.Json.Linq.JToken.Parse(eskiKayit.EtkiOlasilikMatrisi);
                    var yeni = Newtonsoft.Json.Linq.JToken.Parse(gelenNesne.EtkiOlasilikMatrisi);
                    var degisen = Arac.DegisenleriBul(yeni, eski);
                    var degisenDegerEtkiOlasilikMatrisi = degisen.ToString(Newtonsoft.Json.Formatting.None);

                    var eski_EtkiOlasilikMatrisi = "";
                    var yeni_EtkiOlasilikMatrisi = "";
                    if (!string.IsNullOrWhiteSpace(degisenDegerEtkiOlasilikMatrisi) && degisenDegerEtkiOlasilikMatrisi != "{}")
                    {
                        dynamic obj = JsonConvert.DeserializeObject<dynamic>(degisenDegerEtkiOlasilikMatrisi);
                        eski_EtkiOlasilikMatrisi = obj["eski"].ToString(Newtonsoft.Json.Formatting.None);
                        yeni_EtkiOlasilikMatrisi = obj["yeni"].ToString(Newtonsoft.Json.Formatting.None);
                    }
                    //-----------------------------------------------------------------------------------------

                    //RiskKategoriFinansal alaný json olduðu için sadece deðiþen json alanlarýný almak için 
                    eski = Newtonsoft.Json.Linq.JToken.Parse(eskiKayit.RiskKategoriFinansal);
                    yeni = Newtonsoft.Json.Linq.JToken.Parse(gelenNesne.RiskKategoriFinansal);
                    degisen = Arac.DegisenleriBul(yeni, eski);
                    var degisenDegerRiskKategoriFinansal = degisen.ToString(Newtonsoft.Json.Formatting.None);

                    var eski_RiskKategoriFinansal = "";
                    var yeni_RiskKategoriFinansal = "";
                    if (!string.IsNullOrWhiteSpace(degisenDegerRiskKategoriFinansal) && degisenDegerRiskKategoriFinansal != "{}")
                    {
                        dynamic obj2 = JsonConvert.DeserializeObject<dynamic>(degisenDegerRiskKategoriFinansal);
                        eski_RiskKategoriFinansal = obj2["eski"].ToString(Newtonsoft.Json.Formatting.None);
                        yeni_RiskKategoriFinansal = obj2["yeni"].ToString(Newtonsoft.Json.Formatting.None);
                    }
                    //-----------------------------------------------------------------------------------------

                    var tarihce_EskiKayit = new Konfigurasyon
                    {
                        Kod = eskiKayit.Kod,
                        Durum = eskiKayit.Durum,
                        AzaltmaPlaniBitis = eskiKayit.AzaltmaPlaniBitis,
                        AzaltmaPlaniOlusturulmadi = eskiKayit.AzaltmaPlaniOlusturulmadi,
                        EtkiOlasilikMatrisi = eski_EtkiOlasilikMatrisi,
                        OnayBekleyenAzaltma = eskiKayit.OnayBekleyenAzaltma,
                        OnayBekleyenRiskler = eskiKayit.OnayBekleyenRiskler,
                        RiskKategoriFinansal = eski_RiskKategoriFinansal
                    };

                    var tarihce_YeniKayit = new Konfigurasyon
                    {
                        Kod = eskiKayit.Kod,
                        Durum = eskiKayit.Durum,
                        AzaltmaPlaniBitis = gelenNesne.AzaltmaPlaniBitis,
                        AzaltmaPlaniOlusturulmadi = gelenNesne.AzaltmaPlaniOlusturulmadi,
                        EtkiOlasilikMatrisi = yeni_EtkiOlasilikMatrisi,
                        OnayBekleyenAzaltma = gelenNesne.OnayBekleyenAzaltma,
                        OnayBekleyenRiskler = gelenNesne.OnayBekleyenRiskler,
                        RiskKategoriFinansal = yeni_RiskKategoriFinansal
                    };

                    Tarihce tarihce = new Tarihce();
                    tarihce.IlgiKod = gelenNesne.Kod;
                    tarihce.IlgiTur = EnumTarihceIslemTur.Konfigurasyon;
                    tarihce.IslemYapanKod = kullanan.PersonelKod;
                    tarihce.Durum = (int)ENUMDurum.Onayli;
                    tarihce.EskiDeger = Arac.JSONSerialize(tarihce_EskiKayit);
                    tarihce.YeniDeger = Arac.JSONSerialize(tarihce_YeniKayit);

                    var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);
                    //Tarihçe Bitiþ

                    eskiKayit.AzaltmaPlaniBitis = gelenNesne.AzaltmaPlaniBitis;
                    eskiKayit.AzaltmaPlaniOlusturulmadi = gelenNesne.AzaltmaPlaniOlusturulmadi;
                    eskiKayit.EtkiOlasilikMatrisi = gelenNesne.EtkiOlasilikMatrisi;
                    eskiKayit.OnayBekleyenAzaltma = gelenNesne.OnayBekleyenAzaltma;
                    eskiKayit.OnayBekleyenRiskler = gelenNesne.OnayBekleyenRiskler;
                    eskiKayit.RiskKategoriFinansal = gelenNesne.RiskKategoriFinansal;
                }

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
        /// Istemciden parametere ile gönderilen bilgilere ait etki kriteri adýný döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="seviye"></param>
        /// <param name="riskKategorisi"></param>
        /// <returns>
        /// Etki kriter adýný göndürür
        /// </returns>
        public async Task<string> EtkiKriteriAdiVer(KullaniciDto kullanan, int seviye, string riskKategorisi)
        {
            //riskKategorisi==>FINANSAL, TEKNOLOJIK, ITIBAR, UYUM, STRATEJIK, OPERASYON
            //seviye==>1 ile 5 arasý

            string donenDeger = "";
            if (!string.IsNullOrWhiteSpace(riskKategorisi))
            {
                string[] riskKategorileri = riskKategorisi.ToUpper(new System.Globalization.CultureInfo("en-US")).Replace(" ", "").Split(',');

                foreach (var rk in riskKategorileri)
                {
                    if (rk == "FINANSAL")
                    {
                        Sonuc sonuc = await KayitGetirAsync(kullanan, (int)ENUMDurum.Onayli);
                        if (sonuc.IslemSonuc)
                        {
                            Konfigurasyon kayit = (Konfigurasyon)sonuc.Nesne;
                            string json = kayit.RiskKategoriFinansal;

                            dynamic[] obj = JsonConvert.DeserializeObject<dynamic[]>(json);
                            foreach (var item in obj)
                            {
                                if (item["puan"] == seviye)
                                {
                                    donenDeger += "<div>" + item["aciklama"] + "</div>";
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        string tur = "RISKKATEGORI" + rk + seviye;
                        var kayit = await _unitOfWorkGenel.KayitGetirAsync(k => k.Tur == tur);

                        if (kayit != null)
                            donenDeger += "<div>" + kayit.Adi + "</div>";
                    }
                }
            }

            return donenDeger;
        }


        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere ait yapýsal risk seviyesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="etki"></param>
        /// <param name="olasilik"></param>
        /// <returns>
        /// Yapýsal risk seviyesini göndürür
        /// </returns>
        public async Task<object> YapisalRiskSeviyesiVer(KullaniciDto kullanan, int etki, int olasilik)
        {
            Sonuc sonuc = await KayitGetirAsync(kullanan, (int)ENUMDurum.Onayli);
            if (sonuc.IslemSonuc)
            {
                Konfigurasyon kayit = (Konfigurasyon)sonuc.Nesne;
                string json = kayit.EtkiOlasilikMatrisi;

                dynamic[] obj = JsonConvert.DeserializeObject<dynamic[]>(json);
                foreach (var item in obj)
                {
                    if (Arac.ConvertToInt(item["etki"]) == etki &&
                        Arac.ConvertToInt(item["olasilik"]) == olasilik)
                    {
                        string seviyeAdi = _sharedResource["Konfigurasyon.YapisalRiskSeviyeAdi." + item["seviye"]];

                        object donenDeger = new { renk = item["renk"].Value, seviyeNo = item["seviye"].Value, seviyeAdi = seviyeAdi };

                        return donenDeger;
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// Etki olasýlýk matrisine ait tüm bilgileri döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// object nesnesi döndürür
        /// </returns>
        public async Task<object> EtkiOlasilikMatrisiVer(KullaniciDto kullanan)
        {
            List<object> donenDeger = new List<object>();

            Sonuc sonuc = await KayitGetirAsync(kullanan, (int)ENUMDurum.Onayli);
            if (sonuc.IslemSonuc)
            {
                Konfigurasyon kayit = (Konfigurasyon)sonuc.Nesne;
                string json = kayit.EtkiOlasilikMatrisi;

                dynamic[] obj = JsonConvert.DeserializeObject<dynamic[]>(json);
                foreach (var item in obj)
                {
                    string seviyeAdi = _sharedResource["Konfigurasyon.YapisalRiskSeviyeAdi." + item["seviye"]];

                    donenDeger.Add(new { etki = item["etki"].Value, olasilik = item["olasilik"].Value, renk = item["renk"].Value, seviyeNo = item["seviye"].Value, seviyeAdi = seviyeAdi });
                }
            }
            return donenDeger;
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilere ait artýk risk seviyesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="artikRiskPuani"></param>
        /// <returns>
        /// object nesnesi döndürür
        /// </returns>
        public async Task<object> ArtikRiskSeviyesiVer(KullaniciDto kullanan, double artikRiskPuani)
        {
            if (20 <= artikRiskPuani && 25 >= artikRiskPuani)
                return new { renk = "rgb(192, 0, 2)", seviyeNo = 5, seviyeAdi = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.5"].Value };
            else if (15 <= artikRiskPuani && 20 > artikRiskPuani)
                return new { renk = "rgb(253, 0, 3)", seviyeNo = 4, seviyeAdi = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.4"].Value };
            else if (10 <= artikRiskPuani && 15 > artikRiskPuani)
                return new { renk = "rgb(255, 192, 1)", seviyeNo = 3, seviyeAdi = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.3"].Value };
            else if (5 <= artikRiskPuani && 10 > artikRiskPuani)
                return new { renk = "rgb(0, 178, 78)", seviyeNo = 2, seviyeAdi = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.2"].Value };
            else if (0.2 <= artikRiskPuani && 5 > artikRiskPuani)
                return new { renk = "rgb(147, 209, 76)", seviyeNo = 1, seviyeAdi = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.1"].Value };

            return new { renk = "", seviyeNo = 0, seviyeAdi = "" };
        }

        /// <summary>
        /// Istemciden parametere ile talep edilen bilgiye göre yetki bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="tur"></param>
        /// <returns>
        /// string türünde yetki bilgisi döndürür
        /// </returns>
        private string YetkisiVarmi(KullaniciDto kullanan, string tur)
        {
            bool yetki = false;

            if (tur == "KAYDET")
                yetki = Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan);
            else if (tur == "ONAYAGONDERME")
                yetki = Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan);
            else if (tur == "ONAY")
                yetki = Arac.YetkisiVarmi("YETKILIRISKGOREVLISI", kullanan);
            else
                yetki = Arac.YetkisiVarmi("RISKSEKRETARYASI,YETKILIRISKGOREVLISI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }
    }
}