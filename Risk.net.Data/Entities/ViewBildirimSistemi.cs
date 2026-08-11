using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Risk.net.Data.Entities
{
    /// <summary>
    /// Veritabanýndaki ViewBildirimSistemi view ile yazýlým arasýnda iliþki kurmamýzý saðlayan kalýcý nesnedir
    /// </summary>
    public class ViewBildirimSistemi : IEntity
    {
        [Key]
        public int Kod { get; set; }

        public int BelgeTipi { get; set; }

        public string BelgeKod { get; set; }

        [ForeignKey("ViewKoordinatorluk")]
        public string KoordinatorlukKod { get; set; }

        [ForeignKey("ViewBirim")]
        public string BirimKod { get; set; }

        [ForeignKey("ViewPersonel")]
        public string IslemYapanKod { get; set; }

        [Column(TypeName = "dateTime")]
        public DateTime? IslemTarihi { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string OnaylayacakYetki { get; set; }

        [Column(TypeName = "int")]
        public int Durum { get; set; }

        [Column(TypeName = "varchar(4000)")]
        public string Aciklama { get; set; }

        [Column(TypeName = "int")]
        public EnumBildirimSistemiIslem? Islem { get; set; }


        public ViewKoordinatorluk Koordinatorluk { get; set; }
        public ViewBirim Birim { get; set; }
        public ViewPersonel IslemYapan { get; set; }

        public virtual string GoruntuleUrl
        {
            get
            {
                string url = "";

                if (BelgeTipi == 1)
                    url = "/RiskKaydiOnay";
                if (BelgeTipi == 2)
                    url = "/OlayRaporlamaOnay";
                else if (BelgeTipi == 6)
                {
                    if (Durum == (int)ENUMBildirimSistemiDurum.YapisalRiskSeviyesiDustu || Durum == (int)ENUMBildirimSistemiDurum.YapisalRiskSeviyesiDustu
                        || Durum == (int)ENUMBildirimSistemiDurum.ArtikRiskSeviyesiDustu || Durum == (int)ENUMBildirimSistemiDurum.ArtikRiskSeviyesiYukseldi)
                        url = "/RisklerinDegerlendirilmesi?ry=" + BelgeKod;
                    else
                        url = "/RisklerinDegerlendirilmesiOnay";
                }
                else if (BelgeTipi == 7)
                    url = "/RisklerinYonetilmesiOnay";
                else if (BelgeTipi == 5)
                {
                    if (Durum == (int)ENUMBildirimSistemiDurum.ARGDegeriKirmiziDegerinUzerineCikti)
                        url = "/AnahtarRiskGostergesi?a=" + BelgeKod;
                    else
                        url = "/AnahtarRiskGostergesi";
                }
                else if (BelgeTipi == 12)
                    url = "/BulguYonetimiCevap?onay=1";

                return url;
            }
        }
    }
}
