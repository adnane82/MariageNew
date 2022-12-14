using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MariageApp.API.Models;
using Microsoft.EntityFrameworkCore;
using ZwajApp.API.Helpers;

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

        public async Task<PagedList<User>> GetUsers( UserParams userParams)
        {
           var users=  _context.Users.Include(u=>u.Photos).OrderByDescending(u=>u.LastActive).AsQueryable();  
            users = users.Where(u=>u.Id!=userParams.UserId);
            users = users.Where(u=>u.Gender==userParams.Gender);
             if(userParams.MinAge!=18||userParams.MaxAge!=99){
               var minDob = DateTime.Today.AddYears(-userParams.MaxAge-1);
               var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
               users = users.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth<=maxDob);
           }
            if(!string.IsNullOrEmpty(userParams.OrderBy)){
               switch (userParams.OrderBy)
               {
                   case "created":
                   users=users.OrderByDescending(u=>u.Created);
                   break;
                   default:
                   users= users.OrderByDescending(u=>u.LastActive);
                   break;
               }
           }

           return await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);     
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync()>0;

      
        }
    }
}