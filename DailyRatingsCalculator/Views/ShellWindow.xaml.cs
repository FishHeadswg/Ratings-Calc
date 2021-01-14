using DailyRatingsCalculator.Constants;
using DailyRatingsCalculator.Contracts.Services;

using MahApps.Metro.Controls;

using Prism.Regions;

namespace DailyRatingsCalculator.Views
{
    public partial class ShellWindow : MetroWindow
    {
        public ShellWindow(IRegionManager regionManager, IRightPaneService rightPaneService)
        {
            InitializeComponent();
            RegionManager.SetRegionName(menuContentControl, Regions.Main);
            RegionManager.SetRegionManager(menuContentControl, regionManager);
            rightPaneService.Initialize(splitView, rightPaneContentControl);
        }
    }
}
