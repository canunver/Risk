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
    /// Veritabanındaki RaporRiskIzleme view ile yazılım arasında ilişki kurmamızı sağlayan kalıcı nesnedir
    /// </summary>
    public class RaporRiskIzleme : RaporEntityBase, IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public int? BaslamaYil { get; set; }
        public int? BitisYil { get; set; }
        public string Amac { get; set; }
        public string Hedef { get; set; }
        public string SurecAdi { get; set; }
        public string AltSurecAdi { get; set; }
        public string RiskNo { get; set; }
        public string RiskAdi { get; set; }
        public string RiskTanimi { get; set; }
        public string RiskKategorisiAdi { get; set; }
        public string IlIrtibatOfisiAdi { get; set; }
        public DateTime? RiskBaslangicTarihi { get; set; }
        public DateTime? RiskBitisTarihi { get; set; }
        public string IliskiliOlaylarAdi { get; set; }
        public string AnahtarRiskGostergesi { get; set; }
        public int? Etki { get; set; }
        public int? Olasilik { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public decimal? KontrolKriteriAgirligi { get; set; }
        public string EtkiOlasilikMatrisi { get; set; }
        public string KontrolAdi { get; set; }
        public int? ArtikRiskSeviyesi { get; set; }
        public int? RiskeVerilecekCevap { get; set; }
        public string AzaltmaPlani { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public decimal? YesilDeger { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? KirmiziDeger { get; set; }


        public virtual string RiskGecerlilikTarihi
        {
            get
            {
                string deger = "";

                if (RiskBitisTarihi.HasValue && RiskBitisTarihi.Value.ToString("dd.MM.yyyy") == "31.12.9999")
                    deger = "Belirsiz";
                else
                {
                    if (RiskBaslangicTarihi.HasValue)
                        deger += RiskBaslangicTarihi.Value.ToString("dd.MM.yyyy");

                    deger += "-";

                    if (RiskBitisTarihi.HasValue)
                        deger += RiskBitisTarihi.Value.ToString("dd.MM.yyyy");
                }

                return deger;
            }
        }

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

       
        public virtual string StratejikPlanDonemAdi
        {
            get
            {
                if (BaslamaYil > 0)
                    return BaslamaYil + " - " + BitisYil;

                return "";
            }
        }

        [NotMapped]
        public virtual int? DurumSorgu { get; set; }


    }
}
