using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.Implementation;
using Models;
using MailKit;

namespace OnlineFastFoodDelivery.Controllers
{
    public class OrderController : Controller
    {
        private readonly SendMailDAO mailService;
        private OrderStatusDAO DAL;
        public OrderController(SendMailDAO _mailService,OrderStatusDAO _orderServices) 
        {
            this.mailService = _mailService;
            this.DAL= _orderServices;  
        }

       
        //OrderStatusDAO DAL=new OrderStatusDao();
        public async Task<IActionResult> Index()
        {
            orderModel listorder=await DAL.GetAllOrders(1);
            if (listorder != null)
            {
                return View(listorder);
            }else
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Index(int CurrentPageIndex)
        {
            orderModel listorder = await DAL.GetAllOrders(CurrentPageIndex);
            if (listorder != null)
            {
                return View(listorder);
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> Deliver(int Id)
        {
            bool IsSuccess = await DAL.Delivered(Id);
            if (IsSuccess)
            {
                string Email = await DAL.GetEmailbyOrderID(Id);
                bool res = await SendEmailNotifications("has been Delivered", Email);
                if (res)
                {
                    TempData["Success"] = "Order Status Changed";
                }
                
                ModelState.Clear();

            }
            else
            {
                TempData["Error"] = "Failed ! Please Try Again";

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> InTransit(int id)
        {
            bool IsSuccess = await DAL.InTransit(id);
            if (IsSuccess)
            {
                string Email = await DAL.GetEmailbyOrderID(id);
                bool res = await SendEmailNotifications("is In Transit", Email);
                if (res)
                {
                    TempData["Success"] = "Order Status Changed";
                }
                ModelState.Clear();

            }
            else
            {
                TempData["Error"] = "Failed ! Please Try Again";

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Waiting(int id)
        {
            bool IsSuccess = await DAL.Waiting(id);
            if (IsSuccess)
            {
                string Email = await DAL.GetEmailbyOrderID(id);
                bool res = await SendEmailNotifications("is Waiting To be Picked Up by Courier Partner", Email);
                if (res)
                {
                    TempData["Success"] = "Order Status Changed";
                }
                ModelState.Clear();

            }
            else
            {
                TempData["Error"] = "Failed ! Please Try Again";

            }
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Cancelled(int id)
        {
            bool IsSuccess=await DAL.Cancel(id);
            if (IsSuccess)
            {
                string Email = await DAL.GetEmailbyOrderID(id);
                bool res = await SendEmailNotifications("has been Cancelled", Email);
                if (res)
                {
                    TempData["Success"] = "Order Status Changed";
                }
                ModelState.Clear();

            }
            else
            {
                TempData["Error"] = "Failed ! Please Try Again";

            }
            return RedirectToAction("Index");  
        }
        public async Task<IActionResult> OutForDelivery(int id)
        {
            bool IsSuccess = await DAL.OutForDelivery(id);
            if (IsSuccess)
            {
                string Email = await DAL.GetEmailbyOrderID(id);
                bool res = await SendEmailNotifications("is Out For Delivery", Email);
                if (res)
                {
                    TempData["Success"] = "Order Status Changed";
                }
                ModelState.Clear();

            }
            else
            {
                TempData["Error"] = "Failed ! Please Try Again";

            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Picked(int id)
        {
            bool IsSuccess = await DAL.PickedByDeliveryPerson(id);
            if (IsSuccess)
            {
                string Email = await DAL.GetEmailbyOrderID(id);
                bool res = await SendEmailNotifications("has been Picked Up By Courier Partner", Email);
                if (res)
                {
                    TempData["Success"] = "Order Status Changed";
                }
                ModelState.Clear();

            }
            else
            {
                TempData["Error"] = "Failed ! Please Try Again";

            }
            return RedirectToAction("Index");
        }
        public async Task<bool> SendEmailNotifications(string message, string emailID)
        {
            bool isSuccess = false;

            // Dynamically build the absolute logo URL
            string logoUrl = $"{Request.Scheme}://{Request.Host}/img/logo.png";

            Email emailMessage = new Email
            {
                ToEmail = emailID,
                Subject = "📦 RE: Order Status Changed",
                Body = $@"
            <div style='font-family:Segoe UI, sans-serif; background-color:#ffffff; padding:20px; border-radius:10px; color:#000; max-width:600px; margin:auto; border:1px solid #f1c40f;'>
                <div style='text-align:center; margin-bottom:20px;'>
                    <img src='{logoUrl}' alt='Zaika Logo' style='max-width:150px; height:auto;' />
                </div>
                <h2 style='color:#111;'>👋 Hello Zaika User,</h2>
                <p style='font-size:16px;'>We wanted to inform you that your order status has been updated:</p>
                
                <div style='background-color:#f1c40f; padding:15px; border-radius:8px; font-size:18px; font-weight:bold; color:#000; text-align:center;'>
                    {message}
                </div>

                <p style='font-size:16px; margin-top:20px;'>Thank you for ordering with us. 🍽️</p>
                <hr style='margin:30px 0; border:none; border-top:1px solid #eee;' />
                <p style='font-size:14px; color:#888;'>This is an automated message from <strong>Zaika Junction</strong>. If you have questions, please contact our support.</p>
                <p style='font-size:13px; color:#aaa;'>Sent on: <b>{DateTime.Now:dddd, MMMM d, yyyy - hh:mm tt}</b></p>
            </div>"
            };

            isSuccess = await mailService.SendMailAsync(emailMessage);
            return isSuccess;
        }


    }
}
