using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NudesForFree.Models;

namespace NudesForFree.Repositories
{
    public class CommentRepository : BaseRepository
    {
        private static string insertColumns = "postID,userID,username,text,creationDate";
        private static string selectColumns = " `commentID`,`postID`,`userID`,`username`,`text`,`creationDate` ";
        public const string cTableName = "comment";
        public CommentRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public bool Insert(Comment comment)
        {
            // buildding a command T-SQL
            string commandText = "insert into " + cTableName + " (" + insertColumns + ") values (@" + insertColumns.Replace(",", ",@") + ");";
            return Execute(commandText, comment);
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

        public List<Comment> FindFilter(int? postID, int? userID, int lastShowedCommentID, int limit)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + " where  1=1 ";

            if (postID!=null)
                commandText += " and postID = @postID ";
            if (userID!=null)
                commandText += " and userID = @userID ";
            if (lastShowedCommentID!=0)
                commandText += " and commentID < @lastShowedCommentID ";
            commandText += " order by 1 desc limit " + limit;
            return QueryList<Comment>(commandText, new { postID = postID, userID = userID, lastShowedCommentID = lastShowedCommentID });
        }
    }
}
