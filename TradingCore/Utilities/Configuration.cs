using System.Configuration;

namespace TradingCore.Utilities
{
    /// <summary>
    /// System configuration items.
    /// </summary>
    public static class Configuration
    {
        public static readonly string AlgoTraderDBConStr = ConfigurationManager.ConnectionStrings["AlgoTraderDB"].ConnectionString;
    }
}
