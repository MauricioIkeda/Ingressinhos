using Generic.Application.Interface;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ingressinhos.Application.Catalog.Interfaces
{
    internal interface IUseCaseSellerCollection : IUseCaseCrudCollection<Seller, SellerDto>
    {
    }
}
