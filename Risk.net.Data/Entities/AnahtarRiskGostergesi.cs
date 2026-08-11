using Risk.net.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki AnahtarRiskGostergesi tablosu ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class AnahtarRiskGostergesi : EntityBase, IEntity
    {
        [Key]
        [Column(TypeName = "varchar(40)")]
        public string Kod { get; set; } = "";

        [ForeignKey("RiskEvreni")]
        public string RiskEvreniKod { get; set; }

        [Column(TypeName = "varchar(150)")]
        public string Adi { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string VeriDayanagi { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double YesilDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double KirmiziDeger { get; set; }

        [Column(TypeName = "decimal(13, 4)")]
        public double HedefDeger { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        public List<AnahtarRiskGostergesiDonem> Donemler { get; set; }

        public virtual int SonDonem
        {
            get
            {
                int sonDonem = 0;
                if (Donemler == null) return sonDonem;

                foreach (var item in Donemler)
                {
                    if (item.Donem > sonDonem)
                        sonDonem = item.Donem;
                }

                return sonDonem;
            }
        }

        public virtual double SonDonemDeger
        {
            get
            {
                if (Donemler == null) return 0;

                foreach (var item in Donemler)
                {
                    if (item.Donem == SonDonem)
                        return item.GerceklesenDeger;
                }

                return 0;
            }
        }

        public virtual string DurumDeger
        {
            get
            {
                if (SonDonemDeger > 0)
                {
                    //YD:belirsiz, YDSON:40
                    //KD:75,       KDSSON:belirsiz
                    //SonDonemDeger:60 ise Sarý
                    //SonDonemDeger:15 ise Yeþil (40 isede)
                    //SonDonemDeger:85 ise Kýrmýzý (75 isede)

                    if (SonDonemDeger >= KirmiziDeger) return "KIRMIZI";
                    else if (SonDonemDeger <= YesilDeger) return "YEÞÝL";
                    else return "SARI";
                }

                return "";
            }
        }

        public virtual string Trend
        {
            get
            {
                double SonDeger2 = 0.0;
                double SonDeger1 = 0.0;

                int sonDonem = SonDonem;
                int oncekiDonem = sonDonem - 1;

                if (sonDonem > 1)
                {
                    foreach (var item in Donemler)
                    {
                        if (item.Donem == sonDonem)
                            SonDeger2 = item.GerceklesenDeger;
                        if (item.Donem == oncekiDonem)
                            SonDeger1 = item.GerceklesenDeger;
                    }

                    if (SonDeger1 + SonDeger2 > 0)
                    {
                        if (SonDeger2 > SonDeger1) return "ARTAN";
                        if (SonDeger2 < SonDeger1) return "AZALAN";
                        else return "SABÝT";
                    }
                }

                return "";
            }
        }
    }
}
