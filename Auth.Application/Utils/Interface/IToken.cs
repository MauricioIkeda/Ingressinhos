using Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Application.Utils.Interface
{
    public interface IToken
    {
        string Generate(UserAuth user);
    }
}
