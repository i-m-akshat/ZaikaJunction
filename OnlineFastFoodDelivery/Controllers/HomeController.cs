using Microsoft.AspNetCore.Mvc;
using Models;
using OnlineFastFoodDelivery.Models;
using System.Diagnostics;
using Models.ViewModel;
using MediatR;
using Mediator.Query.Home;


namespace OnlineFastFoodDelivery.Controllers
{

    public class HomeController : Controller
    {

        private readonly IMediator _mediator;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,IMediator _mediator)
        {
            _logger = logger;
            this._mediator= _mediator;
        }
        [Route("")]
        public async Task<IActionResult> Index()
        {
            int? UserID = 0;
            UserID = HttpContext.Session.GetInt32("UserID");

            HomePageViewModel _viewModel=new HomePageViewModel();
            _viewModel = await _mediator.Send(new GetContentForHomepage());

            return View(_viewModel);
        }
        [Route("Foods")]
        public async Task<IActionResult> Foods(int? FoodTypeID, int? CatID, int? SubCatID)
        {

            HomePageViewModel _viewModel = new HomePageViewModel();
            _viewModel = await _mediator.Send(new GetFoods_Query(FoodTypeID, CatID, SubCatID));
            return View(_viewModel);

        }
        public async Task<IActionResult> Filter(List<int>? listCat, List<int>? listSubCat, List<int>? listFoodType)
        {

            List<Food> listFood = new List<Food>();
            listFood = await _mediator.Send(new FilterFoods_Query(listCat, listSubCat, listFoodType));
            return PartialView("_Food", listFood);
        }

        public async Task<IActionResult> FoodDetails(int FoodID)
        {
            Food model = new Food();
            model=await _mediator.Send(new FoodDetails_Query(FoodID));
            return View(model);
        }

        public async Task<IActionResult> UserProfile(int id)
        {
            try
            {
                User_ViewModel viewModel = await _mediator.Send(new UserProfile_Query(id));
                return View(viewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<List<SubCategory>> getSubCategoryBasedOnCat(List<int> list)
        {

            List<SubCategory> listSub=await _mediator.Send(new GetSubCatBasedOnCat_Query(list));
            return listSub;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.GetString("UserName");

            if (HttpContext.Session.GetString("UserName").ToString() != null && HttpContext.Session.GetInt32("UserID") != null)
            {
                HttpContext.Session.Remove("UserName");
                HttpContext.Session.Remove("UserID");
                HttpContext.Session.Remove("CartNumber");
            }
            TempData["Success"] = "Logged Out Successfully !";
            return RedirectToAction("Index");

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<List<String>> getAllFoodsName()
        {
            List<string> listFoodNames = await _mediator.Send(new GetAllFoodNames_Query());
            return listFoodNames;
        }
        public async Task<long> getFoodIDbyName(string FoodName)
        {
            long FoodID = await _mediator.Send(new GetFoodIdByName_Query(FoodName));
            if (FoodID != 0)
            {
                return FoodID;
            }else
            {
                return 0;
            }
           
        }

    }
}