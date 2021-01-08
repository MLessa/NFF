using NudesForFreeV2.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NudesForFreeV2.Models;
using NudesForFreeV2.Models.ViewModel;

namespace NudesForFreeV2.Services
{
    public class PostService
    {
        public PostService()
        {
        }

        public static Post GetPost(int id)
        {
            return PostRepository.GetInstance().FindByPK(id);
        }

        public static Publication GetPublication(int id)
        {
            Publication pub = new Publication();
            pub.Post = PostRepository.GetInstance().FindByPK(id);
            pub.CreatorUser = new User() { Username = pub.Post != null ? pub.Post.Username : "" };
            if (pub.Post != null)            
                pub.Comments = CommentRepository.GetInstance().FindFilter(id, null, 0, 10000);            
            return pub;
        }

        public static List<Post> GetPostIDsByUserID(int userID)
		{
			return PostRepository.GetInstance().GetPostIDsByUserID(userID);
		}

        public static int Delete(int postID, int userID)
		{
			return PostRepository.GetInstance().Delete(postID, userID);
		}

        public static bool Insert(Post model)
        {
            return PostRepository.GetInstance().Insert(model);
        }

        public static bool Update(Post model)
        {
            return PostRepository.GetInstance().Update(model);
        }

        public static bool UpdateComments(int postID, int count)
		{
            return PostRepository.GetInstance().UpdateComments(postID, count);
        }

        public static bool UpdateLikes(int postID, int count) 
        {
            return PostRepository.GetInstance().UpdateLikes(postID, count);
        }
        public static bool Delete( Post model)
        {
            return PostRepository.GetInstance().Delete(model);
        }

        public static List<Post> FindAll()
        {
            return PostRepository.GetInstance().FindAll();
        }

        public static List<Post> FindAllWithInactive()
        {
            return PostRepository.GetInstance().FindAllWithInactive();
        }

        public static List<Post> GetPostToProfileScreen(List<int> postIDs)
		{
			return PostRepository.GetInstance().GetPostToProfileScreen(postIDs);
		}

        public static List<Post> FindByFilter(int? userID, int? category,int lastID, int limit)
        {
            return PostRepository.GetInstance().FindFilter(userID, category, lastID, limit);
        }
    }
}
