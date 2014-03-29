using AutoMapper;
using TradingCore.DTO;
using TradingCore.Model;

namespace TradingCore.AutoMapper
{
    public class AutoMapperConfig
    {
        /// <summary>
        /// Call this at app start to configure automapper.
        /// </summary>
        public static void Configure()
        {
            Mapper.CreateMap<Tick, TickDTO>();

            Mapper.AssertConfigurationIsValid();
        }
    }
}
