using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RadarFamilyCore.Service.Interface
{
    public interface IPositionService
    {
        Task<Dictionary<Double, Double>> GetLastPositionAsync();
    }
}
