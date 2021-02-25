using System.Collections.Generic;
using System.Threading.Tasks;
using SocialMedia.Core.Entities;

namespace SocialMedia.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll ();
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}