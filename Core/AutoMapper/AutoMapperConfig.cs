using AutoMapper;
using Core.DTO;
using Core.Model;

namespace Core.AutoMapper
{
    public class AutoMapperConfig
    {
        /// <summary>
        /// Call this at app start to configure automapper.
        /// </summary>
        public static void Configure()
        {
            Mapper.CreateMap<Tick, TickDTO>();
            Mapper.CreateMap<HistoricalStock, HistoricalStockDTO>();

            Mapper.AssertConfigurationIsValid();
        }
    }
}
