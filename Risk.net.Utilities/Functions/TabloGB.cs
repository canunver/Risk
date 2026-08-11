using GemBox.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Risk.net.Utilities.Functions
{
    //https://github.com/GemBoxLtd/GemBox.Spreadsheet.Examples/tree/master/C%23
    public class TabloGB : ITablo
    {
        private static int UZUNLUKCARPAN = 1;
        private ExcelFile XLS;
        private ExcelWorksheet worksheet;

        private string tmpFile;
        int aktifSheetNo;
        private string encoding;
        bool okuModu = false;
        private string dosyaSaklamaFormat;

        private System.Drawing.Color RenkBul(TabloRenk renk)
        {
            if (renk == TabloRenk.AQUA) return System.Drawing.Color.Aqua;
            if (renk == TabloRenk.BLACK) return System.Drawing.Color.Black;
            if (renk == TabloRenk.CORAL) return System.Drawing.Color.Coral;
            if (renk == TabloRenk.GRAY_25) return System.Drawing.Color.Gray;
            if (renk == TabloRenk.ICE_BLUE) return System.Drawing.Color.FromArgb(212, 240, 255);
            if (renk == TabloRenk.IVORY) return System.Drawing.Color.Ivory;
            if (renk == TabloRenk.LIGHT_TURQUOISE2) return System.Drawing.Color.LightBlue;
            if (renk == TabloRenk.ORANGE) return System.Drawing.Color.Orange;
            if (renk == TabloRenk.PALE_BLUE) return System.Drawing.Color.PaleTurquoise;
            if (renk == TabloRenk.PERIWINKLE) return System.Drawing.Color.FromArgb(204, 204, 255);
            if (renk == TabloRenk.PINK2) return System.Drawing.Color.Pink;
            if (renk == TabloRenk.RED) return System.Drawing.Color.Red;
            if (renk == TabloRenk.ROSE) return System.Drawing.Color.MistyRose;
            if (renk == TabloRenk.VERY_LIGHT_YELLOW) return System.Drawing.Color.Yellow;
            if (renk == TabloRenk.YELLOW2) return System.Drawing.Color.YellowGreen;

            if (renk == TabloRenk.ALICEBLUE) return System.Drawing.Color.AliceBlue;
            if (renk == TabloRenk.BLUE) return System.Drawing.Color.Blue;
            if (renk == TabloRenk.GREEN) return System.Drawing.Color.Green;
            if (renk == TabloRenk.LAVENDER) return System.Drawing.Color.Lavender;
            if (renk == TabloRenk.DARKBLUE) return System.Drawing.Color.DarkBlue;
            if (renk == TabloRenk.DARKGREEN) return System.Drawing.Color.DarkGreen;
            if (renk == TabloRenk.DARKORANGE) return System.Drawing.Color.DarkOrange;
            if (renk == TabloRenk.DARKRED) return System.Drawing.Color.DarkRed;
            if (renk == TabloRenk.DARKSALMON) return System.Drawing.Color.DarkSalmon;
            if (renk == TabloRenk.DEEPPINK) return System.Drawing.Color.DeepPink;
            if (renk == TabloRenk.DEEPSKYBLUE) return System.Drawing.Color.DeepSkyBlue;
            if (renk == TabloRenk.DODGERBLUE) return System.Drawing.Color.DodgerBlue;
            if (renk == TabloRenk.FIREBRICK) return System.Drawing.Color.Firebrick;
            if (renk == TabloRenk.FORESTGREEN) return System.Drawing.Color.ForestGreen;
            if (renk == TabloRenk.GOLD) return System.Drawing.Color.Gold;
            if (renk == TabloRenk.OLIVE) return System.Drawing.Color.Olive;
            if (renk == TabloRenk.ORANGERED) return System.Drawing.Color.OrangeRed;
            if (renk == TabloRenk.WHITE) return System.Drawing.Color.White;

            return System.Drawing.Color.Black;
        }

        private LineStyle BorderStilBul(LineStyle stil)
        {
            if (stil == LineStyle.DOUBLE) return LineStyle.DOUBLE;
            if (stil == LineStyle.MEDIUM) return LineStyle.MEDIUM;
            if (stil == LineStyle.THIN) return LineStyle.THIN;
            if (stil == LineStyle.HAIR) return LineStyle.HAIR;
            if (stil == LineStyle.MEDIUM_DASHED) return LineStyle.MEDIUM_DASHED;

            return LineStyle.NONE;
        }

        public TabloGB()
        {
            aktifSheetNo = 0;
            encoding = "ISO-8859-9";
        }

        public string SonucDosyaAd()
        {
            return tmpFile;
        }

        public void SatirAc(int sheetNo, int satir, int acilacakSatirSayisi)
        {
            worksheet = XLS.Worksheets[sheetNo];
            worksheet.Rows.InsertEmpty(satir, acilacakSatirSayisi);
        }

        public void SatirAc(int satir, int acilacakSatirSayisi)
        {
            SatirAc(aktifSheetNo, satir, acilacakSatirSayisi);
        }

        public void SutunAc(int sheetNo, int sutun, int acilacakSutunSayisi)
        {
            worksheet = XLS.Worksheets[sheetNo];
            worksheet.Columns.InsertEmpty(sutun, acilacakSutunSayisi);
        }

        public void SutunAc(int sutun, int acilacakSutunSayisi)
        {
            SutunAc(aktifSheetNo, sutun, acilacakSutunSayisi);
        }

        private void SatirKontroluYap(int sheetNo, int satir)
        {
        }

        private void SutunKontroluYap(int sheetNo, int sutun)
        {
        }

        public void HucreAdBulYaz(string hucreAd, double deger, string paraIsareti, int kurusHane = 2)
        {
            int sutun1 = -1;
            int satir1 = -1;

            HucreAdAdresCoz(hucreAd, ref satir1, ref sutun1);

            if (satir1 >= 0)
            {
                HucreDegerYaz(satir1, sutun1, deger, paraIsareti, kurusHane);
            }
        }

        public void HucreAdBulYaz(string hucreAd, object deger)
        {
            int sutun1 = -1;
            int satir1 = -1;

            HucreAdAdresCoz(hucreAd, ref satir1, ref sutun1);

            if (satir1 >= 0)
            {
                HucreDegerYaz(aktifSheetNo, satir1, sutun1, deger);
            }
        }

        public void HucreDegerHtmlYaz(int sheetNo, int satir, int sutun, string HtmlString)
        {
            try
            {
                //Worksheet sheet = XLS.Worksheets[sheetNo];
                ExcelCell c = worksheet.Cells[satir, sutun];
                c.SetValue(HtmlString, HtmlLoadOptions.HtmlDefault);
            }
            catch { }
        }

        private void HucreDegerYaz(int sheetNo, int satir, int sutun, object deger)
        {
            try
            {
                bool bosalt;
                worksheet = XLS.Worksheets[sheetNo];
                ExcelCell c = worksheet.Cells[satir, sutun];
                if (deger == null)
                    bosalt = true;
                else
                    bosalt = false;

                if (!bosalt)
                {
                    if (deger.GetType().Equals(typeof(int)))
                        worksheet.Cells[satir, sutun].Value = (int)deger;
                    else if (deger.GetType().Equals(typeof(long)))
                        worksheet.Cells[satir, sutun].Value = (long)deger;
                    else if (deger.GetType().Equals(typeof(double)))
                        worksheet.Cells[satir, sutun].Value = (double)deger;
                    else if (deger.GetType().Equals(typeof(decimal)))
                        worksheet.Cells[satir, sutun].Value = (decimal)deger;
                    else if (deger.GetType().Equals(typeof(string)))
                        worksheet.Cells[satir, sutun].Value = (string)deger;
                    else if (deger.GetType().Equals(typeof(DateTime)))
                        worksheet.Cells[satir, sutun].Value = (DateTime)deger;
                }
                else
                {
                    c.SetValue(null);
                }
            }
            catch (Exception e) { }
        }

        public void HucreDegerYaz(int satir, int sutun, string deger)
        {
            HucreDegerYaz(aktifSheetNo, satir, sutun, deger);
        }

        public void HucreDegerYaz(int satir, int sutun, double deger)
        {
            HucreDegerYaz(aktifSheetNo, satir, sutun, deger);
        }

        public void HucreDegerYaz(int satir, int sutun, decimal deger)
        {
            HucreDegerYaz(aktifSheetNo, satir, sutun, Convert.ToDouble(deger));
        }

        public void HucreDegerYaz(int satir, int sutun, int deger)
        {
            HucreDegerYaz(aktifSheetNo, satir, sutun, Convert.ToDouble(deger));
        }

        public void HucreDegerYaz(int satir, int sutun, DateTime deger)
        {
            HucreDegerYaz(aktifSheetNo, satir, sutun, deger);
        }

        public void HucreDegerYaz(int satir, int sutun, double deger, string paraIsareti, int kurusHane = 2)
        {
            try
            {
                worksheet = XLS.Worksheets[aktifSheetNo];
                ExcelCell c = worksheet.Cells[satir, sutun];

                c.SetValue(deger);

                string kurus = "";
                if (kurusHane > 0)
                {
                    kurus = "".PadRight(kurusHane, '0');
                    kurus = "." + kurus;
                }

                c.Style.NumberFormat = @"#,##0" + kurus + " [$" + paraIsareti + "]";
            }
            catch { }
        }

        public void HucreAdDegerYaz(string hucreAd, object deger)
        {
            int sutun1 = -1;
            int satir1 = -1;

            HucreAdAdresCoz(hucreAd, ref satir1, ref sutun1);

            if (satir1 >= 0)
            {
                HucreDegerYaz(aktifSheetNo, satir1, sutun1, deger);
            }
        }

        public double HucreDegerAlDbl(int sheetNo, int satir, int sutun)
        {
            worksheet = XLS.Worksheets[sheetNo];
            try
            {
                ExcelCell cell = worksheet.Cells[satir, sutun];
                return cell.DoubleValue;
            }
            catch
            {
                return 0;
            }
        }

        public double HucreDegerAlDbl(int satir, int sutun)
        {
            return HucreDegerAlDbl(aktifSheetNo, satir, sutun);
        }

        public string HucreDegerAl(int sheetNo, int satir, int sutun, int noktaSay)
        {
            worksheet = XLS.Worksheets[sheetNo];
            ExcelCell cell = worksheet.Cells[satir, sutun];
            return cell.StringValue;
        }

        public string HucreDegerAl(int satir, int sutun)
        {
            return HucreDegerAl(aktifSheetNo, satir, sutun, 0);
        }

        public string HucreAdDegerAl(string hucreAd)
        {
            int sheetNo = aktifSheetNo;
            int sutun1 = -1;
            int satir1 = -1;

            HucreAdAdresCoz(hucreAd, ref satir1, ref sutun1);

            if (satir1 >= 0)
            {
                return HucreDegerAl(sheetNo, satir1, sutun1, 0);
            }
            else
                return "";
        }

        public string HucreFormulAl(int satir, int sutun)
        {
            worksheet = XLS.Worksheets[aktifSheetNo];
            ExcelCell cell = worksheet.Cells[satir, sutun];
            string f = cell.Formula;
            if (string.IsNullOrWhiteSpace(f)) return f;
            if (f.StartsWith("=")) return f.Substring(1);
            return f;
        }

        public void HucreFormulYaz(int satir, int sutun, string formul)
        {
            worksheet = XLS.Worksheets[aktifSheetNo];
            ExcelCell cell = worksheet.Cells[satir, sutun];
            cell.Formula = formul;
        }

        public void KorumayaAl()
        {
            KorumayaAl("01020304");
        }

        public void KorumayaAl(string sifre)
        {
            worksheet = XLS.Worksheets[aktifSheetNo];
            //sheet.Protect(ProtectionType.All, sifre, null);
        }

        public void HucreAdAdresCoz(string hucreAd, ref int satir, ref int sutun)
        {
            sutun = -1;
            satir = -1;
            try
            {
                worksheet = XLS.Worksheets[aktifSheetNo];
                NamedRange priceRange = worksheet.NamedRanges[hucreAd];
                CellRange range = priceRange.Range;
                satir = range.FirstRowIndex;
                sutun = range.FirstColumnIndex;

                //worksheet.Cells.FindText(hucreAd, out satir, out sutun);
            }
            catch { }
        }

        public void HucreAdAdresCoz(string hucreAd, ref int satir, ref int sutun, ref int satir2, ref int sutun2)
        {
            sutun = -1;
            satir = -1;
            sutun2 = -1;
            satir2 = -1;
            try
            {
                worksheet = XLS.Worksheets[aktifSheetNo];
                NamedRange nRange = worksheet.NamedRanges[hucreAd];
                CellRange range = nRange.Range;
                satir = range.FirstRowIndex;
                sutun = range.FirstColumnIndex;
                satir2 = range.LastRowIndex;
                sutun2 = range.LastColumnIndex;

                //worksheet.Cells.FindText(hucreAd, out satir, out sutun);
            }
            catch { }
        }

        public void SatirKopyala(int kaynakSatir, int hedefSatir, int satirSayisi = 1)
        {
            int kaynakSutun = 10;
        }

        public void HucreKopyala(int kaynakSheet, int satir1, int sutun1, int satir2, int sutun2, int hedefSheet, int hedefSatir, int hedefSutun)
        {
            try
            {
                try
                {
                    worksheet = XLS.Worksheets[kaynakSheet];
                    var range = worksheet.Cells.GetSubrangeAbsolute(satir1, sutun1, satir2, sutun2);

                    worksheet = XLS.Worksheets[hedefSheet];
                    range.CopyTo(worksheet, hedefSatir, hedefSutun);
                }
                catch { }
            }
            catch { }
        }

        public void HucreKopyala(int satir1, int sutun1, int satir2, int sutun2, int hedefSatir, int hedefSutun)
        {
            HucreKopyala(aktifSheetNo, satir1, sutun1, satir2, sutun2, aktifSheetNo, hedefSatir, hedefSutun);
        }

        public void HucreBirlestir(int satir1, int sutun1, int satir2, int sutun2)
        {
            worksheet = XLS.Worksheets[aktifSheetNo];
            try
            {
                var range = worksheet.Cells.GetSubrangeAbsolute(satir1, sutun1, satir2, sutun2);
                range.Merged = true;
                //sheet.Cells.Merge(satir1, sutun1, satir2 - satir1 + 1, sutun2 - sutun1 + 1);
            }
            catch { }
        }

        public void HucreBirlestirme(int satir1, int sutun1, int satir2, int sutun2)
        {
            for (int i = satir1; i <= satir2; i++)
            {
                for (int j = sutun1; j <= sutun2; j++)
                    HucreBirlestirme(i, j);
            }
        }

        public void HucreBirlestirme(int satir, int sutun)
        {
            worksheet = XLS.Worksheets[aktifSheetNo];

        }

        public void SutunGizle(int sutun1, int sutun2, bool gizle)
        {
            SutunKontroluYap(aktifSheetNo, sutun2);
        }

        public void SatirGizle(int satir1, int satir2, bool gizle)
        {
            SatirGizle(aktifSheetNo, satir1, satir2, gizle);
        }

        public void SatirGizle(int sheetNo, int satir1, int satir2, bool gizle)
        {
            SatirKontroluYap(sheetNo, satir2);
        }

        public void SatirSil(int satir1, int satir2)
        {
            SatirKontroluYap(aktifSheetNo, satir2);
        }

        public void SutunSil(int sutun1, int sutun2)
        {
            SutunKontroluYap(aktifSheetNo, sutun2);
        }

        public void HucreRakamFormatla(int satir, int sutun, string deger)
        {
            HucreRakamFormatla(satir, sutun, satir, sutun, deger);
        }

        public void HucreRakamFormatla(int satir1, int sutun1, int satir2, int sutun2, string deger)
        {
            HucreFormatla(satir1, sutun1, satir2, sutun2, deger);
        }

        public void HucreFormatla(int satir1, int sutun1, int satir2, int sutun2, string deger)
        {
        }

        public void DuseyCizgiCiz(int satir1, int satir2, int sutun, LineStyle stil, TabloRenk renk, bool solMu)
        {
        }

        public void DuseyCizgiCiz(int satir1, int satir2, int sutun, LineStyle stil, TabloRenk renk)
        {
            DuseyCizgiCiz(satir1, satir2, sutun, stil, renk, true);
        }

        public void YatayCizgiCiz(int satir, int sutun1, int sutun2, LineStyle stil, TabloRenk renk, bool ustMu)
        {
        }

        public void YatayCizgiCiz(int satir, int sutun1, int sutun2, LineStyle stil, TabloRenk renk)
        {
            YatayCizgiCiz(satir, sutun1, sutun2, stil, renk, true);
        }

        public void CerceveCizgiCiz(int satir1, int satir2, int sutun1, int sutun2, LineStyle stil, TabloRenk renk)
        {
            for (int i = satir1; i <= satir2 + 1; i++)
                YatayCizgiCiz(i, sutun1, sutun2, stil, renk, true);

            for (int i = sutun1; i <= sutun2 + 1; i++)
                DuseyCizgiCiz(satir1, satir2, i, stil, renk, true);
        }

        public void CerceveCiz(int satir1, int satir2, int sutun1, int sutun2, LineStyle stil, TabloRenk renk)
        {
            DuseyCizgiCiz(satir1, satir2 - 1, sutun1, stil, renk, true);
            DuseyCizgiCiz(satir1, satir2 - 1, sutun2 - 1, stil, renk, false);
            YatayCizgiCiz(satir1, sutun1, sutun2 - 1, stil, renk, true);
            YatayCizgiCiz(satir2 - 1, sutun1, sutun2 - 1, stil, renk, false);
        }

        public void HucreMetniKaydir(int satir1, int sutun1, int satir2, int sutun2, bool deger)
        {
        }

        public void HucreMetniKaydir(int satir, int sutun, bool deger)
        {
            HucreMetniKaydir(satir, sutun, satir, sutun, deger);
        }

        public void HucreMetniSigdir(int satir1, int sutun1, int satir2, int sutun2, bool deger)
        {
        }

        public void HucreMetniSigdir(int satir, int sutun, bool deger)
        {
            HucreMetniSigdir(satir, sutun, satir, sutun, deger);
        }

        public void KoyuYap(int satir1, int sutun1, int satir2, int sutun2, bool deger)
        {
        }

        public void KoyuYap(int satir, int sutun, bool deger)
        {
            KoyuYap(satir, sutun, satir, sutun, deger);
        }

        public void YaziTipiAta(int satir1, int sutun1, int satir2, int sutun2, string fontAd)
        {
        }

        public void YaziTipBuyuklugu(int satir, int sutun, int deger)
        {
            YaziTipBuyuklugu(satir, sutun, satir, sutun, deger);
        }

        public void YaziTipBuyuklugu(int satir1, int sutun1, int satir2, int sutun2, int deger)
        {
        }

        public void YatayHizala(int satir, int sutun, int deger)
        {
            YatayHizala(satir, sutun, satir, sutun, deger);
        }

        public void DuseyHizala(int satir1, int sutun1, int satir2, int sutun2, int deger)
        {
        }

        public void DuseyHizala(int satir, int sutun, int deger)
        {
            DuseyHizala(satir, sutun, satir, sutun, deger);
        }

        public void YatayHizala(int satir1, int sutun1, int satir2, int sutun2, int deger)
        {
        }

        public void ArkaPlanRenk(int satir1, int sutun1, int satir2, int sutun2, System.Drawing.Color renk)
        {
            worksheet = XLS.Worksheets[0];
            var range = worksheet.Cells.GetSubrangeAbsolute(satir1, sutun1, satir2, sutun2);

            range.Style.FillPattern.SetPattern(FillPatternStyle.Solid, renk, System.Drawing.Color.Empty);
        }

        public void ArkaPlanRenk(int satir, int sutun, System.Drawing.Color renk)
        {
            ArkaPlanRenk(satir, sutun, satir, sutun, renk);
        }

        public void ArkaPlanRenk(int satir1, int sutun1, int satir2, int sutun2, TabloRenk renk)
        {
            ArkaPlanRenk(satir1, sutun1, satir2, sutun2, RenkBul(renk));
        }

        public void ArkaPlanRenk(int satir, int sutun, TabloRenk renk)
        {
            ArkaPlanRenk(satir, sutun, satir, sutun, renk);
        }

        public void YaziRenk(int satir1, int sutun1, int satir2, int sutun2, TabloRenk renk)
        {
        }

        public void YaziRenk(int satir, int sutun, TabloRenk renk)
        {
            YaziRenk(satir, sutun, satir, sutun, renk);
        }

        public void YaziRenk(int satir1, int sutun1, int satir2, int sutun2, System.Drawing.Color renk)
        {
            worksheet = XLS.Worksheets[0];
            var range = worksheet.Cells.GetSubrangeAbsolute(satir1, sutun1, satir2, sutun2);

            range.Style.FillPattern.SetSolid(renk);
        }

        public void YaziRenk(int satir, int sutun, System.Drawing.Color renk)
        {
            YaziRenk(satir, sutun, satir, sutun, renk);
        }

        public void CokluSatirdaYaz(int satir, int sutun, bool deger)
        {
            HucreMetniKaydir(satir, sutun, deger);
        }

        public void AktifSheetDegistir(int sheetNo)
        {
            aktifSheetNo = sheetNo;
            ExcelWorksheet sheet = XLS.Worksheets[sheetNo];
        }

        public int AktifSheet()
        {
            return aktifSheetNo;
        }

        public int YeniSheetEkle()
        {
            int tut = aktifSheetNo;
            //aktifSheet = XLS.Worksheets.Count;
            //XLS.Worksheets.ActiveSheetIndex = aktifSheet;
            //XLS.Worksheets.AddCopy(aktifSheet);
            return tut;
        }

        public int SheetSayisi()
        {
            return XLS.Worksheets.Count;
        }

        public int YeniSheetEkle(int kaynakSheetNo)
        {
            int tut = aktifSheetNo;
            //aktifSheet = XLS.Worksheets.Count;
            //XLS.Worksheets.ActiveSheetIndex = aktifSheet;
            //XLS.Worksheets.AddCopy(kaynakSheetNo);
            return tut;
        }

        public void SheetSil(int sheetNo)
        {
            XLS.Worksheets.Remove(sheetNo);
            worksheet = XLS.Worksheets[0];
        }

        public void SheetAdiVer(int sheetNo, string adi)
        {
            XLS.Worksheets[sheetNo].Name = adi;
        }

        public string SheetAdiAl()
        {
            return SheetAdiAl(AktifSheet());
        }

        public string SheetAdiAl(int sheetNo)
        {
            return XLS.Worksheets[sheetNo].Name;
        }

        public void IlkSayfaNumarasi(int sayfaNo)
        {

        }

        public void SelectSheet(int sheetNo)
        {
            worksheet = XLS.Worksheets[sheetNo];
        }

        public void SelectSheet()
        {
            SelectSheet(aktifSheetNo);
        }

        public void ResimEkle(double x, double y, double width, double height, string dosya)
        {
            ResimEkle(x, y, width, height, new System.IO.FileStream(dosya, System.IO.FileMode.Open, System.IO.FileAccess.Read), 0, 0, 2, 2);
        }

        public void ResimEkle(double x, double y, double width, double height, Stream stream)
        {
            ResimEkle(x, y, width, height, stream, 0, 0, 2, 2);
        }

        public object ResimEkle(double x, double y, double width, double height, Stream stream, double left, double top, int enBoyOran, int yanasiklik)
        {
            
            return null;
        }

        public void BosDosyaAc(string sonucDosya)
        {
            SpreadsheetInfo.SetLicense("SN-2022Mar10-OfwEvmM4APXWinX6RluGZmNXt6X1ekw0LgHbSCzNgCDBZ29B2xPQzlYK5xjecA1gBd2FWXi15zJGbCy67PeSe1Puanw==A");

            tmpFile = sonucDosya;
            XLS = new ExcelFile();
            worksheet = XLS.Worksheets.Add("Rapor");
        }

        public void DosyaOkuAc(string dosyaAd)
        {
            DosyaAcGenel(dosyaAd, Arac.DosyaAdUret(), true);
        }

        public void DosyaAc(string dosyaAd)
        {
            DosyaAcGenel(dosyaAd, Arac.DosyaAdUret(), false);
        }

        public void DosyaAc(string dosyaAd, string sonucDosya)
        {
            DosyaAcGenel(dosyaAd, sonucDosya, false);
        }

        private void DosyaAcGenel(string dosyaAd, string sonucDosya, bool okuModuMu)
        {
            SpreadsheetInfo.SetLicense("SN-2022Mar10-OfwEvmM4APXWinX6RluGZmNXt6X1ekw0LgHbSCzNgCDBZ29B2xPQzlYK5xjecA1gBd2FWXi15zJGbCy67PeSe1Puanw==A");

            XLS = ExcelFile.Load(dosyaAd);
            okuModu = okuModuMu;
            tmpFile = sonucDosya;
        }

        public void DosyaSaklaTamYol()
        {
            XLS.Calculate();

            if (dosyaSaklamaFormat == "html")
                XLS.Save(tmpFile, SaveOptions.HtmlDefault);
            else if (dosyaSaklamaFormat == "pdf")
                XLS.Save(tmpFile, SaveOptions.PdfDefault);
            else if (dosyaSaklamaFormat == "ods")
                XLS.Save(tmpFile, SaveOptions.OdsDefault);
            //else if (dosyaSaklamaFormat == "xlsm")
            //    XLS.Save(tmpFile, SaveOptions.Xlsm);
            else
                XLS.Save(tmpFile, SaveOptions.XlsxDefault);
        }

        public void DosyaKapat()
        {
        }

        public string UzantiBul()
        {
            return "xlsx";
        }
       
        public void DosyaSaklamaFormatAta(string uzanti)
        {
            dosyaSaklamaFormat = uzanti.ToLower();
        }

        public void CalculateFormula()
        {
            XLS.Calculate();
        }

        public void HucreAdAdresOl(string bolgeAd, int sheetNo, int satir1, int sutun1, int satir2, int sutun2)
        {
            try
            {
                XLS.Worksheets[sheetNo].NamedRanges.Add(bolgeAd, XLS.Worksheets[sheetNo].Cells.GetSubrangeAbsolute(satir1, sutun1, satir2, sutun2));

            }
            catch (Exception)
            {
            }
        }

        public void FormulleriSil()
        {
        }
    }
}
