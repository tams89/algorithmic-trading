using System.Configuration;

namespace Core.Utilities
{
    /// <summary>
    /// System configuration items.
    /// </summary>
    public static class Configuration
    {
        public static readonly string AlgoTraderDbConStr = ConfigurationManager.ConnectionStrings["AlgoTraderDB"].ConnectionString;
    }
}
