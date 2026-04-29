using Generic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Application.Authorization.UserAccess.Interfaces
{
    public interface IUseCaseUserAuthCollection
    {
        ListMessages Messages { get; }
        (bool success, string token) Execute(string email, string password);
    }
}
