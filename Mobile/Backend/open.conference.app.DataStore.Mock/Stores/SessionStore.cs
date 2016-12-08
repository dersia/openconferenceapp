using open.conference.app.DataStore.Abstractions;
using open.conference.app.DataStore.Abstractions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace open.conference.app.DataStore.Mock.Stores
{
    public class SessionStore : BaseStore<Session>, ISessionStore
    {

        List<Session> sessions;
        ISpeakerStore _speakerStore;
        ICategoryStore _categoryStore;
        IFavoriteStore _favoriteStore;
        IFeedbackStore _feedbackStore;
        public SessionStore(ISpeakerStore speakerStore, ICategoryStore categoryStore, IFavoriteStore favoriteStore, IFeedbackStore feedbackStore)
        {
            _speakerStore = speakerStore;
            _favoriteStore = favoriteStore;
            _categoryStore = categoryStore;
            _feedbackStore = feedbackStore;
        }

        #region ISessionStore implementation

        public async override Task<Session> GetItemAsync(string id)
        {
            if (!initialized)
                await InitializeStore();

            return sessions.FirstOrDefault(s => s.Id == id);
        }

        public async override Task<IEnumerable<Session>> GetItemsAsync(bool forceRefresh = false)
        {
            if (!initialized)
                await InitializeStore();

            return sessions as IEnumerable<Session>;
        }

        public async Task<IEnumerable<Session>> GetSpeakerSessionsAsync(string speakerId)
        {
            if (!initialized)
                await InitializeStore();

            var results = from session in sessions
                          where session.StartTime.HasValue
                          orderby session.StartTime.Value
                          from speaker in session.Speakers
                          where speaker.Id == speakerId
                          select session;

            return results;
        }

        public async Task<IEnumerable<Session>> GetNextSessions()
        {
            if (!initialized)
                await InitializeStore();

            var date = DateTime.UtcNow.AddMinutes(-30);

            var results = (from session in sessions
                           where (session.IsFavorite && session.StartTime.HasValue && session.StartTime.Value > date)
                           orderby session.StartTime.Value
                           select session).Take(2);


            var enumerable = results as Session[] ?? results.ToArray();
            return !enumerable.Any() ? null : enumerable;
        }

        #endregion

        #region IBaseStore implementation
        bool initialized = false;
        public async override Task InitializeStore()
        {
            if (initialized)
                return;

            initialized = true;
            var categories = (await _categoryStore.GetItemsAsync()).ToArray();
            var speakers = (await _speakerStore.GetItemsAsync().ConfigureAwait(false)).ToArray();
            sessions = new List<Session>();
            int speaker = 0;
            int speakerCount = 0;
            int room = 0;
            int category = 0;
            var day = new DateTime(2016, 4, 27, 13, 0, 0, DateTimeKind.Utc);
            int dayCount = 0;
            for (int i = 0; i < titles.Length; i++)
            {
                var sessionSpeakers = new List<Speaker>();
                speakerCount++;

                for (int j = 0; j < speakerCount; j++)
                {
                    sessionSpeakers.Add(speakers[speaker]);
                    speaker++;
                    if (speaker >= speakers.Length)
                        speaker = 0;
                }

                if (i == 1)
                    sessionSpeakers.Add(sessions[0].Speakers.ElementAt(0));

                var cat = categories[category];
                category++;
                if (category >= categories.Length)
                    category = 0;

                var ro = rooms[room];
                room++;
                if (room >= rooms.Length)
                    room = 0;

                sessions.Add(new Session
                {
                    Id = i.ToString(),
                    Abstract = "This is an abstract that is going to tell us all about how awsome this session is and that you should go over there right now and get ready for awesome!.",
                    MainCategory = cat,
                    Room = ro,
                    Speakers = sessionSpeakers,
                    Title = titles[i],
                    ShortTitle = titlesShort[i]
                });

                sessions[i].IsFavorite = await _favoriteStore.IsFavorite(sessions[i].Id);
                sessions[i].FeedbackLeft = await _feedbackStore.LeftFeedback(sessions[i]);

                SetStartEnd(sessions[i], day);

                if (i == titles.Length / 2)
                {
                    dayCount = 0;
                    day = new DateTime(2016, 4, 28, 13, 0, 0, DateTimeKind.Utc);
                }
                else
                {
                    dayCount++;
                    if (dayCount == 2)
                    {
                        day = day.AddHours(1);
                        dayCount = 0;
                    }
                }


                if (speakerCount > 2)
                    speakerCount = 0;
            }


            sessions.Add(new Session
            {
                Id = sessions.Count.ToString(),
                Abstract = "Coming soon",
                MainCategory = categories[0],
                Room = rooms[0],
                //Speakers = new List<Speaker>{ speakers[0] },
                Title = "Something awesome!",
                ShortTitle = "Awesome",
            });
            sessions[sessions.Count - 1].IsFavorite = await _favoriteStore.IsFavorite(sessions[sessions.Count - 1].Id);
            sessions[sessions.Count - 1].FeedbackLeft = await _feedbackStore.LeftFeedback(sessions[sessions.Count - 1]);
            sessions[sessions.Count - 1].StartTime = null;
            sessions[sessions.Count - 1].EndTime = null;
        }

        void SetStartEnd(Session session, DateTime day)
        {
            session.StartTime = day;
            session.EndTime = session.StartTime.Value.AddHours(1);
        }

        public Task<Session> GetAppIndexSession(string id)
        {
            return GetItemAsync(id);
        }

        Room[] rooms = new[]
        {
                new Room {Name = "Fossy Salon"},
                new Room {Name = "Crick Salon"},
                new Room {Name = "Franklin Salon"},
                new Room {Name = "Goodall Salon"},
                new Room {Name = "Linnaeus Salon"},
                new Room {Name = "Watson Salon"},
        };




        string[] titles = new[] {
            "Agile Acceptance Test Automation",
            "Entwicklung für Apple, Google und Windows mit Xamarin",
            "Akka.NET",
            "Flow-Based Programming mit Node-RED",
            "Anforderungen zähmen leicht gemacht",
            "Gamestorming",
            "Angular 2",
            "Internet of Things als verteiltes System",
            "Ionic 2 und Angular 2 – Hybrid-Apps auf Steroiden",
            "Automation mit Raspberry Pi – First steps",
            "Kulturwandel für mehr Agilität",
            "Building native iOS and Android apps with Angular 2 and NativeScript",
            "Legal see sharp – Recht und Ethik in der Softwareentwicklung",
            "Codebasen zu async/await refaktorisieren",
            "Microservices mit .NET und RabbitMQ",
            "Cognitive Services",
            "Der Nutzer ist König – Von der Idee zum ersten Prototypen",
            "Modernes JavaScript mit ECMAScript 2015",
            "Dependent Types mit Idris (Programmiersprache)",
            "Python für Schwimmer",
            "Domain Driven Design mit funktionalen Sprachen",
            "Verteilte Compiler mit Roslyn",
            "Einstieg in die Entwicklung mit UWP",
            "Angewandte Konsensdemokratie und der Umgang mit schwierigen Entscheidungen"

        };

        string[] titlesShort = new[] {
            "Agile Acceptance Test Automation",
            "Entwicklung für Apple, Google und Windows mit Xamarin",
            "Akka.NET",
            "Flow-Based Programming mit Node-RED",
            "Anforderungen zähmen leicht gemacht",
            "Gamestorming",
            "Angular 2",
            "Internet of Things als verteiltes System",
            "Ionic 2 und Angular 2 – Hybrid-Apps auf Steroiden",
            "Automation mit Raspberry Pi – First steps",
            "Kulturwandel für mehr Agilität",
            "Building native iOS and Android apps with Angular 2 and NativeScript",
            "Legal see sharp – Recht und Ethik in der Softwareentwicklung",
            "Codebasen zu async/await refaktorisieren",
            "Microservices mit .NET und RabbitMQ",
            "Cognitive Services",
            "Der Nutzer ist König – Von der Idee zum ersten Prototypen",
            "Modernes JavaScript mit ECMAScript 2015",
            "Dependent Types mit Idris (Programmiersprache)",
            "Python für Schwimmer",
            "Domain Driven Design mit funktionalen Sprachen",
            "Verteilte Compiler mit Roslyn",
            "Einstieg in die Entwicklung mit UWP",
            "Angewandte Konsensdemokratie und der Umgang mit schwierigen Entscheidungen"
        };

        #endregion
    }
}
