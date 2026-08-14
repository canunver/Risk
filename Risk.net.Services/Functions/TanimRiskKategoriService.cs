using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// TanimRiskKategori işlemlerinin yapıldığı servis
    /// </summary>
    public class TanimRiskKategoriService : ITanimRiskKategoriService
    {
        /// <summary>
        /// IUnitOfWork<TanimRiskKategori> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<TanimRiskKategori> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.TanimRiskKategoriService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public TanimRiskKategoriService(IUnitOfWork<TanimRiskKategori> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
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
        /// <param name="durumKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, int durumKod)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.Durum == durumKod, o => o.Adi);
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null);

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                TanimRiskKategori aramaObj = Arac.DataTablesAramaNesne<TanimRiskKategori>(new TanimRiskKategori(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.Adi))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(aramaObj.Adi));
                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif || a.Durum == (int)ENUMDurum.Onayli);
            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif || a.Durum == (int)ENUMDurum.Onayli);
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Adi.Contains(dataTablesParam.searchValue));
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
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, TanimRiskKategori gelenNesne)
        {
            TanimRiskKategori islemYapilan = new TanimRiskKategori();

            string hata = "";

            if (string.IsNullOrWhiteSpace(gelenNesne.Adi))
                hata += "<li>" + _sharedResource["Kontrol.Duzenle.AdiAlaniBos"] + "</li>";

            if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                hata += YetkisiVarmi(kullanan, "YENI");
            else
                hata += YetkisiVarmi(kullanan, "GUNCELLEME");

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);

            try
            {
                gelenNesne.Durum = (int)ENUMDurum.Aktif;

                if (string.IsNullOrWhiteSpace(gelenNesne.Kod))
                {
                    Sonuc kontrol = await VarMiAsync(kullanan, gelenNesne.Adi);
                    if (kontrol.IslemSonuc)
                    {
                        if (kontrol.Liste.Count > 0)
                        {
                            hata = "Daha önce kaydedilmiş. Aynı isimde kayıt yapamazsınız.";
                            return new Sonuc(ENUMIslemDurum.Uyari, hata);
                        }
                    }

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

                    eskiKayit.Adi = gelenNesne.Adi;
                    eskiKayit.Durum = gelenNesne.Durum;
                    islemYapilan = await _unitOfWork.GuncelleAsync(eskiKayit);
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
        public async Task<Sonuc> DurumDegistirAsync(KullaniciDto kullanan, TanimRiskKategori gelenNesne)
        {
            TanimRiskKategori islemYapilan = new TanimRiskKategori();

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

                if (gelenNesne.Durum == (int)ENUMDurum.Pasif && eskiKayit.Durum == (int)ENUMDurum.Onayli)
                {
                    hata = YetkisiVarmi(kullanan, "ONAY");
                }

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
        /// Istemciden parametere ile talep edilen bilgilere göre kayıt listesisi döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="adi"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        private async Task<Sonuc> VarMiAsync(KullaniciDto kullanan, string adi)
        {
            var kayitlar = await _unitOfWork.ListeleAsync(a => a.Adi == adi);
            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
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
            bool yetki = false;

            if (tur == "YENI" || tur == "SIL")
                yetki = Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan);
            else if (tur == "ONAY")
                yetki = Arac.YetkisiVarmi("YETKILIRISKGOREVLISI", kullanan);
            else
                yetki = Arac.YetkisiVarmi("RISKSEKRETARYASI,YETKILIRISKGOREVLISI", kullanan);

            if (yetki)
                return "";
            else
                return "<li>" + _sharedResource["Kontrol.YetkiYok"] + "</li>";
        }
    }
}
