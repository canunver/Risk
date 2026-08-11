using Microsoft.Extensions.Localization;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Services.Interfaces;
using Risk.net.Utilities.Functions;
using Risk.net.Services.Objects;
using System.Linq;
using System.Threading.Tasks;
using Risk.net.Utilities.Objects;
using System.IO;
using System;
using System.Web;
using MimeKit;
using System.Linq.Expressions;

namespace Risk.net.Services.Functions
{
    /// <summary>
    /// RiskSahibiDegistir iþlemlerinin yapýldýðý servis
    /// </summary>
    public class RiskSahibiDegistirService : IRiskSahibiDegistirService
    {
        /// <summary>
        /// IUnitOfWork<RiskEvreni> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IUnitOfWork<RiskEvreni> _unitOfWorkRiskEvreni;
        /// <summary>
        /// IStringLocalizer<CustomResource> servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly IStringLocalizer<CustomResource> _sharedResource;

        /// <summary>
        /// <see cref="Risk.net.Services.Functions.RiskSahibiDegistirService" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="unitOfWorkRiskEvreni"></param>
        /// <param name="sharedResource"></param>
        /// <remarks></remarks>
        public RiskSahibiDegistirService(IUnitOfWork<RiskEvreni> unitOfWorkRiskEvreni,
                                IStringLocalizer<CustomResource> sharedResource)
        {
            _unitOfWorkRiskEvreni = unitOfWorkRiskEvreni;
            _sharedResource = sharedResource;
        }

        /// <summary>
        /// Istemciden parametere ile gönderilen bilgilerine yeniKod bilgisini eskiKod bilgisi ile güncelleyen metod
        /// </summary>
        /// <param name="kullanan"></param>
        /// <param name="eskiKod"></param>
        /// <param name="yeniKod"></param>
        /// <returns>
        /// Sonuc nesnesi döndürür
        /// </returns>
        public async Task<Sonuc> KaydetAsync(KullaniciDto kullanan, string eskiKod, string yeniKod)
        {
            string hata = "";

            if (string.IsNullOrWhiteSpace(eskiKod))
                hata += "<li>" + _sharedResource["Kontrol.RiskSahibiDegistir.EskiKodBos"] + "</li>";
            if (string.IsNullOrWhiteSpace(yeniKod))
                hata += "<li>" + _sharedResource["Kontrol.RiskSahibiDegistir.YeniKodBos"] + "</li>";
            if (!string.IsNullOrWhiteSpace(eskiKod) && !string.IsNullOrWhiteSpace(yeniKod) && eskiKod == yeniKod)
                hata += "<li>" + _sharedResource["Kontrol.RiskSahibiDegistir.EsitOlamaz"] + "</li>";

            if (hata != "")
                return new Sonuc(ENUMIslemDurum.Uyari, hata);


            eskiKod = Arac.TirnakYoket(eskiKod);
            yeniKod = Arac.TirnakYoket(yeniKod);

            string sql = @"UPDATE RiskEvreni SET RiskSahibiKod = '{yeniKod}' WHERE RiskSahibiKod = '{eskiKod}'";

            sql = sql.Replace("{eskiKod}", eskiKod).Replace("{yeniKod}", yeniKod);

            try
            {
                var sonucAdet = await _unitOfWorkRiskEvreni.SQLNonQueryCalistirAsync(sql);
                if (sonucAdet > 0)
                    return new Sonuc(ENUMIslemDurum.Basarili, sonucAdet + " " + _sharedResource["Bildirim.RiskSahibiDegistir.KayitBasarili"]);
                else
                    return new Sonuc(ENUMIslemDurum.Hata, _sharedResource["Bildirim.KayitBulunamadi"]);
            }
            catch (Exception ex)
            {
                return new Sonuc(ENUMIslemDurum.Hata, "<small><li>" + ex.Message + "</li><li>" + ex.InnerException + "</li></small>");
            }

        }

    }
}
