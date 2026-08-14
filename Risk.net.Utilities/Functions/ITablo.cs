using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk.net.Utilities.Functions
{
    public enum LineStyle
    {
        DOUBLE,
        NONE,
        THIN,
        MEDIUM,
        HAIR,
        MEDIUM_DASHED
    }

    public enum TabloRenk
    {
        BLACK,
        GRAY_25,
        RED,
        ROSE,
        LIGHT_TURQUOISE2,
        ICE_BLUE,
        CORAL,
        VERY_LIGHT_YELLOW,
        PALE_BLUE,
        IVORY,
        YELLOW2,
        AQUA,
        ORANGE,
        PERIWINKLE,
        PINK2,
        ALICEBLUE,
        BLUE,
        GREEN,
        LAVENDER,
        DARKBLUE,
        DARKGREEN,
        DARKORANGE,
        DARKRED,
        DARKSALMON,
        DEEPPINK,
        DEEPSKYBLUE,
        DODGERBLUE,
        FIREBRICK,
        FORESTGREEN,
        GOLD,
        OLIVE,
        ORANGERED,
        WHITE
    }

    public enum SayfaYonu
    {
        YATAY, DUSEY
    }
    public interface ITablo
    {
        //tur=1 TIFF, 0/Diğer=JPEG
        //void SheetToResim(int horRes, int verRes, string dosyaAd, int tur);
        //void YeniSheetEkle(string dosyaYol, string dosyaAd, int index);

        int SheetSayisi();

        string SonucDosyaAd();

        void SatirAc(int sheetNo, int satir, int acilacakSatirSayisi);

        void SatirAc(int satir, int acilacakSatirSayisi);

        void SutunAc(int sheetNo, int sutun, int acilacakSutunSayisi);

        void SutunAc(int sutun, int acilacakSutunSayisi);

        void HucreAdBulYaz(string hucreAd, object deger);

        void HucreAdBulYaz(string hucreAd, double deger, string paraIsareti, int kurusHane = 2);

        void HucreDegerHtmlYaz(int sheetNo, int satir, int sutun, string HtmlString);

        void HucreDegerYaz(int satir, int sutun, string deger);

        void HucreDegerYaz(int satir, int sutun, double deger);

        void HucreDegerYaz(int satir, int sutun, decimal deger);

        void HucreDegerYaz(int satir, int sutun, int deger);

        void HucreDegerYaz(int satir, int sutun, DateTime deger);

        void HucreDegerYaz(int satir, int sutun, double deger, string paraIsareti, int kurusHane = 2);

        void HucreAdDegerYaz(string hucreAd, object deger);

        double HucreDegerAlDbl(int sheetNo, int satir, int sutun);

        double HucreDegerAlDbl(int satir, int sutun);

        string HucreDegerAl(int sheetNo, int satir, int sutun, int noktaSay);

        string HucreDegerAl(int satir, int sutun);

        string HucreAdDegerAl(string hucreAd);

        string HucreFormulAl(int satir, int sutun);

        void HucreFormulYaz(int satir, int sutun, string formul);

        void KorumayaAl();

        void KorumayaAl(string sifre);

        void HucreAdAdresCoz(string hucreAd, ref int satir, ref int sutun);

        void SatirKopyala(int kaynakSatir, int hedefSatir, int satirSayisi);

        void HucreKopyala(int kaynakSheet, int satir1, int sutun1, int satir2, int sutun2, int hedefSheet, int hedefSatir, int hedefSutun);
        void HucreKopyala(int satir1, int sutun1, int satir2, int sutun2, int hedefSatir, int hedefSutun);

        void HucreBirlestir(int satir1, int sutun1, int satir2, int sutun2);

        void HucreBirlestirme(int satir1, int sutun1, int satir2, int sutun2);

        void HucreBirlestirme(int satir, int sutun);

        void SutunGizle(int sutun1, int sutun2, bool gizle);

        void SatirGizle(int satir1, int satir2, bool gizle);

        void SatirSil(int satir1, int satir2);

        void SutunSil(int sutun1, int sutun2);

        void DuseyCizgiCiz(int satir1, int satir2, int sutun, LineStyle stil, TabloRenk renk, bool solMu);

        void DuseyCizgiCiz(int satir1, int satir2, int sutun, LineStyle stil, TabloRenk renk);

        void YatayCizgiCiz(int satir, int sutun1, int sutun2, LineStyle stil, TabloRenk renk, bool ustMu);

        void YatayCizgiCiz(int satir, int sutun1, int sutun2, LineStyle stil, TabloRenk renk);

        void CerceveCizgiCiz(int satir1, int satir2, int sutun1, int sutun2, LineStyle stil, TabloRenk renk);

        void CerceveCiz(int satir1, int satir2, int sutun1, int sutun2, LineStyle stil, TabloRenk renk);

        void HucreMetniKaydir(int satir1, int sutun1, int satir2, int sutun2, bool deger);

        void HucreMetniKaydir(int satir, int sutun, bool deger);

        void KoyuYap(int satir1, int sutun1, int satir2, int sutun2, bool deger);

        void KoyuYap(int satir, int sutun, bool deger);

        void YaziTipiAta(int satir1, int sutun1, int satir2, int sutun2, string fontAd);

        void YaziTipBuyuklugu(int satir, int sutun, int deger);

        void YaziTipBuyuklugu(int satir1, int sutun1, int satir2, int sutun2, int deger);

        void DuseyHizala(int satir1, int sutun1, int satir2, int sutun2, int deger);

        void DuseyHizala(int satir, int sutun, int deger);

        void YatayHizala(int satir1, int sutun1, int satir2, int sutun2, int deger);

        void YatayHizala(int satir, int sutun, int deger);

        void ArkaPlanRenk(int satir1, int sutun1, int satir2, int sutun2, System.Drawing.Color renk);

        void ArkaPlanRenk(int satir, int sutun, System.Drawing.Color renk);

        void ArkaPlanRenk(int satir1, int sutun1, int satir2, int sutun2, TabloRenk renk);

        void ArkaPlanRenk(int satir, int sutun, TabloRenk renk);

        void YaziRenk(int satir1, int sutun1, int satir2, int sutun2, TabloRenk renk);

        void YaziRenk(int satir, int sutun, TabloRenk renk);

        void YaziRenk(int satir1, int sutun1, int satir2, int sutun2, System.Drawing.Color renk);

        void YaziRenk(int satir, int sutun, System.Drawing.Color renk);

        void CokluSatirdaYaz(int satir, int sutun, bool deger);

        void AktifSheetDegistir(int sheetNo);

        int AktifSheet();

        int YeniSheetEkle();

        int YeniSheetEkle(int kaynakSheetNo);

        void SheetSil(int sheetNo);

        void SheetAdiVer(int sheetNo, string adi);

        string SheetAdiAl();
        string SheetAdiAl(int sheetNo);

        void IlkSayfaNumarasi(int sayfaNo);

        void SelectSheet(int sheetNo);

        void SelectSheet();

        void ResimEkle(double x, double y, double width, double height, string dosya);

        void ResimEkle(double x, double y, double width, double height, Stream stream);

        object ResimEkle(double x, double y, double width, double height, Stream stream, double left, double top, int enBoyOran, int yanasiklik);

        void DosyaAc(string dosyaAd, string sonucDosya);

        void DosyaAc(string dosyaAd);

        void BosDosyaAc(string sonucDosya);

        void DosyaOkuAc(string dosyaAd);

        void DosyaSaklaTamYol();

        void DosyaKapat();

        string UzantiBul();

        void DosyaSaklamaFormatAta(string uzanti);

        void CalculateFormula();

        void HucreFormatla(int satir1, int sutun1, int satir2, int sutun2, string deger);

        void HucreMetniSigdir(int satir1, int sutun1, int satir2, int sutun2, bool deger);

        void HucreMetniSigdir(int satir, int sutun, bool deger);

        void FormulleriSil();
    }
}
