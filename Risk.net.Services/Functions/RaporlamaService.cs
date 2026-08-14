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
    /// Raporlama işlemlerinin yapıldığı servis
    /// </summary>
    public class RaporlamaService : IRaporlamaService
    {
        /// <summary>
        /// IUnitOfWork<RaporUstYonetimRisk> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporUstYonetimRisk> _unitOfWorkRaporUstYonetimRisk;
        /// <summary>
        /// IUnitOfWork<RaporKoordinatorlerRisk> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporKoordinatorlerRisk> _unitOfWorkRaporKoordinatorlerRisk;
        /// <summary>
        /// IUnitOfWork<RaporRiskSekretaryasiRisk> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporRiskSekretaryasiRisk> _unitOfWorkRaporRiskSekretaryasiRisk;
        /// <summary>
        /// IUnitOfWork<RaporRiskSahipleriRisk> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporRiskSahipleriRisk> _unitOfWorkRaporRiskSahipleriRisk;
        /// <summary>
        /// IUnitOfWork<RaporTrend> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporTrend> _unitOfWorkRaporTrend;
        /// <summary>
        /// IUnitOfWork<RaporIcKontrolZayifliklari> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporIcKontrolZayifliklari> _unitOfWorkRaporIcKontrolZayifliklari;
        /// <summary>
        /// IUnitOfWork<RaporYillikRiskPlani> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporYillikRiskPlani> _unitOfWorkRaporYillikRiskPlani;
        /// <summary>
        /// IUnitOfWork<RaporYariYilRiskAzaltmaPlani> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporYariYilRiskAzaltmaPlani> _unitOfWorkRaporYariYilRiskAzaltmaPlani;
        /// <summary>
        /// IUnitOfWork<RaporUstYonetimStratejikPlanlama> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporUstYonetimStratejikPlanlama> _unitOfWorkRaporUstYonetimStratejikPlanlama;
        /// <summary>
        /// IUnitOfWork<RaporKoordinatorlerStratejikPlanlama> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporKoordinatorlerStratejikPlanlama> _unitOfWorkRaporKoordinatorlerStratejikPlanlama;
        /// <summary>
        /// IUnitOfWork<RaporSurecAltSurec> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporSurecAltSurec> _unitOfWorkRaporSurecAltSurec;
        /// <summary>
        /// IUnitOfWork<RaporRiskIzleme> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporRiskIzleme> _unitOfWorkRaporRiskIzleme;
        /// <summary>
        /// IUnitOfWork<RaporAnahtarRiskGostergesi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporAnahtarRiskGostergesi> _unitOfWorkRaporAnahtarRiskGostergesi;
        /// <summary>
        /// IUnitOfWork<RaporOlay> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporOlay> _unitOfWorkRaporOlay;
        /// <summary>
        /// IUnitOfWork<RaporRiskYonetimiBeyannamesi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RaporRiskYonetimiBeyannamesi> _unitOfWorkRaporRiskYonetimiBeyannamesi;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTE;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.RaporlamaService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWorkRaporUstYonetimRisk"></param>
        /// <param name="unitOfWorkRaporKoordinatorlerRisk"></param>
        /// <param name="unitOfWorkRaporRiskSekretaryasiRisk"></param>
        /// <param name="unitOfWorkRaporRiskSahipleriRisk"></param>
        /// <param name="unitOfWorkRaporTrend"></param>
        /// <param name="unitOfWorkRaporIcKontrolZayifliklari"></param>
        /// <param name="unitOfWorkRaporYillikRiskPlani"></param>
        /// <param name="unitOfWorkRaporYariYilRiskAzaltmaPlani"></param>
        /// <param name="unitOfWorkRaporUstYonetimStratejikPlanlama"></param>
        /// <param name="unitOfWorkRaporKoordinatorlerStratejikPlanlama"></param>
        /// <param name="unitOfWorkRaporSurecAltSurec"></param>
        /// <param name="unitOfWorkRaporRiskIzleme"></param>
        /// <param name="sharedResource"></param>
        /// <param name="serviceCTE"></param>
        /// <remarks></remarks>
        public RaporlamaService(IUnitOfWork<RaporUstYonetimRisk> unitOfWorkRaporUstYonetimRisk,
                                IUnitOfWork<RaporKoordinatorlerRisk> unitOfWorkRaporKoordinatorlerRisk,
                                IUnitOfWork<RaporRiskSekretaryasiRisk> unitOfWorkRaporRiskSekretaryasiRisk,
                                IUnitOfWork<RaporRiskSahipleriRisk> unitOfWorkRaporRiskSahipleriRisk,
                                IUnitOfWork<RaporTrend> unitOfWorkRaporTrend,
                                IUnitOfWork<RaporIcKontrolZayifliklari> unitOfWorkRaporIcKontrolZayifliklari,
                                IUnitOfWork<RaporYillikRiskPlani> unitOfWorkRaporYillikRiskPlani,
                                IUnitOfWork<RaporYariYilRiskAzaltmaPlani> unitOfWorkRaporYariYilRiskAzaltmaPlani,
                                IUnitOfWork<RaporUstYonetimStratejikPlanlama> unitOfWorkRaporUstYonetimStratejikPlanlama,
                                IUnitOfWork<RaporKoordinatorlerStratejikPlanlama> unitOfWorkRaporKoordinatorlerStratejikPlanlama,
                                IUnitOfWork<RaporSurecAltSurec> unitOfWorkRaporSurecAltSurec,
                                IUnitOfWork<RaporRiskIzleme> unitOfWorkRaporRiskIzleme,
                                IUnitOfWork<RaporAnahtarRiskGostergesi> unitOfWorkRaporAnahtarRiskGostergesi,
                                IUnitOfWork<RaporOlay> unitOfWorkRaporOlay,
                                IUnitOfWork<RaporRiskYonetimiBeyannamesi> unitOfWorkRaporRiskYonetimiBeyannamesi,
                                IStringLocalizer<CustomResource> sharedResource,
                                ICTEKoordinatorlukService serviceCTE)
        {
            _unitOfWorkRaporUstYonetimRisk = unitOfWorkRaporUstYonetimRisk;
            _unitOfWorkRaporKoordinatorlerRisk = unitOfWorkRaporKoordinatorlerRisk;
            _unitOfWorkRaporRiskSekretaryasiRisk = unitOfWorkRaporRiskSekretaryasiRisk;
            _unitOfWorkRaporRiskSahipleriRisk = unitOfWorkRaporRiskSahipleriRisk;
            _unitOfWorkRaporTrend = unitOfWorkRaporTrend;
            _unitOfWorkRaporIcKontrolZayifliklari = unitOfWorkRaporIcKontrolZayifliklari;
            _unitOfWorkRaporYillikRiskPlani = unitOfWorkRaporYillikRiskPlani;
            _unitOfWorkRaporYariYilRiskAzaltmaPlani = unitOfWorkRaporYariYilRiskAzaltmaPlani;
            _unitOfWorkRaporUstYonetimStratejikPlanlama = unitOfWorkRaporUstYonetimStratejikPlanlama;
            _unitOfWorkRaporKoordinatorlerStratejikPlanlama = unitOfWorkRaporKoordinatorlerStratejikPlanlama;
            _unitOfWorkRaporSurecAltSurec = unitOfWorkRaporSurecAltSurec;
            _unitOfWorkRaporRiskIzleme = unitOfWorkRaporRiskIzleme;
            _unitOfWorkRaporAnahtarRiskGostergesi = unitOfWorkRaporAnahtarRiskGostergesi;
            _unitOfWorkRaporOlay = unitOfWorkRaporOlay;
            _unitOfWorkRaporRiskYonetimiBeyannamesi = unitOfWorkRaporRiskYonetimiBeyannamesi;
            _sharedResource = sharedResource;
            _serviceCTE = serviceCTE;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> UstYonetimRiskHazirlaAsync(KullaniciDto kullanan, RaporUstYonetimRisk kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    YEAR(RiskYonetimi.KayitTarihi) AS Yil,
                    (CASE WHEN MONTH(RiskYonetimi.KayitTarihi) BETWEEN 1 AND 6 THEN 1 WHEN  MONTH(RiskYonetimi.KayitTarihi) BETWEEN 7 AND 12 THEN 2 ELSE 0 END) AS Donem,
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, RiskEvreni.RiskNo AS RiskNumarasi, 
                    RiskEvreni.RiskAdi, 
                    RiskEvreni.RiskTanimi, 
                    RiskAzaltmaPlani.AzaltmaPlani, ArtikRiskSeviyesi,
                    STUFF((SELECT ';' + TanimRiskKategori.Adi
                           FROM TanimRiskKategori,RiskEvreniRiskKategori
                           WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod
                           FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
					(CASE WHEN AnahtarRiskGostergesi.Adi IS NULL THEN RiskEvreni.AnahtarRiskGostergesiAdi ELSE AnahtarRiskGostergesi.Adi END) AS AnahtarRiskGostergesiAdi,
                    ISNULL((SELECT TOP (1) GerceklesenDeger FROM AnahtarRiskGostergesiDonem WHERE AnahtarRiskGostergesiKod = AnahtarRiskGostergesi.Kod ORDER BY Donem DESC),0) AS AnahtarRiskGostergesiDonem
                    FROM RiskYonetimi
                    INNER JOIN RiskAzaltmaPlani ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod
                    INNER JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod
                    LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod
                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10 AND RiskYonetimi.ArtikRiskSeviyesi>=4";

            sql += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskYonetimi.KayitTarihi) <= " + kriter.KriterYil;
            if (kriter.Donem == 1)
                sql += " AND (MONTH(RiskYonetimi.KayitTarihi) BETWEEN 1 AND 6) ";
            else if (kriter.Donem == 2)
                sql += " AND (MONTH(RiskYonetimi.KayitTarihi) BETWEEN 7 AND 12) ";

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }

            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }


            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporUstYonetimRisk.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KoordinatorlerRiskHazirlaAsync(KullaniciDto kullanan, RaporKoordinatorlerRisk kriter)
        {
            string sql = "";
            string kosul = "";

            //!!!!DEĞİŞECEK Anahtar DÖnem tablosu eklendi, dönem alanları AnahtarRiskGostergesi tablosunda çıkarıldı
            sql = @"SELECT 
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
                    StratejikPlanHedef.Adi AS Hedef,
                    RiskEvreni.RiskAdi, 
                    RiskEvreni.RiskTanimi, 
                    RiskEvreni.AnahtarRiskGostergesiAdi AS AnahtarRiskGostergesi, 
                    (SELECT CONVERT(decimal(18,2),GerceklesenDeger) 
                        FROM AnahtarRiskGostergesiDonem t1
	                    INNER JOIN ( SELECT AnahtarRiskGostergesiKod, max(Donem) AS salary FROM AnahtarRiskGostergesiDonem GROUP BY AnahtarRiskGostergesiKod) t2 ON t1.AnahtarRiskGostergesiKod = t2.AnahtarRiskGostergesiKod AND t1.Donem = t2.salary
                        where AnahtarRiskGostergesi.Kod = t1.AnahtarRiskGostergesiKod) As MevcutDeger, 
                    RiskYonetimi.Etki,
					RiskYonetimi.Olasilik,
                    RiskYonetimi.KontrolEtkinlikAgirligi AS KontrolKriteriAgirligi,
                    Konfigurasyon.EtkiOlasilikMatrisi,
					Konfigurasyon.RiskKategoriFinansal,
                    RiskYonetimi.RiskeVerilecekCevap,
                    RiskAzaltmaPlani.AzaltmaPlani,
                    ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi AS AzaltmaPlaniSorumlusu,
                    RiskAzaltmaPlani.BitisTarihi,
                    RiskAzaltmaPlani.MevcutDurum,
                    RiskAzaltmaPlani.ErtelemeBaslangicTarihi,
                    RiskAzaltmaPlani.ErtelemeBitisTarihi,
                    RiskAzaltmaPlani.ErtelemeNot AS ErtlenmisZamanPlaniNedeni,
                    RiskAzaltmaPlani.AzaltmaPlaniNotlar
                    FROM RiskYonetimi
                    INNER JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod 
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN StratejikPlan ON RiskEvreni.AmacKod = StratejikPlan.Kod 
                    LEFT JOIN StratejikPlanHedef ON RiskEvreni.HedefKod = StratejikPlanHedef.Kod 
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod
					LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod
					LEFT JOIN Konfigurasyon ON Konfigurasyon.Durum = 10
                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10";

            sql += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }


            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporKoordinatorlerRisk.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskSekretaryasiRiskHazirlaAsync(KullaniciDto kullanan, RaporRiskSekretaryasiRisk kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    YEAR(RiskEvreni.KayitTarihi) as Yil,
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
                    TanimStratejikPlanDonem.BaslamaYil,
                    TanimStratejikPlanDonem.BitisYil,
                    StratejikPlan.Amac,
                    StratejikPlanHedef.Adi AS Hedef,
                    Surec.Adi AS Surec, 
                    AltSurec.Adi AS AltSurec, 
                    RiskEvreni.RiskNo, 
                    RiskEvreni.RiskAdi, 
                    RiskEvreni.RiskTanimi, 
                    ViewPersonelRisk.Adi + ' ' + ViewPersonelRisk.Soyadi AS RiskSahibi,
                    RiskEvreni.RiskTuru AS TehditFirsat, 
                    RiskEvreni.AnahtarRiskGostergesiAdi AS AnahtarRiskGostergesi, 
                    (SELECT GerceklesenDeger 
                        FROM AnahtarRiskGostergesiDonem t1
	                    INNER JOIN ( SELECT AnahtarRiskGostergesiKod, max(Donem) AS salary FROM AnahtarRiskGostergesiDonem GROUP BY AnahtarRiskGostergesiKod) t2 ON t1.AnahtarRiskGostergesiKod = t2.AnahtarRiskGostergesiKod AND t1.Donem = t2.salary
                        where AnahtarRiskGostergesi.Kod = t1.AnahtarRiskGostergesiKod) As MevcutDeger, 
                    RiskYonetimi.Etki,
					RiskYonetimi.Olasilik,
                    RiskYonetimi.KontrolEtkinlikAgirligi AS KontrolKriteriAgirligi,
                    Konfigurasyon.EtkiOlasilikMatrisi,
					Konfigurasyon.RiskKategoriFinansal,
                    RiskYonetimi.RiskeVerilecekCevap,
                    RiskAzaltmaPlani.AzaltmaPlani,
                    ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi AS AzaltmaPlaniSorumlusu,
                    RiskAzaltmaPlani.BitisTarihi,
                    RiskAzaltmaPlani.MevcutDurum,
                    RiskAzaltmaPlani.ErtelemeBaslangicTarihi,
                    RiskAzaltmaPlani.ErtelemeBitisTarihi,
                    RiskAzaltmaPlani.ErtelemeNot AS ErtlenmisZamanPlaniNedeni,
                    RiskAzaltmaPlani.AzaltmaPlaniNotlar
                    FROM RiskYonetimi
                    INNER JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod 
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN StratejikPlan ON RiskEvreni.AmacKod = StratejikPlan.Kod 
                    LEFT JOIN StratejikPlanHedef ON RiskEvreni.HedefKod = StratejikPlanHedef.Kod 
                    LEFT JOIN TanimStratejikPlanDonem ON TanimStratejikPlanDonem.Kod = StratejikPlan.StratejikPlanDonemKod 
                    LEFT JOIN Surec ON RiskEvreni.SurecKod = Surec.Kod 
                    LEFT JOIN AltSurec ON RiskEvreni.AltSurecKod = AltSurec.Kod
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod
					LEFT JOIN ViewPersonelAdSoyad AS ViewPersonelRisk ON ViewPersonelRisk.Kod = RiskEvreni.RiskSahibiKod
					LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod
					LEFT JOIN Konfigurasyon ON Konfigurasyon.Durum = 10
                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10";

            sql += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " (RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'" + " AND RiskAzaltmaPlani.SorumluKoordinatorlukKod = '" + Arac.TirnakYoket(k) + "') ";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";

                ////42 il koordinatörlüğü
                //if (kriter.KoordinatorlukAdi == "-42")
                //    sql += " AND ViewKoordinatorluk.Tur = 40";
                //else
                //{
                //    sql += " AND RiskEvreni.KoordinatorlukKod = " + kriter.KoordinatorlukAdi;
                //    sql += " AND RiskAzaltmaPlani.SorumluKoordinatorlukKod = " + kriter.KoordinatorlukAdi;
                //}
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }


            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporRiskSekretaryasiRisk.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskSahipleriRiskHazirlaAsync(KullaniciDto kullanan, RaporRiskSahipleriRisk kriter)
        {
            if (!Arac.YetkisiVarmi("UZMAN", kullanan))
                YetkiKontrolundenGecir(kullanan, kriter);

            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    YEAR(RiskEvreni.KayitTarihi) AS Yil,
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
                    TanimStratejikPlanDonem.BaslamaYil,
                    TanimStratejikPlanDonem.BitisYil,                   
                    StratejikPlan.Amac,
                    StratejikPlanHedef.Adi AS Hedef,
                    Surec.Adi AS Surec, 
                    AltSurec.Adi AS AltSurec, 
                    RiskEvreni.RiskNo, 
                    RiskEvreni.RiskAdi, 
                    RiskEvreni.RiskTanimi, 
                    RiskSahibi.Adi + ' ' + RiskSahibi.Soyadi AS RiskSahibi,
                    RiskEvreni.RiskTuru AS TehditFirsat, 
                    RiskEvreni.AnahtarRiskGostergesiAdi AS AnahtarRiskGostergesi, 
                    (SELECT GerceklesenDeger 
                        FROM AnahtarRiskGostergesiDonem t1
	                    INNER JOIN ( SELECT AnahtarRiskGostergesiKod, max(Donem) AS salary FROM AnahtarRiskGostergesiDonem GROUP BY AnahtarRiskGostergesiKod) t2 ON t1.AnahtarRiskGostergesiKod = t2.AnahtarRiskGostergesiKod AND t1.Donem = t2.salary
                        where AnahtarRiskGostergesi.Kod = t1.AnahtarRiskGostergesiKod) As MevcutDeger, 
                    RiskYonetimi.Etki,
					RiskYonetimi.Olasilik,
                    RiskYonetimi.KontrolEtkinlikAgirligi AS KontrolKriteriAgirligi,
                    Konfigurasyon.EtkiOlasilikMatrisi,
					Konfigurasyon.RiskKategoriFinansal,
                    RiskYonetimi.RiskeVerilecekCevap,
                    RiskAzaltmaPlani.AzaltmaPlani,
                    ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi AS AzaltmaPlaniSorumlusu,
                    RiskAzaltmaPlani.BitisTarihi,
                    RiskAzaltmaPlani.MevcutDurum,
                    RiskAzaltmaPlani.ErtelemeBaslangicTarihi,
                    RiskAzaltmaPlani.ErtelemeBitisTarihi,
                    RiskAzaltmaPlani.ErtelemeNot AS ErtlenmisZamanPlaniNedeni,
                    RiskAzaltmaPlani.AzaltmaPlaniNotlar
                    FROM RiskYonetimi
                    INNER JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod 
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN StratejikPlan ON RiskEvreni.AmacKod = StratejikPlan.Kod 
                    LEFT JOIN StratejikPlanHedef ON RiskEvreni.HedefKod = StratejikPlanHedef.Kod 
                    LEFT JOIN TanimStratejikPlanDonem ON TanimStratejikPlanDonem.Kod = StratejikPlan.StratejikPlanDonemKod 
                    LEFT JOIN Surec ON RiskEvreni.SurecKod = Surec.Kod 
                    LEFT JOIN AltSurec ON RiskEvreni.AltSurecKod = AltSurec.Kod
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod
                    LEFT JOIN ViewPersonelAdSoyad AS RiskSahibi ON RiskSahibi.Kod = RiskEvreni.RiskSahibiKod
					LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod
					LEFT JOIN Konfigurasyon ON Konfigurasyon.Durum = 10
                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10 ";

            sql += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }


            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporRiskSahipleriRisk.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> TrendHazirlaAsync(KullaniciDto kullanan, RaporTrend kriter)
        {
            if (!Arac.YetkisiVarmi("UZMAN", kullanan))
                YetkiKontrolundenGecir(kullanan, kriter);

            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    YEAR(RiskEvreni.KayitTarihi) AS Yil,
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    RiskEvreni.RiskNo, 
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    RiskEvreni.RiskAdi, 
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

                    FROM RiskYonetimi
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod

                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }


            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporTrend.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> IcKontrolZayifliklariHazirlaAsync(KullaniciDto kullanan, RaporIcKontrolZayifliklari kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
                    RiskYonetimiKontrol.KontrolTanimi,
                    RiskYonetimiKontrol.KontrolEtkinligi,
                    RiskAzaltmaPlani.AzaltmaPlani,
                    ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi AS AzaltmaPlaniSorumlusu,
                    RiskAzaltmaPlani.BitisTarihi,
                    RiskAzaltmaPlani.MevcutDurum,
                    RiskAzaltmaPlani.ErtelemeBaslangicTarihi,
                    RiskAzaltmaPlani.ErtelemeBitisTarihi,
                    RiskAzaltmaPlani.ErtelemeNot AS ErtlenmisZamanPlaniNedeni,
                    RiskAzaltmaPlani.AzaltmaPlaniNotlar
                    FROM RiskYonetimi
                    INNER JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod 
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod
                    INNER JOIN (
					    SELECT SUM(Etkinlik*OnemDuzeyi/100) AS EtkinlikOrt, rk.RiskYonetimiKod,
					    STUFF((SELECT ',' + k.Tanim FROM RiskYonetimiKontrol as k WHERE k.RiskYonetimiKod = rk.RiskYonetimiKod FOR XML PATH('')), 1, 1, '') AS KontrolTanimi,
					    STUFF((SELECT ',' + CONVERT(varchar, k.Etkinlik) FROM RiskYonetimiKontrol as k WHERE k.RiskYonetimiKod = rk.RiskYonetimiKod FOR XML PATH('')), 1, 1, '') AS KontrolEtkinligi
					    FROM  RiskYonetimiKontrol as rk GROUP BY RiskYonetimiKod
				    ) AS RiskYonetimiKontrol 
				    ON RiskYonetimi.Kod = RiskYonetimiKontrol.RiskYonetimiKod AND RiskYonetimiKontrol.EtkinlikOrt <= 2
                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10";

            sql += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }


            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporIcKontrolZayifliklari.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> YillikRiskPlaniHazirlaAsync(KullaniciDto kullanan, RaporYillikRiskPlani kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT
                    YEAR(RiskEvreni.KayitTarihi) AS Yil,
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    TanimStratejikPlanDonem.BaslamaYil,
                    TanimStratejikPlanDonem.BitisYil,                    
                    StratejikPlan.Amac,
                    StratejikPlanHedef.Adi AS Hedef,
                    Surec.Adi AS Surec,
                    AltSurec.Adi AS AltSurec,
                    RiskEvreni.RiskNo, 
                    RiskEvreni.RiskAdi, 
                    RiskEvreni.RiskTanimi, 
                    RiskEvreni.RiskTuru, 
                    RiskEvreni.AnahtarRiskGostergesiAdi AS AnahtarRiskGostergesi, 
                    RiskEvreni.RiskBaslangicTarihi, 
                    RiskEvreni.RiskBitisTarihi, 
                    '' AS IliskiliOlay, 
                    (SELECT GerceklesenDeger 
                        FROM AnahtarRiskGostergesiDonem t1
	                    INNER JOIN ( SELECT AnahtarRiskGostergesiKod, max(Donem) AS salary FROM AnahtarRiskGostergesiDonem GROUP BY AnahtarRiskGostergesiKod) t2 ON t1.AnahtarRiskGostergesiKod = t2.AnahtarRiskGostergesiKod AND t1.Donem = t2.salary
                        where AnahtarRiskGostergesi.Kod = t1.AnahtarRiskGostergesiKod) As MevcutDeger, 
                    STUFF((SELECT '|' + Tanim + '@'+ Cast(Etkinlik as varchar) + '@' + Cast(OnemDuzeyi as varchar) FROM RiskYonetimiKontrol WHERE RiskYonetimiKontrol.RiskYonetimiKod = RiskYonetimi.Kod AND RiskYonetimiKontrol.Durum = 1 FOR XML PATH('')), 1, 1, '') AS MevcutKontroller,
                    RiskYonetimi.Etki,
					RiskYonetimi.Olasilik,
                    RiskYonetimi.KontrolEtkinlikAgirligi AS KontrolKriteriAgirligi,
                    Konfigurasyon.EtkiOlasilikMatrisi,
					Konfigurasyon.RiskKategoriFinansal,
                    RiskYonetimi.RiskeVerilecekCevap,
                    RiskAzaltmaPlani.AzaltmaPlani,
                    ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi AS AzaltmaPlaniSorumlusu,
                    RiskAzaltmaPlani.BitisTarihi,
                    RiskAzaltmaPlani.MevcutDurum,
                    RiskAzaltmaPlani.ErtelemeBaslangicTarihi,
                    RiskAzaltmaPlani.ErtelemeBitisTarihi,
                    RiskAzaltmaPlani.ErtelemeNot AS ErtlenmisZamanPlaniNedeni,
                    RiskAzaltmaPlani.AzaltmaPlaniNotlar
                    FROM RiskYonetimi
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    LEFT JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod 
                    LEFT JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN StratejikPlan ON RiskEvreni.AmacKod = StratejikPlan.Kod 
                    LEFT JOIN StratejikPlanHedef ON RiskEvreni.HedefKod = StratejikPlanHedef.Kod 
                    LEFT JOIN TanimStratejikPlanDonem ON TanimStratejikPlanDonem.Kod = StratejikPlan.StratejikPlanDonemKod 
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod
					LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod
					LEFT JOIN Konfigurasyon ON Konfigurasyon.Durum = 10
                    LEFT JOIN Surec ON RiskEvreni.SurecKod = Surec.Kod 
                    LEFT JOIN AltSurec ON RiskEvreni.AltSurecKod = AltSurec.Kod 

                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10";

            sql += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;



            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }



            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporYillikRiskPlani.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> YariYilRiskAzaltmaPlaniHazirlaAsync(KullaniciDto kullanan, RaporYariYilRiskAzaltmaPlani kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT
                    YEAR(RiskEvreni.KayitTarihi) AS Yil,
                    0 AS Donem,
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    SorumluKoordinatorluk.Adi AS SorumluKoordinatorlukAdi, 
                    SorumluBirim.Adi AS SorumluBirimAdi, 
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    TanimStratejikPlanDonem.BaslamaYil,
                    TanimStratejikPlanDonem.BitisYil,                    
                    StratejikPlan.Amac,
                    StratejikPlanHedef.Adi AS Hedef,
                    RiskEvreni.RiskNo, 
                    RiskEvreni.RiskAdi, 
                    RiskEvreni.RiskTanimi, 
                    RiskEvreni.AnahtarRiskGostergesiAdi AS AnahtarRiskGostergesi, 
                    (SELECT GerceklesenDeger 
                        FROM AnahtarRiskGostergesiDonem t1
	                    INNER JOIN ( SELECT AnahtarRiskGostergesiKod, max(Donem) AS salary FROM AnahtarRiskGostergesiDonem GROUP BY AnahtarRiskGostergesiKod) t2 ON t1.AnahtarRiskGostergesiKod = t2.AnahtarRiskGostergesiKod AND t1.Donem = t2.salary
                        where AnahtarRiskGostergesi.Kod = t1.AnahtarRiskGostergesiKod) As MevcutDeger, 
				    STUFF((SELECT '|' + Tanim + '@'+ Cast(Etkinlik as varchar) + '@' + Cast(OnemDuzeyi as varchar) FROM RiskYonetimiKontrol WHERE RiskYonetimiKontrol.RiskYonetimiKod = RiskYonetimi.Kod AND RiskYonetimiKontrol.Durum = 1 FOR XML PATH('')), 1, 1, '') AS MevcutKontroller,
                    RiskYonetimi.Etki,
					RiskYonetimi.Olasilik,
                    RiskYonetimi.KontrolEtkinlikAgirligi AS KontrolKriteriAgirligi,
                    Konfigurasyon.EtkiOlasilikMatrisi,
					Konfigurasyon.RiskKategoriFinansal,
                    RiskYonetimi.RiskeVerilecekCevap,
                    RiskAzaltmaPlani.AzaltmaPlani,
                    ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi AS AzaltmaPlaniSorumlusu,
                    RiskAzaltmaPlani.BitisTarihi,
                    RiskAzaltmaPlani.MevcutDurum,
                    RiskAzaltmaPlani.ErtelemeBaslangicTarihi,
                    RiskAzaltmaPlani.ErtelemeBitisTarihi,
                    RiskAzaltmaPlani.ErtelemeNot AS ErtlenmisZamanPlaniNedeni,
                    RiskAzaltmaPlani.AzaltmaPlaniNotlar
                    FROM RiskYonetimi
                    INNER JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod 
                    INNER JOIN RiskEvreni ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN ViewBirim SorumluBirim ON RiskAzaltmaPlani.SorumluKoordinatorlukKod = SorumluBirim.KoordinatorlukKod AND RiskAzaltmaPlani.SorumluBirimKod = SorumluBirim.Kod
                    LEFT JOIN ViewKoordinatorluk SorumluKoordinatorluk ON RiskAzaltmaPlani.SorumluKoordinatorlukKod = SorumluKoordinatorluk.Kod 
                    LEFT JOIN StratejikPlan ON RiskEvreni.AmacKod = StratejikPlan.Kod 
                    LEFT JOIN StratejikPlanHedef ON RiskEvreni.HedefKod = StratejikPlanHedef.Kod 
                    LEFT JOIN TanimStratejikPlanDonem ON TanimStratejikPlanDonem.Kod = StratejikPlan.StratejikPlanDonemKod 
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod
					LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod
					LEFT JOIN Konfigurasyon ON Konfigurasyon.Durum = 10

                    WHERE RiskEvreni.Durum = 10 AND RiskYonetimi.Durum = 10";

            sql += " AND ((RiskYonetimi.RiskeVerilecekCevap IN(1,2)) OR (RiskYonetimi.RiskeVerilecekCevap IN(4) AND (RiskAzaltmaPlani.Durum is null OR RiskAzaltmaPlani.Durum < 90) ))";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            //if (kriter.Donem == 1)
            //    sql += " AND (MONTH(RiskYonetimi.KayitTarihi) BETWEEN 1 AND 6) ";
            //else if (kriter.Donem == 2)
            //    sql += " AND (MONTH(RiskYonetimi.KayitTarihi) BETWEEN 7 AND 12) ";

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";

                ////42 il koordinatörlüğü
                //if (kriter.KoordinatorlukAdi == "-42")
                //    sql += " AND ViewKoordinatorluk.Tur = 40";
                //else
                //    sql += " AND RiskEvreni.KoordinatorlukKod = " + kriter.KoordinatorlukAdi;
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }


            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporYariYilRiskAzaltmaPlani.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> UstYonetimStratejikPlanlamaHazirlaAsync(KullaniciDto kullanan, RaporUstYonetimStratejikPlanlama kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    [v].[Adi] as KoordinatorlukAdi,
                    [v0].[Adi] as BirimAdi,
                    [s6].[IsbirligiYapacakKoordinatorlukler],
                    [s6].[IsbirligiYapacakBirimler],
                    TanimStratejikPlanDonem.BaslamaYil,
                    TanimStratejikPlanDonem.BitisYil,
                    [s0].[Amac] as Amac, 
                    [s1].[Adi] as Hedef, 
                    [s2].[Adi] as AnahtarRiskGostergesi,
                    [s2].GostergeNo,
                    [s2].[Etki] as HedefeEtkisi,
                    [s2].BaslangicTarihi,
                    [s2].BitisTarihi,
                    [s2].BaslangicDegeri,
                    [s4].SapmaOrani,
                    [s4].SapmaNedeni,
                    [s4].IzlemeDonemleri
                    FROM [ViewStratejikPlanIzleme] AS [s]
                    INNER JOIN [StratejikPlan] AS [s0] ON [s].[AmacKod] = [s0].[Kod]
                    INNER JOIN [StratejikPlanHedef] AS [s1] ON [s0].[Kod] = [s1].[StratejikPlanKod] AND [s].[HedefKod] = [s1].[Kod] 
                    INNER JOIN [StratejikPlanHedefGosterge] AS [s2] ON [s1].[Kod] = [s2].[StratejikPlanHedefKod]
                    LEFT JOIN TanimStratejikPlanDonem ON TanimStratejikPlanDonem.Kod = [s0].StratejikPlanDonemKod 
                    LEFT JOIN [ViewKoordinatorluk] AS [v] ON [s1].[KoordinatorlukKod] = [v].[Kod]
                    LEFT JOIN [ViewBirim] AS [v0] ON [s1].[BirimKod] = [v0].[Kod]
                    LEFT JOIN (
                                SELECT d.StratejikPlanHedefGostergeKod as Kod, 
                                COUNT(*) ADET,
                                CONVERT(decimal(13,4), SUM(((CASE WHEN d.GerceklesenDegerYilSonu > 0 THEN d.GerceklesenDegerYilSonu ELSE d.GerceklesenDeger END)*100.00/NULLIF(d.PlanlananDeger,0)))/COUNT(*)) AS SapmaOrani,
                                STUFF((SELECT ',' + CONVERT(varchar, k.Donem) +';'+ CONVERT(varchar, k.PlanlananDeger) +';'+ CONVERT(varchar, k.GerceklesenDeger), +';'+ CONVERT(varchar, k.GerceklesenDegerYilSonu) FROM StratejikPlanIzlemeDonem as k WHERE k.StratejikPlanHedefGostergeKod = d.StratejikPlanHedefGostergeKod ORDER BY k.Donem FOR XML PATH('')), 1, 1, '') AS IzlemeDonemleri,
                                (SELECT d2.SapmaNedeni FROM StratejikPlanIzlemeDonem as d2 WHERE d2.Kod = (SELECT TOP 1 d3.Kod FROM StratejikPlanIzlemeDonem as d3 WHERE d3.StratejikPlanHedefGostergeKod = d.StratejikPlanHedefGostergeKod ORDER BY d3.Donem DESC)) AS SapmaNedeni
                                FROM  StratejikPlanIzlemeDonem as d GROUP BY d.StratejikPlanHedefGostergeKod
                              ) AS [s4] ON [s4].Kod = [s2].Kod
                    LEFT JOIN (
							    SELECT b.StratejikPlanHedefKod
                                ,STUFF((SELECT ',' + ViewKoordinatorluk.Adi FROM StratejikPlanIsbirligiBirim, ViewKoordinatorluk WHERE ViewKoordinatorluk.Kod = StratejikPlanIsbirligiBirim.KoordinatorlukKod AND  b.StratejikPlanHedefKod = StratejikPlanIsbirligiBirim.StratejikPlanHedefKod  FOR XML PATH('')), 1, 1, '') AS IsbirligiYapacakKoordinatorlukler
							    ,STUFF((SELECT ',' + ViewBirim.Adi FROM StratejikPlanIsbirligiBirim, ViewBirim WHERE ViewBirim.Kod = StratejikPlanIsbirligiBirim.BirimKod AND  b.StratejikPlanHedefKod = StratejikPlanIsbirligiBirim.StratejikPlanHedefKod FOR XML PATH('')), 1, 1, '') AS IsbirligiYapacakBirimler
							    FROM StratejikPlanIsbirligiBirim as b GROUP BY b.StratejikPlanHedefKod
					          ) AS [s6] ON [s1].Kod = [s6].StratejikPlanHedefKod

                    WHERE [s].[Durum] = 1
                    ";


            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " [v].Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " [s0].Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " [v].KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND [v0].[BirimKod] = " + kriter.BirimAdi;


            sql += "ORDER BY [v].Adi, [s].[Kod], [s0].[Kod], [s1].[Kod]";


            try
            {
                var kayitlar = await _unitOfWorkRaporUstYonetimStratejikPlanlama.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KoordinatorlerStratejikPlanlamaHazirlaAsync(KullaniciDto kullanan, RaporKoordinatorlerStratejikPlanlama kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    [v].[Adi] as KoordinatorlukAdi,
                    [v0].[Adi] as BirimAdi,
                    [s6].[IsbirligiYapacakKoordinatorlukler],
                    [s6].[IsbirligiYapacakBirimler],
                    TanimStratejikPlanDonem.BaslamaYil,
                    TanimStratejikPlanDonem.BitisYil,
                    [s0].[Amac] as Amac, 
                    [s1].[Adi] as Hedef, 
                    [s2].[Adi] as AnahtarRiskGostergesi,
                    [s2].GostergeNo,
                    [s2].[Etki] as HedefeEtkisi,
                    [s2].BaslangicTarihi,
                    [s2].BitisTarihi,
                    [s2].BaslangicDegeri,
                    [s4].SapmaOrani,
                    [s4].SapmaNedeni,
                    [s4].IzlemeDonemleri
                    FROM [ViewStratejikPlanIzleme] AS [s]
                    INNER JOIN [StratejikPlan] AS [s0] ON [s].[AmacKod] = [s0].[Kod]
                    INNER JOIN [StratejikPlanHedef] AS [s1] ON [s0].[Kod] = [s1].[StratejikPlanKod] AND [s].[HedefKod] = [s1].[Kod]
                    INNER JOIN [StratejikPlanHedefGosterge] AS [s2] ON [s1].[Kod] = [s2].[StratejikPlanHedefKod]
                    LEFT JOIN TanimStratejikPlanDonem ON TanimStratejikPlanDonem.Kod = [s0].StratejikPlanDonemKod 
                    LEFT JOIN [ViewKoordinatorluk] AS [v] ON [s1].[KoordinatorlukKod] = [v].[Kod]
                    LEFT JOIN [ViewBirim] AS [v0] ON [s1].[BirimKod] = [v0].[Kod]
                    LEFT JOIN (
                                SELECT d.StratejikPlanHedefGostergeKod as Kod, 
                                COUNT(*) ADET,
                                CONVERT(decimal(13,4), SUM(((CASE WHEN d.GerceklesenDegerYilSonu > 0 THEN d.GerceklesenDegerYilSonu ELSE d.GerceklesenDeger END)*100.00/NULLIF(d.PlanlananDeger,0)))/COUNT(*)) AS SapmaOrani,
                                STUFF((SELECT ',' + CONVERT(varchar, k.Donem) +';'+ CONVERT(varchar, k.PlanlananDeger) +';'+ CONVERT(varchar, k.GerceklesenDeger), +';'+ CONVERT(varchar, k.GerceklesenDegerYilSonu) FROM StratejikPlanIzlemeDonem as k WHERE k.StratejikPlanHedefGostergeKod = d.StratejikPlanHedefGostergeKod ORDER BY k.Donem FOR XML PATH('')), 1, 1, '') AS IzlemeDonemleri,
                                (SELECT d2.SapmaNedeni FROM StratejikPlanIzlemeDonem as d2 WHERE d2.Kod = (SELECT TOP 1 d3.Kod FROM StratejikPlanIzlemeDonem as d3 WHERE d3.StratejikPlanHedefGostergeKod = d.StratejikPlanHedefGostergeKod ORDER BY d3.Donem DESC)) AS SapmaNedeni
                                FROM  StratejikPlanIzlemeDonem as d GROUP BY d.StratejikPlanHedefGostergeKod
                              ) AS [s4] ON [s4].Kod = [s2].Kod
                    LEFT JOIN (
							    SELECT b.StratejikPlanHedefKod
                                ,STUFF((SELECT ',' + ViewKoordinatorluk.Adi FROM StratejikPlanIsbirligiBirim, ViewKoordinatorluk WHERE ViewKoordinatorluk.Kod = StratejikPlanIsbirligiBirim.KoordinatorlukKod AND  b.StratejikPlanHedefKod = StratejikPlanIsbirligiBirim.StratejikPlanHedefKod  FOR XML PATH('')), 1, 1, '') AS IsbirligiYapacakKoordinatorlukler
							    ,STUFF((SELECT ',' + ViewBirim.Adi FROM StratejikPlanIsbirligiBirim, ViewBirim WHERE ViewBirim.Kod = StratejikPlanIsbirligiBirim.BirimKod AND  b.StratejikPlanHedefKod = StratejikPlanIsbirligiBirim.StratejikPlanHedefKod FOR XML PATH('')), 1, 1, '') AS IsbirligiYapacakBirimler
							    FROM StratejikPlanIsbirligiBirim as b GROUP BY b.StratejikPlanHedefKod
					          ) AS [s6] ON [s1].Kod = [s6].StratejikPlanHedefKod

                    WHERE [s].[Durum] = 1
                    ";

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " [v].Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " [s0].Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " [v].KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND [v0].[BirimKod] = " + kriter.BirimAdi;




            sql += "ORDER BY [v].Adi, [s].[Kod], [s0].[Kod], [s1].[Kod]";


            try
            {
                var kayitlar = await _unitOfWorkRaporKoordinatorlerStratejikPlanlama.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SurecAltSurecHazirlaAsync(KullaniciDto kullanan, RaporSurecAltSurec kriter)
        {
            if (!Arac.YetkisiVarmi("ICDENETIMUZMANI", kullanan))
                YetkiKontrolundenGecir(kullanan, kriter);

            string sql = "";
            string kosul = "";

            sql = @"SELECT ViewKoordinatorluk.Adi AS KoordinatorlukAdi,
                    ViewBirim.Adi AS BirimAdi,
                    Surec.Numara AS SurecKod,
                    Surec.Adi AS SurecAdi,
                    AltSurec.Adi AS AltSurecAdi,
                    ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi  AS SurecSahibi,
					ViewUnvan.Adi AS SurecSahibiUnvan
                    FROM Surec
                    LEFT JOIN AltSurec ON AltSurec.SurecKod = Surec.Kod 
                    INNER JOIN ViewKoordinatorluk ON Surec.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON Surec.BirimKod = ViewBirim.Kod AND Surec.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON Surec.SurecSahibiPersonelKod = ViewPersonel.Kod
                    LEFT JOIN ViewUnvan ON Surec.SurecSahibiUnvanKod = ViewUnvan.Kod
                    WHERE Surec.Durum = 1 ";

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " Surec.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND Surec.BirimKod = " + kriter.BirimAdi;

            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporSurecAltSurec.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Gönderilen kullanıcı parametresini yetki kontrolünden geçirilerek kriter parametresini düzenleyen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        public void YetkiKontrolundenGecir(KullaniciDto kullanan, dynamic kriter)
        {
            if (Arac.YetkisiVarmi("PLANLAMAUNITESI", kullanan))
                return;

            //https://app.clickup.com/t/8694tkywt : İç Denetçi/İç Denetim Koordinatörü Raporlar ekranında her koordinatörlüğü dökebilmeli - sadece kendi risklerini değil
            if (Arac.YetkisiVarmi("UZMAN", kullanan))
            {
                if (!string.IsNullOrWhiteSpace(kullanan.KoordinatorlukKod))
                    kriter.KoordinatorlukAdi = kullanan.KoordinatorlukKod;
            }
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskIzlemeHazirlaAsync(KullaniciDto kullanan, RaporRiskIzleme kriter)
        {
            YetkiKontrolundenGecir(kullanan, kriter);

            bool yonetici = Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI", kullanan);

            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi, 
                    ViewBirim.Adi AS BirimAdi, 
                    TanimStratejikPlanDonem.BaslamaYil,
                    TanimStratejikPlanDonem.BitisYil,
                    StratejikPlan.Amac,
                    StratejikPlanHedef.Adi AS Hedef,
                    Surec.Adi AS SurecAdi, 
                    AltSurec.Adi AS AltSurecAdi, 
                    RiskEvreni.RiskNo, 
                    RiskEvreni.RiskAdi, 
                    RiskEvreni.RiskTanimi, 
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    STUFF((SELECT ';' + TanimIlIrtibatOfisi.Adi FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IlIrtibatOfisiAdi,
                    RiskEvreni.RiskBaslangicTarihi,
                    RiskEvreni.RiskBitisTarihi,
                    STUFF((SELECT ';' + OlayRaporlama.OlayTanimi FROM RiskEvreniOlayRaporlama,OlayRaporlama  WHERE RiskEvreniOlayRaporlama.OlayRaporlamaKod =OlayRaporlama.Kod and RiskEvreniOlayRaporlama.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS IliskiliOlaylarAdi,
                    (CASE WHEN AnahtarRiskGostergesi.Adi IS NULL THEN RiskEvreni.AnahtarRiskGostergesiAdi ELSE AnahtarRiskGostergesi.Adi END) AS AnahtarRiskGostergesi,
                    RiskYonetimi.Etki,
					RiskYonetimi.Olasilik,
                    RiskYonetimi.KontrolEtkinlikAgirligi AS KontrolKriteriAgirligi,
                    Konfigurasyon.EtkiOlasilikMatrisi,
                    STUFF((SELECT ';' + RiskYonetimiKontrol.Tanim FROM RiskYonetimi,RiskYonetimiKontrol   WHERE RiskYonetimi.Kod =RiskYonetimiKontrol.RiskYonetimiKod and RiskYonetimi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS KontrolAdi,
                    RiskYonetimi.ArtikRiskSeviyesi,
                    RiskYonetimi.RiskeVerilecekCevap,
                    RiskAzaltmaPlani.AzaltmaPlani,

                    AnahtarRiskGostergesi.YesilDeger,
                    AnahtarRiskGostergesi.KirmiziDeger,
                    STUFF((SELECT ',' + CONVERT(varchar, k.Donem) +';'+ CONVERT(varchar, k.GerceklesenDeger) +';'+ CONVERT(varchar, k.Periyot) FROM AnahtarRiskGostergesiDonem as k WHERE k.AnahtarRiskGostergesiKod = AnahtarRiskGostergesi.Kod ORDER BY k.Donem FOR XML PATH('')), 1, 1, '') AS IzlemeDonemleri

                    FROM RiskEvreni
                    INNER JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN RiskYonetimi ON RiskYonetimi.RiskEvreniKod = RiskEvreni.Kod 
                    LEFT JOIN RiskAzaltmaPlani  ON RiskAzaltmaPlani.RiskYonetimiKod = RiskYonetimi.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
                    LEFT JOIN StratejikPlan ON RiskEvreni.AmacKod = StratejikPlan.Kod 
                    LEFT JOIN StratejikPlanHedef ON RiskEvreni.HedefKod = StratejikPlanHedef.Kod 
                    LEFT JOIN TanimStratejikPlanDonem ON TanimStratejikPlanDonem.Kod = StratejikPlan.StratejikPlanDonemKod 
                    LEFT JOIN Surec ON RiskEvreni.SurecKod = Surec.Kod 
                    LEFT JOIN AltSurec ON RiskEvreni.AltSurecKod = AltSurec.Kod
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskAzaltmaPlani.AzaltmaPlaniSorumlusuKod
					LEFT JOIN AnahtarRiskGostergesi ON RiskEvreni.Kod = AnahtarRiskGostergesi.RiskEvreniKod
					LEFT JOIN Konfigurasyon ON Konfigurasyon.Durum = 10
                     ";

            if (kriter.DurumSorgu == null || kriter.DurumSorgu == 0)
                sql += " WHERE RiskEvreni.Durum IN(1,2,3,10,98) ";
            else
                sql += " WHERE RiskEvreni.Durum = " + kriter.DurumSorgu;

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " RiskEvreni.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }
            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND RiskEvreni.BirimKod = " + kriter.BirimAdi;


            //////Birim koşulunu uygula
            ////****************************************************************************************
            //string[] birimKosul = await OrtakService.ListeleBirimKosulAsync(kullanan, _serviceCTE, "RISKYONETIMI", "", "");

            //////Koordinatörlük
            //var koordinatorlukKosul = birimKosul[0].Split(",");//Genel koord birden fazla koordinatörlüğe sahip olduğu için
            //if (!string.IsNullOrWhiteSpace(koordinatorlukKosul[0]))
            //{
            //    var ekKosul = "";
            //    foreach (var item in koordinatorlukKosul)
            //    {
            //        if (ekKosul != "")
            //            ekKosul += " OR ";
            //        ekKosul += " ViewKoordinatorluk.Kod = '" + item + "'";
            //    }

            //    if (ekKosul != "")
            //        kosul = " (" + ekKosul + ") ";
            //}

            ////Birim
            //if (!string.IsNullOrWhiteSpace(birimKosul[1]))
            //{
            //    var ekKosul = " (RiskEvreni.BirimKod = '" + birimKosul[1] + "')";

            //    if (kosul != "")
            //        kosul = "(" + kosul + " AND " + ekKosul + ")";
            //    else
            //        kosul = ekKosul;
            //}

            ////Sadece Kendi kayıtları
            //if (kosul != "")
            //    kosul += " OR ";
            //kosul += " (RiskEvreni.RiskSahibiKod = '" + kullanan.PersonelKod + "')";


            //sql += " AND (" + kosul + ") ";




            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }



            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporRiskIzleme.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> AnahtarRiskGostergesiHazirlaAsync(KullaniciDto kullanan, RaporAnahtarRiskGostergesi kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi,
                    ViewBirim.Adi AS BirimAdi,
                    Surec.Adi Surec,
                    AltSurec.Adi AltSurec,
                    RiskEvreni.RiskAdi,
                    RiskEvreni.RiskTanimi,
                    STUFF((SELECT ';' + TanimRiskKategori.Adi FROM TanimRiskKategori,RiskEvreniRiskKategori WHERE TanimRiskKategori.Kod=RiskEvreniRiskKategori.RiskKategoriKod and RiskEvreniRiskKategori.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') AS RiskKategorisiAdi,
                    AnahtarRiskGostergesi.Adi AnahtarRiskGostergesi,
                    AnahtarRiskGostergesi.VeriDayanagi,
                    AnahtarRiskGostergesi.YesilDeger,
                    0.0 SariDeger,
                    AnahtarRiskGostergesi.KirmiziDeger,
                    AnahtarRiskGostergesi.HedefDeger,
                    STUFF((SELECT ',' + CONVERT(varchar, k.Donem) +';'+ CONVERT(varchar, k.GerceklesenDeger) +';'+ CONVERT(varchar, k.Periyot) FROM AnahtarRiskGostergesiDonem as k WHERE k.AnahtarRiskGostergesiKod = AnahtarRiskGostergesi.Kod ORDER BY k.Donem FOR XML PATH('')), 1, 1, '') AS IzlemeDonemleri

                    FROM AnahtarRiskGostergesi

                    INNER JOIN RiskEvreni ON AnahtarRiskGostergesi.RiskEvreniKod = RiskEvreni.Kod 
                    LEFT JOIN Surec ON Surec.Kod = RiskEvreni.SurecKod 
                    LEFT JOIN AltSurec ON AltSurec.SurecKod = RiskEvreni.AltSurecKod
                    LEFT JOIN ViewKoordinatorluk ON RiskEvreni.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON RiskEvreni.BirimKod = ViewBirim.Kod AND RiskEvreni.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 

                    WHERE RiskEvreni.Durum = 10
                    ";

            //if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi))
            //{
            //    //42 il koordinatörlüğü
            //    if (kriter.KoordinatorlukAdi == "-42")
            //        sql += " AND [s0].[Tur] = 40";
            //    else
            //        sql += " AND [s0].[KoordinatorlukKod] = " + kriter.KoordinatorlukAdi;
            //}
            //if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
            //    sql += " AND [s0].[BirimKod] = " + kriter.BirimAdi;

            //sql += "ORDER BY [s].[Kod], [s0].[Kod], [s1].[Kod]";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(RiskEvreni.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.lIlIrtibatOfisiKod) && kriter.lIlIrtibatOfisiKod != "-1")
            {
                var sqlOfisler = "";
                var ofisler = kriter.lIlIrtibatOfisiKod.Split(",");

                foreach (var o in ofisler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlOfisler))
                        sqlOfisler += " OR ";

                    sqlOfisler += " STUFF((SELECT ';' + TanimIlIrtibatOfisi.Kod FROM TanimIlIrtibatOfisi,RiskEvreniIlIrtibatOfisi WHERE TanimIlIrtibatOfisi.Kod=RiskEvreniIlIrtibatOfisi.IlIrtibatOfisiKod and RiskEvreniIlIrtibatOfisi.RiskEvreniKod=RiskEvreni.Kod        FOR XML PATH('')), 1, 1, '') LIKE '%" + Arac.TirnakYoket(o) + "%'";
                }

                if (!string.IsNullOrWhiteSpace(sqlOfisler))
                    sql += " AND (" + sqlOfisler + ")";
            }



            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporAnahtarRiskGostergesi.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> OlayRaporlamaHazirlaAsync(KullaniciDto kullanan, RaporOlay kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi,
                    ViewBirim.Adi AS BirimAdi,
					OlayRaporlama.OlayTarihi,
					OlayRaporlama.OlayYeri,
					OlayRaporlama.OlayTanimi,
					TanimOlayKategori.Adi OlayKategorisi,
					OlayRaporlama.Tutari,

                    STUFF((SELECT ';' + 
					RiskEvreni.RiskAdi + '|' + ISNULL(CASE WHEN AnahtarRiskGostergesi.Adi IS NULL THEN RiskEvreni.AnahtarRiskGostergesiAdi ELSE AnahtarRiskGostergesi.Adi END,'')
					FROM OlayRaporlamaRisk 
					INNER JOIN RiskEvreni ON RiskEvreni.Kod = OlayRaporlamaRisk.RiskEvreniKod 
					LEFT JOIN AnahtarRiskGostergesi ON AnahtarRiskGostergesi.RiskEvreniKod = RiskEvreni.Kod
					WHERE OlayRaporlamaKod = OlayRaporlama.Kod
					FOR XML PATH('')), 1, 1, '')  AS Riskler

                    FROM OlayRaporlama

                    LEFT JOIN ViewKoordinatorluk ON OlayRaporlama.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewBirim ON OlayRaporlama.BirimKod = ViewBirim.Kod AND OlayRaporlama.KoordinatorlukKod = ViewBirim.KoordinatorlukKod 
					LEFT JOIN TanimOlayKategori ON TanimOlayKategori.Kod = OlayRaporlama.OlayKategoriKod

                    WHERE OlayRaporlama.Durum = 10
                    ";

            if (kriter.KriterYil > 0)
                sql += " AND YEAR(OlayRaporlama.KayitTarihi) <= " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " ViewKoordinatorluk.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }

            if (!string.IsNullOrWhiteSpace(kriter.BirimAdi))
                sql += " AND ViewBirim.Kod = " + kriter.BirimAdi;

            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporOlay.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> RiskYonetimiBeyannamesiHazirlaAsync(KullaniciDto kullanan, RaporRiskYonetimiBeyannamesi kriter)
        {
            string sql = "";
            string kosul = "";

            sql = @"SELECT 
                    ViewKoordinatorluk.Adi AS KoordinatorlukAdi,
                    '' AS BirimAdi,
					RiskYonetimiBeyannamesi.Yil,
					RiskYonetimiBeyannamesi.IslemTarihi,
					ViewPersonel.Adi + ' ' + ViewPersonel.Soyadi AS IslemYapanAdi,
					RiskYonetimiBeyannamesi.IslemYapanRol

                    FROM RiskYonetimiBeyannamesi

                    INNER JOIN ViewKoordinatorluk ON RiskYonetimiBeyannamesi.KoordinatorlukKod = ViewKoordinatorluk.Kod 
                    LEFT JOIN ViewPersonelAdSoyad AS ViewPersonel ON ViewPersonel.Kod = RiskYonetimiBeyannamesi.IslemYapanKod

                    WHERE RiskYonetimiBeyannamesi.Durum = 10
                    ";

            if (kriter.KriterYil > 0)
                sql += " AND RiskYonetimiBeyannamesi.Yil = " + kriter.KriterYil;

            if (!string.IsNullOrWhiteSpace(kriter.KoordinatorlukAdi) && kriter.KoordinatorlukAdi != "-1")
            {
                var sqlKoordiler = "";
                var koordiler = kriter.KoordinatorlukAdi.Split(",");

                foreach (var k in koordiler)
                {
                    if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                        sqlKoordiler += " OR ";

                    if (k == "-42")
                        sqlKoordiler += " ViewKoordinatorluk.Tur = 40"; //42 il koordinatörlüğü
                    //else if (k == "-1")
                    //    sqlKoordiler += " ViewKoordinatorluk.Tur < 40"; //-1 Tüm kurum
                    else
                        sqlKoordiler += " ViewKoordinatorluk.KoordinatorlukKod = '" + Arac.TirnakYoket(k) + "'";
                }

                if (!string.IsNullOrWhiteSpace(sqlKoordiler))
                    sql += " AND (" + sqlKoordiler + ")";
            }

            sql += " ORDER BY ViewKoordinatorluk.Adi";

            try
            {
                var kayitlar = await _unitOfWorkRaporRiskYonetimiBeyannamesi.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);

        }
    }
}
