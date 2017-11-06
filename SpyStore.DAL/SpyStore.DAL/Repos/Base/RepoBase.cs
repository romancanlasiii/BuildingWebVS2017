using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using SpyStore.Models.Entities.Base;
using SpyStore.DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SpyStore.DAL.Repos.Base
{
    public class RepoBase<T> : IDisposable, IRepo<T> where T : EntityBase, new()
    {
        #region Members
        public StoreContext Context => _storeContext;
        protected readonly StoreContext _storeContext;
        protected DbSet<T> _table;
        private bool _disposed = false;
        #endregion

        #region Constructor
        protected RepoBase ()
        {
            _storeContext = new StoreContext();
            _table = _storeContext.Set<T>();
        }

        protected RepoBase(DbContextOptions<StoreContext> options)
        {
            _storeContext = new StoreContext(options);
            _table = _storeContext.Set<T>();
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose (bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {

            }

            _storeContext.Dispose();
            _disposed = true;
        }
        #endregion

        #region Methods
        public int Count => _table.Count();

        public bool HasChanges => _storeContext.ChangeTracker.HasChanges();

        public virtual int Add(T entity, bool persist = true)
        {
            _table.Add(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            _table.AddRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public virtual int Delete(T entity, bool persist = true)
        {
            _table.Remove(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int Delete(int id, byte[] timeStamp, bool persist = true)
        {
            var entry = GetEntryFromChangeTracker(id);
            if (entry != null)
            {
                if (entry.TimeStamp == timeStamp)
                    return Delete(entry, persist);

                throw new Exception("Unable to delete due to concurrency violation.");
            }

            _storeContext.Entry(new T { Id = id, TimeStamp = timeStamp }).State = EntityState.Deleted;
            return persist ? SaveChanges() : 0;
        }

        public virtual int DeleteRange(IEnumerable<T> entities, bool persist = true)
        {
            _table.RemoveRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public T Find(int? id) => _table.Find(id);

        public virtual IEnumerable<T> GetAll() => _table;

        public virtual T GetFirst() => _table.First();

        public virtual IEnumerable<T> GetRange(int skip, int take) => GetRange(_table, skip, take);
                

        public virtual int SaveChanges()
        {
            try
            {
                return _storeContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //A concurrency error occurred
                //Should handle intelligently
                Console.WriteLine(ex);
                throw;
            }
            catch (RetryLimitExceededException ex)
            {
                //DbResiliency retry limit exceeded
                //Should handle intelligently
                Console.WriteLine(ex);
                throw;
            }
            catch (Exception ex)
            {
                //Should handle intelligently
                Console.WriteLine(ex);
                throw;
            }
        }

        public virtual int Update(T entity, bool persist = true)
        {
            _table.Update(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            _table.UpdateRange(entities);
            return persist ? SaveChanges() : 0;
        }
        #endregion

        #region Internal Methods 
        internal T GetEntryFromChangeTracker(int id)
        {
            return _storeContext.ChangeTracker.Entries<T>().Select((EntityEntry e) => (T)e.Entity)
                    .FirstOrDefault(x => x.Id == id);
        }

        internal IEnumerable<T> GetRange(IQueryable<T> query, int skip, int take)
                                    => query.Skip(skip).Take(take);
                    
        #endregion
    }
}
