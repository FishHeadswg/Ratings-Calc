using System;

using DailyRatingsCalculator.Models;

namespace DailyRatingsCalculator.Contracts.Services
{
    public interface IThemeSelectorService
    {
        void InitializeTheme();

        void SetTheme(AppTheme theme);

        AppTheme GetCurrentTheme();
    }
}
