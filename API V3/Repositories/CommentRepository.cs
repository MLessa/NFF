using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NudesForFreeV2.Models;
using NudesForFreeV2.Models.ViewModel;

namespace NudesForFreeV2.Repositories
{
    public class CommentRepository : BaseRepository
    {
        private static string insertColumns = "postID,userID,username,text,creationDate";
        private static string selectColumns = " `commentID`,`postID`,`userID`,`username`,`text`,`creationDate` ";
        public const string cTableName = "comment";
        public CommentRepository() 
        {
        }
        private static CommentRepository pRepository = null;
        public static CommentRepository GetInstance()
        {
            if (pRepository == null)
                pRepository = new CommentRepository();
            return pRepository;
        }
        public int? Insert(Comment comment)
        {
            // buildding a command T-SQL
            string commandText = "insert into " + cTableName + " (" + insertColumns + ") values (@" + insertColumns.Replace(",", ",@") + ");" +
				"SELECT LAST_INSERT_ID();";
            return QuerySingle<int?>(commandText, comment);
        }

        public bool Update(Comment comment)
        {
            // buildding a command T-SQL
            string commandText = "update " + cTableName + " set `text`=@text where commentID=@commentID ;";

            return Execute(commandText, comment);
        }

        public bool Delete(Comment comment)
        {
            // buildding a command T-SQL
            string commandText = "delete from " + cTableName + " where commentID=@id";
            return Execute(commandText, new { id = comment.CommentID });
        }

        public List<Comment> FindAll()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where 1= 1 ";
            return QueryList<Comment>(commandText);
        }

        public List<Comment> FindAllWithInactive()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where   1=1 ";
            return QueryList<Comment>(commandText, new { });
        }

        public Comment FindByPK(int id)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + "  where commentID = @id ";

            return QuerySingle<Comment>(commandText, new { id });
        }

        public List<PublicationComment> FindFilter(int? postID, int? userID, int lastShowedCommentID, int limit)
        {
            // buildding a command T-SQL
            string commandText = "select distinct commentID,postID,c.userID,c.username,text,c.creationDate, u.avatar from " + cTableName + " c inner join user u on u.userID = c.userID where 1=1";

            if (postID!=null)
                commandText += " and c.postID = @postID ";
            if (userID!=null)
                commandText += " and c.userID = @userID ";
            if (lastShowedCommentID!=0)
                commandText += " and c.commentID > @lastShowedCommentID ";
            commandText += " order by 1 asc limit " + limit;
            return QueryList<PublicationComment>(commandText, new { postID = postID, userID = userID, lastShowedCommentID = lastShowedCommentID });
        }
    }
}
