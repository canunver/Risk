using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// StratejikPlanEylemPlani işlemlerinin yapıldığı servis
    /// </summary>
    public class StratejikPlanEylemPlaniService : IStratejikPlanEylemPlaniService
    {
        /// <summary>
        /// IUnitOfWork<StratejikPlanEylemPlani> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<StratejikPlanEylemPlani> _unitOfWork;
        /// <summary>
        /// ITarihceService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ITarihceService _serviceTarihce;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.StratejikPlanEylemPlaniService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanEylemPlaniService(IUnitOfWork<StratejikPlanEylemPlani> unitOfWork,
                                            ITarihceService serviceTarihce,
                                            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceTarihce = serviceTarihce;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="hedefKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string hedefKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.StratejikPlanHedefKod == hedefKod, o => o.EylemNo);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, StratejikPlanHedef gelenNesne)
        {
            StratejikPlanEylemPlani islemYapilan = new StratejikPlanEylemPlani();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.StratejikPlanIzlemeGostergeKodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {

                await SilAsync(kullanan, new StratejikPlanEylemPlani() { StratejikPlanHedefKod = gelenNesne.Kod });

                if (gelenNesne.Eylemler != null)
                {
                    foreach (var eylem in gelenNesne.Eylemler)
                    {
                        if (string.IsNullOrWhiteSpace(eylem.Notlar)) eylem.Notlar = "";

                        eylem.Kod = Arac.GetGuid();
                        eylem.StratejikPlanHedefKod = gelenNesne.Kod;
                        islemYapilan = await _unitOfWork.KayitEkleAsync(eylem);
                    }

                    await _unitOfWork.KaydetAsync();
                }

                //Tarihçe kaydı
                Tarihce tarihce = new Tarihce();
                tarihce.YeniDeger = Arac.JSONSerialize(gelenNesne);
                tarihce.IlgiKod = gelenNesne.Kod + "iz";//Tarihçe izleme kod kaydına göre tutulduğu için
                tarihce.IlgiTur = EnumTarihceIslemTur.StratejikPlanIzleme;
                tarihce.IslemYapanKod = kullanan.PersonelKod;
                tarihce.Durum = (int)ENUMDurum.Aktif;
                var sonuc = await _serviceTarihce.KaydetAsync(kullanan, tarihce);

            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, StratejikPlanEylemPlani gelenNesne)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            try
            {
                if (!string.IsNullOrWhiteSpace(gelenNesne.Kod))
                    await _unitOfWork.SilAsync(d => d.Kod == gelenNesne.Kod);
                else if (!string.IsNullOrWhiteSpace(gelenNesne.StratejikPlanHedefKod))
                    await _unitOfWork.SilAsync(d => d.StratejikPlanHedefKod == gelenNesne.StratejikPlanHedefKod);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

    }
}
