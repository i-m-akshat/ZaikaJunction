
using BLL.Interfaces;
using Mediator.Query.Home;
using MediatR;
using Models;
using Models.ViewModel;

namespace Mediator.Handler.Home
{
    public class GetContentForHomePageCommandHandler : IRequestHandler<GetContentForHomepage, HomePageViewModel>
    {
        private readonly HomePageDAO DAL;
        private readonly UserLogin userDAL;
        public GetContentForHomePageCommandHandler(HomePageDAO DAL,UserLogin userDAL)
        {
        
            this.DAL= DAL;
            this.userDAL= userDAL;
        }
        public async Task<HomePageViewModel> Handle(GetContentForHomepage request, CancellationToken cancellationToken)
        {
			try
			{
                List<Category> categories = new List<Category>();
                List<Category> topcategories = new List<Category>();
                List<SubCategory> subCategories = new List<SubCategory>();
                List<Food> foods = new List<Food>();
                List<Food> topFoods = new List<Food>();
                List<FoodType> foodTypes = new List<FoodType>();
                categories = await DAL.GetAllCategories();
                topcategories = await DAL.GetCategoriesForHomepage();
                foodTypes = await DAL.GetAllFoodTypesForHomepage();
                subCategories = await DAL.GetSubCategoriesForHomePage();
                topFoods = await DAL.GetFoodsForHomepage_TopRated();
                foods = await DAL.GetFoodsForHomepage();
                var _viewModel = new HomePageViewModel()
                {
                    Categories = categories,
                    TopCategories = topcategories,
                    SubCategories = subCategories,
                    Food = foods,
                    FoodTypes = foodTypes,
                    TopFoods = topFoods
                };
                return _viewModel;
            }
            catch (Exception ex)
			{

				throw;
			}
        }
    }
}
