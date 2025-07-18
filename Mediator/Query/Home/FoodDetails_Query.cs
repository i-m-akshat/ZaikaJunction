using MediatR;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Query.Home
{
   public class FoodDetails_Query:IRequest<Food>
    {
        public readonly int id;
        public FoodDetails_Query(int id)
        {
            this.id = id;
        }
    }
}
