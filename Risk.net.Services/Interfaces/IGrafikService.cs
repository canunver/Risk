using Risk.net.Data.Entities;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Threading.Tasks;

namespace Risk.net.Services.Interfaces
{
    public interface IGrafikService
    {
        Task<Sonuc> BulguDurumuHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> SurecHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        //Task<Sonuc> EylemDurumuHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> BulguOnemDuzeyiHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> YapisalRiskSeviyesiHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> ArtikRiskSeviyesiHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> RiskKategorileriHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> RiskPuaniHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        //Task<Sonuc> RiskIzlemeMatrisiHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> RiskYonetimiHazirlaAsync(KullaniciDto kullanan, GrafikRiskYonetimi kriter);

        Task<Sonuc> AnahtarRiskGostergesiHazirlaAsync(KullaniciDto kullanan, Grafik kriter);
        Task<Sonuc> RiskeVerilenYanitHazirlaAsync(KullaniciDto kullanan, Grafik kriter);

        Task<Sonuc> PerformansIzlemeHazirlaAsync(KullaniciDto kullanan, Grafik kriter);

        Task<Sonuc> KRIToleransHazirlaAsync(KullaniciDto kullanan, Grafik kriter);

        Task<Sonuc> KRIToleransPieHazirlaAsync(KullaniciDto kullanan, Grafik kriter);

        Task<Sonuc> KRIToleransPaneliHazirlaAsync(KullaniciDto kullanan, GrafikRiskYonetimi kriter);
    }
}
