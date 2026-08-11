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
    /// CTEKoordinatorluk iþlemlerinin yapýldýðý servis
    /// </summary>
    public class CTEKoordinatorlukService : ICTEKoordinatorlukService
    {
        /// <summary>
        /// IUnitOfWork<CTEKoordinatorluk> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<CTEKoordinatorluk> _unitOfWork;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.CTEKoordinatorlukService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public CTEKoordinatorlukService(IUnitOfWork<CTEKoordinatorluk> unitOfWork, IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWork = unitOfWork;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametre ile talep edilen bilgilere ait kayýtlarýn listesini döndüren metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="kriter"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> ListeleAsync(KullaniciDto kullanan, CTEKoordinatorluk kriter)
        {
            string sql = @"
                    WITH CTE_Koordinatorluk AS (
                        SELECT Kod, Adi, BagliKod FROM ViewKoordinatorluk
                        WHERE BagliKod = '{BAGLIKOD}'
                    UNION ALL
                        SELECT e.Kod, e.Adi, e.BagliKod
                    FROM ViewKoordinatorluk e
                        INNER JOIN CTE_Koordinatorluk o ON o.Kod = e.BagliKod
                    )
                    SELECT Kod, Adi, BagliKod FROM CTE_Koordinatorluk;
                ";

            sql = sql.Replace("{BAGLIKOD}", kriter.BagliKod);

            try
            {
                var kayitlar = await _unitOfWork.SQLCalistirAsync(sql);
                if (kayitlar.Count > -1)
                {
                    return new Sonuc(ENUMIslemDurum.Basarili, kayitlar.Cast<object>().ToList());
                }
            }
            catch (Exception e)
            {

            }
            return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
        }

    }
}
