using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki RaporUstYonetimStratejikPlanlama view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class RaporUstYonetimStratejikPlanlama : RaporEntityBase, IEntity
    {
        public string KoordinatorlukAdi { get; set; }
        public string BirimAdi { get; set; }
        public string IsbirligiYapacakKoordinatorlukler { get; set; }
        public string IsbirligiYapacakBirimler { get; set; }
        public int? BaslamaYil { get; set; }
        public int? BitisYil { get; set; }
        public string Amac { get; set; }
        public string Hedef { get; set; }
        public string AnahtarRiskGostergesi { get; set; }
        public string GostergeNo { get; set; }
        public int? HedefeEtkisi { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? BaslangicTarihi { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? BitisTarihi { get; set; }
        public int? BaslangicDegeri { get; set; }
        [Column(TypeName = "decimal(13, 4)")]
        public decimal? SapmaOrani { get; set; }
        public string SapmaNedeni { get; set; }
        public string IzlemeDonemleri { get; set; }

        public virtual string StratejikPlanDonemAdi
        {
            get
            {
                if (BaslamaYil > 0)
                    return BaslamaYil + " - " + BitisYil;

                return "";
            }
        }

        public virtual List<RaporStratejikPlanIzlemeDonem> Donemler
        {
            get
            {
                var donenDeger = new List<RaporStratejikPlanIzlemeDonem>();

                try
                {
                    if (!string.IsNullOrWhiteSpace(IzlemeDonemleri))
                    {
                        //1;35;30;32,2;50;40;35 (Dönem,Planlanan,Gerçekleþen,GerçekleþenYilSonu)

                        var d = IzlemeDonemleri.Split(',');
                        foreach (var deger in d)
                        {
                            if (!string.IsNullOrWhiteSpace(deger))
                            {
                                var degerler = deger.Split(';');

                                var izlemeDonem = new RaporStratejikPlanIzlemeDonem();
                                izlemeDonem.Donem = Convert.ToInt32(degerler[0]);
                                izlemeDonem.PlanlananDeger = Convert.ToInt32(degerler[1]);
                                izlemeDonem.GerceklesenDeger = Utilities.Functions.Arac.ConvertToDouble(degerler[2]?.Replace(".", ","));
                                izlemeDonem.GerceklesenDegerYilSonu = Utilities.Functions.Arac.ConvertToDouble(degerler[3]?.Replace(".", ","));

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

        public virtual string Trend
        {
            get
            {
                if (Donemler != null && Donemler.Count > 1)
                {
                    var SonDeger2 = Donemler[Donemler.Count - 1].GerceklesenDegerYilSonu;
                    var SonDeger1 = Donemler[Donemler.Count - 2].GerceklesenDegerYilSonu;

                    if (SonDeger2 == 0)
                        SonDeger2 = Donemler[Donemler.Count - 1].GerceklesenDeger;
                    if (SonDeger1 == 0)
                        SonDeger1 = Donemler[Donemler.Count - 2].GerceklesenDeger;

                    if (SonDeger2 > SonDeger1) return "ARTAN";
                    if (SonDeger2 < SonDeger1) return "AZALAN";
                    else return "SABÝT";
                }

                return "";
            }
        }

    }


}
