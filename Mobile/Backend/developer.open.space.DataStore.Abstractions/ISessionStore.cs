using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Abstractions
{
    public interface ISessionStore : IBaseStore<Session>
    {
        Task<IEnumerable<Session>> GetSpeakerSessionsAsync(string speakerId);
        Task<IEnumerable<Session>> GetNextSessions();
        Task<Session> GetAppIndexSession(string id);
    }
}
