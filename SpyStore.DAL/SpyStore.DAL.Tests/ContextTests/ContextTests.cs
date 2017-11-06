
using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Xunit;

using SpyStore.DAL.EF;
using SpyStore.Models.Entities;

namespace SpyStore.DAL.Tests.ContextTests
{
    [Collection("SpyStore.DAL")]
    public class ContextTests : IDisposable
    {
        private readonly StoreContext _storeContext;

        public ContextTests ()
        {
            _storeContext = new StoreContext();
            CleanDatabase();
        }

        public void Dispose()
        {
            CleanDatabase();
            _storeContext.Dispose();
        }

        private void CleanDatabase()
        {
            _storeContext.Database.ExecuteSqlCommand("Delete from Store.Categories");
            _storeContext.Database.ExecuteSqlCommand($"DBCC CHECKIDENT (\"Store.Categories\", RESEED, -1);");
        }

        [Fact]
        public void FirstTest()
        {
            Assert.True(true);
        }

        [Fact]
        public void ShouldAddACategoryWithDbSet()
        {
            var category = new Category { CategoryName = "Foo" };
            _storeContext.Categories.Add(category);
            Assert.Equal(EntityState.Added, _storeContext.Entry(category).State);
            Assert.True(category.Id < 0);
            Assert.Null(category.TimeStamp);
            _storeContext.SaveChanges();

            Assert.Equal(EntityState.Unchanged, _storeContext.Entry(category).State);
            Assert.True(category.Id == 0);
            Assert.NotNull(category.TimeStamp);
            Assert.Equal(1, _storeContext.Categories.Count());
        }

        [Fact]
        public void ShouldAddACategoryWithContext()
        {
            var category = new Category { CategoryName = "Foo" };
            _storeContext.Add(category);
            Assert.Equal(EntityState.Added, _storeContext.Entry(category).State);
            Assert.True(category.Id < 0);
            Assert.Null(category.TimeStamp);
            _storeContext.SaveChanges();

            Assert.Equal(EntityState.Unchanged, _storeContext.Entry(category).State);
            Assert.True(category.Id == 0);
            Assert.NotNull(category.TimeStamp);
            Assert.Equal(1, _storeContext.Categories.Count());
        }

        [Fact]
        public void ShouldGetAllCategoriesOrderByName()
        {
            _storeContext.Add(new Category { CategoryName = "Foo" });
            _storeContext.Add(new Category { CategoryName = "Bar" });
            _storeContext.SaveChanges();

            var categories = _storeContext.Categories.OrderBy(c => c.CategoryName).ToList();
            Assert.Equal(2, categories.Count());
            Assert.Equal("Bar",categories[0].CategoryName);
            Assert.Equal("Foo", categories[1].CategoryName);
        }

        [Fact]
        public void ShouldUpdateACategory()
        {
            var category = new Category { CategoryName = "Foo" };
            _storeContext.Add(category);
            _storeContext.SaveChanges();

            category.CategoryName = "Bar";
            _storeContext.Update(category);
            Assert.Equal(EntityState.Modified, _storeContext.Entry(category).State);
            _storeContext.SaveChanges();

            Assert.Equal(EntityState.Unchanged, _storeContext.Entry(category).State);
            
            using (var context = new StoreContext ())
            {
                Assert.Equal("Bar", context.Categories.First().CategoryName);
            }
        }

        [Fact]
        public void ShouldNotUpdateANonAttachedCategory()
        {
            var category = new Category { CategoryName = "Foo" };
            _storeContext.Add(category);

            category.CategoryName = "Bar";
            Assert.Throws<InvalidOperationException>(() => _storeContext.Update(category));
        }

        [Fact]
        public void ShouldDeleteACategory()
        {
            var category = new Category { CategoryName = "Foo" };
            _storeContext.Add(category);
            _storeContext.SaveChanges();
            Assert.Equal(1, _storeContext.Categories.Count());

            _storeContext.Remove(category);
            Assert.Equal(EntityState.Deleted, _storeContext.Entry(category).State);
            _storeContext.SaveChanges();

            Assert.Equal(EntityState.Detached, _storeContext.Entry(category).State);
            Assert.Equal(0, _storeContext.Categories.Count());
        }

        [Fact]
        public void ShouldDeleteACategoryWithTimestampData()
        {
            var category = new Category { CategoryName = "Foo" };
            _storeContext.Add(category);
            _storeContext.SaveChanges();

            var delCategory = new Category { Id = category.Id, TimeStamp = category.TimeStamp };
            using (var context = new StoreContext())
            {
                context.Entry(delCategory).State = EntityState.Deleted;
                var affected = context.SaveChanges();
                Assert.Equal(1, affected);
            }
        }

        [Fact]
        public void ShouldNotDeleteACategoryWithoutTimestampData()
        {
            var category = new Category { CategoryName = "Foo" };
            _storeContext.Add(category);
            _storeContext.SaveChanges();

            var delCategory = new Category { Id = category.Id };
            using (var context = new StoreContext())
            {
                context.Entry(delCategory).State = EntityState.Deleted;
                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());
                Assert.Equal(1, ex.Entries.Count);
                Assert.Equal(category.Id, ((Category)ex.Entries[0].Entity).Id);
            }
        }
    }
}
