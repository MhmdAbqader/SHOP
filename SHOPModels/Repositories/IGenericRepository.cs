using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SHOPModels.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T,bool>> criteria , string[]? includes =null );
        T GetById(int id);
        T GetById(Expression<Func<T,bool>> criteria , string? includeTable = null);
        void Add(T entity);
        //void Update(T entity);
        void Remove(T entity);
        void RomoveRange(IEnumerable<T> entities);
    }
}
