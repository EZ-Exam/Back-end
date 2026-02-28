using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace teamseven.EzExam.Repository.Basic
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        T GetById(string code);
        Task<T> GetByIdAsync(string code);
        T GetById(Guid code);
        Task<T> GetByIdAsync(Guid code);
        List<T> GetAll();
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        void Create(T entity);
        Task<int> CreateAsync(T entity);
        Task<TKey> CreateReturnKeyAsync<TKey>(T entity);
        Task<int> CreateAsyncWithCheckExist(T entity);

        void Update(T entity);
        Task<int> UpdateAsync(T entity);

        bool Remove(T entity);
        Task<bool> RemoveAsync(T entity);
        Task DeleteAsync(T entity);

        void PrepareCreate(T entity);
        void PrepareUpdate(T entity);
        void PrepareRemove(T entity);

        int Save();
        Task<int> SaveAsync();

        Task AddAsync(T entity);
    }
}
