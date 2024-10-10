using Microsoft.EntityFrameworkCore;
using SHOPDataAccessLayer.Data;
using SHOPModels.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SHOPDataAccessLayer.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); 
        }
        public void Add(T entity)
        {
            //_dbSet.Add(entity);
            _context.Set<T>().Add(entity);
        }

        public IEnumerable<T> GetAll()
        {
            //IQueryable<T> query = _dbSet.AsQueryable();
            IQueryable<T> query = _dbSet;

            return query.ToList();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> criteria, string[]? includes = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (criteria != null) {
                query = query.Where(criteria);
            }
            if (includes?.Count() > 0) {
                foreach (var res in includes) 
                {
                    query = query.Include(res);
                }
            }
            return query.ToList();
        }

        public T GetById(int id)
        {
          return _dbSet.Find(id);
        }

        public T GetById(Expression<Func<T, bool>> criteria, string? includeTable = null)
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (criteria != null)
            {
                query = query.Where(criteria);
            }
            if (includeTable != null)
            {
                foreach (var res in includeTable.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(res);
                }
            }
            //return query.SingleOrDefault(); // i change that when creating the cart summary when i need to retun the user from shoppingCart
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void RomoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
    }
}
