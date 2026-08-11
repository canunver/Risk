using Microsoft.Extensions.DependencyInjection;
using Risk.net.Data.DataContext;
using Risk.net.Data.Entities;
using Risk.net.Data.Functions;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Functions;
using Risk.net.Services.Interfaces;

namespace Risk.net.Services.Extensions
{
    public static class ServiceExtensions
    {
        /// 
        /// <summary>
        /// Yazýlýmda yer alan Entity ve Servicelerin gelen her bir web requesti için bir instance oluþturmasý ve gelen her ayný requestte ayný instance'ý kullanýlmasýný, farklý web requestler içinde yeni bir instance oluþturmasýný saðlamak için kullanýlan metod
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static IServiceCollection LoadMyServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<DatabaseContext>();

            serviceCollection.AddScoped<IUnitOfWork<Yardim>, UnitOfWork<Yardim>>();
            serviceCollection.AddScoped<IUnitOfWork<Grafik>, UnitOfWork<Grafik>>();
            serviceCollection.AddScoped<IUnitOfWork<GrafikIzlemeMatrisi>, UnitOfWork<GrafikIzlemeMatrisi>>();
            serviceCollection.AddScoped<IUnitOfWork<GrafikRiskYonetimi>, UnitOfWork<GrafikRiskYonetimi>>();
            serviceCollection.AddScoped<IUnitOfWork<Konfigurasyon>, UnitOfWork<Konfigurasyon>>();
            serviceCollection.AddScoped<IUnitOfWork<AnahtarRiskGostergesi>, UnitOfWork<AnahtarRiskGostergesi>>();
            serviceCollection.AddScoped<IUnitOfWork<AnahtarRiskGostergesiDonem>, UnitOfWork<AnahtarRiskGostergesiDonem>>();
            serviceCollection.AddScoped<IUnitOfWork<Surec>, UnitOfWork<Surec>>();
            serviceCollection.AddScoped<IUnitOfWork<AltSurec>, UnitOfWork<AltSurec>>();
            serviceCollection.AddScoped<IUnitOfWork<OlayRaporlama>, UnitOfWork<OlayRaporlama>>();
            serviceCollection.AddScoped<IUnitOfWork<OlayRaporlamaRisk>, UnitOfWork<OlayRaporlamaRisk>>();
            serviceCollection.AddScoped<IUnitOfWork<OlayRaporlamaKategori>, UnitOfWork<OlayRaporlamaKategori>>();
            serviceCollection.AddScoped<IUnitOfWork<BulguYonetimi>, UnitOfWork<BulguYonetimi>>();
            serviceCollection.AddScoped<IUnitOfWork<BulguYonetimiCevap>, UnitOfWork<BulguYonetimiCevap>>();
            serviceCollection.AddScoped<IUnitOfWork<BulguYonetimiBirim>, UnitOfWork<BulguYonetimiBirim>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewBulguYonetimi>, UnitOfWork<ViewBulguYonetimi>>();
            serviceCollection.AddScoped<IUnitOfWork<Denetim>, UnitOfWork<Denetim>>();
            serviceCollection.AddScoped<IUnitOfWork<DenetimBirim>, UnitOfWork<DenetimBirim>>();
            serviceCollection.AddScoped<IUnitOfWork<DenetimDenetci>, UnitOfWork<DenetimDenetci>>();
            serviceCollection.AddScoped<IUnitOfWork<DenetimGorevlendirme>, UnitOfWork<DenetimGorevlendirme>>();
            serviceCollection.AddScoped<IUnitOfWork<DenetimSorumlu>, UnitOfWork<DenetimSorumlu>>();
            serviceCollection.AddScoped<IUnitOfWork<Dosya>, UnitOfWork<Dosya>>();
            serviceCollection.AddScoped<IUnitOfWork<BilgiMerkezi>, UnitOfWork<BilgiMerkezi>>();
            serviceCollection.AddScoped<IUnitOfWork<TanimGenel>, UnitOfWork<TanimGenel>>();
            serviceCollection.AddScoped<IUnitOfWork<TanimRiskKategori>, UnitOfWork<TanimRiskKategori>>();
            serviceCollection.AddScoped<IUnitOfWork<TanimIlIrtibatOfisi>, UnitOfWork<TanimIlIrtibatOfisi>>();
            serviceCollection.AddScoped<IUnitOfWork<TanimOlayKategori>, UnitOfWork<TanimOlayKategori>>();
            serviceCollection.AddScoped<IUnitOfWork<TanimStratejikPlanDonem>, UnitOfWork<TanimStratejikPlanDonem>>();
            serviceCollection.AddScoped<IUnitOfWork<StratejikPlan>, UnitOfWork<StratejikPlan>>();
            serviceCollection.AddScoped<IUnitOfWork<StratejikPlanHedef>, UnitOfWork<StratejikPlanHedef>>();
            serviceCollection.AddScoped<IUnitOfWork<StratejikPlanHedefGosterge>, UnitOfWork<StratejikPlanHedefGosterge>>();
            serviceCollection.AddScoped<IUnitOfWork<StratejikPlanIsbirligiBirim>, UnitOfWork<StratejikPlanIsbirligiBirim>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewStratejikPlanIzleme>, UnitOfWork<ViewStratejikPlanIzleme>>();
            serviceCollection.AddScoped<IUnitOfWork<StratejikPlanIzlemeDonem>, UnitOfWork<StratejikPlanIzlemeDonem>>();
            serviceCollection.AddScoped<IUnitOfWork<StratejikPlanEylemPlani>, UnitOfWork<StratejikPlanEylemPlani>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskEvreni>, UnitOfWork<RiskEvreni>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskEvreniRiskKategori>, UnitOfWork<RiskEvreniRiskKategori>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskEvreniIlIrtibatOfisi>, UnitOfWork<RiskEvreniIlIrtibatOfisi>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskEvreniOlayKategori>, UnitOfWork<RiskEvreniOlayKategori>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskEvreniOlayRaporlama>, UnitOfWork<RiskEvreniOlayRaporlama>>();
            serviceCollection.AddScoped<IUnitOfWork<Tarihce>, UnitOfWork<Tarihce>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskYonetimi>, UnitOfWork<RiskYonetimi>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskYonetimiKontrol>, UnitOfWork<RiskYonetimiKontrol>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskAzaltmaPlani>, UnitOfWork<RiskAzaltmaPlani>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskAzaltmaPlaniIliski>, UnitOfWork<RiskAzaltmaPlaniIliski>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskAzaltmaPlaniIsbirligiBirim>, UnitOfWork<RiskAzaltmaPlaniIsbirligiBirim>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskAzaltmaPlaniRisk>, UnitOfWork<RiskAzaltmaPlaniRisk>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskAzaltmaPlaniNot>, UnitOfWork<RiskAzaltmaPlaniNot>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporUstYonetimRisk>, UnitOfWork<RaporUstYonetimRisk>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporKoordinatorlerRisk>, UnitOfWork<RaporKoordinatorlerRisk>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporRiskSekretaryasiRisk>, UnitOfWork<RaporRiskSekretaryasiRisk>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporRiskSahipleriRisk>, UnitOfWork<RaporRiskSahipleriRisk>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporTrend>, UnitOfWork<RaporTrend>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporIcKontrolZayifliklari>, UnitOfWork<RaporIcKontrolZayifliklari>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporYillikRiskPlani>, UnitOfWork<RaporYillikRiskPlani>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporYariYilRiskAzaltmaPlani>, UnitOfWork<RaporYariYilRiskAzaltmaPlani>>();
            serviceCollection.AddScoped<IUnitOfWork<BildirimSistemi>, UnitOfWork<BildirimSistemi>>();
            serviceCollection.AddScoped<IUnitOfWork<CTEKoordinatorluk>, UnitOfWork<CTEKoordinatorluk>>();
            serviceCollection.AddScoped<IUnitOfWork<PersonelAktifRol>, UnitOfWork<PersonelAktifRol>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporUstYonetimStratejikPlanlama>, UnitOfWork<RaporUstYonetimStratejikPlanlama>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporKoordinatorlerStratejikPlanlama>, UnitOfWork<RaporKoordinatorlerStratejikPlanlama>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporSurecAltSurec>, UnitOfWork<RaporSurecAltSurec>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporRiskIzleme>, UnitOfWork<RaporRiskIzleme>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporAnahtarRiskGostergesi>, UnitOfWork<RaporAnahtarRiskGostergesi>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporOlay>, UnitOfWork<RaporOlay>>();
            serviceCollection.AddScoped<IUnitOfWork<RaporRiskYonetimiBeyannamesi>, UnitOfWork<RaporRiskYonetimiBeyannamesi>>();

            serviceCollection.AddScoped<IUnitOfWork<MailTarihce>, UnitOfWork<MailTarihce>>();
            serviceCollection.AddScoped<IUnitOfWork<RiskYonetimiBeyannamesi>, UnitOfWork<RiskYonetimiBeyannamesi>>();

            serviceCollection.AddScoped<IUnitOfWork<ViewUnvan>, UnitOfWork<ViewUnvan>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewPersonel>, UnitOfWork<ViewPersonel>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewPersonelResim>, UnitOfWork<ViewPersonelResim>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewYetki>, UnitOfWork<ViewYetki>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewKoordinatorluk>, UnitOfWork<ViewKoordinatorluk>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewBirim>, UnitOfWork<ViewBirim>>();
            serviceCollection.AddScoped<IUnitOfWork<ViewBildirimSistemi>, UnitOfWork<ViewBildirimSistemi>>();

            serviceCollection.AddScoped<IYardimService, YardimService>();
            serviceCollection.AddScoped<IGrafikService, GrafikService>();
            serviceCollection.AddScoped<IKonfigurasyonService, KonfigurasyonService>();
            serviceCollection.AddScoped<IAnahtarRiskGostergesiService, AnahtarRiskGostergesiService>();
            serviceCollection.AddScoped<IAnahtarRiskGostergesiDonemService, AnahtarRiskGostergesiDonemService>();
            serviceCollection.AddScoped<ISurecService, SurecService>();
            serviceCollection.AddScoped<IAltSurecService, AltSurecService>();
            serviceCollection.AddScoped<IOlayRaporlamaService, OlayRaporlamaService>();
            serviceCollection.AddScoped<IOlayRaporlamaRiskService, OlayRaporlamaRiskService>();
            serviceCollection.AddScoped<IOlayRaporlamaOlayKategoriService, OlayRaporlamaOlayKategoriService>();
            serviceCollection.AddScoped<IBulguYonetimiService, BulguYonetimiService>();
            serviceCollection.AddScoped<IBulguYonetimiBirimService, BulguYonetimiBirimService>();
            serviceCollection.AddScoped<IBulguYonetimiCevapService, BulguYonetimiCevapService>();
            serviceCollection.AddScoped<IDenetimService, DenetimService>();
            serviceCollection.AddScoped<IDenetimBirimService, DenetimBirimService>();
            serviceCollection.AddScoped<IDenetimDenetciService, DenetimDenetciService>();
            serviceCollection.AddScoped<IDenetimGorevlendirmeService, DenetimGorevlendirmeService>();
            serviceCollection.AddScoped<IDenetimSorumluService, DenetimSorumluService>();
            serviceCollection.AddScoped<ITanimGenelService, TanimGenelService>();
            serviceCollection.AddScoped<IDosyaService, DosyaService>();
            serviceCollection.AddScoped<IBilgiMerkeziService, BilgiMerkeziService>();
            serviceCollection.AddScoped<ITanimRiskKategoriService, TanimRiskKategoriService>();
            serviceCollection.AddScoped<ITanimIlIrtibatOfisiService, TanimIlIrtibatOfisiService>();
            serviceCollection.AddScoped<ITanimOlayKategoriService, TanimOlayKategoriService>();
            serviceCollection.AddScoped<ITanimStratejikPlanDonemService, TanimStratejikPlanDonemService>();
            serviceCollection.AddScoped<IStratejikPlanService, StratejikPlanService>();
            serviceCollection.AddScoped<IStratejikPlanHedefService, StratejikPlanHedefService>();
            serviceCollection.AddScoped<IStratejikPlanHedefGostergeService, StratejikPlanHedefGostergeService>();
            serviceCollection.AddScoped<IStratejikPlanIsbirligiBirimService, StratejikPlanIsbirligiBirimService>();
            serviceCollection.AddScoped<IStratejikPlanIzlemeService, StratejikPlanIzlemeService>();
            serviceCollection.AddScoped<IStratejikPlanIzlemeDonemService, StratejikPlanIzlemeDonemService>();
            serviceCollection.AddScoped<IStratejikPlanEylemPlaniService, StratejikPlanEylemPlaniService>();
            serviceCollection.AddScoped<IRiskEvreniService, RiskEvreniService>();
            serviceCollection.AddScoped<IRiskYonetimiKontrolService, RiskYonetimiKontrolService>();
            serviceCollection.AddScoped<IRiskEvreniRiskKategoriService, RiskEvreniRiskKategoriService>();
            serviceCollection.AddScoped<IRiskEvreniIlIrtibatOfisiService, RiskEvreniIlIrtibatOfisiService>();
            serviceCollection.AddScoped<IRiskEvreniOlayKategoriService, RiskEvreniOlayKategoriService>();
            serviceCollection.AddScoped<IRiskEvreniOlayRaporlamaService, RiskEvreniOlayRaporlamaService>();
            serviceCollection.AddScoped<IRiskYonetimiService, RiskYonetimiService>();
            serviceCollection.AddScoped<IRiskYonetimiKontrolService, RiskYonetimiKontrolService>();
            serviceCollection.AddScoped<IRiskAzaltmaPlaniService, RiskAzaltmaPlaniService>();
            serviceCollection.AddScoped<IRiskAzaltmaPlaniIliskiService, RiskAzaltmaPlaniIliskiService>();
            serviceCollection.AddScoped<IRiskAzaltmaPlaniIsbirligiBirimService, RiskAzaltmaPlanIsbirligiBirimService>();
            serviceCollection.AddScoped<IRiskAzaltmaPlaniRiskService, RiskAzaltmaPlaniRiskService>();
            serviceCollection.AddScoped<IRiskAzaltmaPlaniNotService, RiskAzaltmaPlaniNotService>();
            serviceCollection.AddScoped<ITarihceService, TarihceService>();
            serviceCollection.AddScoped<IRaporlamaService, RaporlamaService>();
            serviceCollection.AddScoped<IBildirimSistemiService, BildirimSistemiService>();
            serviceCollection.AddScoped<ICTEKoordinatorlukService, CTEKoordinatorlukService>();
            serviceCollection.AddScoped<IPersonelAktifRolService, PersonelAktifRolService>();
            serviceCollection.AddScoped<IRiskSahibiDegistirService, RiskSahibiDegistirService>();
            serviceCollection.AddScoped<IMailTarihceService, MailTarihceService>();
            serviceCollection.AddScoped<IRiskYonetimiBeyannamesiService, RiskYonetimiBeyannamesiService>();

            serviceCollection.AddScoped<IViewUnvanService, ViewUnvanService>();
            serviceCollection.AddScoped<IViewPersonelService, ViewPersonelService>();
            serviceCollection.AddScoped<IViewYetkiService, ViewYetkiService>();
            serviceCollection.AddScoped<IViewKoordinatorlukService, ViewKoordinatorlukService>();
            serviceCollection.AddScoped<IViewBirimService, ViewBirimService>();
            serviceCollection.AddScoped<IViewBildirimSistemiService, ViewBildirimSistemiService>();

            return serviceCollection;
        }
    }
}
