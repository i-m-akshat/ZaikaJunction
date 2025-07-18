using BLL.Implementation;
using BLL.Interfaces;
using Mediator.Query.Home;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Handler.Home
{
    public class GetFoodIdByName_Handler : IRequestHandler<GetFoodIdByName_Query, long>
    {
        private readonly HomePageDAO DAL;
        public GetFoodIdByName_Handler(HomePageDAO dAL)
        {
            DAL = dAL;
        }

        public async Task<long> Handle(GetFoodIdByName_Query request, CancellationToken cancellationToken)
        {
            try
            {
                string FoodName = request.FoodName;
                long foodid= await DAL.getFoodByName(FoodName);
                return foodid;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
