using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Risk.net.Services.Interfaces;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Functions;
using Risk.net.Data.Entities;
using Risk.net.Utilities.Objects;
using Risk.net.Utilities.Functions;
using System.Collections.Generic;

namespace Risk.net.WebUI.Quartz
{
    [DisallowConcurrentExecution]
    public class BildirimSistemiMailGonderJob : IJob
    {
        private readonly IBildirimSistemiService _service;
        private readonly IViewBildirimSistemiService _serviceView;
        public BildirimSistemiMailGonderJob(IBildirimSistemiService service, IViewBildirimSistemiService serviceView)
        {
            _service = service;
            _serviceView = serviceView;
        }

        public Task Execute(IJobExecutionContext context)
        {
            string hata = "Bildirim Sistemi - Mail\n";

            var sonuc = _serviceView.ListeMailGonderilecekKayitlarAsync();
            if (sonuc.Result.IslemSonuc && sonuc.Result.Liste.Count > 0)
            {
                hata = "Gönderme Listesi\n";

                foreach (ViewBildirimSistemi bs in sonuc.Result.Liste)
                    hata += "Belge Tipi: " + (EnumTarihceIslemTur)bs.BelgeTipi + "\t Yetki: " + bs.OnaylayacakYetki + "\t İşlem: " + bs.Islem + "\n";
                hata += "****************************************************************************************\n";


                foreach (ViewBildirimSistemi bs in sonuc.Result.Liste)
                {
                    var b = new BildirimSistemi
                    {
                        Kod = bs.Kod,
                        BelgeTipi = bs.BelgeTipi,
                        BelgeKod = bs.BelgeKod,
                        KoordinatorlukKod = bs.KoordinatorlukKod,
                        BirimKod = bs.BirimKod,
                        OnaylayacakYetki = bs.OnaylayacakYetki,
                        Durum = bs.Durum,
                        Islem = bs.Islem,
                    };

                    sonuc = _service.MailGonderAsync(new KullaniciDto() { PersonelKod = "sistem" }, b);
                    if (sonuc.Result.IslemSonuc)
                        hata += (EnumTarihceIslemTur)bs.BelgeTipi + ": Mail gönderildi.\n";
                    else
                        hata += "Mail gönderme işemi sırasında hata oluştu!\n" + sonuc.Result.Mesaj;
                }
            }
            else
                hata += "Mail gönderilecek kayıt bulunamadı\n";


            string hataDosyaYol = Arac.ConfigOku("Genel:HataDosyaYol");
            string dosyaAdi = Path.Combine(hataDosyaYol, "BildirimSistemiMailGonderJob.txt");

            Arac.HataStrYaz(dosyaAdi, hata);

            return Task.CompletedTask;
        }

    }

}