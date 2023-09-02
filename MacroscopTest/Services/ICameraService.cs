using MacroscopTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroscopTest.Services
{
    public interface ICameraService
    {
        Task<List<CameraModel>> GetCamerasAsync();
    }
}
