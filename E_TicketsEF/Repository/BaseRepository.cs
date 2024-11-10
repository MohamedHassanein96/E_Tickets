using E_TicketsCore.IRepository;
using E_TicketsEF.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_TicketsEF.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Edit(T entity)
        {
            _context.Set<T>().Update(entity);
            
        }


        public IEnumerable<T> Get(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeProps != null)
            {
                foreach (var includeProp in includeProps)
                {
                    query = query.Include(includeProp);
                }
            }
            if (expression!=null)
            {
                query = query.Where(expression);
            }
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            return query.ToList();

        }

        public T1 GetByCompositeKeys <T1>(params object[] keys) where T1 : class
        {
            var entity =  _context.Set<T1>().Find(keys);
            return entity;
        }

        public T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
            return Get(includeProps, expression, tracked).FirstOrDefault();
        }

        public IEnumerable<T> GetWithIncludes(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IQueryable<T>>? includes = null, bool tracked = true)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                query = includes(query);
            }
            if (expression != null)
            {
                query = query.Where(expression);
            }
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            return query.ToList();
        }

        
    }
}
