using System;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;

using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

using Torshify.Server.Extensions;
using Torshify.Server.Interfaces;
using Torshify.Server.Services;

using WcfContrib.Hosting;

namespace Torshify.Server
{
    public class Bootstrapper : IDisposable
    {
        #region Fields

        private CassiniDev.Server _server;

        #endregion Fields

        #region Constructors

        public Bootstrapper()
        {
            WebsiteEnabled = true;
            WebsitePath = Path.Combine(Environment.CurrentDirectory, "web");
            WebsitePort = 8080;
            WcfHttpPort = 1338;
            WcfTcpPort = 1337;
        }

        #endregion Constructors

        #region Properties

        public bool WebsiteEnabled
        {
            get;
            set;
        }

        public string WebsitePath
        {
            get; 
            set;
        }

        public int WebsitePort
        {
            get;
            set;
        }

        public int WcfHttpPort
        {
            get;
            set;
        }

        public int WcfTcpPort
        {
            get;
            set;
        }

        public string UserName
        {
            get; 
            set;
        }

        public string Password
        {
            get; 
            set;
        }

        protected IUnityContainer Container
        {
            get;
            private set;
        }

        protected ILog Logger
        {
            get;
            private set;
        }

        #endregion Properties

        #region Public Methods

        public void Run()
        {
            InitializeWebServer();
            InitializeLogging();
            InitializeContainer();
            InitializeSpotify();
            InitializeServices();
            InitializeStartables();

            Logger.Info("torshify server initialized");

            Console.ReadLine();
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        #endregion Public Methods

        #region Private Methods

        private ServiceHost CreateNetTcpServiceHost<T>(string name, Action<ServiceHost<T>> extraConfiguration = null)
        {
            ServiceHost<T> host = ServiceConfigurationDescription.Create(name)
                                    .WithNetTcp(WcfTcpPort, KnownEndpointConfiguration.NetTcp, KnownSecurityMode.None)
                                    .MakeDiscoverable()
                                    .GenerateServiceHost<T>(h => h.ApplyBoosting = true);

            if (extraConfiguration != null)
            {
                extraConfiguration(host);
            }

            return host;
        }

        private ServiceHost CreateWebHttpServiceHost<T>(string name, Action<ServiceHost<T>> extraConfiguration = null)
        {
            var host = ServiceConfigurationDescription.Create(name)
                        .WithWebHttp(WcfHttpPort, "web", KnownEndpointConfiguration.WebHttp, KnownSecurityMode.None)
                        .MakeDiscoverable()
                        .GenerateServiceHost<T>(h => h.ApplyBoosting = true);

            var endPoint = host.Description.Endpoints.FirstOrDefault(p => p.Binding.GetType() == typeof(WebHttpBinding));

            if (endPoint != null)
            {
                var webBehavior = endPoint.Behaviors.Find<WebHttpBehavior>();
                webBehavior.HelpEnabled = true;
            }

            if (extraConfiguration != null)
            {
                extraConfiguration(host);
            }

            return host;
        }

        private void InitializeWebServer()
        {
            if (WebsiteEnabled)
            {
                _server = new CassiniDev.Server(WebsitePort, "/torshify", WebsitePath, IPAddress.Any);
                _server.Start();
            }
        }

        private void InitializeLogging()
        {
            var fileAppender = new RollingFileAppender();
            fileAppender.File = Path.Combine(Constants.LogFolder, "Torshify.Server.log");
            fileAppender.AppendToFile = true;
            fileAppender.MaxSizeRollBackups = 10;
            fileAppender.MaxFileSize = 1024 * 1024;
            fileAppender.RollingStyle = RollingFileAppender.RollingMode.Size;
            fileAppender.StaticLogFileName = true;
            fileAppender.Layout = new PatternLayout("%date{dd MMM yyyy HH:mm} [%thread] %-5level %logger - %message%newline");
            fileAppender.Threshold = Level.Info;
            fileAppender.ActivateOptions();

            var consoleAppender = new ColoredConsoleAppender();
            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity,
                    BackColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Fatal
                });
            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Error
                });
            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Yellow | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Warn
                });
            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.Green | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Info
                });
            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                {
                    ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity,
                    Level = Level.Info
                });
            consoleAppender.Layout = new PatternLayout("%date{dd MM HH:mm} %-5level - %message%newline");
#if DEBUG
            consoleAppender.Threshold = Level.All;
#else
            consoleAppender.Threshold = Level.Info;
#endif
            consoleAppender.ActivateOptions();

            Logger root;
            root = ((Hierarchy)LogManager.GetRepository()).Root;
            root.AddAppender(consoleAppender);
            root.AddAppender(fileAppender);
            root.Repository.Configured = true;

            Logger = LogManager.GetLogger("Bootstrapper");

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception exception = (Exception)e.ExceptionObject;
                Logger.Fatal(exception);
            };
        }

        private void InitializeContainer()
        {
            Container = new UnityContainer();
            Container.RegisterStartable<IPlayer, Player>();
            Container.RegisterType<IPlayerPlaylist, PlayerPlaylist>(new ContainerControlledLifetimeManager());
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));
        }

        private void InitializeSpotify()
        {
            Directory.CreateDirectory(Constants.AppDataFolder);
            Directory.CreateDirectory(Constants.CacheFolder);
            Directory.CreateDirectory(Constants.SettingsFolder);

            ISession session =
                SessionFactory
                    .CreateSession(
                        Constants.ApplicationKey,
                        Constants.CacheFolder,
                        Constants.SettingsFolder,
                        Constants.UserAgent)
                    .SetPrefferedBitrate(Bitrate.Bitrate320k);

            session.ConnectionError += (sender, e) => Logger.Debug(e.Status + " - " + e.Message);
            session.EndOfTrack += (sender, e) => Logger.Debug(e.Status + " - " + e.Message);
            session.LoginComplete += (sender, e) =>
                                         {
                                             if (e.Status != Error.OK)
                                             {
                                                 Logger.Fatal("Unable to log in to Spotify. " + e.Status.GetMessage());
                                                 Environment.Exit(-1);
                                             }
                                         };
            session.LogoutComplete += (sender, e) => Logger.Debug(e.Status + " - " + e.Message);
            session.MessageToUser += (sender, e) => Logger.Debug(e.Status + " - " + e.Message);

            // Set up basic spotify logging
            session.LogMessage += (s, e) => Logger.Debug(e.Message);
            session.Exception += (s, e) => Logger.Error(e.Status.GetMessage() + " - " + e.Message);

            Container.RegisterInstance(session);

            Logger.Debug("Spotify session created");

            try
            {
                session.Login(UserName, Password);
            }
            catch (Exception e)
            {
                Logger.Fatal(e.Message);
                Environment.Exit(-1);
            }
        }

        private void InitializeServices()
        {
            ServiceHost[] hosts =
            {
                CreateWebHttpServiceHost<ToplistBrowseService>("torshify/toplists"),
                CreateNetTcpServiceHost<ToplistBrowseService>("torshify/toplists"),
                CreateWebHttpServiceHost<SearchService>("torshify/search"),
                CreateNetTcpServiceHost<SearchService>("torshify/search"),
                CreateWebHttpServiceHost<PlaylistWebService>("torshify/playlists"),
                CreateNetTcpServiceHost<PlaylistService>("torshify/playlists"),
                CreateWebHttpServiceHost<ImageService>("torshify/image"),
                CreateNetTcpServiceHost<ImageService>("torshify/image"),
                CreateWebHttpServiceHost<PlayerWebService>("torshify/player"),
                CreateNetTcpServiceHost<PlayerService>("torshify/player")
            };

            foreach (var host in hosts)
            {
                host.Open();

                foreach (var endpoint in host.Description.Endpoints)
                {
                    Logger.DebugFormat("Endpoint: {0}", endpoint.Address);
                }
            }
        }

        private void InitializeStartables()
        {
            var startables = Container.ResolveAll<IStartable>();

            foreach (var startable in startables)
            {
                startable.Start();
            }
        }

        #endregion Private Methods
    }
}