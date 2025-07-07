using BLL.Implementation;
using BLL.Interfaces;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Models;
using static System.Net.WebRequestMethods;

namespace OnlineFastFoodDelivery.Controllers
{
    public class EmailController : Controller
    {
        private readonly SendMailDAO mailService;
        public EmailController(SendMailDAO mailService) 
        {
            this.mailService=mailService;
        }
        [HttpPost]
        public async Task<bool> SendOTP([FromBody] string EMailID)
        {
            bool isSuccess = false;
            try
            {
               UserLogin DAL=new UserLoginDao();
                string OTP = await DAL.GenerateOTP();
                if (OTP != null)
                {
                    Email emailMessage = new Email
                    {
                        ToEmail = EMailID,
                        Subject = "🔐 Zaika Junction - Password Reset OTP",
                        Body = $@"
        <div style='font-family:Segoe UI, sans-serif; background-color:#f8f9fa; padding:20px; border-radius:10px; color:#333; max-width:600px; margin:auto; border:1px solid #dee2e6;'>
            <h2 style='color:#2c3e50;'>👋 Hello Zaika User!</h2>
            <p style='font-size:16px;'>We received a request to reset your password.</p>
            <p style='font-size:18px; font-weight:bold; color:#444;'>🔐 Your One-Time Password (OTP) is:</p>
            <div style='font-size:28px; font-weight:bold; background-color:#eaf4ff; padding:15px; text-align:center; border-radius:8px; border:1px dashed #007bff; color:#007bff; margin:20px 0;'>{OTP}</div>
            <p style='font-size:16px;'>Please enter this OTP on the password reset page. This OTP is valid for a limited time and can only be used once. ⏳</p>
            <hr style='border:none; border-top:1px solid #ccc; margin:20px 0;' />
            <p style='font-size:14px; color:#666;'>If you didn’t request a password reset, you can ignore this email. Your password will remain unchanged.</p>
            <p style='font-size:14px; color:#888;'>Sent on: <b>{DateTime.Now:dddd, MMMM d, yyyy - hh:mm tt}</b></p>
            <p style='font-size:13px; color:#aaa;'>Thank you,<br/>💡 Zaika Junction Security Team</p>
        </div>"
                    };


                    isSuccess = await mailService.SendMailAsync(emailMessage);
                    var creationTime=DateTime.Now;
                    HttpContext.Session.SetString("OTP", OTP);
                    HttpContext.Session.SetString("OTPCreationTimeStamp", creationTime.ToString());
                }
                if (isSuccess)
                {
                   
                    return true;
                    
                }
                else
                {
                    
                    return false;
                }
                
            }
            catch (Exception ex)
            {
                
                TempData["Error"] = "Authentication Failed";
                return false;
            }
        }
        public async Task<bool> SendEmail([FromBody] string EMailID)
        {
            bool isSuccess = false;
            try
            {
                UserLogin DAL = new UserLoginDao();
                string OTP = await DAL.GenerateOTP();
                if (OTP != null)
                {
                    Email emailMessage = new Email
                    {
                        ToEmail = EMailID,
                        Subject = "🔐 Zaika Junction - Password Reset OTP",
                        Body = $@"
        <div style='font-family:Segoe UI, sans-serif; background-color:#f8f9fa; padding:20px; border-radius:10px; color:#333; max-width:600px; margin:auto; border:1px solid #dee2e6;'>
            <h2 style='color:#2c3e50;'>👋 Hello Zaika User!</h2>
            <p style='font-size:16px;'>We received a request to reset your password.</p>
            <p style='font-size:18px; font-weight:bold; color:#444;'>🔐 Your One-Time Password (OTP) is:</p>
            <div style='font-size:28px; font-weight:bold; background-color:#eaf4ff; padding:15px; text-align:center; border-radius:8px; border:1px dashed #007bff; color:#007bff; margin:20px 0;'>{OTP}</div>
            <p style='font-size:16px;'>Please enter this OTP on the password reset page. This OTP is valid for a limited time and can only be used once. ⏳</p>
            <hr style='border:none; border-top:1px solid #ccc; margin:20px 0;' />
            <p style='font-size:14px; color:#666;'>If you didn’t request a password reset, you can ignore this email. Your password will remain unchanged.</p>
            <p style='font-size:14px; color:#888;'>Sent on: <b>{DateTime.Now:dddd, MMMM d, yyyy - hh:mm tt}</b></p>
            <p style='font-size:13px; color:#aaa;'>Thank you,<br/>💡 Zaika Junction Security Team</p>
        </div>"
                    };


                    isSuccess = await mailService.SendMailAsync(emailMessage);
                    var creationTime = DateTime.Now;
                    HttpContext.Session.SetString("OTP", OTP);
                    HttpContext.Session.SetString("OTPCreationTimeStamp", creationTime.ToString());
                }
                if (isSuccess)
                {

                    return true;

                }
                else
                {

                    return false;
                }

            }
            catch (Exception ex)
            {

                TempData["Error"] = "Authentication Failed";
                return false;
            }
        }
        
    }
}
