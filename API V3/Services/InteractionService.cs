using NudesForFreeV2.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NudesForFreeV2.Models;
using NudesForFreeV2.Models.DTO;

namespace NudesForFreeV2.Services
{
	public class InteractionService
	{
		
		public InteractionService()
		{
			
		}

		public static Interaction GetInteraction(int id)
		{
			return InteractionRepository.GetInstance().FindByPK(id);
		}

		public static bool Insert(Interaction model)
		{
			return InteractionRepository.GetInstance().Insert(model);
		}

		public static bool Delete(Interaction model)
		{
			return InteractionRepository.GetInstance().Delete(model);
		}

		public static List<Interaction> FindAll()
		{
			return InteractionRepository.GetInstance().FindAll();
		}

		public static List<InteractionDTO> GetInteractionsDTO(int userID, int limit, int lastID)
		{
			return InteractionRepository.GetInstance().GetInteractionsDTO(userID, limit, lastID);
		}

		public static List<Interaction> FindAllWithInactive()
		{
			return InteractionRepository.GetInstance().FindAllWithInactive();
		}

		public static List<Interaction> FindByFilter(int? postID, int? userID, int limit, int lastID, string postIDsToFindLike)
		{
			return InteractionRepository.GetInstance().FindFilter(postID, userID, limit, lastID, postIDsToFindLike);
		}

		public static List<Interaction> GetByPostIDList(List<int> postIDs, int? userID)
		{
			return InteractionRepository.GetInstance().GetByPostIDList(postIDs, userID);
		}
	}
}
