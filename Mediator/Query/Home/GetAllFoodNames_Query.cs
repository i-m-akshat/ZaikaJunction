using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Query.Home
{
    public class GetAllFoodNames_Query:IRequest<List<string>>
    {
    }
}
