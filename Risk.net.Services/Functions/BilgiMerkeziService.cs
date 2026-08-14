using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;
using System;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// BilgiMerkezi işlemlerinin yapıldığı servis
    /// </summary>
    public class BilgiMerkeziService : IBilgiMerkeziService
    {
        /// <summary>
        /// IUnitOfWork<BilgiMerkezi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<BilgiMerkezi> _unitOfWork;
        /// <summary>
        /// IDosyaService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IDosyaService _serviceDosya;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.BilgiMerkeziService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceDosya"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public BilgiMerkeziService(IUnitOfWork<BilgiMerkezi> unitOfWork, IDosyaService serviceDosya, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceDosya = serviceDosya;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydın tüm bilgisini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KayitGetirAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata, null);

            try
            {
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod);

                if (kayit != null)
                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
            }
            catch (System.Exception e)
            {
                return new Sonuc(ENUMIslemDurum.Hata, e.Message);
            }

            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.Durum == (int)ENUMDurum.Aktif, null);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(k => k.Durum > (int)ENUMDurum.Tanimsiz, "Dosya");

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                BilgiMerkezi aramaObj = Arac.DataTablesAramaNesne<BilgiMerkezi>(new BilgiMerkezi(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi == aramaObj.Adi);
                if (!string.IsNullOrWhiteSpace(aramaObj.KayitEden))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitEden.Contains(aramaObj.KayitEden));
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else if (kullanan.AktifRolKod != "RISKSEKRETARYASI")
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif || a.Durum == (int)ENUMDurum.Onayli || a.Durum == (int)ENUMDurum.OnayaGonderdi);
            }
            else
            {
                if (kullanan.AktifRolKod != "RISKSEKRETARYASI")
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif || a.Durum == (int)ENUMDurum.Onayli || a.Durum == (int)ENUMDurum.OnayaGonderdi);
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue)
                                                                       || a.KayitEden.Contains(dataTablesParam.searchValue));
            }

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgileri kaydeden metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, BilgiMerkezi gelenNesne)
        {
            BilgiMerkezi islemYapilan = new BilgiMerkezi();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Adi))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.SurumNo))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.SurumAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "KAYDET");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                if (!gelenNesne.KayitTarihi.HasValue)
                    gelenNesne.KayitTarihi = DateTime.Now;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    //Revize işlemi yapılıyorsa ilk önce eski kaydı pasif yap
                    if (!string.IsNullOrWhiteSpace(gelenNesne.RevizeKod))
                    {
                        var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.RevizeKod);

                        eskiKayit.Durum = (int)ENUMDurum.Pasif;
                        islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                    }

                    gelenNesne.Kod = Arac.GetGuid();
                    gelenNesne.Durum = (int)ENUMDurum.Aktif;
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);
                    eskiKayit.Adi = gelenNesne.Adi;
                    eskiKayit.SurumNo = gelenNesne.SurumNo;
                    eskiKayit.KayitTarihi = gelenNesne.KayitTarihi;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"], islemYapilan.Kod);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydı silen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> SilAsync(KullaniciDto kullanan, string kod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "SIL");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                await _unitOfWork.SilAsync(d => d.Kod == kod);
                await _unitOfWork.KaydetAsync();

                await _serviceDosya.SilBaglantiKodAsync(kullanan, kod);
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.SilmeBasarili"]);
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen kaydın durumunu değiştiren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kod"></param>
        /// <param name="durum"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, string kod, int durum)
        {
            BilgiMerkezi islemYapilan = new BilgiMerkezi();

            string hata = "";

            if (string.IsNullOrWhiteSpace(kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "DURUM");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod);

                eskiKayit.Durum = durum;

                islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.DurumBasarili"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kullanıcının yetkisinin olup olmadığını döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="tur"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        private string YetkisiVarmi(KullaniciDto kullanan, string tur)
        {
            bool yetki = Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }
    }
}