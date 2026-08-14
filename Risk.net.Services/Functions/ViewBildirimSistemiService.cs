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
using System.Text.Json.Serialization;
using System;
using System.Linq.Expressions;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// ViewBildirimSistemi işlemlerinin yapıldığı servis
    /// </summary>
    public class ViewBildirimSistemiService : IViewBildirimSistemiService
    {
        /// <summary>
        /// IUnitOfWork<ViewBildirimSistemi> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewBildirimSistemi> _unitOfWork;
        /// <summary>
        /// ICTEKoordinatorlukService servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly ICTEKoordinatorlukService _serviceCTEKoordinatorluk;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaşmak için kullanılan değişken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.ViewBildirimSistemiService" /> 'ın yeni bir örneğini başlatan sınıf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceCTEKoordinatorluk"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public ViewBildirimSistemiService(IUnitOfWork<ViewBildirimSistemi> unitOfWork,
            ICTEKoordinatorlukService serviceCTEKoordinatorluk,
            IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _serviceCTEKoordinatorluk = serviceCTEKoordinatorluk;
            _sharedResource = sharedResource;
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
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan", a => a.Koordinatorluk);

            //if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            //{
            //    BildirimSistemi aramaObj = Arac.DataTablesAramaNesne<BildirimSistemi>(new BildirimSistemi(), aramaDegeri);

            //    if (!string.IsNullOrWhiteSpace(aramaObj.KoordinatorlukKod))
            //        selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod==aramaObj.KoordinatorlukKod);
            //}
            //else
            //{
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue));
            //}

            if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
            {
                //List<string> koordinatorlukKodlar = new List<string>();
                //koordinatorlukKodlar.Add(kullanan.KoordinatorlukKod);

                //var kriterKoordinator = new CTEKoordinatorluk() { BagliKod = kullanan.KoordinatorlukKod };
                //Sonuc koordinatorSonuc = await _serviceCTEKoordinatorluk.ListeleAsync(kullanan, kriterKoordinator);
                //if (koordinatorSonuc.IslemSonuc)
                //{
                //    foreach (CTEKoordinatorluk item in koordinatorSonuc.Liste)
                //    {
                //        koordinatorlukKodlar.Add(item.Kod);
                //    }
                //}

                //selectData = await _unitOfWork.KosulEkleAsync(selectData, a => koordinatorlukKodlar.ToArray().Contains(a.KoordinatorlukKod));
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kullanan.KoordinatorlukKod);

                if (!string.IsNullOrWhiteSpace(kullanan.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kullanan.BirimKod);
            }

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OnaylayacakYetki == kullanan.AktifRolKod);

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMBildirimSistemiDurum.OnayaGonderdi || a.Durum > (int)ENUMBildirimSistemiDurum.OnayDisiBildirimler);

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, BildirimSistemi kriter)
        {
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "IslemYapan", a => a.Koordinatorluk);

            if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
            {
                //List<string> koordinatorlukKodlar = new List<string>();
                //koordinatorlukKodlar.Add(kullanan.KoordinatorlukKod);

                //var kriterKoordinator = new CTEKoordinatorluk() { BagliKod = kullanan.KoordinatorlukKod };
                //Sonuc koordinatorSonuc = await _serviceCTEKoordinatorluk.ListeleAsync(kullanan, kriterKoordinator);
                //if (koordinatorSonuc.IslemSonuc)
                //{
                //    foreach (CTEKoordinatorluk item in koordinatorSonuc.Liste)
                //    {
                //        koordinatorlukKodlar.Add(item.Kod);
                //    }
                //}

                //selectData = await _unitOfWork.KosulEkleAsync(selectData, a => koordinatorlukKodlar.ToArray().Contains(a.KoordinatorlukKod));

                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KoordinatorlukKod == kullanan.KoordinatorlukKod);

                if (!string.IsNullOrWhiteSpace(kullanan.BirimKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BirimKod == kullanan.BirimKod);
            }

            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.BelgeTipi == kriter.BelgeTipi);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.OnaylayacakYetki == kullanan.AktifRolKod);
            selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMBildirimSistemiDurum.OnayaGonderdi || a.Durum > (int)ENUMBildirimSistemiDurum.OnayDisiBildirimler);

            var kayitlar = selectData.ToList();

            if (kayitlar.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Onay işlemleri mail gönderilecek kayıtların listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeMailGonderilecekKayitlarAsync()
        {
            /*
            1- Risk evreni, 10 gün geçen onaylanmamış kayıtlar için mail gönderilecek
            2- Risk yönetimi için, azaltma planı seçilirse ve 5 gün içinde azaltma planı oluşturulmazsa Risk Sahibine uyarı maili gidecek.
            3- Risk azaltma planı, 10 gün geçen onaylanmamış kayıtlar için mail gönderilecek, azaltma planı sorumlusunada ayrı mail gidecek.
            4- Risk azaltma planı, riskin sahibine azaltma planının bitiş tarihine 1 hafta kala “Azaltma planının bitiş tarihi yaklaşıyor.” şeklinde mail gönderilir. 	
            5- Yetkili risk görevlisine 3 ayda 1 "İç Kontrol Zayıflıkları Raporunu UYG'ye bildiriniz." maili gönderilir.
            6- Yetkili risk görevlisine 6 ayda 1 "Onaylanmış tüm riskler ve ilgili azaltma planlarının listesini UYG'ye bildiriniz." maili gönderilir.
            7- Risk sekretaryasına 3 ayda 1 "Yüksek ve orta seviyedeki aktif riskleri ve azaltma planlarını merkez ve il koordinatörlüklerine bildiriniz." maili gönderilir.
            8- Risk sekretaryasına yılda 1 "Risk Yönetimi Prosedürünü gözden geçiriniz." maili gönderilir.
            9- Yıllık Risk Beyannamesini 1 ocak tarihinde imzalayın diye mail atılacak
            10- Yıllık Risk Beyannamesini 1 ay içinde imzalanmazsa il ve merkez koordinatörlerine hatırlatma maili gönderilecek

             */
            var sql = "SELECT Kod, BelgeTipi, BelgeKod, KoordinatorlukKod, BirimKod, IslemYapanKod, IslemTarihi, OnaylayacakYetki, Durum, Aciklama, Islem FROM ViewBildirimSistemiMailGonderilecekler";

            var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);

            if (kayitlar?.Count > -1)
            {
                return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayıtların sayısını döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="gelenNesne"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<int> BildirimSayisiAsync(KullaniciDto kullanan)
        {
            Expression<Func<ViewBildirimSistemi, bool>> predicate = null;

            predicate = a => a.OnaylayacakYetki == kullanan.AktifRolKod && (a.Durum == (int)ENUMBildirimSistemiDurum.OnayaGonderdi || a.Durum > (int)ENUMBildirimSistemiDurum.OnayDisiBildirimler);

            if (!Arac.YetkisiVarmi("RISKSEKRETARYASI", kullanan))
            {
                predicate = predicate.And(a => a.KoordinatorlukKod == kullanan.KoordinatorlukKod);

                if (!string.IsNullOrWhiteSpace(kullanan.BirimKod))
                    predicate = predicate.And(a => a.BirimKod == kullanan.BirimKod);
            }

            int adet = await _unitOfWork.KayitSayisiAsync(predicate);

            return adet;
        }
    }
}
