using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Web_CuaHangCafe.Models.Authentication
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Lấy thông tin đăng nhập và role từ session
            string tenTaiKhoan = context.HttpContext.Session.GetString("TenTaiKhoan");
            string role = context.HttpContext.Session.GetString("Role");

            // Kiểm tra nếu chưa đăng nhập hoặc role không phải là Admin
            if (string.IsNullOrEmpty(tenTaiKhoan) && (role != "Admin" && role != "Employee"))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "access1" },
                    { "action", "login1" }
                });
            }

            base.OnActionExecuting(context);
        }
    }
}
