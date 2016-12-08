using Plugin.Calendars.Abstractions;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Abstractions
{
    public interface IReminderService
    {
        Task<bool> AddReminderAsync(string id, CalendarEvent calEvent);
        Task<bool> HasReminderAsync(string id);
        Task<bool> RemoveReminderAsync(string id);
    }
}