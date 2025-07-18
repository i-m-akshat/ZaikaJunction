using BLL.Interfaces;
using Mediator.Query.Home;
using MediatR;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Handler.Home
{
    public class FilterFoods_Handler : IRequestHandler<FilterFoods_Query,List<Food>>
    {
		private readonly HomePageDAO DAL;
		public FilterFoods_Handler(HomePageDAO DAL)
		{
			this.DAL = DAL;
		}
        public async Task<List<Food>> Handle(FilterFoods_Query request, CancellationToken cancellationToken)
        {
			try
			{
				List<int>? listCat = request.listCat;
				List<int>? listSubCat = request.listSubCat;
				List<int>? listFoodType=request.listFoodType;
             List<Food> listFood = await DAL.GetAllFoods_Filter(listCat, listSubCat, listFoodType);
				return listFood;
            }
			catch (Exception ex)
			{

				throw;
			}
        }
    }
}
