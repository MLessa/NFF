using NudesForFree.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NudesForFree.Models;
using NudesForFree.Models.ViewModel;

namespace NudesForFree.Services
{
    public class PostService
    {
        public PostRepository PostRepository { get; set; }
        public CommentRepository CommentRepository { get; set; }
        public PostService(PostRepository postRepository, CommentRepository commentRepository)
        {
            this.PostRepository = postRepository;
            this.CommentRepository = commentRepository;
        }

        public Post GetPost(int id)
        {
            return PostRepository.FindByPK(id);
        }

        public Publication GetPublication(int id)
        {
            Publication pub = new Publication();
            pub.Post = PostRepository.FindByPK(id);
            pub.CreatorUser = new User() { Username = pub.Post != null ? pub.Post.Username : "" };
            if (pub.Post != null)            
                pub.Comments = CommentRepository.FindFilter(id, null, 0, 10000);            
            return pub;
        }

		public List<Post> GetPostIDsByUserID(int userID)
		{
			return PostRepository.GetPostIDsByUserID(userID);
		}

		public int Delete(int postID, int userID)
		{
			return PostRepository.Delete(postID, userID);
		}

		public bool Insert(Post model)
        {
            return PostRepository.Insert(model);
        }

        public bool Update(Post model)
        {
            return PostRepository.Update(model);
        }

        public bool UpdateComments(int postID, int count)
		{
            return PostRepository.UpdateComments(postID, count);
        }

        public bool UpdateLikes(int postID, int count) 
        {
            return PostRepository.UpdateLikes(postID, count);
        }
        public bool Delete( Post model)
        {
            return PostRepository.Delete(model);
        }

        public List<Post> FindAll()
        {
            return PostRepository.FindAll();
        }

        public List<Post> FindAllWithInactive()
        {
            return PostRepository.FindAllWithInactive();
        }

		public List<Post> GetPostToProfileScreen(List<int> postIDs)
		{
			return PostRepository.GetPostToProfileScreen(postIDs);
		}

		public List<Post> FindByFilter(int? userID, int? category,int lastID, int limit)
        {
            return PostRepository.FindFilter(userID, category, lastID, limit);
        }
    }
}
