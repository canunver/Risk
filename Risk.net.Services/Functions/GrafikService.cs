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
using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// GrafikService işlemlerinin yapıldığı servis
    /// </summary>
    public class GrafikService : IGrafikService
    {
        /// <summary>
        /// IUnitOfWork<Grafik> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<Grafik> _unitOfWork;
        /// <summary>
        /// IUnitOfWork<GrafikIzlemeMatrisi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<GrafikIzlemeMatrisi> _unitOfWorkIzlemeMatris;
        /// <summary>
        /// IUnitOfWork<GrafikRiskYonetimi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<GrafikRiskYonetimi> _unitOfWorkRiskYonetimi;
        /// <summary>
        /// IUnitOfWork<RaporTrend> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporTrend> _unitOfWorkRaporTrend;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.GrafikService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="unitOfWorkIzlemeMatris"></param>
        /// <param name="unitOfWorkRiskYonetimi"></param>
        /// <param name="serviceCTE"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public GrafikService(IUnitOfWork<Grafik> unitOfWork,
            IUnitOfWork<GrafikIzlemeMatrisi> unitOfWorkIzlemeMatris,
            IUnitOfWork<GrafikRiskYonetimi> unitOfWorkRiskYonetimi,
            IUnitOfWork<RaporTrend> unitOfWorkRaporTrend,
            ICTEKoordinatorlukService serviceCTE,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkIzlemeMatris = unitOfWorkIzlemeMatris;
            _unitOfWorkRiskYonetimi = unitOfWorkRiskYonetimi;
            _unitOfWorkRaporTrend = unitOfWorkRaporTrend;
            _serviceCTE = serviceCTE;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// BulguDurumu için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> BulguDurumuHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("BulguYonetimiBirim.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("BulguYonetimiBirim.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.BitisTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.BitisTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.Durum", "<>", (int)ENUMDurum.Pasif));

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT Convert(varchar,BulguYonetimi.Durum) AS Aciklama, Count(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama ";
            sql += " FROM BulguYonetimi";
            sql += " WHERE BulguYonetimi.Kod IN ";
            sql += $" (SELECT BulguYonetimi.Kod FROM Denetim INNER JOIN BulguYonetimi ON Denetim.Kod = BulguYonetimi.DenetimKod INNER JOIN BulguYonetimiBirim ON BulguYonetimi.Kod = BulguYonetimiBirim.BulguYonetimiKod {kosul})";
            sql += " GROUP BY BulguYonetimi.Durum";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// BulguOnemDuzeyi için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> BulguOnemDuzeyiHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("BulguYonetimiBirim.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("BulguYonetimiBirim.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.BitisTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.BitisTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.Durum", "<>", (int)ENUMDurum.Pasif));

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT TanimGenel.Adi AS Aciklama, Count(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama  ";
            sql += " FROM BulguYonetimi,TanimGenel";
            sql += " WHERE BulguYonetimi.OnemDuzeyiKod=TanimGenel.Kod";
            sql += " AND BulguYonetimi.Kod IN ";
            sql += $" (SELECT BulguYonetimi.Kod FROM Denetim INNER JOIN BulguYonetimi ON Denetim.Kod = BulguYonetimi.DenetimKod INNER JOIN BulguYonetimiBirim ON BulguYonetimi.Kod = BulguYonetimiBirim.BulguYonetimiKod {kosul})";
            sql += " GROUP BY TanimGenel.Adi";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Surec için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SurecHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Surec.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Surec.BirimKod", "=", kriter.BirimKod));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Surec.Durum", "<>", (int)ENUMDurum.Pasif));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            //kosul += " AND RiskEvreni.Durum NOT IN(99, 999)";

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT Surec.Adi AS Aciklama, Count(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama ";
            sql += " FROM RiskEvreni INNER JOIN";
            sql += " Surec ON RiskEvreni.SurecKod = Surec.Kod";
            sql += kosul;
            sql += " GROUP BY Surec.Adi";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// YapisalRiskSeviyesi için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> YapisalRiskSeviyesiHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));

            kosul += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";   

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT ViewRiskSeviyeleri.Seviye AS Aciklama, Count(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama ";
            sql += " FROM RiskYonetimi ";
            sql += " LEFT JOIN ViewRiskSeviyeleri ON RiskYonetimi.Etki=ViewRiskSeviyeleri.Etki AND RiskYonetimi.Olasilik=ViewRiskSeviyeleri.Olasilik";
            sql += " INNER JOIN RiskEvreni ON RiskEvreni.Kod=RiskYonetimi.RiskEvreniKod";
            sql += " LEFT JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod";
            sql += kosul;
            sql += " GROUP BY ViewRiskSeviyeleri.Seviye";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// ArtıkRiskSeviyesi için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ArtikRiskSeviyesiHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));
            kosul += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT Convert(varchar,Convert(int,RiskYonetimi.ArtikRiskSeviyesi)) AS Aciklama, Count(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama ";
            sql += " FROM RiskYonetimi ";
            sql += " INNER JOIN RiskEvreni ON RiskEvreni.Kod=RiskYonetimi.RiskEvreniKod";
            sql += " LEFT JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod";
            sql += kosul;
            sql += " GROUP BY RiskYonetimi.ArtikRiskSeviyesi";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// RiskPuani için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskPuaniHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("ArtikRiskSeviyesi", ">=", 3));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT RiskEvreni.RiskNo AS Aciklama, Etki * Olasilik as Deger1, Convert(decimal(13,2),Etki * Olasilik * KontrolEtkinlikAgirligi) AS Deger2,'' as EkAciklama ";
            sql += " FROM RiskYonetimi ";
            sql += " INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod";
            sql += kosul;

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// RiskKategorileri için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskKategorileriHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));
            kosul += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT TanimRiskKategori.Adi AS Aciklama, COUNT(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama ";
            sql += " FROM RiskYonetimi ";
            sql += " INNER JOIN RiskEvreniRiskKategori ON RiskYonetimi.RiskEvreniKod = RiskEvreniRiskKategori.RiskEvreniKod ";
            sql += " INNER JOIN TanimRiskKategori ON RiskEvreniRiskKategori.RiskKategoriKod = TanimRiskKategori.Kod";
            sql += " INNER JOIN RiskEvreni ON RiskEvreni.Kod=RiskYonetimi.RiskEvreniKod";
            sql += " LEFT JOIN RiskAzaltmaPlani ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod";
            sql += kosul;
            sql += " GROUP BY TanimRiskKategori.Adi";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// RiskIzlemeMatrisi için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        //public async Task<Sonuc> RiskIzlemeMatrisiHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        //{
        //    string sql = "";
        //    string kosul = "";

        //    if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
        //        kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

        //    if (!string.IsNullOrEmpty(kriter.BirimKod))
        //        kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

        //    if (kriter.sorguTarihi1.HasValue)
        //        kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

        //    if (kriter.sorguTarihi2.HasValue)
        //        kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

        //    kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
        //    kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));

        //    if (kosul != "")
        //        kosul = " WHERE " + kosul;

        //    sql = "SELECT RiskEvreni.RiskNo, RiskYonetimi.Etki, RiskYonetimi.Olasilik, RiskYonetimi.KontrolEtkinlikAgirligi AS KontrolKriteriAgirligi, convert(int,ViewRiskSeviyeleri.Seviye) AS YapisalRiskSeviyesi ";
        //    sql += " FROM RiskYonetimi ";
        //    sql += " INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod ";
        //    sql += " INNER JOIN ViewRiskSeviyeleri ON RiskYonetimi.Etki = ViewRiskSeviyeleri.Etki AND RiskYonetimi.Olasilik = ViewRiskSeviyeleri.Olasilik";
        //    sql += kosul;

        //    var kayitlar = await _unitOfWorkIzlemeMatris.SQLCalistirAsync(sql);
        //    if (kayitlar.Count > -1)
        //    {
        //        return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
        //    }
        //    return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        //}

        //public async Task<Sonuc> EylemDurumuHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        //{
        //    string sql = "";
        //    string kosul = Kosul(kullanan);
        //    if (kosul != "")
        //    {
        //        kosul = kosul.Replace("|KOORDINATORLUK|", "KoordinatorlukKod");
        //        kosul = kosul.Replace("|BIRIM|", "BirimKod");
        //        kosul = " WHERE " + kosul;
        //    }

        //    sql = "SELECT TanimGenel.Adi AS Aciklama, Count(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama  ";
        //    sql += " FROM BulguYonetimi,TanimGenel";
        //    sql += " WHERE BulguYonetimi.EylemDurumKod=TanimGenel.Kod";
        //    sql += " AND BulguYonetimi.Kod IN ";
        //    sql += $" (SELECT Kod FROM BulguYonetimi {kosul})";
        //    sql += " GROUP BY TanimGenel.Adi";
        //    sql += " ORDER BY Deger1 DESC";

        //    var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
        //    if (kayitlar.Count > -1)
        //    {
        //        return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
        //    }
        //    return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        //}

        public async Task<Sonuc> RiskYonetimiHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT TanimRiskKategori.Adi AS Aciklama, COUNT(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama ";
            sql += " FROM RiskYonetimi ";
            sql += " INNER JOIN RiskEvreniRiskKategori ON RiskYonetimi.RiskEvreniKod = RiskEvreniRiskKategori.RiskEvreniKod ";
            sql += " INNER JOIN TanimRiskKategori ON RiskEvreniRiskKategori.RiskKategoriKod = TanimRiskKategori.Kod";
            sql += " INNER JOIN RiskEvreni ON RiskEvreni.Kod=RiskYonetimi.RiskEvreniKod";
            sql += kosul;
            sql += " GROUP BY TanimRiskKategori.Adi";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// RiskYonetimi için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskYonetimiHazirlaAsync(KullaniciDto kullanan, GrafikRiskYonetimi kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));
            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));
            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));
            if (kriter.YapisalRiskSeviyesi > 0)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("ViewRiskSeviyeleri.Seviye", "=", kriter.YapisalRiskSeviyesi));
            if (kriter.ArtikRiskSeviyesi > 0)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.ArtikRiskSeviyesi", "=", kriter.ArtikRiskSeviyesi));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));
            kosul += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = @"SELECT ViewKoordinatorluk.Adi as KoordinatorlukAdi,
                    ViewBirim.Adi as BirimAdi,
                    RiskEvreni.RiskNo,
                    RiskEvreni.RiskAdi
                    FROM RiskYonetimi
                    INNER JOIN RiskEvreni ON RiskEvreni.Kod=RiskYonetimi.RiskEvreniKod
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod
                    LEFT JOIN ViewBirim ON ViewKoordinatorluk.Kod = ViewBirim.KoordinatorlukKod AND RiskEvreni.BirimKod = ViewBirim.Kod
                    LEFT JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod
                    ";
            if (kriter.YapisalRiskSeviyesi > 0)
                sql += " INNER JOIN ViewRiskSeviyeleri ON RiskYonetimi.Etki=ViewRiskSeviyeleri.Etki AND RiskYonetimi.Olasilik=ViewRiskSeviyeleri.Olasilik ";

            sql += kosul;

            sql += " ORDER BY RiskEvreni.RiskNo";

            var kayitlar = await _unitOfWorkRiskYonetimi.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// BulguOnemDuzeyi için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> AnahtarRiskGostergesiHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("BirimKod", "=", kriter.BirimKod));

            //if (kriter.sorguTarihi1.HasValue)
            //    kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.BitisTarihi", ">=", kriter.sorguTarihi1.Value));

            //if (kriter.sorguTarihi2.HasValue)
            //    kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.BitisTarihi", "<=", kriter.sorguTarihi2.Value));

            //kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("Denetim.Durum", "<>", (int)ENUMDurum.Pasif));

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "select Adi AS Aciklama, count(*) AS Deger1,0.0 As Deger2, Durum As EkAciklama";
            sql += " from(";
            sql += " select trk.Adi, r1.KoordinatorlukKod, r1.BirimKod,";
            sql += " case ";
            sql += " when k1.GerceklesenDeger >= a1.KirmiziDeger then 'KIRMIZI'";
            sql += " when k1.GerceklesenDeger <= a1.YesilDeger then 'YESIL'";
            sql += " else 'SARI'";
            sql += " END Durum FROM";
            sql += " RiskEvreni r1";
            sql += " INNER JOIN RiskEvreniRiskKategori rk ON r1.Kod = rk.RiskEvreniKod";
            sql += " INNER JOIN TanimRiskKategori trk ON rk.RiskKategoriKod = trk.Kod";
            sql += " INNER JOIN AnahtarRiskGostergesi a1 ON r1.Kod = a1.RiskEvreniKod";
            sql += " INNER JOIN AnahtarRiskGostergesiDonem k1 ON a1.Kod = k1.AnahtarRiskGostergesiKod";
            sql += " INNER JOIN(";
            sql += " SELECT AnahtarRiskGostergesiKod, MAX(Donem) AS MAX_Donem";
            sql += " FROM AnahtarRiskGostergesiDonem";
            sql += " GROUP BY AnahtarRiskGostergesiKod";
            sql += " ) k2 ON k1.AnahtarRiskGostergesiKod = k2.AnahtarRiskGostergesiKod AND k1.Donem = k2.MAX_DONEM";
            sql += " ) Sorgu";
            sql += kosul;
            sql += " group by Adi, Durum";
            sql += " order by Adi, Durum";


            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// BulguDurumu için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskeVerilenYanitHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));
            kosul += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = "SELECT Convert(varchar,Convert(int,RiskYonetimi.RiskeVerilecekCevap)) AS Aciklama, Count(*) AS Deger1, 0.0 AS Deger2,'' as EkAciklama ";
            sql += " FROM RiskYonetimi ";
            sql += " INNER JOIN RiskEvreni ON RiskEvreni.Kod=RiskYonetimi.RiskEvreniKod";
            sql += " LEFT JOIN RiskAzaltmaPlani ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod";
            sql += kosul;
            sql += " GROUP BY RiskYonetimi.RiskeVerilecekCevap";
            sql += " ORDER BY Deger1 DESC";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Performans İzleme için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> PerformansIzlemeHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod));

            if (kriter.sorguTarihi1.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value));

            if (kriter.sorguTarihi2.HasValue)
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskEvreni.Durum", "=", (int)ENUMDurum.Onayli));
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("RiskYonetimi.Durum", "=", (int)ENUMDurum.Onayli));

            if (kosul != "")
                kosul = " WHERE " + kosul;


            var kayitlar = new List<Grafik>();


            //Risk bildiriminin Risk Sekretaryası tarafından  onaylanması ile kaydının tamamlanması ile riske cevap verilmesi arasında geçen ortalama süre
            //**Risk Kaydı ekranında kaydın Risk Sekretaryası tarafından onaylanması ile Risklerin Değerlendirilmesi ekranında kaydın onaylanması arasında geçen ortalama süre**

            sql = @"SELECT 
                    CONVERT(varchar,YEAR(GETDATE())) AS Aciklama,
                    AVG(CONVERT(int, DATEDIFF(DAY,RiskEvreni.KayitTarihi,RiskAzaltmaPlani.KayitTarihi))) AS Deger1, 0.0 AS Deger2, '' as EkAciklama
                    FROM RiskEvreni
                    INNER JOIN RiskYonetimi ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod
                    INNER JOIN RiskAzaltmaPlani ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod
                    ";
            sql += kosul;
            sql += " AND " + OrtakService.KosulOl("RiskAzaltmaPlani.Durum", "=", (int)ENUMDurum.Onayli);
            sql = sql.Replace("RiskYonetimi.KayitTarihi", "RiskEvreni.KayitTarihi");

            var kayitlar1 = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar1 != null && kayitlar1.Count > 0)
                kayitlar.Add(kayitlar1[0]);
            else
                kayitlar.Add(new Grafik());



            //Bir önceki yıla göre azalan olay raporlaması sayısı * *
            //Örneğin geçen sene 10 tane olay girilip onaylanmış, bu sene 8 tane girilmiş onaylanmış, bir önceki yıla göre azalan olay raporlama sayısı 2 olarak görünecek
            sql = "SELECT";
            sql += " CONVERT(varchar,YEAR(GETDATE())) AS Aciklama,";
            sql += " (SELECT COUNT(*) FROM OlayRaporlama WHERE Durum = 10 AND YEAR(OlayRaporlama.KayitTarihi) = YEAR(GETDATE())-1";
            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                sql += " AND " + OrtakService.KosulOl("KoordinatorlukKod", "=", kriter.KoordinatorlukKod);
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                sql += " AND " + OrtakService.KosulOl("BirimKod", "=", kriter.BirimKod);
            sql += ") - ";

            sql += " (SELECT COUNT(*) FROM OlayRaporlama WHERE Durum = 10 AND YEAR(OlayRaporlama.KayitTarihi) = YEAR(GETDATE())";
            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                sql += " AND " + OrtakService.KosulOl("KoordinatorlukKod", "=", kriter.KoordinatorlukKod);
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                sql += " AND " + OrtakService.KosulOl("BirimKod", "=", kriter.BirimKod);
            sql += ") AS Deger1, 0.0 AS Deger2, '' as EkAciklama";

            var kayitlar2 = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar2 != null && kayitlar2.Count > 0)
                kayitlar.Add(kayitlar2[0]);
            else
                kayitlar.Add(new Grafik());


            //Mevcut kontroller ile risk iştahının altına düşürülen risklerin sayısı **
            //Risklerin değerlendirilmesi ekranında bir risk kaydı için(yapısal risk seviyesi orta, yüksek veya çok yüksekken) kontroller uygulandıktan sonra artık risk seviyesi risk iştahının altına düşen(Düşük ve Çok Düşük olan) ve onaylı risk sayısı**
            sql = @"SELECT 
                    CONVERT(varchar,YEAR(GETDATE())) AS Aciklama,
                    COUNT(*) AS Deger1, 0.0 AS Deger2, '' as EkAciklama
                    FROM RiskYonetimi
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    LEFT JOIN Konfigurasyon ON Konfigurasyon.Durum = 10
                    LEFT JOIN ViewRiskSeviyeleri ON RiskYonetimi.Etki=ViewRiskSeviyeleri.Etki AND RiskYonetimi.Olasilik=ViewRiskSeviyeleri.Olasilik 
                    ";
            sql += kosul;
            sql += " AND ViewRiskSeviyeleri.Seviye >= 3 AND RiskYonetimi.ArtikRiskSeviyesi < 3";

            var kayitlar3 = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar3 != null && kayitlar3.Count > 0)
                kayitlar.Add(kayitlar3[0]);
            else
                kayitlar.Add(new Grafik());


            //Risk azaltma planlarının uygulanması sonrası risk iştahının altına düşürülen risklerin sayısı **
            //Risklerin yönetilmesi ekranında bir risk kaydı için(artık risk seviyesi orta, yüksek veya çok yüksekken) azaltma planı oluşturulup onaylandıktan sonra artık risk seviyesi risk iştahının altına düşen(Düşük ve Çok Düşük olan) risk sayısı**
            sql = @"SELECT 
                    CONVERT(varchar,YEAR(GETDATE())) AS Aciklama,
                    COUNT(*) AS Deger1, 0.0 AS Deger2, '' as EkAciklama
                    FROM RiskEvreni
                    INNER JOIN RiskYonetimi ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod
                    INNER JOIN RiskAzaltmaPlani ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod
                    ";
            sql += kosul;
            sql += " AND RiskAzaltmaPlani.Durum = 10 AND RiskYonetimi.ArtikRiskSeviyesi < 3 AND RiskYonetimi.ArtikRiskSeviyesiOncekiDeger >= 3";

            var kayitlar4 = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar4 != null && kayitlar4.Count > 0)
                kayitlar.Add(kayitlar4[0]);
            else
                kayitlar.Add(new Grafik());



            //Yıl içinde risk yönetimi yazılımı üzerinden risk belirleyen personel sayısının tüm personele oranı **
            //Yıl içinde risk kaydı ekranında risk kaydeden(ve onaylı durumuna geçen) personel sayısının tüm personele oranı * *
            sql = "SELECT";
            sql += " CONVERT(varchar,YEAR(GETDATE())) AS Aciklama,";
            sql += " CONVERT(int, ROUND((CONVERT(decimal, 100 * (SELECT COUNT(*) FROM (SELECT COUNT(*) AS PERSONELSAYISI FROM RiskEvreni";
            sql += " WHERE RiskEvreni.Durum = 10";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod);
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod);
            if (kriter.sorguTarihi1.HasValue)
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value);
            if (kriter.sorguTarihi2.HasValue)
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value);

            sql += " GROUP BY RiskEvreni.RiskSahibiKod) AS PERSONELSAYISI))";
            sql += " / ";//----------------------Sonraki SQL 0 döndüğünde 0 bölünme hatası alıyoruz 
            sql += " (SELECT COUNT(*) FROM ViewPersonel WHERE 1=1";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                sql += " AND " + OrtakService.KosulOl("ViewPersonel.KoordinatorlukKod", "=", kriter.KoordinatorlukKod);
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                sql += " AND " + OrtakService.KosulOl("ViewPersonel.BirimKod", "=", kriter.BirimKod);

            sql += ")),0)) AS Deger1, 0.0 AS Deger2, '' as EkAciklama";

            try
            {
                var kayitlar5 = await _unitOfWork.SQLCalistirAsync(sql);
                if (kayitlar5 != null && kayitlar5.Count > 0)
                    kayitlar.Add(kayitlar5[0]);
                else
                    kayitlar.Add(new Grafik());
            }
            catch { }



            //Yıl içinde oluşturulan risk azaltma planı sayısının yıl içinde belirlenen risklere oranı * *
            //Yıl içinde risklerin yönetilmesi ekranında onaylanan risk azaltma planı sayısının yıl içinde risk kaydı ekranında onaylanan risklere oranı * *
            sql = @"SELECT ";
            sql += " CONVERT(varchar,YEAR(GETDATE())) AS Aciklama,";
            sql += " CONVERT(int, ROUND((CONVERT(decimal, 100 * (SELECT COUNT(*) FROM RiskAzaltmaPlani WHERE RiskAzaltmaPlani.Durum=10 ";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                sql += " AND " + OrtakService.KosulOl("RiskAzaltmaPlani.SorumluKoordinatorlukKod", "=", kriter.KoordinatorlukKod);//RiskAzaltmaPlani.KoordinatorlukKod alanı yok SorumluKoordinatorlukKod var. Melih 28.07.2023
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                sql += " AND " + OrtakService.KosulOl("RiskAzaltmaPlani.BirimKod", "=", kriter.BirimKod);
            if (kriter.sorguTarihi1.HasValue)
                sql += " AND " + OrtakService.KosulOl("RiskAzaltmaPlani.KayitTarihi", ">=", kriter.sorguTarihi1.Value);
            if (kriter.sorguTarihi2.HasValue)
                sql += " AND " + OrtakService.KosulOl("RiskAzaltmaPlani.KayitTarihi", "<=", kriter.sorguTarihi2.Value);

            sql += ")) / "; //----------------------Sonraki SQL 0 döndüğünde 0 bölünme hatası alıyoruz 

            sql += "(SELECT COUNT(*) FROM RiskEvreni WHERE RiskEvreni.Durum=10 ";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.KoordinatorlukKod", "=", kriter.KoordinatorlukKod);
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.BirimKod", "=", kriter.BirimKod);
            if (kriter.sorguTarihi1.HasValue)
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.KayitTarihi", ">=", kriter.sorguTarihi1.Value);
            if (kriter.sorguTarihi2.HasValue)
                sql += " AND " + OrtakService.KosulOl("RiskEvreni.KayitTarihi", "<=", kriter.sorguTarihi2.Value);

            sql += ")),0)) AS Deger1, 0.0 AS Deger2, '' as EkAciklama";

            try
            {
                var kayitlar6 = await _unitOfWork.SQLCalistirAsync(sql);
                if (kayitlar6 != null && kayitlar6.Count > 0)
                    kayitlar.Add(kayitlar6[0]);
                else
                    kayitlar.Add(new Grafik());
            }
            catch { }


            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// KRITolerans için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KRIToleransHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("BirimKod", "=", kriter.BirimKod));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("re.Durum", "=", (int)ENUMDurum.Onayli)); //RiskEvreni
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("ry.Durum", "=", (int)ENUMDurum.Onayli)); //RiskYonetimi
            kosul += " AND ((ry.RiskeVerilecekCevap IN(1,2)) OR (ry.RiskeVerilecekCevap IN(4) AND (ra.Durum is null OR ra.Durum < 90) ))";

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = @"select Adi AS Aciklama, 
                    count(*) AS Deger1,
                    0.0 As Deger2, 
                    Durum As EkAciklama 
                    from( 
                    select trk.Adi, re.KoordinatorlukKod, re.BirimKod,
                    case  
                    when (arg.GerceklesenDeger is null OR arg.GerceklesenDeger = 0) then 'GRI' 
                    when arg.GerceklesenDeger >= arg.KirmiziDeger then 'KIRMIZI' 
                    when arg.GerceklesenDeger <= arg.YesilDeger then 'YESIL' 
                    else 'SARI' END Durum 
                    FROM RiskEvreni re 
                    INNER JOIN RiskYonetimi ry ON ry.RiskEvreniKod = re.Kod 
                    INNER JOIN RiskEvreniRiskKategori rk ON re.Kod = rk.RiskEvreniKod
                    INNER JOIN TanimRiskKategori trk ON rk.RiskKategoriKod = trk.Kod
                    LEFT JOIN RiskAzaltmaPlani ra ON ra.RiskYonetimiKod = ry.Kod

                    LEFT OUTER JOIN ( SELECT a1.RiskEvreniKod, a1.KirmiziDeger, a1.YesilDeger, k1.GerceklesenDeger  FROM AnahtarRiskGostergesi AS a1
                    INNER JOIN AnahtarRiskGostergesiDonem k1 ON a1.Kod = k1.AnahtarRiskGostergesiKod 
                    INNER JOIN(SELECT AnahtarRiskGostergesiKod, MAX(Donem) AS MAX_Donem FROM AnahtarRiskGostergesiDonem WHERE not AnahtarRiskGostergesiKod is null GROUP BY AnahtarRiskGostergesiKod ) k2 
                    ON k1.AnahtarRiskGostergesiKod = k2.AnahtarRiskGostergesiKod AND k1.Donem = k2.MAX_DONEM AND not k1.AnahtarRiskGostergesiKod is null

                    ) AS arg ON re.Kod = arg.RiskEvreniKod 

                    {KOSUL}
                    
                    ) Sorgu 

                    WHERE Sorgu.Durum <> 'GRI'

                    group by Adi, Durum 
                    order by Durum, Adi";

            if (!string.IsNullOrWhiteSpace(kosul))
                sql = sql.Replace("{KOSUL}", kosul);


            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// KRITolerans için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KRIToleransPieHazirlaAsync(KullaniciDto kullanan, Grafik kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("KoordinatorlukKod", "=", kriter.KoordinatorlukKod));

            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("BirimKod", "=", kriter.BirimKod));

            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("re.Durum", "=", (int)ENUMDurum.Onayli)); //RiskEvreni
            kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("ry.Durum", "=", (int)ENUMDurum.Onayli)); //RiskYonetimi
            kosul += " AND ((ry.RiskeVerilecekCevap IN(1,2)) OR (ry.RiskeVerilecekCevap IN(4) AND (ra.Durum is null OR ra.Durum < 90) ))";

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = @"select '' AS Aciklama, 
                    count(*) AS Deger1,
                    0.0 As Deger2, 
                    Durum As EkAciklama 
                    from( 
                    select re.KoordinatorlukKod, re.BirimKod,
                    case  
                    when (arg.GerceklesenDeger is null OR arg.GerceklesenDeger = 0) then 'Henüz değerlendirilmemiş' 
                    when arg.GerceklesenDeger >= arg.KirmiziDeger then 'Kırmızı' 
                    when arg.GerceklesenDeger <= arg.YesilDeger then 'Yeşil' 
                    else 'Sarı' END Durum 
                    FROM RiskEvreni re 
                    INNER JOIN RiskYonetimi ry ON ry.RiskEvreniKod = re.Kod 
                    LEFT JOIN RiskAzaltmaPlani ra ON ra.RiskYonetimiKod = ry.Kod

                    LEFT OUTER JOIN ( SELECT a1.RiskEvreniKod, a1.KirmiziDeger, a1.YesilDeger, k1.GerceklesenDeger  FROM AnahtarRiskGostergesi AS a1
                    INNER JOIN AnahtarRiskGostergesiDonem k1 ON a1.Kod = k1.AnahtarRiskGostergesiKod 
                    INNER JOIN(SELECT AnahtarRiskGostergesiKod, MAX(Donem) AS MAX_Donem FROM AnahtarRiskGostergesiDonem WHERE not AnahtarRiskGostergesiKod is null GROUP BY AnahtarRiskGostergesiKod ) k2 
                    ON k1.AnahtarRiskGostergesiKod = k2.AnahtarRiskGostergesiKod AND k1.Donem = k2.MAX_DONEM AND not k1.AnahtarRiskGostergesiKod is null

                    ) AS arg ON re.Kod = arg.RiskEvreniKod 

                    {KOSUL}
                    
                    ) Sorgu 

                    group by Durum 
                    order by Durum";

            if (!string.IsNullOrWhiteSpace(kosul))
                sql = sql.Replace("{KOSUL}", kosul);

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// KRIToleransPaneli için, istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KRIToleransPaneliHazirlaAsync(KullaniciDto kullanan, GrafikRiskYonetimi kriter)
        {
            string sql = "";
            string kosul = "";

            if (!string.IsNullOrEmpty(kriter.KoordinatorlukKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("A.KoordinatorlukKod", "=", kriter.KoordinatorlukKod));
            if (!string.IsNullOrEmpty(kriter.BirimKod))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("A.BirimKod", "=", kriter.BirimKod));
            if (!string.IsNullOrWhiteSpace(kriter.Kategori))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("A.RiskKategorisiAdi", " like ", "'%" + Arac.TirnakYoket(kriter.Kategori) + "%'"));
            if (!string.IsNullOrWhiteSpace(kriter.Durum))
                kosul = OrtakService.KosulEkle(kosul, "AND", OrtakService.KosulOl("A.Durum", "=", "'" + Arac.TirnakYoket(kriter.Durum) + "'"));

            if (kosul != "")
                kosul = " WHERE " + kosul;

            sql = @"SELECT * FROM (SELECT 
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    RiskEvreni.RiskNo, 
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    RiskEvreni.RiskTanimi, 
                    RiskYonetimi.ArtikRiskPuaniOncekiDeger,
                    RiskYonetimi.Olasilik*RiskYonetimi.Etki*RiskYonetimi.KontrolEtkinlikAgirligi AS ArtikRiskPuaniGuncelDeger,
                    RiskYonetimi.ArtikRiskSeviyesiOncekiDeger,
                    RiskYonetimi.ArtikRiskSeviyesi AS ArtikRiskSeviyesiGuncelDeger,
                    RiskYonetimi.GirisTarihi AS ArtikRiskSeviyesiGirisTarihi,
                    RiskYonetimi.GuncellemeTarihi AS GuncellemeTarihi,
					(CASE WHEN AnahtarRiskGostergesi.Adi IS NULL THEN RiskEvreni.AnahtarRiskGostergesiAdi ELSE AnahtarRiskGostergesi.Adi END) AS AnahtarRiskGostergesiAdi,
                    AnahtarRiskGostergesi.KirmiziDeger KirmiziDeger,
					AnahtarRiskGostergesi.YesilDeger YesilDeger,
					STUFF((SELECT ';' + CONVERT(varchar, AnahtarRiskGostergesiDonem.GerceklesenDeger) FROM AnahtarRiskGostergesiDonem WHERE AnahtarRiskGostergesiDonem.AnahtarRiskGostergesiKod= AnahtarRiskGostergesi.Kod ORDER BY AnahtarRiskGostergesiDonem.Donem  FOR XML PATH('')), 1, 1, '')  AS AnahtarRiskGostergesiDonem
                    ,case  
                    when (arg.GerceklesenDeger is null OR arg.GerceklesenDeger = 0) then 'GRI' 
					when arg.GerceklesenDeger >= arg.KirmiziDeger then 'KIRMIZI' 
                    when arg.GerceklesenDeger <= arg.YesilDeger then 'YESIL' 
                    else 'SARI' END Durum 

                    FROM RiskYonetimi
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod

                    LEFT OUTER JOIN ( SELECT a1.RiskEvreniKod, a1.KirmiziDeger, a1.YesilDeger, k1.GerceklesenDeger  FROM AnahtarRiskGostergesi AS a1
                    INNER JOIN AnahtarRiskGostergesiDonem k1 ON a1.Kod = k1.AnahtarRiskGostergesiKod 
                    INNER JOIN(SELECT AnahtarRiskGostergesiKod, MAX(Donem) AS MAX_Donem FROM AnahtarRiskGostergesiDonem WHERE not AnahtarRiskGostergesiKod is null GROUP BY AnahtarRiskGostergesiKod ) k2 
                    ON k1.AnahtarRiskGostergesiKod = k2.AnahtarRiskGostergesiKod AND k1.Donem = k2.MAX_DONEM AND not k1.AnahtarRiskGostergesiKod is null

                    ) AS arg ON RiskEvreni.Kod = arg.RiskEvreniKod 

                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10
	                ) A ";
            sql += kosul;

            var kayitlar = await _unitOfWorkRaporTrend.SQLCalistirAsync(sql);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }
    }
}
