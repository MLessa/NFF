using NudesForFreeV2.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NudesForFreeV2.Models;
using NudesForFreeV2.Models.ViewModel;

namespace NudesForFreeV2.Services
{
    public class CommentService
    {
        public CommentService()
        {            
        }

        public static Comment GetComment(int id)
        {
            return CommentRepository.GetInstance().FindByPK(id);
        }

        public static int? Insert(Comment model)
        {
            return CommentRepository.GetInstance().Insert(model);
        }

        public static bool Update(Comment model)
        {
            return CommentRepository.GetInstance().Update(model);
        }

        public static bool Delete(Comment model)
        {
            return CommentRepository.GetInstance().Delete(model);
        }

        public static List<Comment> FindAll()
        {
            return CommentRepository.GetInstance().FindAll();
        }

        public static List<Comment> FindAllWithInactive()
        {
            return CommentRepository.GetInstance().FindAllWithInactive();
        }

        public static List<PublicationComment> FindByFilter(int? postID, int? userID, int lastShowedCommentID, int limit)
        {
            return CommentRepository.GetInstance().FindFilter(postID, userID, lastShowedCommentID, limit);
        }
    }
}
