using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;

namespace Yui
{
    public class Scheduler
    {
        public Scheduler(CancellationTokenSource cts) => _cts = cts;
        private readonly CancellationTokenSource _cts;

        public async Task Run()
        {
#pragma warning disable 4014
            Task.Run(async () =>
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        OnTenSecondsPassed();
                        await Task.Delay(TimeSpan.FromSeconds(10));
                    }
                }
            );

            Task.Run(async () =>
#pragma warning restore 4014
                {
                    while (!_cts.IsCancellationRequested)
                    {
                        OnMinutePassed();
                        await Task.Delay(TimeSpan.FromMinutes(1));
                    }
                }
            );
        }
        public event AsyncEventHandler TenSecondsPassed;
        public event AsyncEventHandler MinutePassed;
        
        private void OnTenSecondsPassed() => TenSecondsPassed?.Invoke();
        private void OnMinutePassed() => MinutePassed?.Invoke();
    }

}