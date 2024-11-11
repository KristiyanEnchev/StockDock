namespace Web.Controllers.Users
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;

    using Application.Handlers.Users.Commands;

    using Web.Extensions;

    using Shared;

    public class UsersController : ApiController
    {
        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<bool>>> Deactivate(string id)
        {
            return await Mediator.Send(new DeactivateUserCommand(id)).ToActionResult();
        }

        [HttpPost("{id}/reactivate")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Result<bool>>> Reactivate(string id)
        {
            return await Mediator.Send(new ReactivateUserCommand(id)).ToActionResult();
        }
    }
}