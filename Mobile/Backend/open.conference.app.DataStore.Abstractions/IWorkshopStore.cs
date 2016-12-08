using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IWorkshopStore : IBaseStore<Workshop>
    {
        Task<IEnumerable<Workshop>> GetSpeakerWorkshopsAsync(string speakerId);
        Task<IEnumerable<Workshop>> GetNextWorkshops();
        Task<Workshop> GetAppIndexWorkshops(string id);
    }
}
