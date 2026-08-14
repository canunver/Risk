using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;
using System.IO;
using System;
using System.Web;
using MimeKit;
using System.Linq.Expressions;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// Dosya işlemlerinin yapıldığı servis
    /// </summary>
    public class DosyaService : IDosyaService
    {
        /// <summary>
        /// IUnitOfWork<Dosya> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<Dosya> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.DosyaService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public DosyaService(IUnitOfWork<Dosya> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string kod, string baglantiKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod) && string.IsNullOrWhiteSpace(baglantiKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                Expression<Func<Dosya, bool>> predicate = null;

                if (!string.IsNullOrWhiteSpace(kod))
                    predicate = k => k.Kod == kod;
                if (!string.IsNullOrWhiteSpace(baglantiKod))
                    predicate = k => k.BaglantiKod == baglantiKod;

                var kayit = await _unitOfWork.KayitGetirAsync(predicate);

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
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string baglantiKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.Durum == (int)ENUMDurum.Aktif && k.BaglantiKod == baglantiKod, o => o.Adi, a => a.KayitEden);
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
                Dosya aramaObj = Arac.DataTablesAramaNesne<Dosya>(new Dosya(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi == aramaObj.Adi);
                if (!string.IsNullOrWhiteSpace(aramaObj.KayitEdenKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitEden == aramaObj.KayitEden);
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue)
                                                                       || a.KayitEden.Adi.Contains(dataTablesParam.searchValue));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Dosya gelenNesne)
        {
            Dosya islemYapilan = new Dosya();

            if (string.IsNullOrWhiteSpace(gelenNesne.IcerikBase64) && gelenNesne.Icerik == null)
                return new Sonuc(ENUMIslemDurum.Uyari, "Dosya icerik bilgisi boş");

            try
            {
                int boyut = 0;
                if (gelenNesne.Icerik != null)
                    boyut = gelenNesne.Icerik.Length;
                else
                    boyut = gelenNesne.IcerikBase64.Length;

                gelenNesne.Kod = Arac.GetGuid();
                gelenNesne.Durum = (int)ENUMDurum.Aktif;
                gelenNesne.KayitTarihi = System.DateTime.Now;
                gelenNesne.Boyut = boyut;
                gelenNesne.KayitEdenKod = kullanan.PersonelKod;

                islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                await _unitOfWork.KaydetAsync();

                DosyayaKaydet(kullanan, gelenNesne);

                gelenNesne.Icerik = null;
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], gelenNesne);
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

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                await _unitOfWork.SilAsync(d => d.Kod == kod);
                await _unitOfWork.KaydetAsync();

                DosyayiSil(kullanan, kod);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bağlantı kaydını silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilBaglantiKodAsync(KullaniciDto kullanan, string baglantiKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(baglantiKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                var kayitlar = await _unitOfWork.ListeleAsync(k => k.BaglantiKod == baglantiKod, null);
                await _unitOfWork.KaydetAsync();
                if (kayitlar.Count > 0)
                {
                    foreach (Dosya d in kayitlar)
                    {
                        await SilAsync(kullanan, d.Kod);
                    }
                }
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bağlatı kod bilgisini güncelleyen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="eskiKod"></param>
        /// <param name="yeniKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> BaglantiKodGuncelleAsync(KullaniciDto kullanan, string eskiKod, string yeniKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(eskiKod))
                hata += "<li>" + _sharedResource["Kontrol.EskiKodBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(yeniKod))
                hata += "<li>" + _sharedResource["Kontrol.YeniKodBos"] + "</li>";
            if (!string.IsNullOrWhiteSpace(eskiKod) && !string.IsNullOrWhiteSpace(yeniKod) && eskiKod == yeniKod)
                hata += "<li>" + _sharedResource["Kontrol.EsitOlamaz"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);


            eskiKod = Arac.TirnakYoket(eskiKod);
            yeniKod = Arac.TirnakYoket(yeniKod);

            string sql = @"UPDATE Dosya SET BaglantiKod = '{yeniKod}' WHERE BaglantiKod = '{eskiKod}'";

            sql = sql.Replace("{eskiKod}", eskiKod).Replace("{yeniKod}", yeniKod);

            try
            {
                var sonucAdet = await _unitOfWork.SQLNonQueryCalistirAsync(sql);
                if (sonucAdet > 0)
                    return new Sonuc(ENUMIslemDurum.Basarili, sonucAdet + " " + _sharedResource["Bildirim.KayitBasarili"]);
                else
                    return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
            }
            catch (Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li><li>" + ex.InnerException + "</li></small>");
            }
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen dosya bilgisini kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        private Sonuc DosyayaKaydet(KullaniciDto kullanan, Dosya gelenNesne)
        {
            string yol = Arac.ConfigOku("DosyaKayitYolu", "");

            if (string.IsNullOrWhiteSpace(yol))
                return new Sonuc(ENUMIslemDurum.Basarili, "");

            try
            {
                //Yolu oluştur
                yol = Path.Combine(yol, "Dosya");

                if (!Directory.Exists(yol))
                    Directory.CreateDirectory(yol);

                //Yola dosya adını ekle
                yol = Path.Combine(yol, gelenNesne.Kod);

                try
                {
                    if (File.Exists(yol))
                        File.Delete(yol);
                }
                catch { }

                if (!string.IsNullOrWhiteSpace(gelenNesne.IcerikBase64))
                {
                    string ayirac = ";base64,";
                    int dyer = gelenNesne.IcerikBase64.IndexOf(ayirac);
                    gelenNesne.IcerikBase64 = gelenNesne.IcerikBase64.Substring(dyer + ayirac.Length);
                    gelenNesne.Icerik = System.Convert.FromBase64String(gelenNesne.IcerikBase64);
                }

                File.WriteAllBytes(yol, gelenNesne.Icerik);
            }
            catch (Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }

            return new Sonuc(ENUMIslemDurum.Basarili, "");

        }

        /// <summary>
        /// Istemciden parametere ile gönderilen dosya kaydını silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        private Sonuc DosyayiSil(KullaniciDto kullanan, string kod)
        {
            string yol = Arac.ConfigOku("DosyaKayitYolu", "");

            if (string.IsNullOrWhiteSpace(yol))
                return new Sonuc(ENUMIslemDurum.Basarili, "");

            try
            {
                //Yolu oluştur
                yol = Path.Combine(yol, "Dosya");

                //Yola dosya adını ekle
                yol = Path.Combine(yol, kod);

                try
                {
                    if (File.Exists(yol))
                        File.Delete(yol);
                }
                catch { }
            }
            catch (Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }

            return new Sonuc(ENUMIslemDurum.Basarili, "");
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen dosya bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        private async Task<Dosya> DosyayiOkuAsync(KullaniciDto kullanan, string kod)
        {
            Dosya dosya = new Dosya();

            string yol = Arac.ConfigOku("DosyaKayitYolu", "");

            if (string.IsNullOrWhiteSpace(yol))
                return dosya;

            try
            {
                //Yolu oluştur
                yol = Path.Combine(yol, "Dosya");

                //Yola dosya adını ekle
                yol = Path.Combine(yol, kod);

                try
                {
                    if (File.Exists(yol))
                    {
                        dosya.MimeType = Arac.GetMimeType(yol);
                        dosya.Icerik = await File.ReadAllBytesAsync(yol);
                    }
                }
                catch { }
            }
            catch (Exception e)
            {
                return dosya;
            }

            return dosya;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen dosya bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Dosya> IndirAsync(KullaniciDto kullanan, string kod, string baglantiKod)
        {
            Dosya gidecekDosya = new Dosya();

            Expression<Func<Dosya, bool>> predicate = null;

            if (!string.IsNullOrWhiteSpace(kod))
                predicate = k => k.Kod == kod;
            if (!string.IsNullOrWhiteSpace(baglantiKod))
                predicate = k => k.BaglantiKod == baglantiKod;

            var kayitlar = await _unitOfWork.ListeleAsync(predicate, null);
            if (kayitlar.Count > 0)
            {
                foreach (Dosya d in kayitlar)
                {
                    gidecekDosya = await DosyayiOkuAsync(kullanan, d.Kod);
                    gidecekDosya.Kod = d.Kod;
                    gidecekDosya.Adi = d.Adi;
                    break;
                }
            }
            return gidecekDosya;
        }

        /// <summary>
        /// Istemciden parametere ile kayda ait dosya sayısını döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="baglantiKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SayiVerAsync(KullaniciDto kullanan, string baglantiKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(baglantiKod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            var adet = await _unitOfWork.KayitSayisiAsync(k => k.BaglantiKod == baglantiKod);
            return new Sonuc(ENUMIslemDurum.Basarili, adet);
        }

        /// <summary>
        /// Istemciden parametere ile kayda ait açıklamayı güncelleyen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> AciklamaKaydetAsync(KullaniciDto kullanan, Dosya gelenNesne)
        {
            Dosya islemYapilan = new Dosya();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                eskiKayit.Aciklama = gelenNesne.Aciklama.Replace("\n", " ").Replace("'", " ");
                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
        }
    }
}
