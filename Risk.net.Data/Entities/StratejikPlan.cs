using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki StratejikPlan tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class StratejikPlan : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        //[ForeignKey("ViewKoordinatorluk")]
        //public string KoordinatorlukKod { get; set; } = "";

        //[ForeignKey("ViewBirim")]
        //public string BirimKod { get; set; } = "";

        [ForeignKey("TanimStratejikPlanDonem")]
        public string StratejikPlanDonemKod { get; set; } = "";

        [Column(TypeName = "varchar(10)")]
        public string AmacNo { get; set; } = "";

        [Column(TypeName = "varchar(250)")]
        public string Amac { get; set; } = "";

        [Column(TypeName = "date")]
        public DateTime? KayitTarihi { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        //public ViewKoordinatorluk Koordinatorluk { get; set; }
        //public ViewBirim Birim { get; set; }
        //public List<StratejikPlanIsbirligiBirim> IsbirligiBirimler { get; set; }
        public TanimStratejikPlanDonem StratejikPlanDonem { get; set; }
        public List<StratejikPlanHedef> Hedefler { get; set; }

        public virtual string StratejikPlanDonemAdi
        {
            get
            {
                if (StratejikPlanDonem != null)
                    return StratejikPlanDonem?.BaslamaYil + " - " + StratejikPlanDonem?.BitisYil;

                return "";
            }
        }

        public virtual string HedefAdlari
        {
            get
            {
                string adlar = "";
                if (Hedefler == null) return "";

                foreach (StratejikPlanHedef item in Hedefler)
                {
                    if (adlar != "") adlar += ", ";
                    adlar += item.Adi;
                }
                return adlar;

            }
        }

        //public virtual string IsbirligiBirimAdlari
        //{
        //    get
        //    {
        //        string adlar = "";
        //        if (IsbirligiBirimler == null) return "";

        //        int sayac = 0;
        //        foreach (StratejikPlanIsbirligiBirim item in IsbirligiBirimler)
        //        {
        //            if (adlar != "") adlar += ", ";
        //            adlar += item.Koordinatorluk?.Adi + "-" + item.Birim?.Adi;

        //            if (item.Koordinatorluk.Adi.IndexOf(" Ýl ") > -1)
        //                sayac++;
        //        }

        //        if (sayac == 42)
        //            adlar = "Bütün Ýl Koordinatörlükleri";

        //        return adlar;
        //    }
        //}

        public virtual int Yil
        {
            get
            {
                if (KayitTarihi != null)
                    return KayitTarihi.Value.Year;

                return 0;
            }
        }

    }
}
