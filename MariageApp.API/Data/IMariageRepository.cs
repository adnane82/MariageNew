using System.Collections.Generic;
using System.Threading.Tasks;
using MariageApp.API.Models;
using ZwajApp.API.Helpers;

namespace MariageApp.API.Data
{
    public interface IMariageRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task <bool> SaveAll();
        Task<PagedList<User>>  GetUsers(UserParams userParams);
        Task<User> GetUser(int Id);
         Task<Photo> GetPhoto(int Id);
            Task<Photo> GetMainPhotoForUser(int userId);

    }
}