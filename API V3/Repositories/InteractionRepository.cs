using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NudesForFreeV2.Models;
using NudesForFreeV2.Models.DTO;

namespace NudesForFreeV2.Repositories
{
	public class InteractionRepository : BaseRepository
	{
        private static string insertColumns = "userID,postID,isLike,isComment,commentID,isReport,reportType,creationDate";
        private static string selectColumns = " `interactionID`,`userID`,`postID`,`isLike`,`isComment`,`commentID`,`isReport`,`reportType`,`creationDate` ";
        public const string cTableName = "interaction";
        public InteractionRepository()
        {
        }
        private static InteractionRepository pRepository = null;
        public static InteractionRepository GetInstance()
        {
            if (pRepository == null)
                pRepository = new InteractionRepository();
            return pRepository;
        }
        public bool Insert(Interaction interaction)
        {
            // buildding a command T-SQL
            string commandText = "insert into " + cTableName + " (" + insertColumns + ") values (@" + insertColumns.Replace(",", ",@") + ");";
            if (interaction.IsComment)
                commandText += "update post set comments = comments + 1 where postID = @postID;";
            else if(interaction.IsLike)
                commandText += "update post set likes = likes + 1 where postID = @postID;";
            return Execute(commandText, interaction);
        }

		public List<InteractionDTO> GetInteractionsDTO(int userID, int limit, int lastID)
		{
			string commandText = @"select i.interactionID, i.postID, p.filename, u.avatar, u.username, p.likes, p.comments, p.description, p.category, p.creationDate as postDate
								from interaction i 
								inner join post p on i.postID = p.postID
								inner join user u on u.userID = p.userID
								where (isLike = 1 or isComment = 1) and
								i.UserID = @userID and {0} 
								GROUP BY i.postID 
								order by 1 desc limit @limit";
			if (lastID != 0)
				commandText = commandText.Replace("{0}", "i.interactionID < @lastID");
			else
				commandText = commandText.Replace("{0}", "1=1");

			return QueryList<InteractionDTO>(commandText, new { userID,  limit,  lastID });
		}

		public bool Delete(Interaction interaction)
        {
            // buildding a command T-SQL
            string commandText = "delete from " + cTableName + " where interactionID=@id";
            return Execute(commandText, new { id = interaction.InteractionID });
        }

        public List<Interaction> FindAll()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where 1= 1 ";
            return QueryList<Interaction>(commandText);
        }

		public List<Interaction> GetByPostIDList(List<int> postIDs, int? userID)
		{
            string sql = "select interactionID, isLike, isComment, postID from " + cTableName + " where postID in @postIDs ";
            if (userID != null)
                sql += " and userID = " + userID.Value + " ";
            string commandText = WithNolock(sql);
			return QueryList<Interaction>(commandText, new { postIDs });
		}

		public List<Interaction> FindAllWithInactive()
        {
            // buildding a command T-SQL
            string commandText = "select " + selectColumns + " from " + cTableName + " where   1=1 ";
            return QueryList<Interaction>(commandText, new { });
        }

        public Interaction FindByPK(int id)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + "  where interactionID = @id ";

            return QuerySingle<Interaction>(commandText, new { id });
        }

        public List<Interaction> FindFilter(int? postID, int? userID, int limit, int lastID, string postIDsToFindLike)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + " where  1=1 ";

            if (postID != null)
                commandText += " and postID = @postID ";
            if (userID != null)
                commandText += " and userID = @userID ";
            if (lastID != 0)
                commandText += " and interactionID < @lastID ";
            if (!string.IsNullOrEmpty(postIDsToFindLike))
                commandText += " and postID in ( " + postIDsToFindLike + ") and isLike = 1 ";
            commandText += " order by 1 desc limit " + limit;
            return QueryList<Interaction>(commandText, new { postID = postID, userID = userID, lastID = lastID });
        }
    }
}
