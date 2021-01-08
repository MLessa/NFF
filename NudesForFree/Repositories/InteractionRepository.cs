using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NudesForFree.Models;

namespace NudesForFree.Repositories
{
	public class InteractionRepository : BaseRepository
	{
        private static string insertColumns = "userID,postID,isLike,isHot,isComment,commentID,isReport,reportType,creationDate";
        private static string selectColumns = " `interactionID`,`userID`,`postID`,`isLike`,`isHot`,`isComment`,`commentID`,`isReport`,`reportType`,`creationDate` ";
        public const string cTableName = "interaction";
        public InteractionRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public bool Insert(Interaction interaction)
        {
            // buildding a command T-SQL
            string commandText = "insert into " + cTableName + " (" + insertColumns + ") values (@" + insertColumns.Replace(",", ",@") + ");";
            return Execute(commandText, interaction);
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

		public List<Interaction> GetByPostIDList(List<int> postIDs)
		{
			string commandText = WithNolock("select interactionID, isLike, isComment, postID from " + cTableName + " where postID in @postIDs ");
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

        public List<Interaction> FindFilter(int? postID, int? userID, int limit, int lastID)
        {
            // buildding a command T-SQL
            string commandText = "select  " + selectColumns + "  from " + cTableName + " where  1=1 ";

            if (postID != null)
                commandText += " and postID = @postID ";
            if (userID != null)
                commandText += " and userID = @userID ";
            if (lastID != 0)
                commandText += " and interactionID < @lastID ";
            commandText += " order by 1 desc limit " + limit;
            return QueryList<Interaction>(commandText, new { postID = postID, userID = userID, lastID = lastID });
        }
    }
}
