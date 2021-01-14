using System;

namespace DailyRatingsCalculator.Contracts.Services
{
    public interface IApplicationInfoService
    {
        Version GetVersion();
    }
}
