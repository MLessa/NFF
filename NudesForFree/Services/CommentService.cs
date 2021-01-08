using NudesForFree.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NudesForFree.Models;

namespace NudesForFree.Services
{
    public class CommentService
    {
        public CommentRepository CommentRepository { get; set; }
        public CommentService(CommentRepository commentRepository)
        {
            this.CommentRepository = commentRepository;
        }

        public Comment GetComment(int id)
        {
            return CommentRepository.FindByPK(id);
        }

        public bool Insert(Comment model)
        {
            return CommentRepository.Insert(model);
        }

        public bool Update(Comment model)
        {
            return CommentRepository.Update(model);
        }

        public bool Delete(Comment model)
        {
            return CommentRepository.Delete(model);
        }

        public List<Comment> FindAll()
        {
            return CommentRepository.FindAll();
        }

        public List<Comment> FindAllWithInactive()
        {
            return CommentRepository.FindAllWithInactive();
        }

        public List<Comment> FindByFilter(int? postID, int? userID, int lastShowedCommentID, int limit)
        {
            return CommentRepository.FindFilter(postID, userID, lastShowedCommentID, limit);
        }
    }
}
