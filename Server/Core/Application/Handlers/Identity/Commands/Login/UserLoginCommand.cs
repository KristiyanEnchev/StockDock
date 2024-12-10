﻿namespace Application.Handlers.Identity.Commands.Login
{
    using System.Threading;
    using System.Threading.Tasks;

    using MediatR;

    using Models.User;

    using Shared;
    using Application.Interfaces.Identity;

    public class UserLoginCommand : UserRequestModel, IRequest<Result<UserResponseModel>>
    {
        public UserLoginCommand(string email, string password)
            : base(email, password)
        {
        }

        public class UserLoginCommandHandler : IRequestHandler<UserLoginCommand, Result<UserResponseModel>>
        {
            private readonly IIdentityService identity;

            public UserLoginCommandHandler(IIdentityService identity)
            {
                this.identity = identity;
            }

            public async Task<Result<UserResponseModel>> Handle(UserLoginCommand request, CancellationToken cancellationToken)
            {
                var result = await identity.Login(request);

                return result;
            }
        }
    }
}