using NudesForFree.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NudesForFree.Models;
using NudesForFree.Models.DTO;

namespace NudesForFree.Services
{
	public class InteractionService
	{
		public InteractionRepository InteractionRepository { get; set; }
		public InteractionService(InteractionRepository interactionRepository)
		{
			this.InteractionRepository = interactionRepository;
		}

		public Interaction GetInteraction(int id)
		{
			return InteractionRepository.FindByPK(id);
		}

		public bool Insert(Interaction model)
		{
			return InteractionRepository.Insert(model);
		}

		public bool Delete(Interaction model)
		{
			return InteractionRepository.Delete(model);
		}

		public List<Interaction> FindAll()
		{
			return InteractionRepository.FindAll();
		}

		public List<InteractionDTO> GetInteractionsDTO(int userID, int limit, int lastID)
		{
			return InteractionRepository.GetInteractionsDTO(userID, limit, lastID);
		}

		public List<Interaction> FindAllWithInactive()
		{
			return InteractionRepository.FindAllWithInactive();
		}

		public List<Interaction> FindByFilter(int? postID, int? userID, int limit, int lastID, string postIDsToFindLike)
		{
			return InteractionRepository.FindFilter(postID, userID, limit, lastID, postIDsToFindLike);
		}

		public List<Interaction> GetByPostIDList(List<int> postIDs, int? userID)
		{
			return InteractionRepository.GetByPostIDList(postIDs, userID);
		}
	}
}
