using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions
{
    public interface IWorkshopStore : IBaseStore<Workshop>
    {
        Task<IEnumerable<Workshop>> GetSpeakerWorkshopsAsync(string speakerId);
        Task<IEnumerable<Workshop>> GetNextWorkshops();
        Task<Workshop> GetAppIndexWorkshops(string id);
    }
}
