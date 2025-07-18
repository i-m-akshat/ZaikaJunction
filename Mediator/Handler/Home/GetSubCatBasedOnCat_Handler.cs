using BLL.Implementation;
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
    public class GetSubCatBasedOnCat_Handler : IRequestHandler<GetSubCatBasedOnCat_Query, List<SubCategory>>
    {
        private readonly HomePageDAO DAL;
        public GetSubCatBasedOnCat_Handler(HomePageDAO DAL)
        {
            this.DAL = DAL;
        }
        public async Task<List<SubCategory>> Handle(GetSubCatBasedOnCat_Query request, CancellationToken cancellationToken)
        {
            try
            {
                var list = request.list;
                List<SubCategory> listSub = await DAL.GetSubCategoriesForHomePage(list);
                return listSub;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
