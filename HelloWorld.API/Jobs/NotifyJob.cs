using HelloWorld.Services;
using Quartz;

namespace HelloWorld.API.Jobs
{
    public class NotifyJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await NotifyHelper.CheckStatusUpdated("Response/response.json");
            //Write your custom code here
        }
    }
}
