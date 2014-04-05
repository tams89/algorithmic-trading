using System.Configuration;

namespace Core.Utilities
{
    /// <summary>
    /// System configuration items.
    /// </summary>
    public static class Configuration
    {
        public static readonly string AlgoTraderDBConStr = ConfigurationManager.ConnectionStrings["AlgoTraderDB"].ConnectionString;
    }
}
