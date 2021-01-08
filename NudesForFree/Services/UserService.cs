using NudesForFree.Models;
using NudesForFree.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Services
{
	public class UserService
	{
        private static System.Collections.Hashtable tokenCache = new System.Collections.Hashtable();
        private static DateTime lastRefresh = DateTime.Now.AddHours(2);
		public UserRepository UserRepository { get; set; }
		public UserService(UserRepository userRepository)
		{
			this.UserRepository = userRepository;
		}

        public User GetUser(int id)
        {
            return UserRepository.FindByPK(id);
        }

        public User GetUser(string token)
        {
            User user = (User)tokenCache[token];
            if (lastRefresh < DateTime.Now) 
            {
                tokenCache = new System.Collections.Hashtable();
                lastRefresh = DateTime.Now.AddHours(2);
            }
            if (user == null)
            {
                user = UserRepository.FindByToken(token);
                tokenCache[token] = user;
            }
            return user;
        }

        public bool Insert(User model)
        {
            return UserRepository.Insert(model);
        }
                
        public bool Update(User model)
        {
            return UserRepository.Update(model);
        }
                
        public bool Delete(User model)
        {            
            return UserRepository.Delete(model);
        }

        public List<User> FindAll()
        {
            return UserRepository.FindAll();
        }
        
        public List<User> FindAllWithInactive()
        {
            return UserRepository.FindAllWithInactive();
        }

		public bool UpdateAvatar(int userID, string filename)
		{
			return UserRepository.UpdateAvatar(userID, filename);
		}

		public List<User> FindByFilter(string email, string username)
        {
            return UserRepository.FindFilter(email, username);
        }        
    }
}
