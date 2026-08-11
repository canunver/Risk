using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Risk.net.Data.Interfaces
{
    /// <summary>
    /// Veritabaný ile yapýlacak olan tüm iþlemleri, tek bir kanal aracýlýðý ile gerçekleþtirme ve hafýzada tutma iþlemlerini sunmaktadýr.Bu sayede iþlemlerin toplu halde gerçekleþtirilmesi ve hata durumunda geri alýnabilmesi saðlamaktadýr.
    /// </summary>
    public interface IUnitOfWork<TEntity> : IAsyncDisposable where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp uyuþan kaydýn (tek) döndürmeyi saðlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="stringIncludes"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<TEntity> KayitGetirAsync(Expression<Func<TEntity, bool>> kosul, string stringIncludes = "", params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp uyuþan kayýtlarý döndürmeyi saðlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="siralama"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<IList<TEntity>> ListeleAsync(Expression<Func<TEntity, bool>> kosul = null, Expression<Func<TEntity, object>> siralama = null, params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// entity'ye ait tabloya parametre olarak gelen entity sýnýfýnýn kayýt (insert) edilmesini saðlayan metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayýt edilen Entity nesnesi döndürür
        /// </returns>
        Task<TEntity> KayitEkleAsync(TEntity entity);

        /// <summary>
        /// entity'ye ait tabloya parametre olarak gelen entity sýnýfýnýn kayýt (update) edilmesini saðlayan metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayýt edilen Entity nesnesi döndürür
        /// </returns>
        Task<TEntity> GuncelleAsync(TEntity entity);

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp uyuþan kayýtlarý silmeyi saðlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        Task SilAsync(Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// entity'ye ait tabloda, parametre olarak gelen koþul ile sorgulayýp veri olmasýný kontrol eden metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="kosul"></param>
        /// <returns>
        /// bool dondürür
        /// </returns>
        Task<bool> VarmiAsync(Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp kayýt sayýsýný  almayý saðlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<int> KayitSayisiAsync(Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// entity'ye ait tablodaki kayýt sayýsýný almayý saðlayan metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<int> KayitSayisiAsync();

        /// <summary>
        /// entity'ye ait tablodan sorgulma yapýlmasý için koþullarýn IQueryable nesnesine eklenmesini saðlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="stringIncludes"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<IQueryable<TEntity>> SorguHazirlaAsync(Expression<Func<TEntity, bool>> kosul, string stringIncludes = "", params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// Önceden oluþturulan IQueryable nesnesine ek koþullarýn eklenmesini saðlayan metodun arayüzü
        /// </summary>
        /// <param name="query"></param>
        /// <param name="kosul"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<IQueryable<TEntity>> KosulEkleAsync(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// Raporlara ve grafiklere ait sql cümlelerinin çalýþtýrýlmasýný saðlayan metodun arayüzü
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>
        /// Sorgulama sonucunda alýnan verilerin Entity tipinde Listesi
        /// </returns>
        Task<IList<TEntity>> SQLCalistirAsync(string sql);

        /// <summary>
        /// Toplu güncelleme veya silme iþlemlerinin yapýlmasý için sql cümlelerinin çalýþtýrýlmasýný saðlayan metodun arayüzü
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<int> SQLNonQueryCalistirAsync(string sql);

        /// <summary>
        /// Kaydet, Güncelleme ve Silme iþlemleri sonucunda commit iþleminin yapýlmasýný saðlayan metodun arayüzü
        /// </summary>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        Task<int> KaydetAsync();
    }
}
