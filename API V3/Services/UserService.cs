using NudesForFreeV2.Models;
using NudesForFreeV2.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFreeV2.Services
{
	public class UserService
	{
        private static System.Collections.Hashtable tokenCache = new System.Collections.Hashtable();
        private static DateTime lastRefresh = DateTime.Now.AddHours(2);
		
		public UserService()
		{			
		}

        public static User GetUser(int id)
        {
            return UserRepository.GetInstance().FindByPK(id);
        }

        public static User GetUser(string token, bool ignoreCash = false)
        {
            User user = (User)tokenCache[token];
            if (lastRefresh < DateTime.Now) 
            {
                tokenCache = new System.Collections.Hashtable();
                lastRefresh = DateTime.Now.AddHours(2);
            }
            if (user == null || ignoreCash)
            {
                user = UserRepository.GetInstance().FindByToken(token);
                tokenCache[token] = user;
            }
            return user;
        }

        public static bool Insert(User model)
        {
            return UserRepository.GetInstance().Insert(model);
        }

        public static bool Update(User model)
        {
            return UserRepository.GetInstance().Update(model);
        }

        public static bool Delete(User model)
        {            
            return UserRepository.GetInstance().Delete(model);
        }

        public static List<User> FindAll()
        {
            return UserRepository.GetInstance().FindAll();
        }

        public static List<User> FindAllWithInactive()
        {
            return UserRepository.GetInstance().FindAllWithInactive();
        }

        public static bool UpdateAvatar(int userID, string filename)
		{
			return UserRepository.GetInstance().UpdateAvatar(userID, filename);
		}

        public static List<User> FindByFilter(string email, string username)
        {
            return UserRepository.GetInstance().FindFilter(email, username);
        }        
    }
}
