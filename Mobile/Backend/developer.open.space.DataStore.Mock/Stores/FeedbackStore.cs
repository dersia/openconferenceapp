using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace developer.open.space.DataStore.Mock.Stores
{
    public class FeedbackStore : BaseStore<Feedback>, IFeedbackStore
    {
        private developer.open.space.DataStore.Mock.Interfaces.ISettings _settings;

        public FeedbackStore(developer.open.space.DataStore.Mock.Interfaces.ISettings settings)
        {
            _settings = settings;
        }
        public Task<bool> LeftFeedback(Session session)
        {
            return Task.FromResult(_settings.LeftFeedback(session.Id));
        }

        public Task<bool> LeftFeedback(Workshop workshop)
        {
            return Task.FromResult(_settings.LeftFeedback(workshop.Id));
        }

        public async Task DropFeedback()
        {
            await _settings.ClearFeedback();
        }

        public override Task<bool> InsertAsync(Feedback item)
        {
            _settings.LeaveFeedback(item.SessionId, true);
            return Task.FromResult(true);
        }

        public override Task<bool> RemoveAsync(Feedback item)
        {
            _settings.LeaveFeedback(item.SessionId, false);
            return Task.FromResult(true);
        }
    }
}
