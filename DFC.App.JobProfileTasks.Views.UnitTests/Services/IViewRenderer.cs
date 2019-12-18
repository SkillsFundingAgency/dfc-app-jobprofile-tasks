using System.Collections.Generic;

namespace DFC.App.JobProfileTasks.Views.UnitTests.Services
{
    public interface IViewRenderer
    {
        string Render(string templateValue, object model, IDictionary<string, object> viewBag);
    }
}