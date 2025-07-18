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
    public class FoodDetails_Handler : IRequestHandler<FoodDetails_Query, Food>
    {
        private readonly HomePageDAO DAL;
        public FoodDetails_Handler(HomePageDAO DAL) {
        this.DAL= DAL;
        }
        public async Task<Food> Handle(FoodDetails_Query request, CancellationToken cancellationToken)
        {
			try
			{
				int id =request.id;
                Food model = await DAL.GetFoodByID(id);
                return model;
            }
			catch (Exception)
			{

				throw;
			}
        }
    }
}
