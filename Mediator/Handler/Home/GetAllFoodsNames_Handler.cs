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
    public class GetAllFoodsNames_Handler : IRequestHandler<GetAllFoodNames_Query, List<string>>
    {
        private readonly HomePageDAO DAL;
        public GetAllFoodsNames_Handler(HomePageDAO dAL)
        {
            DAL = dAL;
        }

        public async Task<List<string>> Handle(GetAllFoodNames_Query request, CancellationToken cancellationToken)
        {
            try
            {
                var res = await DAL.getAllFoodNames();
                return res;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
