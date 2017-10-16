using System;

namespace Refit.Insane.PowerPack.Configuration
{
    public static class BaseApiConfiguration
    {
        public static Uri ApiUri { get; set; }
        public static TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(90); 
    }
}