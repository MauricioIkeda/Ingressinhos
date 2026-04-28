using Generic.Application.Crud.Interface;
using Ingressinhos.Application.Catalog.Dtos;
using Ingressinhos.Domain.Catalog.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ingressinhos.Application.Catalog.Interfaces
{
    public interface IUseCaseSellerCollection : IUseCaseCrudCollection<Seller, SellerDto>
    {
    }
}
