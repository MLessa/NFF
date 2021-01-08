using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NudesForFree.Models;

namespace NudesForFree.Repositories
{
	public class PostRepository : BaseRepository
	{
        private static string insertColumns = "userID,username,description,filename,likes,comments,creationDate,category";
        private static string selectColumns = " `postID`,`userID`,`username`,`description`,`filename`,`likes`,`comments`,`creationDate`,`category` ";    
        public const string cTableName = "post";
        public PostRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public bool Insert(Post post)
        {
            // buildding a command T-SQL
            string commandText = "insert into " + cTableName + " (" + insertColumns + ") values (@" + insertColumns.Replace(",", ",@") + ");";
            return Execute(commandText, post);
        }

        public bool Update(Post post)
        {
            // buildding a command T-SQL
            string commandText = "update " + cTableName + " set category=@category, hots=@hots,likes=@likes where postID=@postID ;";

            return Execute(commandText, post);
        }

		public List<int> GetPostIDsByUserID(int userID)
		{
			string commandText = "select postID from " + cTableName + " where userID=@userID ;";

			return QueryList<int>(commandText, new { userID });
		}

		public int Delete(int postID, int userID)
		{
			// buildding a command T-SQL
			string commandText = "delete from " + cTableName + " where postID=@postID and userID=@userID ;";

			return ExecuteGetAffected(commandText, new { postID, userID });
		}

		public bool Delete(Post post)
        {
            // buildding a command T-SQL
            string commandText = "delete from " + cTableName + " where postID=@id";
            return Execute(commandText, new { id = post.PostID });
        }

        public List<Post> FindAll()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where 1= 1 ";
            return QueryList<Post>(commandText);
        }

		public List<Post> GetPostToProfileScreen(List<int> postIDs)
		{
			// buildding a command T-SQL
			string commandText = "select postID, description, filename, creationDate, category from " + cTableName + " where postID in @postIDs";
			return QueryList<Post>(commandText, new { postIDs });
		}

		public List<Post> FindAllWithInactive()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where   1=1 ";
            return QueryList<Post>(commandText, new { });
        }

        public Post FindByPK(int id)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + "  where postID = @id ";

            return QuerySingle<Post>(commandText, new { id });
        }

        public List<Post> FindFilter(int? userID, int? category, int lastID, int limit)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + " where  1=1 ";

            if (userID != null)
                commandText += " and userID = @userID ";
            if (category != null)
                commandText += " and category = @category ";
            if (lastID != 0)
                commandText += " and postID<@lastID ";
            commandText += " order by 1 desc limit " + limit;
            return QueryList<Post>(commandText, new { userID = userID, category = category, lastID = lastID });
        }
    }
}
