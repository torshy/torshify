using System;
using System.Reflection;

using NDesk.Options;

namespace Torshify.Server
{
    class Program
    {
        #region Properties

        public static Bootstrapper Bootstrapper
        {
            get;
            private set;
        }

        #endregion Properties

        #region Private Static Methods

        static void Main(string[] args)
        {
            InitializeAssemblyResolve();

            Bootstrapper = new Bootstrapper();

            InitializeCommandLineOptions(args);

            Bootstrapper.Run();
        }

        private static void InitializeAssemblyResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                                                           {
                                                               String resourceName = "Torshify.Server.Dependencies." +
                                                                                     new AssemblyName(args.Name).Name + ".dll";

                                                               using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                                                               {
                                                                   Byte[] assemblyData = new Byte[stream.Length];
                                                                   stream.Read(assemblyData, 0, assemblyData.Length);
                                                                   return Assembly.Load(assemblyData);
                                                               }
                                                           };
        }

        private static void InitializeCommandLineOptions(string[] args)
        {
            bool showHelp = false;

            var p = new OptionSet
                        {
                            { "u|username=", "spotify username", userName => Bootstrapper.UserName = userName },
                            { "p|password=", "spotify password", password => Bootstrapper.Password = password },
                            { "websiteEnabled=", "host website", (bool enabled) => Bootstrapper.WebsiteEnabled = enabled },
                            { "websitePath=", "path for the web site", path => Bootstrapper.WebsitePath = path },
                            { "websitePort=", "the port the website will be hosted on", (int port) => Bootstrapper.WebsitePort = port },
                            { "wcfHttpPort=", "the port the http wcf services will be hosted on", (int port) => Bootstrapper.WcfHttpPort = port },
                            { "wcfTcpPort=", "the port the tcp wcf services will be hosted on", (int port) => Bootstrapper.WcfTcpPort = port },
                            { "h|help",  "show this message and exit", v => showHelp = v != null }
                        };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
            }

            if (showHelp)
            {
                p.WriteOptionDescriptions(Console.Out);
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        #endregion Private Static Methods
    }
}