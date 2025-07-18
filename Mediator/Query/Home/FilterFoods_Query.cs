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
    public record FilterFoods_Query:IRequest<List<Food>>
    {
        public readonly List<int>? listCat;
        public readonly List<int>? listSubCat;
        public readonly List<int>? listFoodType;
        public FilterFoods_Query(List<int>? listCat, List<int>? listSubCat, List<int>? listFoodType)
        {
           this.listCat = listCat;
            this.listSubCat=listSubCat;
            this.listFoodType = listFoodType;
        }
    }
}
