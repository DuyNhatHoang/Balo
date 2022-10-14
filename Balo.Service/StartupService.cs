using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balo.Service
{
    public interface IStartupService
    {
        public void Start();

    }
    public class StartupService : IStartupService
    {
        public void Start()
        {
            Console.WriteLine("StartupService Start ...");
        }
    }
}
