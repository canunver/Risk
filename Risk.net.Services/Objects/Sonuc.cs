using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Risk.net.Services.Objects
{
    /// <summary>
    /// Entity verilerinin istemci katmanına göndermek için kullanılan değişken
    /// </summary>
    /// <remarks></remarks>
    public class Sonuc
    {
        public Sonuc()
        {
            IslemSonuc = false;
        }
        public Sonuc(ENUMIslemDurum durum, List<object> liste)
        {
            if (durum == ENUMIslemDurum.Basarili) IslemSonuc = true;
            Durum = durum;
            Liste = liste;
        }
        public Sonuc(ENUMIslemDurum durum, object nesne)
        {
            if (durum == ENUMIslemDurum.Basarili) IslemSonuc = true;
            Durum = durum;
            Nesne = nesne;
        }
        public Sonuc(ENUMIslemDurum durum, string mesaj, string anahtar = "")
        {
            if (durum == ENUMIslemDurum.Basarili) IslemSonuc = true;
            Durum = durum;
            Mesaj = MesajDuzenle(mesaj);
            AnahtarAlan = anahtar;
        }
        public Sonuc(ENUMIslemDurum durum, string mesaj, object nesne, string anahtar = "")
        {
            if (durum == ENUMIslemDurum.Basarili) IslemSonuc = true;
            Durum = durum;
            Mesaj = mesaj;
            Nesne = nesne;
            AnahtarAlan = anahtar;
        }
        public Sonuc(ENUMIslemDurum durum, string mesaj, object nesne, Exception hataBilgi)
        {
            if (durum == ENUMIslemDurum.Basarili) IslemSonuc = true;
            Durum = durum;
            Mesaj = mesaj;
            Nesne = nesne;
            HataBilgi = hataBilgi;
        }
        public bool IslemSonuc { get; } = false;
        public ENUMIslemDurum Durum { get; }
        public string Mesaj { get; }
        public string AnahtarAlan { get; set; } = "";
        public Exception HataBilgi { get; }
        public object Nesne { get; set; }
        public List<object> Liste { get; set; }

        public static string MesajDuzenle(string deger)
        {
            //Örnek: String or binary data would be truncated in table 'Risk.dbo.RiskEvreni', column 'AnahtarRiskGostergesiAdi'. Truncated value: 'yk sürecindeki proje sayısı, ekip sayısı, YK tamalanma süresinide içerir bir proje ağırlık oranı ile'.

            if (!string.IsNullOrWhiteSpace(deger))
            {
                if (deger.Contains("truncated"))
                {
                    try
                    {
                        string ara1 = ", column '";
                        string ara2 = "'. Truncated value:";

                        int sira1 = deger.IndexOf(ara1) + ara1.Length;
                        int sira2 = deger.IndexOf(ara2) - sira1;

                        string alanAdi = deger.Substring(sira1, sira2);


                        ara1 = "truncated in table '";
                        ara2 = "', column '";

                        sira1 = deger.IndexOf(ara1) + ara1.Length;
                        sira2 = deger.IndexOf(ara2) - sira1;

                        string tabloAdi = deger.Substring(sira1, sira2);

                        string[] parcalar = tabloAdi.Split('.');

                        tabloAdi = parcalar[parcalar.Length - 1];



                        string alanBoyut = "?";

                        var liste = from t in Assembly.Load("Risk.net.Data").GetTypes()
                                    where t.IsClass && t.Namespace == "Risk.net.Data.Entities" && t.Name == tabloAdi
                                    select t;

                        foreach (var item in liste)
                        {
                            var propertyInfo = typeof(Risk.net.Data.Entities.RiskEvreni).GetProperty(alanAdi);
                            var columnAttribute = (ColumnAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(ColumnAttribute));

                            if (columnAttribute != null)
                                alanBoyut = columnAttribute.TypeName.Replace("varchar", "").Replace("nvarchar", "").Replace("(", "").Replace(")", "");
                        }

                        string mesaj = alanAdi + " alanı için en fazla " + alanBoyut + " karakterlik veri girişi yapılabilir!";

                        if (deger.Contains("<small><li>"))
                            deger = "<small><li>" + mesaj + "</li></small>";
                        else
                            deger = mesaj;
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }

            return deger;
        }
    }
}
