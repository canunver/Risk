using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using Risk.net.Utilities.Objects;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// Surec işlemlerinin yapıldığı servis
    /// </summary>
    public class SurecService : ISurecService
    {
        /// <summary>
        /// IUnitOfWork<Surec> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<Surec> _unitOfWork;
        /// <summary>
        /// IAltSurecService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IAltSurecService _serviceAltSurec;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.SurecService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceAltSurec"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public SurecService(IUnitOfWork<Surec> unitOfWork, IAltSurecService serviceAltSurec, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceAltSurec = serviceAltSurec;
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "", c => c.AltSurecler, c => c.Koordinatorluk, c => c.Birim, c => c.SurecSahibiPersonel);

                if (kayit != null)
                {

                    for (int i = 0; i < kayit.AltSurecler.Count; i++)
                    {
                        if (kayit.AltSurecler[i].Durum == (int)ENUMDurum.Pasif)
                        {
                            kayit.AltSurecler.RemoveAt(i);
                            i--;
                        }
                    }
                    if (kayit.AltSurecler != null)
                        kayit.AltSurecler.Sort((a, b) => (a.Numara + "").CompareTo((b.Numara + "")));


                    return new Sonuc(ENUMIslemDurum.Basarili, kayit);
                }
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
        /// <param name="koordinatorlukKod"></param>
        /// <param name="birimKod"></param>
        /// <param name="durumKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, string koordinatorlukKod, string birimKod, int durumKod)
        {
            if (string.IsNullOrWhiteSpace(birimKod)) birimKod = "";
            var kayitlar = await _unitOfWork.ListeleAsync(k => k.KoordinatorlukKod == koordinatorlukKod && (k.BirimKod == birimKod || k.BirimKod == (string.IsNullOrWhiteSpace(birimKod) ? null : birimKod)) && k.Durum == durumKod, o => o.Adi);
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "", a => a.Birim, a => a.Koordinatorluk, a => a.AltSurecler);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                Surec aramaObj = Arac.DataTablesAramaNesne<Surec>(new Surec(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == aramaObj.KoordinatorlukKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == aramaObj.BirimKod);
                if (!string.IsNullOrWhiteSpace(aramaObj.Numara))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Numara.StartsWith(aramaObj.Numara));
                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(aramaObj.Adi));
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif);
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.Birim.Adi.Contains(dataTablesParam.searchValue)
                                                                || a.Numara.Contains(dataTablesParam.searchValue));

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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, Surec gelenNesne)
        {
            Surec islemYapilan = new Surec();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.KoordinatorlukKod))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.KoordinatorlukAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.Adi))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(gelenNesne.Adi))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.NumaraAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "KAYDET");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                gelenNesne.Adi = gelenNesne.Adi.Trim();

                //Koordinatörlük, Birim ve Süreç bilgileri aynı olan kayıt eklenmemeli.
                var kontrolListe = await _unitOfWork.ListeleAsync(c => c.KoordinatorlukKod == gelenNesne.KoordinatorlukKod &&
                                                                c.BirimKod == gelenNesne.BirimKod &&
                                                                c.Adi == gelenNesne.Adi);

                foreach (Surec item in kontrolListe)
                {
                    if (string.IsNullOrWhiteSpace(gelenNesne.Kod) || gelenNesne.Kod != item.Kod)
                    {
                        hata += "<li>" + _sharedResource["Kontrol.Duzenle.AyniSurecKayitli"] + "</li>";
                        break;
                    }
                }

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);

                //sureckaydet işleminde altsurecler dolu olursa hata veriyor EF den dolayı
                List<AltSurec> altSurecler = gelenNesne.AltSurecler;
                gelenNesne.AltSurecler = null;

                //Kayıt yapıldığında durum aktif değerini alsın
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    gelenNesne.Kod = Arac.GetGuid();
                    islemYapilan = await _unitOfWork.KayitEkleAsync(gelenNesne);
                }
                else
                {
                    var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                    if (eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    {
                        return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + _sharedResource["Kontrol.Kaydet.OnayliKaydedilemez"] + "</li></small>");
                    }

                    eskiKayit.KoordinatorlukKod = gelenNesne.KoordinatorlukKod;
                    eskiKayit.BirimKod = gelenNesne.BirimKod;
                    eskiKayit.Adi = gelenNesne.Adi;
                    eskiKayit.Numara = gelenNesne.Numara;
                    eskiKayit.Durum = gelenNesne.Durum;
                    eskiKayit.SurecSahibiPersonelKod = gelenNesne.SurecSahibiPersonelKod;
                    eskiKayit.SurecSahibiUnvanKod = gelenNesne.SurecSahibiUnvanKod;

                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
                }

                if (altSurecler != null)
                {
                    foreach (var item in altSurecler)
                    {
                        AltSurec giden = new AltSurec();
                        item.SurecKod = gelenNesne.Kod;
                        item.Durum = (int)ENUMDurum.Aktif;

                        await _serviceAltSurec.KaydetAsync(kullanan, item);
                    }
                }

                await _unitOfWork.KaydetAsync();
            }
            catch (System.Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li></small>");
            }

            return new Sonuc(ENUMIslemDurum.Basarili, _sharedResource["Bildirim.KayitBasarili"]);
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

                await _serviceAltSurec.SilAsync(kullanan, kod);

                await _unitOfWork.KaydetAsync();
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
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, Surec gelenNesne)
        {
            Surec islemYapilan = new Surec();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata = "<li>" + _sharedResource["Kontrol.Duzenle.KodAlaniBos"] + "</li>";

            hata = YetkisiVarmi(kullanan, "DURUM");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                var eskiKayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == gelenNesne.Kod);

                //Durum değişikliğine uygun mu?
                if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum == (int)ENUMDurum.Pasif)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.PasifKayitOnaylanamaz"] + "</li>";
                if (gelenNesne.Durum == (int)ENUMDurum.Onayli && eskiKayit.Durum == (int)ENUMDurum.Onayli)
                    hata = "<li>" + _sharedResource["Kontrol.DurumDegistir.ZatenOnayli"] + "</li>";

                if (hata != "")
                    return new Sonuc(ENUMIslemDurum.Uyari, hata);
                //**********************************

                eskiKayit.Durum = gelenNesne.Durum;
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
            bool yetki = Arac.YetkisiVarmi("PLANLAMAUNITESI,RISKSEKRETARYASI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }

    }
}