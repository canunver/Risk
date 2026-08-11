using Microsoft.EntityFrameworkCore;
using Risk.net.Data.DataContext;
using Risk.net.Data.Entities;
using Risk.net.Data.Interfaces;
using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

//https://www.gokhan-gokalp.com/generic-repository-ve-unit-of-work-kullanarak-basic-infrastructure-tasarlamak
namespace Risk.net.Data.Functions
{
    /// <summary>
    /// Veritabaný ile yapýlacak olan tüm iþlemleri, tek bir kanal aracýlýðý ile gerçekleþtirme ve hafýzada tutma iþlemlerini sunmaktadýr.Bu sayede iþlemlerin toplu halde gerçekleþtirilmesi ve hata durumunda geri alýnabilmesi saðlamaktadýr.
    /// </summary>
    public class UnitOfWork<TEntity> : IUnitOfWork<TEntity> where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// DatabaseContext servisine ulaþmak için kullanýlan deðiþken
        /// </summary>
        /// <remarks></remarks>
        private readonly DatabaseContext _context;

        /// <summary>
        /// <see cref="Risk.net.Data.Functions.UnitOfWork" /> 'ýn yeni bir örneðini baþlatan sýnýf
        /// </summary>
        /// <param name="context"></param>
        /// <remarks></remarks>
        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// entity'ye ait tabloya parametre olarak gelen entity sýnýfýnýn kayýt (insert) edilmesini saðlayan metot
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayýt edilen Entity nesnesi döndürür
        /// </returns>
        public async Task<TEntity> KayitEkleAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// entity'ye ait tabloya parametre olarak gelen entity sýnýfýnýn kayýt (update) edilmesini saðlayan metot
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayýt edilen Entity nesnesi döndürür
        /// </returns>
        public async Task<TEntity> GuncelleAsync(TEntity entity)
        {
            await Task.Run(() => { _context.Set<TEntity>().Update(entity); });
            return entity;
        }

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp uyuþan kayýtlarý döndürmeyi saðlayan metot
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="siralama"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<IList<TEntity>> ListeleAsync(Expression<Func<TEntity, bool>> kosul = null, Expression<Func<TEntity, object>> siralama = null, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            try
            {
                IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();
                if (kosul != null)
                {
                    query = query.Where(kosul);
                }

                if (includeProperties.Any())
                {
                    foreach (var includeProperty in includeProperties)
                    {
                        query = query.Include(includeProperty);
                    }
                }

                if (siralama != null)
                    query = query.OrderBy(siralama);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                string aa = ex.Message;

            }

            return null;

        }

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp uyuþan kaydýn (tek) döndürmeyi saðlayan metot
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="stringIncludes"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<TEntity> KayitGetirAsync(Expression<Func<TEntity, bool>> kosul, string stringIncludes, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (kosul != null)
            {
                query = query.Where(kosul);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            //includeProperties Linq tipinde deðer taþýyor, join (A ve B tablolarý) ile yapýlan sorguda B tablosuna baðlý kayýtlarýn
            //baðlý olduðu 3. derece kayýtlarý getirmek için kullanýlýyor. Örnek RiskEvreni->RiskEvreniRiskKategori->TanimRiskKategori->Adi
            string[] si = stringIncludes.Split(',');
            foreach (var item in si)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                query = query.Include(item.Trim());
            }

            return await query.SingleOrDefaultAsync();
        }

        /// <summary>
        /// entity'ye ait tablodan sorgulma yapýlmasý için koþullarýn IQueryable nesnesine eklenmesini saðlayan metot
        /// </summary>
        /// <param name="kosul"></param>
        /// <param name="stringIncludes"></param>
        /// <param name="includeProperties"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<IQueryable<TEntity>> SorguHazirlaAsync(Expression<Func<TEntity, bool>> kosul, string stringIncludes, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = await Task.Run(() => _context.Set<TEntity>());

            if (kosul != null)
            {
                query = query.Where(kosul);
            }

            if (includeProperties.Any())
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            //includeProperties Linq tipinde deðer taþýyor, join (A ve B tablolarý) ile yapýlan sorguda B tablosuna baðlý kayýtlarýn
            //baðlý olduðu 3. derece kayýtlarý getirmek için kullanýlýyor. Örnek RiskEvreni->RiskEvreniRiskKategori->TanimRiskKategori->Adi
            string[] si = stringIncludes.Split(',');
            foreach (var item in si)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                query = query.Include(item.Trim());
            }

            return query;
        }

        /// <summary>
        /// Önceden oluþturulan IQueryable nesnesine ek koþullarýn eklenmesini saðlayan metot
        /// </summary>
        /// <param name="query"></param>
        /// <param name="kosul"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<IQueryable<TEntity>> KosulEkleAsync(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> kosul)
        {
            if (kosul != null)
            {
                query = await Task.Run(() => query.Where(kosul));
            }

            return query;
        }

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp uyuþan kayýtlarý silmeyi saðlayan metot
        /// </summary>
        /// <param name="kosul"></param>
        public async Task SilAsync(Expression<Func<TEntity, bool>> kosul)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().Where(kosul);
            await Task.Run(() => { _context.Set<TEntity>().RemoveRange(query.AsNoTracking()); });
        }

        /// <summary>
        /// entity'ye ait tabloda, parametre olarak gelen koþul ile sorgulayýp veri olmasýný kontrol eden metot
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="kosul"></param>
        /// <returns>
        /// bool dondürür
        /// </returns>
        public async Task<bool> VarmiAsync(Expression<Func<TEntity, bool>> kosul)
        {
            return await _context.Set<TEntity>().AnyAsync(kosul);
        }

        /// <summary>
        /// entity'ye ait tablodaki kayýt sayýsýný almayý saðlayan metot
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<int> KayitSayisiAsync()
        {
            return await _context.Set<TEntity>().CountAsync();
        }

        /// <summary>
        /// entity'ye ait tablodan parametre olarak gelen koþul ile sorgulayýp kayýt sayýsýný  almayý saðlayan metot
        /// </summary>
        /// <param name="kosul"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<int> KayitSayisiAsync(Expression<Func<TEntity, bool>> kosul)
        {
            return await _context.Set<TEntity>().CountAsync(kosul);
        }

        /// <summary>
        /// Raporlara ve grafiklere ait sql cümlelerinin çalýþtýrýlmasýný saðlayan metot
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>
        /// Sorgulama sonucunda alýnan verilerin Entity tipinde Listesi
        /// </returns>
        public async Task<IList<TEntity>> SQLCalistirAsync(string sql)
        {
            try
            {
                IQueryable<TEntity> query = _context.Set<TEntity>().FromSqlRaw(sql).AsNoTracking();

                return await query.ToListAsync();
            }
            catch (Exception e)
            {
                string hata = e.Message;
            }

            return null;
        }

        /// <summary>
        /// Toplu güncelleme veya silme iþlemlerinin yapýlmasý için sql cümlelerinin çalýþtýrýlmasýný saðlayan metot
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<int> SQLNonQueryCalistirAsync(string sql)
        {
            try
            {
                return await _context.Database.ExecuteSqlRawAsync(sql);

            }
            catch (Exception e)
            {
                string hata = e.Message;
            }

            return 0;
        }

        /// <summary>
        /// Kaydet, Güncelleme ve Silme iþlemleri sonucunda commit iþleminin yapýlmasýný saðlayan metot
        /// </summary>
        /// <returns>
        /// Kayýtsayýsýný döndürür
        /// </returns>
        public async Task<int> KaydetAsync()
        {
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Baðlantý kaynakðýný serbest býrakýlmasýný saðlayan metod
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }


    }
}
