using Microsoft.EntityFrameworkCore;
using Risk.net.Data.Entities;
using Risk.net.Utilities.Functions;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Risk.net.Data.DataContext
{
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// Veritabanı işlemlerinin yapıldığı DBContext sınıfının kurucuları değiştirilerek DbContextOptions türünden bağlantı ayarlarını almaktadır.
        /// </summary>
        /// <param name="options"></param>
        /// <returns>
        /// </returns>
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        /// <summary>
        /// Veritabanı bağlantı ayarları için kullanılan metodtur.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        /// <returns>
        /// </returns>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baglantiTipi = Arac.ConfigOku("BaglantiSatirlari:BaglantiTipi", "");
            string baglantiSatiri = Arac.ConfigOku("BaglantiSatirlari:Connection" + baglantiTipi, "");

            if (baglantiTipi == "SqlServer")
            {
                optionsBuilder.UseSqlServer(baglantiSatiri);
            }
            else
            {
                //optionsBuilder.UseMySql(baglantiSatiri, ServerVersion.AutoDetect(baglantiSatiri));
                //update-database ile mysql de table ları oluşturabilirsin
                //Mysql download indir kur (Server kurulacak)
                //DBeaver indir kur
                //database adı risk > schema risk oluştur
                //insert cümlesi insert into olacak [], 'N ve dbo olmayacak noktalı virgülle bitecek
                //ör => INSERT INTO TanimBirim (Kod, Adi, Durum) VALUES ('F780B1C783B3480394DF3D19C2C65DEC', 'İç Denetim Koordinatörlüğü', 1);


            }
        }

        #region DbSet Alanı
        public DbSet<Yardim> Yardim { get; set; }
        public DbSet<Konfigurasyon> Konfigurasyon { get; set; }
        public DbSet<BulguYonetimi> BulguYonetimi { get; set; }
        public DbSet<BulguYonetimiCevap> BulguYonetimiCevap { get; set; }
        public DbSet<BulguYonetimiBirim> BulguYonetimiBirim { get; set; }
        public DbSet<Denetim> Denetim { get; set; }
        public DbSet<DenetimBirim> DenetimBirim { get; set; }
        public DbSet<DenetimDenetci> DenetimDenetci { get; set; }
        public DbSet<DenetimGorevlendirme> DenetimGorevlendirme { get; set; }
        public DbSet<DenetimSorumlu> DenetimSorumlu { get; set; }
        public DbSet<OlayRaporlama> OlayRaporlama { get; set; }
        public DbSet<OlayRaporlamaRisk> OlayRaporlamaRisk { get; set; }
        public DbSet<OlayRaporlamaKategori> OlayRaporlamaKategori { get; set; }

        public DbSet<TanimGenel> TanimGenel { get; set; }
        public DbSet<Surec> Surec { get; set; }
        public DbSet<AltSurec> AltSurec { get; set; }
        public DbSet<TanimRiskKategori> TanimRiskKategori { get; set; }
        public DbSet<TanimIlIrtibatOfisi> TanimIlIrtibatOfisi { get; set; }
        public DbSet<TanimOlayKategori> TanimOlayKategori { get; set; }
        public DbSet<Dosya> Dosya { get; set; }
        public DbSet<BilgiMerkezi> BilgiMerkezi { get; set; }
        public DbSet<StratejikPlan> StratejikPlan { get; set; }
        public DbSet<TanimStratejikPlanDonem> TanimStratejikPlanDonem { get; set; }
        public DbSet<StratejikPlanIsbirligiBirim> StratejikPlanIsbirligiBirim { get; set; }
        public DbSet<StratejikPlanHedef> StratejikPlanHedef { get; set; }
        public DbSet<StratejikPlanHedefGosterge> StratejikPlanHedefGosterge { get; set; }
        public DbSet<ViewStratejikPlanIzleme> ViewStratejikPlanIzleme { get; set; }
        public DbSet<StratejikPlanIzlemeDonem> StratejikPlanIzlemeDonem { get; set; }
        public DbSet<StratejikPlanEylemPlani> StratejikPlanEylemPlani { get; set; }
        public DbSet<RiskEvreni> RiskEvreni { get; set; }
        public DbSet<RiskEvreniRiskKategori> RiskEvreniRiskKategori { get; set; }
        public DbSet<RiskEvreniIlIrtibatOfisi> RiskEvreniIlIrtibatOfisi { get; set; }
        public DbSet<RiskEvreniOlayKategori> RiskEvreniOlayKategori { get; set; }
        public DbSet<RiskEvreniOlayRaporlama> RiskEvreniOlayRaporlama { get; set; }
        public DbSet<AnahtarRiskGostergesi> AnahtarRiskGostergesi { get; set; }
        public DbSet<AnahtarRiskGostergesiDonem> AnahtarRiskGostergesiDonem { get; set; }
        public DbSet<RiskYonetimi> RiskYonetimi { get; set; }
        public DbSet<RiskYonetimiKontrol> RiskYonetimiKontrol { get; set; }
        public DbSet<RiskAzaltmaPlani> RiskAzaltmaPlani { get; set; }
        public DbSet<RiskAzaltmaPlaniIliski> RiskAzaltmaPlaniIliski { get; set; }
        public DbSet<RiskAzaltmaPlaniIsbirligiBirim> RiskAzaltmaPlaniIsbirligiBirim { get; set; }
        public DbSet<RiskAzaltmaPlaniRisk> RiskAzaltmaPlaniRisk { get; set; }
        public DbSet<RiskAzaltmaPlaniNot> RiskAzaltmaPlaniNot { get; set; }

        public DbSet<Tarihce> Tarihce { get; set; }
        public DbSet<BildirimSistemi> BildirimSistemi { get; set; }
        public DbSet<PersonelAktifRol> PersonelAktifRol { get; set; }

        public DbSet<ViewUnvan> ViewUnvan { get; set; }
        public DbSet<ViewPersonel> ViewPersonel { get; set; }
        public DbSet<ViewPersonelResim> ViewPersonelResim { get; set; }
        public DbSet<ViewYetki> ViewYetki { get; set; }
        public DbSet<ViewKoordinatorluk> ViewKoordinatorluk { get; set; }
        public DbSet<ViewBirim> ViewBirim { get; set; }
        public DbSet<ViewBildirimSistemi> ViewBildirimSistemi { get; set; }
        public DbSet<ViewBulguYonetimi> ViewBulguYonetimi { get; set; }

        public DbSet<Grafik> Grafik { get; set; }
        public DbSet<GrafikIzlemeMatrisi> GrafikIzlemeMatrisi { get; set; }
        public DbSet<GrafikRiskYonetimi> GrafikRiskYonetimi { get; set; }

        public DbSet<RaporUstYonetimRisk> RaporUstYonetimRisk { get; set; }
        public DbSet<RaporKoordinatorlerRisk> RaporKoordinatorlerRisk { get; set; }
        public DbSet<RaporRiskSekretaryasiRisk> RaporRiskSekretaryasiRisk { get; set; }
        public DbSet<RaporRiskSahipleriRisk> RaporRiskSahipleriRisk { get; set; }
        public DbSet<RaporTrend> RaporTrend { get; set; }
        public DbSet<RaporIcKontrolZayifliklari> RaporIcKontrolZayifliklari { get; set; }
        public DbSet<RaporYillikRiskPlani> RaporYillikRiskPlani { get; set; }
        public DbSet<RaporYariYilRiskAzaltmaPlani> RaporYariYilRiskAzaltmaPlani { get; set; }
        public DbSet<RaporUstYonetimStratejikPlanlama> RaporUstYonetimStratejikPlanlama { get; set; }
        public DbSet<RaporKoordinatorlerStratejikPlanlama> RaporKoordinatorlerStratejikPlanlama { get; set; }
        public DbSet<RaporSurecAltSurec> RaporSurecAltSurec { get; set; }
        public DbSet<RaporRiskIzleme> RaporRiskIzleme { get; set; }
        public DbSet<RaporAnahtarRiskGostergesi> RaporAnahtarRiskGostergesi { get; set; }
        public DbSet<RaporOlay> RaporOlay { get; set; }
        public DbSet<RaporRiskYonetimiBeyannamesi> RaporRiskYonetimiBeyannamesi { get; set; }

        public DbSet<MailTarihce> MailTarihce { get; set; }
        public DbSet<RiskYonetimiBeyannamesi> RiskYonetimiBeyannamesi { get; set; }

        #endregion

        /// <summary>
        /// Veritabanı ilk defa oluşturulurken tetiklenen bir virtual metodtur, veritabanı tabloları oluşturulmadan araya girer, tablo isimlerine müdahale edilebilir.
        /// View, Rapor Entity lerin veri tabanında fiziksel oluşturulması engellenir
        /// View, Rapor Entity lerin anahtar alanlarının olmadığı belirtilir
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <returns>
        /// </returns>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CTEKoordinatorluk>().ToView("CTEKoordinatorluk");
            modelBuilder.Entity<ViewPersonelResim>().ToView("ViewPersonelResim");
            modelBuilder.Entity<ViewPersonel>().ToView("ViewPersonel");
            modelBuilder.Entity<ViewUnvan>().ToView("ViewUnvan");
            modelBuilder.Entity<ViewKoordinatorluk>().ToView("ViewKoordinatorluk");
            modelBuilder.Entity<ViewBirim>().ToView("ViewBirim");
            modelBuilder.Entity<ViewYetki>().ToView("ViewYetki");
            modelBuilder.Entity<ViewBildirimSistemi>().ToView("ViewBildirimSistemi");
            modelBuilder.Entity<ViewBulguYonetimi>().ToView("ViewBulguYonetimi");
            modelBuilder.Entity<ViewBulguYonetimiCevapDurum>().ToView("ViewBulguYonetimiCevapDurum");
            modelBuilder.Entity<ViewStratejikPlanIzleme>().ToView("ViewStratejikPlanIzleme");
            modelBuilder.Entity<Grafik>().ToView("Grafik");
            modelBuilder.Entity<GrafikIzlemeMatrisi>().ToView("GrafikIzlemeMatrisi");
            modelBuilder.Entity<GrafikRiskYonetimi>().ToView("GrafikRiskYonetimi");
            modelBuilder.Entity<RaporUstYonetimRisk>().ToView("RaporUstYonetimRisk");
            modelBuilder.Entity<RaporKoordinatorlerRisk>().ToView("RaporKoordinatorlerRisk");
            modelBuilder.Entity<RaporRiskSekretaryasiRisk>().ToView("RaporRiskSekretaryasiRisk");
            modelBuilder.Entity<RaporRiskSahipleriRisk>().ToView("RaporRiskSahipleriRisk");
            modelBuilder.Entity<RaporTrend>().ToView("RaporTrend");
            modelBuilder.Entity<RaporIcKontrolZayifliklari>().ToView("RaporIcKontrolZayifliklari");
            modelBuilder.Entity<RaporYillikRiskPlani>().ToView("RaporYillikRiskPlani");
            modelBuilder.Entity<RaporYariYilRiskAzaltmaPlani>().ToView("RaporYariYilRiskAzaltmaPlani");
            modelBuilder.Entity<RaporUstYonetimStratejikPlanlama>().ToView("RaporUstYonetimStratejikPlanlama");
            modelBuilder.Entity<RaporKoordinatorlerStratejikPlanlama>().ToView("RaporKoordinatorlerStratejikPlanlama");
            modelBuilder.Entity<RaporSurecAltSurec>().ToView("RaporSurecAltSurec");
            modelBuilder.Entity<RaporRiskIzleme>().ToView("RaporRiskIzleme");
            modelBuilder.Entity<RaporAnahtarRiskGostergesi>().ToView("RaporAnahtarRiskGostergesi");
            modelBuilder.Entity<RaporOlay>().ToView("RaporOlay");
            modelBuilder.Entity<RaporRiskYonetimiBeyannamesi>().ToView("RaporRiskYonetimiBeyannamesi");

            modelBuilder.Entity<Grafik>().HasNoKey();
            modelBuilder.Entity<GrafikRiskYonetimi>().HasNoKey();
            modelBuilder.Entity<GrafikIzlemeMatrisi>().HasNoKey();
            //modelBuilder.Entity<ViewBulguYonetimiCevapDurum>().HasNoKey();
            modelBuilder.Entity<RaporUstYonetimRisk>().HasNoKey();
            modelBuilder.Entity<RaporKoordinatorlerRisk>().HasNoKey();
            modelBuilder.Entity<RaporRiskSekretaryasiRisk>().HasNoKey();
            modelBuilder.Entity<RaporRiskSahipleriRisk>().HasNoKey();
            modelBuilder.Entity<RaporTrend>().HasNoKey();
            modelBuilder.Entity<RaporIcKontrolZayifliklari>().HasNoKey();
            modelBuilder.Entity<RaporYillikRiskPlani>().HasNoKey();
            modelBuilder.Entity<RaporYariYilRiskAzaltmaPlani>().HasNoKey();
            modelBuilder.Entity<RaporUstYonetimStratejikPlanlama>().HasNoKey();
            modelBuilder.Entity<RaporKoordinatorlerStratejikPlanlama>().HasNoKey();
            modelBuilder.Entity<RaporSurecAltSurec>().HasNoKey();
            modelBuilder.Entity<RaporRiskIzleme>().HasNoKey();
            modelBuilder.Entity<RaporAnahtarRiskGostergesi>().HasNoKey();
            modelBuilder.Entity<RaporOlay>().HasNoKey();
            modelBuilder.Entity<RaporRiskYonetimiBeyannamesi>().HasNoKey();

            modelBuilder.Entity<CTEKoordinatorluk>().HasNoKey();

            modelBuilder.Entity<BildirimSistemi>().Property(f => f.Kod).ValueGeneratedOnAdd();

            modelBuilder.Entity<ViewBulguYonetimiCevapDurum>().HasKey(s => new { s.BulguYonetimiKod, s.Tur });

            modelBuilder.Entity<BilgiMerkezi>()
                    .HasOne(d => d.Dosya)
                    .WithOne()
                    .HasForeignKey<Dosya>(d=>d.BaglantiKod);
        }

    }
}
