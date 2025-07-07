
using Microsoft.AspNetCore.Mvc;
//using Stripe.BillingPortal;
using Stripe.Checkout;
using Models;
using BLL.Interfaces;
using BLL.Implementation;
using Stripe;
using MailKit;


namespace OnlineFastFoodDelivery.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CartDAO _cartDAL;
        private readonly CheckoutDAO DAL;
        private readonly SendMailDAO mailService;
        public CheckoutController(SendMailDAO _mailService,CheckoutDAO CheckDAL,CartDAO CartDAL)
        {
            mailService = _mailService;
            _cartDAL = CartDAL;
            DAL = CheckDAL;

        }
       
        public async Task<IActionResult> Index()
        {

            int? UserID = HttpContext.Session.GetInt32("UserID");
            if (UserID.HasValue)
            {
                List<Cart> listCart = new List<Cart>();
                listCart = await _cartDAL.GetAllItems((int)UserID);
                ViewBag.SubTotal = await _cartDAL.GetOverallPrice((int)UserID);
                return View(listCart);
            }
            else
            {
                TempData["Error"] = "Please Login";
                return RedirectToAction("UserLogin", "User");
            }
        }
        public async Task<IActionResult> Checkout()
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            List<Cart> listCart = new List<Cart>();
            if (UserID.HasValue)
            {

                listCart = await _cartDAL.GetAllItems((int)UserID);
                //ViewBag.SubTotal = await DAL.GetOverallPrice((int)UserID);
                //return View(listCart);
            }
            else
            {
                TempData["Error"] = "Please Login";
                return RedirectToAction("UserLogin", "User");
            }
            var domain = "https://localhost:7016/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Checkout/OrderConfirmation",
                CancelUrl = domain + $"Checkout/Login",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };

            foreach (var item in listCart)
            {
                var sessionListitem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.FoodAmount * 100), // Convert to cents or smallest currency unit
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.FoodName.ToString(),
                        }
                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionListitem);
            }
            
            var service = new SessionService();
            Session session = service.Create(options);
            TempData["Session"] = session.Id;
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
           
        }
        public async Task<IActionResult> OrderConfirmation()
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            var service = new SessionService();
            Session session = service.Get(TempData["Session"].ToString());
            //var customerService = new CustomerService();
            //Customer customer = customerService.Get(session.CustomerId);
            var paymentService = new PaymentIntentService();
            var payment = paymentService.Get(session.PaymentIntentId);
            string paymentid = payment.Id;
            List<Cart> listCart = await _cartDAL.GetAllItems((int)UserID);
            PaymentDetail payDetails = new PaymentDetail
            {
                Amount = payment.Amount/100,
                PaidBy = session.CustomerDetails.Name,
                PaymentDate= Convert.ToDateTime(DateTime.Today),
                ProcessedBy=(long)UserID,
                TransactionId=payment.Id
            };
            int _paymentId=await DAL.CheckOut_PaymentDetails(payDetails);
            Order order = new Order
            {
                PaymentId=_paymentId,
                UserId = (int)HttpContext.Session.GetInt32("UserID"),
                ProcessedBy = (int)HttpContext.Session.GetInt32("UserID"),
                OrderStatus = "In Process",
                TotalAmount = (payment.Amount / 100)
            };
            int OrderID = await DAL.Checkout_Order(order);
            foreach (var item in listCart)
            {
                OrderDetail orderDetails = new OrderDetail
                {
                    OrderId= OrderID,
                    FoodId =item.FoodId, 
                    Amount=item.FoodAmount,
                    TotalAmount=item.TotalFoodPrice,    
                    NoOfServings=(short)item.Quantity
                };
                int _orderDetailsID = await DAL.Checkout_OrderDetails(orderDetails);
                DAL.ChangeCartStatus((int)UserID, item.CartId);
                DAL.changeFoodQuantity((int)item.FoodId,item.Quantity);
            }
            
            CartDAO cartDAL = new CartDao();
            int CartNumber = await cartDAL.GetTotalNumberOFItemsInCart((int)UserID);
            if (CartNumber == 0)
            {
                HttpContext.Session.Remove("CartNumber");
            }

            //var sessionService = new SessionService();
            //Session session = sessionService.Get(session_id);

            //var customerService = new CustomerService();
            //Customer customer = customerService.Get(session.CustomerId);
            //var paymentService = new PaymentIntentService();
            //var payment = paymentService.Get(session.PaymentIntentId);

            //string Content=$"<html><body><h1>Thanks for your order, {session.Customer}!</h1></body></html>";

            if (session.PaymentStatus == "paid")
            {
                string Email = await DAL.GetEmailbyUserID((int)UserID);
                bool isSuccess =await SendEmailNotifications_OrderPlaced(Email,OrderID, paymentid, "In Process");
                if (isSuccess)
                {
                    TempData["Success"] = "Payment Done ! " + session.CustomerDetails.Name;
                    return RedirectToAction("Index", "Cart");
                }
                else
                {
                    TempData["Error"] = "Something Went Wrong Please Try again";
                    return RedirectToAction("Index", "Cart");
                }
            }
            else
            {
                TempData["Error"] = "Something Went Wrong Please Try again";
                return RedirectToAction("Index","Cart");
            }
            
        }
        public async Task<bool> SendEmailNotifications_OrderPlaced(string EmailID, int OrderID, string transactionID, string OrderStatus)
        {
            bool isSuccess = false;

            // Get your logo from the root (e.g., wwwroot/img/logo.png)
            //string logoUrl = $"{Request.Scheme}://{Request.Host}/img/Logo.png";
            //string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Logo.png");
            //byte[] imageBytes = System.IO.File.ReadAllBytes(logoPath);
            //string base64Image = Convert.ToBase64String(imageBytes);
            //string logoUrl = $"data:image/png;base64,{base64Image}";
            string logoUrl = $"https://localhost:7016//img/Logo.png";



            Email emailMessage = new Email
            {
                ToEmail = EmailID,
                Subject = "🧾 Order Confirmation – Zaika Junction",

                Body = $@"
        <div style='font-family:Segoe UI, sans-serif; background-color:#ffffff; padding:25px; border-radius:10px; border:2px solid #f1c40f; color:#000; max-width:600px; margin:auto;'>

            <div style='text-align:center; margin-bottom:20px;'>
                <img src='{logoUrl}' alt='Zaika Logo' style='max-width:150px; height:auto;' />
            </div>

            <h2 style='color:#111; text-align:center;'>🎉 Congratulations!</h2>
            <p style='font-size:16px; text-align:center;'>Your order has been placed successfully at <strong>Zaika Junction</strong>.</p>

            <div style='background-color:#fef9e7; padding:20px; border-radius:8px; margin-top:20px;'>
                <h3 style='color:#f1c40f;'>📦 Order Details:</h3>
                <p><strong>Order Number:</strong> #{OrderID}</p>
                <p><strong>Transaction ID:</strong> {transactionID}</p>
                <p><strong>Order Status:</strong> <span style='color:green;'>{OrderStatus}</span></p>
            </div>

            <p style='font-size:15px; margin-top:20px;'>
                Thank you for shopping with us! We appreciate your business. 🍽️
                <br /><br />
                If you have any questions or need further assistance, feel free to contact our customer support at 
                <a href='mailto:i.m.akshat.dwivedi@gmail.com' style='color:#f1c40f;'>i.m.akshat.dwivedi@gmail.com</a>.
            </p>

            <hr style='margin:30px 0; border:none; border-top:1px solid #eee;' />

            <p style='font-size:13px; text-align:center; color:#888;'>Sent on {DateTime.Now:dddd, MMMM d, yyyy - hh:mm tt}</p>
        </div>"
            };

            isSuccess = await mailService.SendMailAsync(emailMessage);
            return isSuccess;
        }

    }
}
