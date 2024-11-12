namespace Web.Controllers.Users
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;

    using Application.Interfaces;
    using Application.Handlers.Users.Commands;

    using Web.Extensions;

    using Models.User;

    using Shared;

    [Authorize]
    public class UsersController : ApiController
    {
        private readonly IUser _user;

        public UsersController(IUser user)
        {
            _user = user;
        }

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

        [HttpPost("{id}/change-password")]
        [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Result<string>>> ChangePassword(string id, ChangePasswordRequest request)
        {
            if (id != _user.Id)
            {
                return BadRequest(Result<string>.Failure(new List<string> { "You can only change your own password." }));
            }

            return await Mediator.Send(new ChangeUserPasswordCommand(id, request.CurrentPassword, request.NewPassword)).ToActionResult();
        }
    }
}