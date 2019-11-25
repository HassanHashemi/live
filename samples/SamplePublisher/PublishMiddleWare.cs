using System.Threading.Tasks;
using Live.Backplane;
using Microsoft.AspNetCore.Http;

namespace SamplePublisher
{
    public class PublishMiddleWare : IMiddleware
    {
        private readonly IBackplaine redisBackplaine;

        public PublishMiddleWare(IBackplaine redisBackplaine)
        {
            this.redisBackplaine = redisBackplaine;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.Contains("push"))
            {
                await redisBackplaine.Publish(new BackplaneMessage { MerchantId = "45", UserId = "xx", Message = new { Name = "Hassan!!" } });
            }
        }
    }
}
