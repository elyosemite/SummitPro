﻿using SummitPro.Application.Model;
using SummitPro.Application.OutputBoundary;
using SummitPro.Application.Repository;
using SummitPro.Core.Aggregate.Barbecue;
using SummitPro.Core.Aggregate.Participant;
using SummitPro.SharedKernel.Interfaces;
using SummitPro.SharedKernel.UseCaseContract;

namespace SummitPro.Application.UseCase.ListBarbecues
{
    public class ListBarbecuesUseCase : IUseCaseSinchronous
        .WithoutInputBoundary
        .WithOutputBoundary<ListBarbecuesQueryModel>
    {

        private readonly IBarbecueRepository _barbecueRepository;
        private readonly IParticipantRepository _participantRepository;
        private ICachedRepository _cachedRepository;

        public ListBarbecuesUseCase(IBarbecueRepository barbecueRepository, IParticipantRepository participantRepository)
        {
            _barbecueRepository = barbecueRepository;
            _participantRepository = participantRepository;
        }

        public ListBarbecuesUseCase SetDistributedCache(ICachedRepository cachedRepository)
        {
            _cachedRepository = cachedRepository;
            return this;
        }

        public override ListBarbecuesQueryModel Execute()
        {
            var participants = new List<Participant>();
            var participantsModel = new List<ParticipantModel>();
            var barbecues = new List<Barbecue>();


            if (_cachedRepository == null)
            {
                barbecues.AddRange(_barbecueRepository.GetAll().AsEnumerable());
                if (!barbecues.Any()) return null;
                participants.AddRange(_participantRepository.GetAll().AsEnumerable());
            }
            else
            {
                barbecues.AddRange(_cachedRepository.GetAll<Barbecue>().AsEnumerable());
                participants.AddRange(_cachedRepository.GetAll<Participant>());
            }

            ListBarbecuesQueryModel output = new ListBarbecuesQueryModel
            {
                Barbecues = barbecues.Select(barbecue => new BarbecueModel
                {
                    barbecueIdentifier = barbecue.Identifier,
                    Description = barbecue.Description,
                    BeginDate = barbecue.BeginDate.ToString(),
                    EndDate = barbecue.EndDate.ToString(),
                    AdditionalRemarks = barbecue.AdditionalRemarks,
                    Participants = participants
                        .Where(p => barbecue.Participants.Contains(p.Identifier))
                        .Select(o => new ParticipantModel
                        {
                            Identifier = o.Identifier,
                            BringDrink = o.BringDrink.ToString(),
                            ContributionValue = o.ContributionValue.Value,
                            Items = o.Items,
                            Name = o.Name.Value,
                            Username = o.Username.Value
                        })
                })
            };

            return output;
        }
    }
}
