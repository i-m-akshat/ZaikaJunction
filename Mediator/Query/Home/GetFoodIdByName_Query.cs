using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Query.Home
{
   public class GetFoodIdByName_Query:IRequest<long>
    {
        public readonly string FoodName;
        public GetFoodIdByName_Query(string foodName)
        {
            FoodName = foodName;
        }
    }
}
