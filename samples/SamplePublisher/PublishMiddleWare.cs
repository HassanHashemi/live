using Live.Backplane;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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
                var message = new BackplaneMessage("OrderCreated", "xx", "45", new { value = "Hassan Hashemi" });
                await redisBackplaine.Publish(message);
            }
        }
    }
}
