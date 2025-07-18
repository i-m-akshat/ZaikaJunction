
using MediatR;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Query.Home
{
    public record GetFoods_Query : IRequest<HomePageViewModel>
    {
        public readonly int? FoodTypeID;
        public readonly int? CatID;
        public readonly int? SubCatID;
        public GetFoods_Query(int? FoodTypeID,int? CatID,int? SubCatID)
        {
            this.FoodTypeID = FoodTypeID;
            this.CatID = CatID;
            this.SubCatID= SubCatID;
        }
    };
   
}
