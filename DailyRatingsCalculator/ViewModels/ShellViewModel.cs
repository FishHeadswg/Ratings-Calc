﻿using System;
using System.Windows;
using System.Windows.Input;

using DailyRatingsCalculator.Constants;
using DailyRatingsCalculator.Contracts.Services;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace DailyRatingsCalculator.ViewModels
{
    // You can show pages in different ways (update main view, navigate, right pane, new windows or dialog)
    // using the NavigationService, RightPaneService and WindowManagerService.
    // Read more about MenuBar project type here:
    // https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/WPF/projectTypes/menubar.md
    public class ShellViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IRightPaneService _rightPaneService;
        private IRegionNavigationService _navigationService;
        private DelegateCommand _goBackCommand;
        private ICommand _menuFileSettingsCommand;
        private ICommand _loadedCommand;
        private ICommand _unloadedCommand;
        private ICommand _menuFileExitCommand;

        public DelegateCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new DelegateCommand(OnGoBack, CanGoBack));

        public ICommand MenuFileSettingsCommand => _menuFileSettingsCommand ?? (_menuFileSettingsCommand = new DelegateCommand(OnMenuFileSettings));

        public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new DelegateCommand(OnLoaded));

        public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new DelegateCommand(OnUnloaded));

        public ICommand MenuFileExitCommand => _menuFileExitCommand ?? (_menuFileExitCommand = new DelegateCommand(OnMenuFileExit));

        public ShellViewModel(IRegionManager regionManager, IRightPaneService rightPaneService)
        {
            _regionManager = regionManager;
            _rightPaneService = rightPaneService;
        }

        private void OnLoaded()
        {
            _navigationService = _regionManager.Regions[Regions.Main].NavigationService;
            _navigationService.Navigated += OnNavigated;
            LoadMainView();
        }

        private void OnUnloaded()
        {
            _navigationService.Navigated -= OnNavigated;
            _regionManager.Regions.Remove(Regions.Main);
            _rightPaneService.CleanUp();
        }

        private bool CanGoBack()
            => _navigationService != null && _navigationService.Journal.CanGoBack;

        private void OnGoBack()
            => _navigationService.Journal.GoBack();

        private bool RequestNavigate(string target)
        {
            if (_navigationService.CanNavigate(target))
            {
                _navigationService.RequestNavigate(target);
                return true;
            }

            return false;
        }

        private void RequestNavigateOnRightPane(string target)
            => _rightPaneService.OpenInRightPane(target);

        private void RequestNavigateAndCleanJournal(string target)
        {
            var navigated = RequestNavigate(target);
            if (navigated)
            {
                _navigationService.Journal.Clear();
            }
        }

        private void OnNavigated(object sender, RegionNavigationEventArgs e)
            => GoBackCommand.RaiseCanExecuteChanged();

        private void OnMenuFileExit()
            => Application.Current.Shutdown();

        private void LoadMainView()
            => RequestNavigateAndCleanJournal(PageKeys.Main);

        private void OnMenuFileSettings()
            => RequestNavigateOnRightPane(PageKeys.Settings);
    }
}
