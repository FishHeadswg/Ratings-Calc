using System.IO;
using System.Reflection;

using DailyRatingsCalculator.Contracts.Services;
using DailyRatingsCalculator.Core.Contracts.Services;
using DailyRatingsCalculator.Core.Services;
using DailyRatingsCalculator.Models;
using DailyRatingsCalculator.Services;
using DailyRatingsCalculator.ViewModels;

using Microsoft.Extensions.Configuration;

using Prism.Regions;

using Unity;

using Xunit;

namespace DailyRatingsCalculator.Tests.XUnit
{
    public class PagesTests
    {
        private readonly IUnityContainer _container;

        public PagesTests()
        {
            _container = new UnityContainer();
            _container.RegisterType<IRegionManager, RegionManager>();

            // Core Services
            _container.RegisterType<IFileService, FileService>();

            // App Services
            _container.RegisterType<IThemeSelectorService, ThemeSelectorService>();
            _container.RegisterType<ISystemService, SystemService>();
            _container.RegisterType<IPersistAndRestoreService, PersistAndRestoreService>();
            _container.RegisterType<IApplicationInfoService, ApplicationInfoService>();

            // Configuration
            var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var configuration = new ConfigurationBuilder()
                .SetBasePath(appLocation)
                .AddJsonFile("appsettings.json")
                .Build();
            var appConfig = configuration
                .GetSection(nameof(AppConfig))
                .Get<AppConfig>();

            // Register configurations to IoC
            _container.RegisterInstance(configuration);
            _container.RegisterInstance(appConfig);
        }

        // TODO WTS: Add tests for functionality you add to MainViewModel.
        [Fact]
        public void TestMainViewModelCreation()
        {
            var vm = _container.Resolve<MainViewModel>();
            Assert.NotNull(vm);
        }

        // TODO WTS: Add tests for functionality you add to SettingsViewModel.
        [Fact]
        public void TestSettingsViewModelCreation()
        {
            var vm = _container.Resolve<SettingsViewModel>();
            Assert.NotNull(vm);
        }
    }
}
