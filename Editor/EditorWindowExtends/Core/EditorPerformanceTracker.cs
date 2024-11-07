using System;
using System.Collections.Generic;
using UnityEngine;
using Yueby.Core.Utils;
using Logger = Yueby.Core.Utils.Logger;

public class EditorPerformanceTracker
{
    private readonly Dictionary<string, System.Diagnostics.Stopwatch> _watches = new();
    private readonly Dictionary<string, long> _averageTimes = new();
    private const int SAMPLE_COUNT = 60;

    public IDisposable Track(string operation)
    {
        if(!_watches.TryGetValue(operation, out var watch))
        {
            watch = new System.Diagnostics.Stopwatch();
            _watches[operation] = watch;
        }
        
        watch.Start();
        return new TrackingScope(() => {
            watch.Stop();
            
            if(!_averageTimes.ContainsKey(operation))
                _averageTimes[operation] = watch.ElapsedMilliseconds;
            else
            {
                var avg = _averageTimes[operation];
                avg = (avg * (SAMPLE_COUNT-1) + watch.ElapsedMilliseconds) / SAMPLE_COUNT;
                _averageTimes[operation] = avg;
                
                if(avg > 16) // 1帧的时间
                    Logger.LogWarning($"{operation} average time: {avg}ms");
            }
            
            watch.Reset();
        });
    }

    private class TrackingScope : IDisposable
    {
        private readonly Action _onDispose;
        public TrackingScope(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => _onDispose?.Invoke();
    }
} 