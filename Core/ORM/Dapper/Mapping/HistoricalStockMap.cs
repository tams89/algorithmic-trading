using DapperExtensions.Mapper;
using TradingCore.Model;

namespace TradingCore.ORM.Dapper.Mapping
{
    public sealed class HistoricalStockMap : ClassMapper<HistoricalStock>
    {
        public HistoricalStockMap()
        {
            Schema("InterDay");
            Table("HistoricalStock");

            // TS 12/02/2014 - Keytype Assigned forces Dapper to use the GUID spec'd in code rather than generating its own (annoying!).
            Map(x => x.HistoricalStockId).Column("HistoricalStockId").Key(KeyType.Assigned);

            AutoMap();
        }
    }
}