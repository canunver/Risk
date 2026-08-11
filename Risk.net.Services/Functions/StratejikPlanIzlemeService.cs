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
using System;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// StratejikPlanIzleme iþlemlerinin yapýldýðý servis
    /// </summary>
    public class StratejikPlanIzlemeService : IStratejikPlanIzlemeService
    {
        /// <summary>
        /// IUnitOfWork<ViewStratejikPlanIzleme> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<ViewStratejikPlanIzleme> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.StratejikPlanIzlemeService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="serviceGosterge"></param>
        /// <param name="serviceTarihce"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public StratejikPlanIzlemeService(IUnitOfWork<ViewStratejikPlanIzleme> unitOfWork
            , IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen kaydýn tüm bilgisini döndüren metod
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
                var kayit = await _unitOfWork.KayitGetirAsync(c => c.Kod == kod, "StratejikPlan,StratejikPlan.Hedefler,StratejikPlan.Hedefler.Koordinatorluk,StratejikPlan.Hedefler.Birim,StratejikPlan.Hedefler.IsbirligiBirimler, StratejikPlan.Hedefler.IsbirligiBirimler.Koordinatorluk, StratejikPlan.Hedefler.IsbirligiBirimler.Birim");
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
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="dataTablesParam"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<object> TabloDoldurAsync(KullaniciDto kullanan, DataTablesParam dataTablesParam)
        {
            var aramaDegeri = dataTablesParam.searchValue;
            var selectData = await _unitOfWork.SorguHazirlaAsync(null, "StratejikPlan,Hedef.Koordinatorluk,Hedef.Birim,StratejikPlan.StratejikPlanDonem");

            if (aramaDegeri.StartsWith("GELISMIS_ARAMA:"))
            {
                ViewStratejikPlanIzleme aramaObj = Arac.DataTablesAramaNesne<ViewStratejikPlanIzleme>(new ViewStratejikPlanIzleme(), aramaDegeri);

                if (!string.IsNullOrWhiteSpace(aramaObj.SorguStratejikPlanDonemKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlan.StratejikPlanDonemKod == aramaObj.SorguStratejikPlanDonemKod);

                if (!string.IsNullOrWhiteSpace(aramaObj.SorguKoordinatorlukKod))
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Hedef.KoordinatorlukKod == aramaObj.SorguKoordinatorlukKod);

                if (aramaObj.Yil > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.KayitTarihi.Value.Year == aramaObj.Yil);

                if (aramaObj.Durum > 0)
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == aramaObj.Durum);
                else
                    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif);

            }
            else
            {
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.Durum == (int)ENUMDurum.Aktif);
                selectData = await _unitOfWork.KosulEkleAsync(selectData, a => //a.StratejikPlan.Koordinatorluk.Adi.Contains(dataTablesParam.searchValue) ||
                                                                                a.StratejikPlan.Amac.Contains(dataTablesParam.searchValue) ||
                                                                                a.KayitTarihi.Value.Year == Arac.ConvertToInt(dataTablesParam.searchValue, 0) ||
                                                                                a.Hedef.Adi.Contains(dataTablesParam.searchValue));
            }

            //if (!Arac.YetkisiVarmi("SISTEMYONETICISI,PLANLAMAUNITESI,RISKSEKRETARYASI", kullanan))
            //{
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlan.KoordinatorlukKod.Contains(kullanan.KoordinatorlukKod));
            //    selectData = await _unitOfWork.KosulEkleAsync(selectData, a => a.StratejikPlan.BirimKod.Contains(kullanan.BirimKod));
            //}

            return Arac.DataTablesJsonData(selectData, dataTablesParam);
        }
    }
}