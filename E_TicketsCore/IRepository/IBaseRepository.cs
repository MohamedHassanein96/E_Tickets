using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsCore.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
        public IEnumerable<T> GetWithIncludes(
        Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IQueryable<T>>? includes = null,
        bool tracked = true
        );

        public IEnumerable<T> Get
        (
        Expression<Func<T, object>>[]? includeProps =null,
        Expression<Func<T, bool>>? expression = null,
        bool tracked= true
        );

        T? GetOne(
        Expression<Func<T, object>>[]? includeProps = null,
        Expression<Func<T, bool>>? expression = null,
        bool tracked = true
        );

        void Create(T entity);

        void Edit(T entity);

        void Delete(T entity);
        public T1 GetByCompositeKeys<T1>(params object[] keys) where T1 : class;



    }
}
