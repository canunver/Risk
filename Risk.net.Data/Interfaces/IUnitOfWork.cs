using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Risk.net.Data.Interfaces
{
    /// <summary>
    /// Veritabanı ile yapılacak olan tüm işlemleri, tek bir kanal aracılığı ile gerçekleştirme ve hafızada tutma işlemlerini sunmaktadır.Bu sayede işlemlerin toplu halde gerçekleştirilmesi ve hata durumunda geri alınabilmesi sağlamaktadır.
    /// </summary>
    public interface IUnitOfWork<TEntity> : IAsyncDisposable where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koşul ile sorgulayıp uyuşan kaydın (tek) döndürmeyi sağlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="stringIncludes"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<TEntity> KayitGetirAsync(Expression<Func<TEntity, bool>> kosul, string stringIncludes = "", params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koşul ile sorgulayıp uyuşan kayıtları döndürmeyi sağlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="siralama"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<IList<TEntity>> ListeleAsync(Expression<Func<TEntity, bool>> kosul = null, Expression<Func<TEntity, object>> siralama = null, params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// entity'ye ait tabloya parametre olarak gelen entity sınıfının kayıt (insert) edilmesini sağlayan metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayıt edilen Entity nesnesi döndürür
        /// </returns>
        Task<TEntity> KayitEkleAsync(TEntity entity);

        /// <summary>
        /// entity'ye ait tabloya parametre olarak gelen entity sınıfının kayıt (update) edilmesini sağlayan metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayıt edilen Entity nesnesi döndürür
        /// </returns>
        Task<TEntity> GuncelleAsync(TEntity entity);

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koşul ile sorgulayıp uyuşan kayıtları silmeyi sağlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        Task SilAsync(Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// entity'ye ait tabloda, parametre olarak gelen koşul ile sorgulayıp veri olmasını kontrol eden metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="kosul"></param>
        /// <returns>
        /// bool dondürür
        /// </returns>
        Task<bool> VarmiAsync(Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koşul ile sorgulayıp kayıt sayısını  almayı sağlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<int> KayitSayisiAsync(Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// entity'ye ait tablodaki kayıt sayısını almayı sağlayan metodun arayüzü
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<int> KayitSayisiAsync();

        /// <summary>
        /// entity'ye ait tablodan sorgulma yapılması için koşulların IQueryable nesnesine eklenmesini sağlayan metodun arayüzü
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="stringIncludes"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<IQueryable<TEntity>> SorguHazirlaAsync(Expression<Func<TEntity, bool>> kosul, string stringIncludes = "", params Expression<Func<TEntity, object>>[] includeProperties);

        /// <summary>
        /// Önceden oluşturulan IQueryable nesnesine ek koşulların eklenmesini sağlayan metodun arayüzü
        /// </summary>
        /// <param name="query"></param>
        /// <param name="kosul"></param>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<IQueryable<TEntity>> KosulEkleAsync(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> kosul);

        /// <summary>
        /// Raporlara ve grafiklere ait sql cümlelerinin çalıştırılmasını sağlayan metodun arayüzü
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>
        /// Sorgulama sonucunda alınan verilerin Entity tipinde Listesi
        /// </returns>
        Task<IList<TEntity>> SQLCalistirAsync(string sql);

        /// <summary>
        /// Toplu güncelleme veya silme işlemlerinin yapılması için sql cümlelerinin çalıştırılmasını sağlayan metodun arayüzü
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<int> SQLNonQueryCalistirAsync(string sql);

        /// <summary>
        /// Kaydet, Güncelleme ve Silme işlemleri sonucunda commit işleminin yapılmasını sağlayan metodun arayüzü
        /// </summary>
        /// <returns>
        /// Kayıtsayısını döndürür
        /// </returns>
        Task<int> KaydetAsync();
    }
}
