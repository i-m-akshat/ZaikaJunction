using MediatR;
using Models;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Query.Home
{
    public record UserProfile_Query:IRequest<User_ViewModel>
    {
        public readonly int id;
        public UserProfile_Query(int id)
        {
            this.id= id;
        }
    }
}
