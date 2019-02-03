using System.Net;  
    using System.Net.Http;  
    using System.Web.Http.Filters;  
namespace CoreAPI.CustomFilter
{

    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;
            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }
            //We can log this exception message to the file or database.  
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("خطا در پردازش عملیات ، لطفا در صورت مشاهده دوباره خطا با مدیر سیستم تماس حاصل فرمایید.")
            };
            actionExecutedContext.Response = response;
        }
    }
}