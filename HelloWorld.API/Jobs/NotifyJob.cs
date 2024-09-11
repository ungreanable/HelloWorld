using HelloWorld.Services;
using Quartz;

namespace HelloWorld.API.Jobs
{
    public class NotifyJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var updated = await NotifyHelper.CheckStatusUpdated("Response/response.json");
                if(updated)
                {
                    await NotifyHelper.NotifyLine($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}]: Claim Status Updated");
                }
                else
                {
                    await NotifyHelper.NotifyLine($"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}]: Claim Status Nothing Changed");
                }
            }
            catch(Exception ex)
            {
                await NotifyHelper.NotifyLine(ex.Message);
            }
            //Write your custom code here
        }
    }
}
