using System;
using System.Collections.Generic;
using System.Text;
using SpyStore.DAL.Repos.Base;
using SpyStore.Models.Entities;
using SpyStore.DAL.EF;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SpyStore.DAL.Repos
{
    public class CategoryRepo : RepoBase<Category>
    {
        #region Constructor
        public CategoryRepo(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

        public CategoryRepo()
        {
        }
        #endregion

        #region Methods
        public override IEnumerable<Category> GetAll() 
            => _table.OrderBy(x => x.CategoryName);

        public override IEnumerable<Category> GetRange(int skip, int take) 
            => GetRange(_table.OrderBy(x => x.CategoryName), skip, take);
        #endregion
    }
}
