using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Azure.Stores
{
    public class FeedbackStore : BaseStore<Feedback>, IFeedbackStore
    {
        public FeedbackStore() : base() { }
        public async Task<bool> LeftFeedback(Session session)
        {
            var items = await Table.Where(s => s.SessionId == session.Id).ToListAsync().ConfigureAwait(false);
            return items.Count > 0;
        }

        public async Task<bool> LeftFeedback(Workshop workshop)
        {
            var items = await Table.Where(s => s.WorkshopId == workshop.Id).ToListAsync().ConfigureAwait(false);
            return items.Count > 0;
        }

        public Task DropFeedback()
        {
            return Task.FromResult(true);
        }



        public override string Identifier => "Feedback";

    }
}
