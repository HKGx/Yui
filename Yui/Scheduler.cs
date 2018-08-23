using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.CodeAnalysis.CSharp;

namespace Yui
{
    public class Scheduler
    {
        public Scheduler()
        {
            _tenSecondsTimer = new Timer(async x => await TenSecondsCallback(x), null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private Timer _tenSecondsTimer;
        
        public event AsyncEventHandler TenSecondsPassed;

        private Task TenSecondsCallback(object o)
        {
            return Task.FromResult(TenSecondsPassed?.Invoke());
        }
    }
}