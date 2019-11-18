using System.Threading.Tasks;

namespace DFC.App.JobProfileTasks.SegmentService
{
    public interface IJobProfileSegmentRefreshService<in TModel>
    {
        Task SendMessageAsync(TModel model);
    }
}
