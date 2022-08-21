using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariageApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MariageApp.API.Data
{
    public class MariageRepository : IMariageRepository
    {
        private readonly DataContext _context;
        public MariageRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

       public async Task<Photo> GetMainPhotoForUser(int userId)
        {
           return await _context.Photos.Where(u=>u.UserId==userId).FirstOrDefaultAsync(p=>p.IsMain);
           
        }

        public async Task<Photo> GetPhoto(int Id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p=>p.Id==Id);
            return photo ;
        }

        public async Task<User> GetUser(int Id)
        {
            return await _context.Users.Include(u=>u.Photos).FirstOrDefaultAsync(u=>u.Id==Id);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
           return await _context.Users.Include(u=>u.Photos).ToListAsync();        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync()>0;

      
        }
    }
}