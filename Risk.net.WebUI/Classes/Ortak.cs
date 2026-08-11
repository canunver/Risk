using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Risk.net.Services;
using Risk.net.Services.Interfaces;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Risk.net.WebUI.Classes
{
    /// <summary>
    /// WebUI projesinde ortak kullanýlan metodlarýn bulunduðu sýnýf
    /// </summary>
    public class Ortak
    {
        /// <summary>
        /// Yazýlýmda kullanýlan Durum bilgisinin ekranlarda kullanýlan seçim kutularýna aktarýlmasýný saðlayan metod
        /// </summary>
        /// <param name="_sharedResource"></param>
        /// <param name="tur"></param>
        /// <returns>
        /// string
        /// </returns>
        public static string DurumListesiVer(IStringLocalizer<CustomResource> _sharedResource, EnumTarihceIslemTur tur, KullaniciDto kullanan)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            if (tur == EnumTarihceIslemTur.RiskEvreni || tur == EnumTarihceIslemTur.RiskYonetimi || tur == EnumTarihceIslemTur.RiskAzaltmaPlani || tur == EnumTarihceIslemTur.OlayRaporlama)
            {
                donenDeger.Add(new SelectListesi { id = (int)ENUMDurum.Aktif + "", text = _sharedResource["Durum.Aktif"] });
                donenDeger.Add(new SelectListesi { id = (int)ENUMDurum.GeriGonderildi + "", text = _sharedResource["Durum.GeriGonderildi"] });
                donenDeger.Add(new SelectListesi { id = (int)ENUMDurum.OnayaGonderdi + "", text = _sharedResource["Durum.OnayaGonderildi"] });
                donenDeger.Add(new SelectListesi { id = (int)ENUMDurum.Onayli + "", text = _sharedResource["Durum.Onayli"] });
                donenDeger.Add(new SelectListesi { id = (int)ENUMDurum.Reddedildi + "", text = _sharedResource["Durum.Reddedildi"] });
                donenDeger.Add(new SelectListesi { id = (int)ENUMDurum.Pasif + "", text = _sharedResource["Durum.Pasif"] });

                if (kullanan.AktifRolKod == "RISKSEKRETARYASI")
                    donenDeger.Add(new SelectListesi { id = (int)ENUMDurum.Sil + "", text = _sharedResource["Durum.Sil"] });
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(donenDeger);
        }

        /// <summary>
        /// Artýk Risk Seviyesini gelen parametreler ile hesaplayýp ismini ve seviye numarasýný döndüren metor
        /// </summary>
        /// <param name="etki"></param>
        /// <param name="olasilik"></param>
        /// <param name="kontrolKriteriAgirligi"></param>
        /// <param name="_sharedResource"></param>
        /// <param name="seviye"></param>
        /// <returns>
        /// string olarak adýný, referans ile gelen seviye deðiþkeninde ise seviyes bilgisini döndürür
        /// </returns>
        public static string ArtikRiskSeviyesiGetir(int etki, int olasilik, decimal kontrolKriteriAgirligi, IStringLocalizer<CustomResource> _sharedResource, ref int seviye)
        {
            double artikRiskPuani = Math.Round(etki * olasilik * (double)kontrolKriteriAgirligi, 2);

            if (20 <= artikRiskPuani && 25 >= artikRiskPuani)
            {
                seviye = 5;
                return _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.5"];
            }
            else if (15 <= artikRiskPuani && 20 > artikRiskPuani)
            {
                seviye = 4;
                return _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.4"];
            }
            else if (10 <= artikRiskPuani && 15 > artikRiskPuani)
            {
                seviye = 3;
                return _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.3"];
            }
            else if (5 <= artikRiskPuani && 10 > artikRiskPuani)
            {
                seviye = 2;
                return _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.2"];
            }
            else if (0.2 <= artikRiskPuani && 5 > artikRiskPuani)
            {
                seviye = 1;
                return _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.1"];
            }

            seviye = 0;
            return "";
        }

        /// <summary>
        /// Artýk Risk Seviyesi bilgilerini renkleri ile birlikte ekranlarda kullanýlan seçim kutularýna aktarýlmasýný saðlayan metod
        /// </summary>
        /// <param name="_sharedResource"></param>
        /// <returns>
        /// string 
        /// </returns>
        public static string ArtikRiskSeviyesiListesiVer(IStringLocalizer<CustomResource> _sharedResource)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            donenDeger.Add(new SelectListesi { id = "1", text = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.1"].Value, ekBilgi = "rgb(147, 209, 76)" });
            donenDeger.Add(new SelectListesi { id = "2", text = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.2"].Value, ekBilgi = "rgb(0, 178, 78)" });
            donenDeger.Add(new SelectListesi { id = "3", text = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.3"].Value, ekBilgi = "rgb(255, 192, 1)" });
            donenDeger.Add(new SelectListesi { id = "4", text = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.4"].Value, ekBilgi = "rgb(253, 0, 3)" });
            donenDeger.Add(new SelectListesi { id = "5", text = _sharedResource["Konfigurasyon.ArtikRiskSeviyeAdi.5"].Value, ekBilgi = "rgb(192, 0, 2)" });

            return Newtonsoft.Json.JsonConvert.SerializeObject(donenDeger);
        }


        /// <summary>
        /// Yazýlýmda kullanýlan RiskTuru bilgisinin ekranlarda kullanýlan seçim kutularýna aktarýlmasýný saðlayan metod
        /// </summary>
        /// <param name="_sharedResource"></param>
        /// <returns>
        /// string
        /// </returns>
        public static string RiskTuruListesiVer(IStringLocalizer<CustomResource> _sharedResource)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>();

            donenDeger.Add(new SelectListesi { id = (int)EnumRiskEvreniRiskTuru.Tehdit + "", text = _sharedResource["RiskEvreni.RiskTuru.Tehdit"] });
            donenDeger.Add(new SelectListesi { id = (int)EnumRiskEvreniRiskTuru.Firsat + "", text = _sharedResource["RiskEvreni.RiskTuru.Firsat"] });

            return Newtonsoft.Json.JsonConvert.SerializeObject(donenDeger);
        }


        /// <summary>
        /// Sayfada gösterilecek olan Risklere Verilecek Cevap listesinin doldurulmasý için
        /// </summary>
        /// <returns>
        /// List<SelectListesi>
        /// </returns>
        /// <remarks></remarks>
        public static List<SelectListesi> RiskeVerilecekCevapListesi(IStringLocalizer<CustomResource> _sharedResource)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>
            {
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.TransferEt + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.TransferEt"] },
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.KabulEt + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.KabulEt"] },
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.Reddet + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.Reddet"] },
                new SelectListesi { id = (int)EnumRiskYonetimiRiskeVerilecekCevap.Azalt + "", text = _sharedResource["RiskYonetimi.RiskeVerilecekCevap.Azalt"] }
            };

            return donenDeger;
        }


        /// <summary>
        /// Sayfada gösterilecek olan RiskAzaltmaPlani MevcutDurum listesinin doldurulmasý için
        /// </summary>
        /// <returns>
        /// List<SelectListesi>
        /// </returns>
        /// <remarks></remarks>
        public static List<SelectListesi> RiskAzaltmaPlaniMevcutDurumListesi(IStringLocalizer<CustomResource> _sharedResource)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>
            {
                new SelectListesi { id = (int)EnumRiskAzaltmaPlaniMevcutDurum.DevamEdiyor + "", text = _sharedResource["RiskAzaltmaPlani.MevcutDurum.DevamEdiyor"] },
                new SelectListesi { id = (int)EnumRiskAzaltmaPlaniMevcutDurum.Ertelendi + "", text = _sharedResource["RiskAzaltmaPlani.MevcutDurum.Ertelendi"] },
                new SelectListesi { id = (int)EnumRiskAzaltmaPlaniMevcutDurum.Durduruldu + "", text = _sharedResource["RiskAzaltmaPlani.MevcutDurum.Durduruldu"] },
                new SelectListesi { id = (int)EnumRiskAzaltmaPlaniMevcutDurum.IptalEdildi + "", text = _sharedResource["RiskAzaltmaPlani.MevcutDurum.IptalEdildi"] },
                new SelectListesi { id = (int)EnumRiskAzaltmaPlaniMevcutDurum.Tamamlandi + "", text = _sharedResource["RiskAzaltmaPlani.MevcutDurum.Tamamlandi"] }
            };

            return donenDeger;
        }


        /// <summary>
        /// AnahtarRiskGöstergesi Dönem Periyot bilgilerini ekranlarda kullanýlan seçim kutularýna aktarýlmasýný saðlayan metod
        /// </summary>
        /// <param name="_sharedResource"></param>
        /// <returns>
        /// string 
        /// </returns>
        public static string AnahtarRiskGostergesiDonemPeriyotListesiVer(IStringLocalizer<CustomResource> _sharedResource)
        {
            List<SelectListesi> donenDeger = new List<SelectListesi>
            {
                new SelectListesi { id = (int)EnumAnahtarRiskGostergesiDonemPeriyot.Aylik + "", text = _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Aylik"] },
                new SelectListesi { id = (int)EnumAnahtarRiskGostergesiDonemPeriyot.Aylik3 + "", text = _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Aylik3"] },
                new SelectListesi { id = (int)EnumAnahtarRiskGostergesiDonemPeriyot.Aylik6 + "", text = _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Aylik6"] },
                new SelectListesi { id = (int)EnumAnahtarRiskGostergesiDonemPeriyot.Yillik + "", text = _sharedResource["AnahtarRiskGostergesiDonemPeriyot.Yillik"] },
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(donenDeger);
        }

    }

}
