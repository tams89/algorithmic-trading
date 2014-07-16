using Core.Model;
using DapperExtensions.Mapper;

namespace Core.Mapping
{
    public sealed class TickMap : ClassMapper<Tick>
    {
        public TickMap()
        {
            Schema("HFT");
            Table("Tick");

            Map(f => f.TickId).Key(KeyType.Guid);

            Map(x => x.Symbol).Column("Symbol");
            Map(x => x.Date).Column("Date");
            Map(x => x.Time).Column("Time");
            Map(x => x.Open).Column("Open");
            Map(x => x.High).Column("High");
            Map(x => x.Low).Column("Low");
            Map(x => x.Close).Column("Close");
            Map(x => x.Volume).Column("Volume");
        }
    }
}
