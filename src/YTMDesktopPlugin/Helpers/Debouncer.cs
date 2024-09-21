namespace Loupedeck.YTMDesktopPlugin.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class Debouncer : IDisposable
{
    readonly TimeSpan _ts;
    readonly Action _action;
    readonly HashSet<ManualResetEvent> _resets = [];
    readonly Object _mutex = new();

    public Debouncer(TimeSpan timespan, Action action)
    {
        this._ts = timespan;
        this._action = action;
    }

    public void Invoke()
    {
        var thisReset = new ManualResetEvent(false);

        lock (this._mutex)
        {
            while (this._resets.Count > 0)
            {
                var otherReset = this._resets.First();
                this._resets.Remove(otherReset);
                otherReset.Set();
            }

            this._resets.Add(thisReset);
        }

        ThreadPool.QueueUserWorkItem(_ =>
        {
            try
            {
                if (!thisReset.WaitOne(this._ts))
                {
                    this._action();
                }
            }
            finally
            {
                lock (this._mutex)
                {
                    using (thisReset)
                    {
                        this._resets.Remove(thisReset);
                    }
                }
            }
        });
    }

    public void Dispose()
    {
        lock (this._mutex)
        {
            while (this._resets.Count > 0)
            {
                var reset = this._resets.First();
                this._resets.Remove(reset);
                reset.Set();
            }
        }
    }
}