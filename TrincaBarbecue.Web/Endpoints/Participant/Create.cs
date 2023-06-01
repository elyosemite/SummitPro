﻿using Microsoft.AspNetCore.Mvc;
using TrincaBarbecue.Application.UseCase.AddParticipante;
using TrincaBarbecue.Application.UseCase.BindParticipant;
using TrincaBarbecue.Infrastructure.Http.Controller;

namespace TrincaBarbecue.Web.Endpoints.Participant
{
    public class Create : EndpointBaseSynchronous
        .WithRequest<AddParticipantInputBoundary>
        .WithActionResult<AddParticipantInputBoundary>
    {
        private readonly AddParticipantController _addParticipantController;
        private readonly BindParticipantTobarbecueController _bindParticipantController;

        public Create(AddParticipantController addParticipantController, BindParticipantTobarbecueController bindParticipantController)
        {
            _addParticipantController = addParticipantController;
            _bindParticipantController = bindParticipantController;
        }

        [HttpPost("/Participant")]
        public override ActionResult<AddParticipantInputBoundary> Handle([FromBody] AddParticipantInputBoundary request)
        {
            var output = _addParticipantController.Handle(request);

            _bindParticipantController.Handle(new BindParticipantInputBoundary
            {
                BarbecueIdentifier = request.BarbecueIdentifier,
                ParticipantIdentifier = output.ParticipantIdentifier,
            });

            return Ok(output);
        }
    }
}
