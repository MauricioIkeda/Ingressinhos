using Generic.Domain.Entities;
using Generic.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ingressinhos.Application.Catalog.UseCases
{
    public class UseCaseSellerCollection //colocar interface depois
    {
        public ListMessages Messages { get; set; } = new ListMessages();
        
        public UseCaseSellerCollection(IRepositorySession repository, SellerUpdate update, SellerInclude sellerInclude, SellerGet sellerGet, SellerDelete sellerDelete)
        {
            Messages = new ListMessages();
        }//colocar metodos depois
    }
}
