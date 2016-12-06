using developer.open.space.Clients.ViewModels.Interfaces;
using developer.open.space.DataStore.Abstractions;
using developer.open.space.DataStore.Abstractions.DataObjects;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Navigation;
using Prism.Services;
using System.Threading.Tasks;
using System.Windows.Input;

namespace developer.open.space.Clients.ViewModels.ViewModels
{
    public class FeedbackViewModel : ViewModelBase
    {
        public FeedbackViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IStoreManager storeManager, IToast toast, IFavoriteService favoriteService, ILoggerFacade logger, ILaunchTwitter twitter, ISSOClient ssoClient, IPushNotifications pushNotifications, IReminderService reminderService, IPageDialogService pageDialogService) : base(navigationService, eventAggregator, storeManager, toast, favoriteService, logger, twitter, ssoClient, pushNotifications, reminderService, pageDialogService)
        {
            eventAggregator.GetEvent<SelectedWorkshopForFeedbackEvent>().Subscribe();
        }

        private void WorkshopFroFeedbackSelected(Workshop workshop)
        {
            Workshop = workshop;
        }

        #region Properties

        private Workshop _workshop;
        public Workshop Workshop
        {
            get { return _workshop; }
            set { SetProperty(ref _workshop, value); }
        }

        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set { SetProperty(ref _rating, value); }
        }

        #endregion

        public ICommand SubmitFeedbackCommand => DelegateCommand.FromAsyncHandler(SubmitFeedback);

        private async Task SubmitFeedback()
        {
            try
            {
                if (!Settings.IsLoggedIn)
                {
                    await PageDialogService.DisplayAlertAsync("Error", "No authenticated user!", "OK");
                    return;
                }

                var feedback = new Feedback();
                feedback.WorkshopId = Workshop.Id;
                feedback.WorkshopRating = Rating;
                feedback.UserId = Settings.Email;

                var response = await StoreManager.FeedbackStore.InsertAsync(feedback);

                if (response)
                {
                    await PageDialogService.DisplayAlertAsync("Thank you!", "Feedback submitted", "OK");
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync("Error", "Feedback not submitted", "OK");
                }
            }
            catch
            {
                await PageDialogService.DisplayAlertAsync("Error", "Feedback not submitted", "OK");
            }
        }
    }
}