using Azure;
using Microsoft.AspNetCore.Mvc;
using Models;
using Stripe.Checkout;
using Stripe;
using BLL.Interfaces;

public class CheckoutController : Controller
{
    private readonly CartDAO _cartDAL;
    private readonly CheckoutDAO _checkoutDAL;
    private readonly SendMailDAO _mailService;

    public CheckoutController(SendMailDAO mailService, CheckoutDAO checkoutDAL, CartDAO cartDAL)
    {
        _mailService = mailService;
        _cartDAL = cartDAL;
        _checkoutDAL = checkoutDAL;
    }

    public async Task<IActionResult> Index()
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        if (!userId.HasValue)
        {
            TempData["Error"] = "Please login to continue.";
            return RedirectToAction("UserLogin", "User");
        }

        var cartItems = await _cartDAL.GetAllItems(userId.Value);
        ViewBag.SubTotal = await _cartDAL.GetOverallPrice(userId.Value);
        return View(cartItems);
    }

    public async Task<IActionResult> Checkout()
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        if (!userId.HasValue)
        {
            TempData["Error"] = "Please login to continue.";
            return RedirectToAction("UserLogin", "User");
        }

        var cartItems = await _cartDAL.GetAllItems(userId.Value);
        if (cartItems == null || !cartItems.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var domain = $"{Request.Scheme}://{Request.Host}/";
        var options = new SessionCreateOptions
        {
            SuccessUrl = domain + "Checkout/OrderConfirmation?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = domain + "Cart",
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment"
        };

        foreach (var item in cartItems)
        {
            if (item.FoodAmount > 0 && item.Quantity > 0)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.FoodAmount * 100),
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.FoodName
                        }
                    },
                    Quantity = item.Quantity
                });
            }
        }

        if (!options.LineItems.Any())
        {
            TempData["Error"] = "No valid items found in cart.";
            return RedirectToAction("Index", "Cart");
        }

        var service = new SessionService();
        var session = service.Create(options);

        // Store session ID for order confirmation
        TempData["SessionID"] = session.Id;

        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public async Task<IActionResult> OrderConfirmation(string session_id)
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        if (!userId.HasValue || string.IsNullOrEmpty(session_id))
        {
            TempData["Error"] = "Invalid session.";
            return RedirectToAction("Index", "Cart");
        }

        var sessionService = new SessionService();
        var session = sessionService.Get(session_id);

        if (session.PaymentStatus != "paid")
        {
            TempData["Error"] = "Payment not successful.";
            return RedirectToAction("Index", "Cart");
        }

        var paymentIntentService = new PaymentIntentService();
        var payment = paymentIntentService.Get(session.PaymentIntentId);

        var cartItems = await _cartDAL.GetAllItems(userId.Value);
        if (cartItems == null || !cartItems.Any())
        {
            TempData["Error"] = "Cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        // Store payment details
        var paymentDetails = new PaymentDetail
        {
            Amount = payment.Amount / 100,
            PaidBy = session.CustomerDetails.Name,
            PaymentDate = DateTime.Now,
            ProcessedBy = userId.Value,
            TransactionId = payment.Id
        };

        int paymentId = await _checkoutDAL.CheckOut_PaymentDetails(paymentDetails);

        // Store order
        var order = new Order
        {
            PaymentId = paymentId,
            UserId = userId.Value,
            ProcessedBy = userId.Value,
            OrderStatus = "In Process",
            TotalAmount = payment.Amount / 100
        };

        int orderId = await _checkoutDAL.Checkout_Order(order);

        foreach (var item in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = orderId,
                FoodId = item.FoodId,
                Amount = item.FoodAmount,
                TotalAmount = item.TotalFoodPrice,
                NoOfServings = (short)item.Quantity
            };

            await _checkoutDAL.Checkout_OrderDetails(orderDetail);
            _checkoutDAL.ChangeCartStatus(userId.Value, item.CartId);
            _checkoutDAL.changeFoodQuantity((int)item.FoodId, item.Quantity);
        }

        int remainingCartItems = await _cartDAL.GetTotalNumberOFItemsInCart(userId.Value);
        if (remainingCartItems == 0)
        {
            HttpContext.Session.Remove("CartNumber");
        }

        string email = await _checkoutDAL.GetEmailbyUserID(userId.Value);
        bool mailSent = await SendEmailNotifications_OrderPlaced(email, orderId, payment.Id, "In Process");

        if (mailSent)
        {
            TempData["Success"] = "Payment successful. Order placed!";
        }
        else
        {
            TempData["Error"] = "Payment received, but failed to send confirmation email.";
        }

        return RedirectToAction("Index", "Cart");
    }

    public async Task<bool> SendEmailNotifications_OrderPlaced(string email, int orderId, string transactionId, string orderStatus)
    {
        var logoUrl = "https://cdn.githubraw.com/AxA-Software/ZaikaJunction/main/OnlineFastFoodDelivery/wwwroot/img/Logo.png";

        var emailMessage = new Email
        {
            ToEmail = email,
            Subject = "🧾 Order Confirmation – Zaika Junction",
            Body = $@"
                <div style='font-family:Segoe UI; background:#fff; padding:25px; border:2px solid #f1c40f; border-radius:10px; max-width:600px; margin:auto;'>
                    <div style='text-align:center;'>
                        <img src='{logoUrl}' alt='Zaika Logo' style='max-width:150px;' />
                    </div>
                    <h2 style='color:#111; text-align:center;'>🎉 Congratulations!</h2>
                    <p style='text-align:center;'>Your order has been placed successfully at <strong>Zaika Junction</strong>.</p>
                    <div style='background-color:#fef9e7; padding:20px; margin-top:20px; border-radius:8px;'>
                        <h3 style='color:#f1c40f;'>📦 Order Details:</h3>
                        <p><strong>Order Number:</strong> #{orderId}</p>
                        <p><strong>Transaction ID:</strong> {transactionId}</p>
                        <p><strong>Status:</strong> <span style='color:green;'>{orderStatus}</span></p>
                    </div>
                    <p style='margin-top:20px;'>If you have questions, contact us at 
                        <a href='mailto:i.m.akshat.dwivedi@gmail.com'>support</a>.
                    </p>
                    <p style='font-size:12px; text-align:center;'>Sent on {DateTime.Now:dddd, MMMM d, yyyy - hh:mm tt}</p>
                </div>"
        };

        return await _mailService.SendMailAsync(emailMessage);
    }
}
