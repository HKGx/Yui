using System.Threading;
using System.Threading.Tasks;

namespace Yui
{
    public class Scheduler
    {
        //TODO: make scheduler

        public Scheduler()
        {
        }

        public Timer SecondTimer = new Timer(async o => await EverySecondCallback(o), null, 0, 1000);

        private static async Task EverySecondCallback(object state)
        {
            
        }
    }
}