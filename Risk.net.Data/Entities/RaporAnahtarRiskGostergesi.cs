using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanındaki RaporAnahtarRiskGostergesi view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RaporAnahtarRiskGostergesi : RaporEntityBase, IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string Surec { get; set; }
        public string AltSurec { get; set; }
        public string RiskAdi { get; set; }
        public string RiskTanimi { get; set; }
        public string RiskKategorisiAdi { get; set; }
        public string AnahtarRiskGostergesi { get; set; }
        public string VeriDayanagi { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? YesilDeger { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? SariDeger { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? KirmiziDeger { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? HedefDeger { get; set; }

        public string IzlemeDonemleri { get; set; }

        public virtual List<RaporAnahtarRiskGostergesiDonem> Donemler
        {
            get
            {
                var donenDeger = new List<RaporAnahtarRiskGostergesiDonem>();

                try
                {
                    if (!string.IsNullOrWhiteSpace(IzlemeDonemleri))
                    {
                        //1;35;1;,2;50;2 (Dönem,Deger,Periyot)

                        var d = IzlemeDonemleri.Split(',');
                        foreach (var deger in d)
                        {
                            if (!string.IsNullOrWhiteSpace(deger))
                            {
                                var degerler = deger.Split(';');

                                var izlemeDonem = new RaporAnahtarRiskGostergesiDonem();
                                izlemeDonem.Donem = Arac.ConvertToInt(degerler[0]);
                                izlemeDonem.Deger = Arac.ConvertToDecimal(degerler[1].Replace(".", ","));
                                izlemeDonem.Periyot = Arac.ConvertToInt(degerler[2]);

                                donenDeger.Add(izlemeDonem);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {

                }

                return donenDeger;
            }
        }

        public virtual string Durum
        {
            get
            {
                decimal? sonDonemDeger = 0;

                if (Donemler != null && Donemler.Count > 1)
                    sonDonemDeger = Donemler[Donemler.Count - 1].Deger;

                if (sonDonemDeger > 0)
                {
                    //YD:belirsiz, YDSON:40
                    //KD:75,       KDSSON:belirsiz
                    //SonDonemDeger:60 ise Sarı
                    //SonDonemDeger:15 ise Yeşil (40 isede)
                    //SonDonemDeger:85 ise Kırmızı (75 isede)

                    if (sonDonemDeger >= KirmiziDeger) return "KIRMIZI";
                    else if (sonDonemDeger <= YesilDeger) return "YEŞİL";
                    else return "SARI";
                }

                return "";
            }
        }

        public virtual string Trend
        {
            get
            {
                if (Donemler != null && Donemler.Count > 1)
                {
                    var SonDeger2 = Donemler[Donemler.Count - 1].Deger;
                    var SonDeger1 = Donemler[Donemler.Count - 2].Deger;

                    if (SonDeger2 > SonDeger1) return "ARTAN";
                    if (SonDeger2 < SonDeger1) return "AZALAN";
                    else return "SABİT";
                }

                return "";
            }
        }


    }


}
