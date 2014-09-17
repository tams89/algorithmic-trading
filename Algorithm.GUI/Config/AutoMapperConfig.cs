using Algorithm.Core;
using Algorithm.GUI.Models;
using AutoMapper;

namespace Algorithm.GUI.Config
{
    public class AutoMapperConfig
    {
        /// <summary>
        /// Call this at app start to configure automapper.
        /// </summary>
        public static void Configure()
        {
            Mapper.CreateMap<Records.Tick, Tick>();
            Mapper.AssertConfigurationIsValid();
        }
    }
}
