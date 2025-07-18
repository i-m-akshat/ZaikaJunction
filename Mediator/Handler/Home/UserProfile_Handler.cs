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
    public class UserProfile_Handler : IRequestHandler<UserProfile_Query, User_ViewModel>
    {
        private readonly HomePageDAO DAL;
        private readonly UserLogin userDAL;
        public UserProfile_Handler(HomePageDAO DAL, UserLogin userDAL)
        {
            this.DAL = DAL;
            this.userDAL = userDAL;
        }
        public async Task<User_ViewModel> Handle(UserProfile_Query request, CancellationToken cancellationToken)
        {
			try
            {
                int id = request.id;
                User user = await DAL.GetUserDetails(id);
                List<Order> listOrders = await userDAL.GetAllOrders((int)id);
                List<OrderDetail> listOrderDetails = new List<OrderDetail>();

                listOrderDetails = await userDAL.GetAllOrderDetails((int)id);
                User_ViewModel viewModel = new User_ViewModel()
                {
                    _user = user,
                    Orders = listOrders,
                    OrderDetails = listOrderDetails
                };

                return viewModel;
			}
			catch (Exception)
			{

				throw;
			}
        }
    }
}
