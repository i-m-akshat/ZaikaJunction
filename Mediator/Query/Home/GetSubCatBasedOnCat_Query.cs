using MediatR;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Query.Home
{
   public class GetSubCatBasedOnCat_Query:IRequest<List<SubCategory>>
    {
        public readonly List<int> list;
        public GetSubCatBasedOnCat_Query(List<int> list)
        {
            this.list = list;
        }
    }
}
