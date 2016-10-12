using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.Utils.Helpers;
using Plugin.Calendars;
using Plugin.Calendars.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace developer.open.space.Clients.Application.Services
{
    public class ReminderService : IReminderService
    {
        private IPushNotifications _pushNotificationService;
        private IPageDialogService _pageDialogService;

        public ReminderService(IPushNotifications pushNotificationService, IPageDialogService pageDialogService)
        {
            _pushNotificationService = pushNotificationService;
            _pageDialogService = pageDialogService;
        }

        public async Task<bool> HasReminderAsync(string id)
        {
            if (!Settings.Current.HasSetReminder)
                return false;

            var ready = await CheckPermissionsGetCalendarAsync(false);
            if (!ready)
                return false;

            var externalId = Settings.Current.GetEventId(id);
            if (string.IsNullOrWhiteSpace(externalId))
                return false;

            try
            {
                var calEvent = await CrossCalendars.Current.GetEventByIdAsync(externalId);
                return calEvent != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Event has an Id, but doesn't exist, removing" + ex);
                Settings.Current.RemoveReminderId(id);

            }
            return false;
        }

        public async Task<bool> AddReminderAsync(string id, CalendarEvent calEvent)
        {
            var ready = await CheckPermissionsGetCalendarAsync();
            if (!ready)
                return false;

            try
            {
                var devopenspaceCal = await GetOrCreateDevopenspaceCalendarAsync();
                //Create event and then create the reminder!
                await CrossCalendars.Current.AddOrUpdateEventAsync(devopenspaceCal, calEvent);
                await CrossCalendars.Current.AddEventReminderAsync(calEvent, new CalendarEventReminder
                {
                    Method = CalendarReminderMethod.Default,
                    TimeBefore = TimeSpan.FromMinutes(20)
                });
                Settings.Current.SaveReminderId(id, calEvent.ExternalID);

                if (!Settings.Current.HasSetReminder)
                {
                    await _pageDialogService.DisplayAlertAsync("Reminder Added", $"Reminder has been added.Please check the {devopenspaceCal.Name} calendar.", "OK");
                }
                Settings.Current.HasSetReminder = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to create event: " + ex);
                await _pageDialogService.DisplayAlertAsync("Event Creation", $"Unable to create calendar event, please check calendar app and try again.", "OK");
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveReminderAsync(string id)
        {
            var ready = await CheckPermissionsGetCalendarAsync();
            if (!ready)
                return false;


            try
            {
                var devopenspaceCal = await GetOrCreateDevopenspaceCalendarAsync();
                var externalId = Settings.Current.GetEventId(id);
                var calEvent = await CrossCalendars.Current.GetEventByIdAsync(externalId);
                await CrossCalendars.Current.DeleteEventAsync(devopenspaceCal, calEvent);
                Settings.Current.RemoveReminderId(id);
                Settings.Current.HasSetReminder = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to delete event: " + ex);
                await _pageDialogService.DisplayAlertAsync("Event Deletion", $"Unable to delete calendar event, please check calendar app and try again.", "OK");
                return false;
            }
            return true;
        }


        private async Task<bool> CheckPermissionsGetCalendarAsync(bool alert = true)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Calendar);
            if (status != PermissionStatus.Granted)
            {
                var request = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Calendar);
                if (!request.ContainsKey(Permission.Calendar) || request[Permission.Calendar] != PermissionStatus.Granted)
                {
                    if (alert)
                    {
                        if (Device.OS == TargetPlatform.iOS)
                        {
                            var result = await _pageDialogService.DisplayAlertAsync("Calendar Permission", $"Unable to set reminders as the Calendar permission was not granted. Please go into Settings and turn on Calendars for Developer Open Space 2016.", "Settings", "Maybe Later");
                            if(result)
                            {
                                _pushNotificationService.OpenSettings();
                            }
                        }
                        else
                        {
                            await _pageDialogService.DisplayAlertAsync("Calendar Permission", $"Unable to set reminders as the Calendar permission was not granted, please check your settings and try again.", "OK");
                        }
                    }

                    return false;
                }
            }

            var currentCalendar = await GetOrCreateDevopenspaceCalendarAsync();

            if (currentCalendar == null)
            {
                if (alert)
                {
                    await _pageDialogService.DisplayAlertAsync("No Calendar", $"We were unable to get or create the Developer Open Space calendar, please check your calendar app and try again.", "OK");
                }
                return false;
            }

            return true;
        }


        private async Task<Calendar> GetOrCreateDevopenspaceCalendarAsync()
        {

            var id = Settings.Current.DevopenspaceCalendarId;
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    var calendar = await CrossCalendars.Current.GetCalendarByIdAsync(id);
                    if (calendar != null)
                        return calendar;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to get calendar.. odd as we created it already: " + ex);

                }

            }

            //if for some reason the calendar does not exist then simply create a enw one.
            if (Device.OS == TargetPlatform.Android)
            {
                //On android it is really hard to delete a calendar made by an app, so just add to default calendar.
                try
                {
                    var calendars = await CrossCalendars.Current.GetCalendarsAsync();
                    foreach (var calendar in calendars)
                    {
                        //find first calendar we can add stuff to
                        if (!calendar.CanEditEvents)
                            continue;

                        Settings.Current.DevopenspaceCalendarId = calendar.ExternalID;
                        return calendar;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to get calendars.. " + ex);
                }
            }
            else
            {
                //try to find Devopenspace app if already uninstalled for some reason
                try
                {
                    var calendars = await CrossCalendars.Current.GetCalendarsAsync();
                    foreach (var calendar in calendars)
                    {
                        //find first calendar we can add stuff to
                        if (calendar.CanEditEvents && calendar.Name == "Developer Open Space")
                        {
                            Settings.Current.DevopenspaceCalendarId = calendar.ExternalID;
                            return calendar;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to get calendars.. " + ex);
                }
            }

            var devopenspaceCalendar = new Calendar();
            devopenspaceCalendar.Color = "#7635EB";
            devopenspaceCalendar.Name = "Developer Open Space";

            try
            {
                await CrossCalendars.Current.AddOrUpdateCalendarAsync(devopenspaceCalendar);
                Settings.Current.DevopenspaceCalendarId = devopenspaceCalendar.ExternalID;
                return devopenspaceCalendar;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to create calendar.. " + ex);
            }

            return null;
        }


    }
}
