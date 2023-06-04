﻿using TrincaBarbecue.Application.Repository;
using TrincaBarbecue.SharedKernel.Interfaces;
using TrincaBarbecue.SharedKernel.UseCaseContract;

namespace TrincaBarbecue.Application.UseCase.GetParticipant
{
    public class GetParticipantsUseCase : IUseCaseSinchronous
        .WithInputBoundary<GetParticipantsInputBoundary>
        .WithOutputBoundary<GetParticipantsOutputBoundary>
    {
        private readonly IParticipantRepository _participantRepository;
        private ICachedRepository _cachedRepository;

        public GetParticipantsUseCase(
            IParticipantRepository participantRepository)
        {
            _participantRepository = participantRepository;
        }

        public GetParticipantsUseCase SetDistributedCache(ICachedRepository cachedRepository)
        {
            _cachedRepository = cachedRepository;
            return this;
        }

        public override GetParticipantsOutputBoundary Execute(GetParticipantsInputBoundary inputBoundary)
        {
            if (_cachedRepository != null)
            {
                var existingParticipants = new List<Core.Aggregate.Participant.Participant>();

                foreach(var participant in inputBoundary.ParticipantIdentifiers)
                {
                    existingParticipants.Add(_cachedRepository.Get<Core.Aggregate.Participant.Participant>(participant.ToString()));
                }

                if (existingParticipants == null) return new GetParticipantsOutputBoundary();

                var participants = existingParticipants
                    .Select(x => new ParticipantOutputBoundary
                    {
                        Identifier = x.Identifier,
                        Name = x.Name.Value,
                        Contribution = x.ContributionValue.Value,
                        BringDrink = x.BringDrink.ToString(),
                        Items = x.Items,
                        Username = x.Username.Value
                    });

                return new GetParticipantsOutputBoundary()
                {
                    Participants = participants
                };
            }
            else
            {
                var existingParticipants = _participantRepository.GetByIdentifiers(inputBoundary.ParticipantIdentifiers);

                if (existingParticipants == null) return new GetParticipantsOutputBoundary();

                var participants = existingParticipants
                    .Select(x => new ParticipantOutputBoundary
                    {
                        Identifier = x.Identifier,
                        Name = x.Name.Value,
                        Contribution = x.ContributionValue.Value,
                        BringDrink = x.BringDrink.ToString(),
                        Items = x.Items,
                        Username = x.Username.Value
                    });

                return new GetParticipantsOutputBoundary()
                {
                    Participants = participants
                };
            }
        }
    }
}
