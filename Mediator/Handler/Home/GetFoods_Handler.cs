using BLL.Interfaces;
using Mediator.Query.Home;
using MediatR;
using Models;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Handler.Home
{
    public class GetFoods_Handler : IRequestHandler<GetFoods_Query, HomePageViewModel>
    {
        private readonly HomePageDAO DAL;
        private readonly UserLogin userDAL;
        public GetFoods_Handler(HomePageDAO dAL, UserLogin userDAL)
        {
            DAL = dAL;
            this.userDAL = userDAL;
        }

        public async Task<HomePageViewModel> Handle(GetFoods_Query request, CancellationToken cancellationToken)
        {
			try
			{
                int? FoodTypeID = request.FoodTypeID;
                int? CatID = request.CatID;
                int? SubCatID= request.SubCatID;
                List<Category> categories = new List<Category>();
                List<Category> topcategories = new List<Category>();
                List<SubCategory> subCategories = new List<SubCategory>();
                List<Food> foods = new List<Food>();
                List<FoodType> foodTypes = new List<FoodType>();
                if (FoodTypeID != null)
                {
                    foods = await DAL.getFoodsByFoodTypoeID((int)FoodTypeID);
                }
                else if (CatID != null)
                {
                    foods = await DAL.getFoodsByCategoryID((int)CatID);
                }
                else if (SubCatID != null)
                {
                    foods = await DAL.getFoodsBySubCategoryID((int)SubCatID);
                }
                else
                {
                    foods = await DAL.GetAllFoods();
                }
                categories = await DAL.GetAllCategories();
                subCategories = await DAL.GetAllSubCategories(subCategories);
                topcategories = await DAL.GetCategoriesForHomepage();
                foodTypes = await DAL.GetAllFoodTypes();
                //foods = await DAL.GetAllFoods();

               HomePageViewModel _viewModel = new HomePageViewModel()
                {
                    Categories = categories,
                    TopCategories = topcategories,
                    SubCategories = subCategories,
                    Food = foods,
                    FoodTypes = foodTypes
                };


                return _viewModel;
            }
			catch (Exception)
			{

				throw;
			}
        }
    }
}
