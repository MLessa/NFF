using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NudesForFree.Models;

namespace NudesForFree.Repositories
{
	public class UserRepository : BaseRepository
	{
        private static string insertColumns = "username,emailAddress,password,avatar,country,preferredCategory,creationDate,token";
		private static string selectColumns = " `userID`,`username`,`emailAddress`,`password`,`avatar`,`country`,`preferredCategory`,`creationDate`,`token` ";
        public const string cTableName = "user";
        public UserRepository(IConfiguration configuration) : base(configuration)
		{
		}
        public bool Insert(User user)
        {
            // buildding a command T-SQL
            string commandText = "insert into "+ cTableName + " (" + insertColumns + ") values (@" + insertColumns.Replace(",", ",@") + ");";
            return Execute(commandText, user);
        }
        
        public bool Update(User user)
        {
            // buildding a command T-SQL
            string commandText = "update " + cTableName + " set token=@token, userName=@username, password=@password,avatar=@avatar,country=@country,preferredCategory=@preferredCategory where userID=@userID ;";

            return Execute(commandText, user);
        }
                
        public bool Delete(User user)
        {
            // buildding a command T-SQL
            string commandText = "delete from " + cTableName + " where userID=@id";
            return Execute(commandText, new { id = user.UserID });
        }

        public List<User> FindAll()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where 1= 1 ";
            return QueryList<User>(commandText);
        }

		public bool UpdateAvatar(int userID, string filename)
		{
			string commandText = "update "+cTableName+" set avatar = @avatar where userID=@id";
			return Execute(commandText, new { avatar = filename,  id = userID });
		}

		public List<User> FindAllWithInactive()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where   1=1 ";
            return QueryList<User>(commandText, new { });
        }

        public User FindByPK(int id)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + "  where userID = @id ";

            return QuerySingle<User>(commandText, new { id });
        }
        public User FindByToken(string token)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + "  where token = @token ";

            return QuerySingle<User>(commandText, new { token });
        }

        public List<User> FindFilter(string email, string username)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + " where  1=1 ";
            if (!string.IsNullOrEmpty(email) && string.IsNullOrEmpty(username))
                commandText += " and emailaddress = @email ";
            if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(email))
                commandText += " and username = @username ";
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
                commandText += " and (username = @username or emailaddress = @email)";
            commandText += " order by username asc ";
            return QueryList<User>(commandText, new { email = email, username = username });
        }
    }
}
