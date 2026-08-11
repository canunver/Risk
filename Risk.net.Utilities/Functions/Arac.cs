using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json.Linq;
using Risk.net.Utilities.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Risk.net.Utilities.Functions
{
    public static class Arac
    {
        public static string GetGuid()
        {
            return DateTime.Now.ToString("yyMMddHHmmssffff") + Guid.NewGuid().ToString("N").Substring(16);
            //return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string ConfigOku(string anahtar, string hataDeger = "")
        {
            string deger;

            try
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                deger = config[anahtar] + "";
            }
            catch
            {
                deger = "";
            }

            if (string.IsNullOrWhiteSpace(deger))
                deger = hataDeger;

            return deger;
        }

        public static IConfigurationSection ConfigSectionOku(string anahtar)
        {
            IConfigurationSection deger = null;

            try
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                deger = config.GetSection(anahtar);
            }
            catch { }

            return deger;
        }

        public static T DataTablesAramaNesne<T>(T entityObj, string aramaBilgi)
        {
            aramaBilgi = aramaBilgi.Replace("GELISMIS_ARAMA:", "");

            Type tModelType = entityObj.GetType();
            PropertyInfo[] arrayPropertyInfos = tModelType.GetProperties();

            string[] satirlar = aramaBilgi.Split("|^|");
            foreach (string satir in satirlar)
            {
                if (string.IsNullOrEmpty(satir)) continue;

                string[] degerler = satir.Split("|~|");
                if (degerler.Length >= 2)
                {
                    string alanAdi = degerler[0];
                    string alanDeger = degerler[1];

                    if (string.IsNullOrWhiteSpace(alanDeger)) continue;

                    foreach (PropertyInfo property in arrayPropertyInfos)
                    {
                        if (property.Name == alanAdi)
                        {
                            if (property.PropertyType.Name == "Int32")
                                property.SetValue(entityObj, ConvertToInt(alanDeger));
                            else if (property.PropertyType.FullName.Contains("DateTime"))
                                property.SetValue(entityObj, ConvertToDateTime(alanDeger));
                            else
                                property.SetValue(entityObj, alanDeger);
                        }
                    }
                }
            }

            return entityObj;
        }

        public static object DataTablesJsonData(IQueryable<object> selectData, DataTablesParam dataTablesParam)
        {
            if (!string.IsNullOrEmpty(dataTablesParam.sortColumn))
            {
                selectData = selectData.OrderBy(dataTablesParam.sortColumn + " " + dataTablesParam.sortColumnDirection);
            }

            try
            {
                int recordsTotal = selectData.Count();

                var data = selectData.Skip(dataTablesParam.skip).Take(dataTablesParam.pageSize).ToList();

                var jsonData = new
                {
                    draw = dataTablesParam.draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = data
                };

                return jsonData;
            }
            catch (Exception e)
            {

                string hata = e.Message;
            }

            return null;
        }

        public static KullaniciDto KullaniciNesnesiOlustur(System.Security.Principal.IPrincipal user)
        {
            KullaniciDto kullanan = new KullaniciDto();

            var identity = ((ClaimsIdentity)user.Identity);
            var claims = identity.Claims;

            foreach (var item in claims)
            {
                if (item.Type == "Adi")
                    kullanan.Adi = item.Value;
                if (item.Type == "Soyadi")
                    kullanan.Soyadi = item.Value;
                if (item.Type == "ResimUrl")
                    kullanan.ResimUrl = item.Value;
                if (item.Type == "Unvan")
                    kullanan.Unvan = item.Value;
                if (item.Type == "EPosta")
                    kullanan.EPosta = item.Value;
                if (item.Type == "PersonelKod")
                    kullanan.PersonelKod = item.Value;
                if (item.Type == "KullaniciKod")
                    kullanan.KullaniciKod = item.Value;
                if (item.Type == "KoordinatorlukKod")
                    kullanan.KoordinatorlukKod = item.Value;
                if (item.Type == "BirimKod")
                    kullanan.BirimKod = item.Value;
                if (item.Type == "AktifRolKod")
                    kullanan.AktifRolKod = item.Value;

                if (item.Type == "Yetki")
                {
                    string[] yetkiStr = item.Value.Split('~');
                    KullaniciRolDto rol = new KullaniciRolDto();
                    rol.Adi = yetkiStr[0];
                    rol.KoordinatorlukKod = yetkiStr[1];
                    rol.BirimKod = yetkiStr[2];

                    kullanan.KoordinatorlukKod = rol.KoordinatorlukKod;
                    kullanan.BirimKod = rol.BirimKod;

                    kullanan.Roller.Add(rol);
                    Arac.HataStrYaz("Yetki alaný:" + rol.KoordinatorlukKod);
                }
            }

            Arac.HataStrYaz("kullanan.KoordinatorlukKod:" + kullanan.KoordinatorlukKod + " , Rolsayýsý:" + kullanan.Roller.Count);

            return kullanan;
        }

        public static List<string> KullaniciYetkiGetir(System.Security.Principal.IPrincipal user)
        {
            var identity = ((ClaimsIdentity)user.Identity);
            var claims = identity.Claims;

            string roller = "";
            foreach (var item in claims)
            {
                if (item.Type == "Yetki")
                {
                    string[] yetkiStr = item.Value.Split('~');

                    //ENUMKullaniciRol yet = Enum.Parse<ENUMKullaniciRol>(yetkiStr[0]);

                    if (roller != "") roller += "|";
                    //roller += (int)yet;
                    roller += yetkiStr[0];
                }
            }
            if (string.IsNullOrWhiteSpace(roller))
                roller = "YETKISIYOK";

            return new List<string>(roller.Split('|'));
        }

        public static bool YetkiKontrolu(List<string> sahipOlunan, string[] kontrolEdilen)
        {
            if (kontrolEdilen == null) return false;

            bool yetkili = false;

            foreach (string ir in kontrolEdilen)
            {
                foreach (string gr in sahipOlunan)
                {
                    if (gr == "PLANLAMAUNITESI") return true;//PLANLAMAUNITESI her yetkisi var
                    if (gr == "SISTEMYONETICISI") return true;//SISTEMYONETICISI her yetkisi var

                    if (gr == ir || ir == "*")
                    {
                        yetkili = true;
                        break;
                    }
                }

                if (yetkili) break;
            }

            return yetkili;
        }

        public static bool YetkisiVarmi(string kontrolRoller, System.Security.Principal.IPrincipal user)
        {
            var identity = ((ClaimsIdentity)user.Identity);
            string[] kontrolRol = kontrolRoller.Split(',');

            bool kontrol = false;
            foreach (string rol in kontrolRol)
            {
                kontrol = identity.HasClaim(ClaimTypes.Role, rol);
                if (kontrol) break;
            }

            return kontrol;
        }

        public static bool YetkisiVarmi(string kontrolRoller, KullaniciDto kullanan)
        {
            string[] kontrolRol = kontrolRoller.Split(',');

            foreach (KullaniciRolDto item in kullanan.Roller)
            {
                if (item.Adi == "PLANLAMAUNITESI") return true;//PLANLAMAUNITESI her yetkisi var
                if (item.Adi == "SISTEMYONETICISI") return true;//SISTEMYONETICISI her yetkisi var

                foreach (string rol in kontrolRol)
                {
                    if (string.IsNullOrWhiteSpace(rol)) continue;
                    if (rol == "*") return true;

                    if (rol == item.Adi)
                        return true;
                }
            }

            return false;
        }

        public static string YetkiAdiVer(dynamic sharedResource, int yetkiKodu)
        {
            return sharedResource["KullaniciRol." + yetkiKodu];
        }

        public static int ConvertToInt(object degisecek, int hataDeger = 0)
        {
            try
            {
                return Convert.ToInt32(degisecek);
            }
            catch (Exception)
            {
                return hataDeger;
            }
        }

        public static double ConvertToDouble(object s, double hataDeger = 0.0)
        {
            try
            {
                return Convert.ToDouble(s);
            }
            catch
            {
                return hataDeger;
            }
        }

        public static decimal ConvertToDecimal(object s, decimal hataDeger = 0)
        {
            try
            {
                return Convert.ToDecimal(s);
            }
            catch
            {
                return hataDeger;
            }
        }

        public static long ConvertToLong(object s, Int64 hataDeger = 0)
        {
            try
            {
                return Convert.ToInt64(s);
            }
            catch
            {
                return hataDeger;
            }
        }

        public static string ConvertToStr(object s, string hataDeger = "")
        {
            try
            {
                return Convert.ToString(s);
            }
            catch
            {
                return hataDeger;
            }
        }

        public static DateTime ConvertToDateTime(object degisecek, DateTime hataDeger = new DateTime())
        {
            try
            {
                return Convert.ToDateTime(degisecek);
            }
            catch (Exception)
            {
                return hataDeger;
            }
        }

        public static double HTDoubleEkle(Hashtable ht, string htKod, double ekleDeger)
        {
            object tut = ht[htKod];
            double tutD;
            if (tut != null)
                tutD = (double)tut;
            else
                tutD = 0;

            tutD += ekleDeger;
            ht[htKod] = tutD;

            return tutD;
        }

        public static bool EPostaDogrula(string mailAdres)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            bool deger = re.IsMatch(mailAdres);
            if (!deger)
                deger = mailAdres.EndsWith("local") || mailAdres.EndsWith("localhost");

            return deger;
        }

        public static bool TCKNDogrula(string kimlikNo)
        {
            //http://blog.rakkoc.com/2014/09/tc-kimlik-no-algoritmasi-ve-hesaplamasi/
            bool returnvalue = false;
            if (kimlikNo.Length == 11)
            {
                Int64 atcNo, btcNo, tcNo;
                long c1, c2, c3, c4, c5, c6, c7, c8, c9, q1, q2;

                tcNo = Int64.Parse(kimlikNo);

                // bolu yuz islemi int tanimlanmis degiskende son 2 haneyi silmek icin kullanýlýr.

                atcNo = tcNo / 100;
                btcNo = tcNo / 100;

                c1 = atcNo % 10; atcNo = atcNo / 10;
                c2 = atcNo % 10; atcNo = atcNo / 10;
                c3 = atcNo % 10; atcNo = atcNo / 10;
                c4 = atcNo % 10; atcNo = atcNo / 10;
                c5 = atcNo % 10; atcNo = atcNo / 10;
                c6 = atcNo % 10; atcNo = atcNo / 10;
                c7 = atcNo % 10; atcNo = atcNo / 10;
                c8 = atcNo % 10; atcNo = atcNo / 10;
                c9 = atcNo % 10; atcNo = atcNo / 10;
                q1 = ((10 - ((((c1 + c3 + c5 + c7 + c9) * 3) + (c2 + c4 + c6 + c8)) % 10)) % 10);
                q2 = ((10 - (((((c2 + c4 + c6 + c8) + q1) * 3) + (c1 + c3 + c5 + c7 + c9)) % 10)) % 10);

                /*
                q1 TC nosunun 10. hanesi
                q2 TC nosunun 11. hanesi
                btcNo son 2 hanesi olmayan tckimlikNo
                */

                returnvalue = ((btcNo * 100) + (q1 * 10) + q2 == tcNo);
            }
            return returnvalue;
        }

        public static void HataStrYaz(string hata)
        {
            string hataDosyaYol = ConfigOku("Genel:HataDosyaYol");
            string dosyaAdi = Path.Combine(hataDosyaYol, "Hata.txt");

            HataStrYaz(dosyaAdi, hata);
        }

        public static void HataStrYaz(string dosyaAdi, string hata)
        {
            string hataDosyaYol = ConfigOku("Genel:HataDosyaYol");
            dosyaAdi = Path.Combine(hataDosyaYol, dosyaAdi);

            LogDosyasinaYaz(dosyaAdi, hata);
        }

        private static void LogDosyasinaYaz(string dosyaAdi, string hata)
        {
            if (dosyaAdi != null && dosyaAdi != "")
            {
                System.Threading.Monitor.Enter(dosyaAdi);
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(dosyaAdi, true, Encoding.GetEncoding("windows-1254"));
                    sw.Write(DateTime.Now.ToString());
                    sw.Write(" >>> ");
                    sw.WriteLine(hata);
                    sw.WriteLine("------------------------------------------------------");
                    sw.Flush();
                    sw.Close();
                    sw = null;
                }
                catch (Exception)
                {
                    if (sw != null)
                        sw.Close();
                }
                System.Threading.Monitor.Exit(dosyaAdi);
            }
        }

        public static ITablo ExcelTablo()
        {
            return new TabloGB();
        }

        public static string GetMimeType(string dosyaAdi)
        {
            return MimeTypes.GetMimeType(dosyaAdi);
        }

        public static FileContentResult DosyaGonder(string dosyaAdi, string gorulenAdi, bool gondermedenOnceSil = false)
        {
            var mimeType = Arac.GetMimeType(dosyaAdi);

            if (!File.Exists(dosyaAdi))
                return null;

            byte[] fileBytes = File.ReadAllBytes(dosyaAdi);

            try
            {
                if (gondermedenOnceSil)
                {
                    File.Delete(dosyaAdi);
                }
            }
            catch { }

            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = gorulenAdi
            };
        }

        public static string JSONSerialize(object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        public static T JSONDeserialize<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public static JObject DegisenleriBul(this JToken _yeni, JToken _eski)
        {
            var degisen = new JObject();
            if (JToken.DeepEquals(_yeni, _eski)) return degisen;

            switch (_yeni.Type)
            {
                case JTokenType.Object:
                    {
                        var yeni = _yeni as JObject;
                        var eski = _eski as JObject;

                        if (yeni == null || eski == null) return degisen;

                        var addedKeys = yeni.Properties().Select(c => c.Name).Except(eski.Properties().Select(c => c.Name));
                        var removedKeys = eski.Properties().Select(c => c.Name).Except(yeni.Properties().Select(c => c.Name));
                        var unchangedKeys = yeni.Properties().Where(c => JToken.DeepEquals(c.Value, eski[c.Name])).Select(c => c.Name);
                        foreach (var k in addedKeys)
                        {
                            degisen[k] = new JObject
                            {
                                ["yeni"] = yeni[k]
                            };
                        }
                        foreach (var k in removedKeys)
                        {
                            degisen[k] = new JObject
                            {
                                ["eski"] = eski[k]
                            };
                        }
                        var potentiallyModifiedKeys = yeni.Properties().Select(c => c.Name).Except(addedKeys).Except(unchangedKeys);
                        foreach (var k in potentiallyModifiedKeys)
                        {
                            var degisenBul = Arac.DegisenleriBul(yeni[k], eski[k]);
                            if (degisenBul.HasValues) degisen[k] = degisenBul;
                        }
                    }
                    break;
                case JTokenType.Array:
                    {
                        var eklenen = new JArray(_yeni.Except(_eski, new JTokenEqualityComparer()));
                        var cikarilan = new JArray(_eski.Except(_yeni, new JTokenEqualityComparer()));
                        if (eklenen.HasValues) degisen["yeni"] = eklenen;
                        if (cikarilan.HasValues) degisen["eski"] = cikarilan;
                    }
                    break;
                default:
                    degisen["yeni"] = _yeni;
                    degisen["eski"] = _eski;
                    break;
            }

            return degisen;
        }

        public static string DosyaAdUret()
        {
            return DosyaAdUret("", "");
        }
        public static string DosyaAdUret(string dosyaAdOnEk, string dosyaAdArkaEk)
        {
            return System.IO.Path.GetTempPath() + dosyaAdOnEk + DosyaAdUretSade() + dosyaAdArkaEk;
        }
        public static string DosyaAdUretSade()
        {
            DateTime simdi = System.DateTime.Now;
            string dosyaAd = simdi.Year.ToString("0000") + simdi.Month.ToString("00") + simdi.Day.ToString("00") + simdi.Hour.ToString("00") + simdi.Minute.ToString("00") + simdi.Second.ToString("00") + System.DateTime.Now.Millisecond.ToString("0000");
            return dosyaAd;
        }

        public static string DateTimeToYYYYMMDD(DateTime? deger)
        {
            if (deger.HasValue)
                return string.Format("{0:yyyy-MM-dd HH:mm}", deger);
            else return "";
        }

        public static string DateTimeToDDMMYYYY(DateTime? deger)
        {
            if (deger.HasValue)
                return string.Format("{0:dd.MM.yyyy}", deger);
            else return "";
        }

        public static string DateTimeToDDMMYYYYHHmmss(DateTime? deger)
        {
            if (deger.HasValue)
                return string.Format("{0:dd.MM.yyyy HH:mm:ss}", deger);
            else return "";
        }

        public static string UstYetkiVer(string yetki, EnumKoordinatorlukTur tur)
        {
            /*
            BASKAN
            ILKOORDINATOR-BASKAN
            MERKEZKOORDINATOR-GENELKOORDINATOR     ---------Hatalý BASKAN
            GENELKOORDINATOR-BASKAN                ---------Hatalý MERKEZKOORDINATOR
            ICDENETIMKOORDINATOR-BASKAN            ---------Hatalý MERKEZKOORDINATOR
            ICDENETIMUZMANI-ICDENETIMKOORDINATOR
            BIRIMAMIRI-MERKEZ/IL/GENELKOORDINATOR/ICDENETIM/HUKUK
            PLANLAMAUNITESI-BIRIMAMIRI
            RISKSEKRETARYASI-BIRIMAMIRI
            YETKILIRISKGOREVLISI-BIRIMAMIRI
            */

            //Baþkan onayýna gönderilmesin

            //if (yetki == "BASKAN")
            //    return "BASKAN";
            //else if (yetki == "ILKOORDINATOR")
            //    return "BASKAN";
            //else if (yetki == "GENELKOORDINATOR")
            //    return "BASKAN";
            //else if (yetki == "ICDENETIMKOORDINATOR")
            //    return "BASKAN";
            //else if (yetki == "HUKUKKOORDINATORU")
            //    return "BASKAN";

            if (yetki == "BASKAN")
                return "BASKAN";
            else if (yetki == "ILKOORDINATOR")
                return "ILKOORDINATOR";
            else if (yetki == "GENELKOORDINATOR")
                return "GENELKOORDINATOR";
            else if (yetki == "ICDENETIMKOORDINATOR")
                return "ICDENETIMKOORDINATOR";
            else if (yetki == "HUKUKKOORDINATORU")
                return "HUKUKKOORDINATORU";

            else if (yetki == "MERKEZKOORDINATOR")
                return "GENELKOORDINATOR";
            else if (yetki == "ICDENETIMUZMANI")
                return "ICDENETIMKOORDINATOR";
            else if (yetki == "BIRIMAMIRI")
            {
                if (tur == EnumKoordinatorlukTur.Il)
                    return "ILKOORDINATOR";
                else if (tur == EnumKoordinatorlukTur.Hukuk)
                    return "HUKUKKOORDINATORU";
                else
                    return "MERKEZKOORDINATOR";
            }
            else if (yetki == "PLANLAMAUNITESI")
                return "BIRIMAMIRI";
            else if (yetki == "RISKSEKRETARYASI")
                return "BIRIMAMIRI";
            else if (yetki == "YETKILIRISKGOREVLISI")
                return "BIRIMAMIRI";
            else if (yetki == "UZMAN")
                return "BIRIMAMIRI";

            return "";
        }

        public static string KoordinatorlukYetkiVer(EnumKoordinatorlukTur tur)
        {
            string yetki = "";

            //Baþkan onayýna gönderilmesin

            if (tur == EnumKoordinatorlukTur.Baskan)
                yetki = "BASKAN*";
            if (tur == EnumKoordinatorlukTur.Genel)
                yetki = "GENELKOORDINATOR";
            else if (tur == EnumKoordinatorlukTur.IcDenetim)
                yetki = "ICDENETIMKOORDINATOR";
            else if (tur == EnumKoordinatorlukTur.Hukuk)
                yetki = "HUKUKKOORDINATORU";
            else if (tur == EnumKoordinatorlukTur.Merkez)
                yetki = "MERKEZKOORDINATOR";
            else if (tur == EnumKoordinatorlukTur.Il)
                yetki = "ILKOORDINATOR";

            return yetki;
        }


        public static string UstYetkiVer(KullaniciDto kullanan, EnumKoordinatorlukTur tur)
        {
            string yetki = "";
            foreach (KullaniciRolDto item in kullanan.Roller)
            {
                yetki = UstYetkiVer(item.Adi, tur);
                if (!string.IsNullOrWhiteSpace(yetki))
                    break;
            }

            return yetki;
        }

        public static string DurumDegisikligiUygunMu(KullaniciDto kullanan, dynamic _sharedResource, dynamic eskiKayit, dynamic gelenNesne)
        {
            string hata = "";

            //Sistem yöneticisi her durumda riski pasif yapabilsin.
            if (gelenNesne.Durum == (int)ENUMDurum.Pasif || gelenNesne.Durum == (int)ENUMDurum.Sil)
            {
                bool yonetici = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", kullanan);
                if (yonetici)
                    return hata;
            }

            if (eskiKayit.Durum == (int)ENUMDurum.Aktif)
            {
                if (!(gelenNesne.Durum == (int)ENUMDurum.Pasif || gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi))
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";
            }
            else if (eskiKayit.Durum == (int)ENUMDurum.Pasif)
            {
                if (gelenNesne.Durum == (int)ENUMDurum.Pasif)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.ZatenPasifYapilmis"] + "</li>";
                else
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";
            }
            else if (eskiKayit.Durum == (int)ENUMDurum.Reddedildi)
            {
                hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";
            }
            else if (eskiKayit.Durum == (int)ENUMDurum.GeriGonderildi)
            {
                if (gelenNesne.Durum != (int)ENUMDurum.Pasif)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";
            }
            else if (eskiKayit.Durum == (int)ENUMDurum.OnayaGonderdi)
            {
                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.ZatenOnayaGonderilmis"] + "</li>";
                else if (!(gelenNesne.Durum == (int)ENUMDurum.Onayli || gelenNesne.Durum == (int)ENUMDurum.GeriGonderildi || gelenNesne.Durum == (int)ENUMDurum.Reddedildi))
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.UygunDegil"] + "</li>";
            }
            else if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
            {
                if (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.Onaylanmis"] + "</li>";
            }

            //Her kullanýcý kendi giriþ yaptýðý kaydý onaya gönderebilmelidir.
            if (eskiKayit.GetType().GetProperty("RiskSahibiKod") != null && eskiKayit.RiskSahibiKod != null && eskiKayit.RiskSahibiKod != kullanan.PersonelKod && (gelenNesne.Durum == (int)ENUMDurum.OnayaGonderdi || gelenNesne.Durum == (int)ENUMDurum.Pasif))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.DuzenlemeYetkinizYok"] + "</li>";

            return hata;
        }

        public static string DurumAdGetir(int durum)
        {
            if (durum == (int)ENUMDurum.Aktif)
                return "Aktif";
            else if (durum == (int)ENUMDurum.GeriGonderildi)
                return "Geri Gönderildi";
            else if (durum == (int)ENUMDurum.OnayaGonderdi)
                return "Onaya Gönderildi";
            else if (durum == (int)ENUMDurum.Onayli)
                return "Onaylandý";
            else if (durum == (int)ENUMDurum.Reddedildi)
                return "Reddedildi";
            else if (durum == (int)ENUMDurum.Pasif)
                return "Pasif yapýldý";
            else if (durum == (int)ENUMDurum.BilgilendirmeMail)
                return "Bilgilendirme Maili Gönderildi";
            else if (durum == (int)ENUMDurum.RiskSahibiDegisti)
                return "Risk Sahibi Deðiþti";
            else if (durum == (int)ENUMDurum.AzaltmaPlaniSorumlusuDegisti)
                return "Azaltma Planý Sorumlusu Deðiþti";
            else if (durum == (int)ENUMDurum.Sil)
                return "Silindi";

            return "";
        }

        public static decimal DegerAl(decimal? deger)
        {
            return deger ?? 0;
        }

        public static int DegerAl(int? deger)
        {
            return deger ?? 0;
        }

        public static double DegerAl(double? deger)
        {
            return deger ?? 0;
        }

        public static string TirnakYoket(string deger)
        {
            deger = deger.Replace("'", "");
            deger = deger.Replace("\"", "");
            deger = deger.Replace("&#39;", "");
            deger = deger.Replace("&#34;", "");
            deger = deger.Replace("&quot;", "");
            deger = deger.Replace("&apos;", "");

            return deger;
        }

    }
}
