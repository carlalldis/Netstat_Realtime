using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Netstat_Realtime
{
    internal enum EventType
    {
        NewSocket,
        DisconnectedSocket,
        ChangedSocket,
        NewListener,
        StoppedListener
    }

    internal class NetstatLogger
    {
        private readonly IPGlobalProperties _ipProp = IPGlobalProperties.GetIPGlobalProperties();
        private readonly int _interval;
        private readonly string _logFile;
        private readonly bool _logToConsole;
        private Dictionary<(string local, string remote), TcpState> _prevConnections;
        private HashSet<string> _prevListeners;

        private Task _runningTask;
        private CancellationTokenSource _cancellationTokenSource;
        private StreamWriter _logWriter;

        public NetstatLogger(int interval, string logFile, bool logToConsole)
        {
            _interval = interval;
            _logFile = logFile;
            _logToConsole = logToConsole;
        }

        private bool IsRunning()
        {
            if (_runningTask == null)
            {
                return false;
            }
            else if (_runningTask.Status == TaskStatus.WaitingForActivation || _runningTask.Status == TaskStatus.Running) {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Start()
        {
            if (IsRunning())
            {
                throw new InvalidOperationException("Already running");
            }
            _prevConnections = GetConnections();
            _prevListeners = GetListeners();
            _cancellationTokenSource = new CancellationTokenSource();
            _runningTask = Task.Run(() => InternalLoop());
        }

        private async Task InternalLoop()
        {
            try
            {
                using (_logWriter = new StreamWriter(_logFile, true, Encoding.UTF8))
                {
                    _logWriter.WriteLine($"Time,EventType,LocalEndpoint,RemoteEndpoint,OldState,NewState");
                    while (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(_interval);

                        // Get connection states
                        var currentConnections = GetConnections();
                        var currentListeners = GetListeners();

                        // Check for new and removed connections
                        var newConnections = currentConnections.Where(c => !_prevConnections.ContainsKey(c.Key)).ToList();
                        var oldConnections = _prevConnections.Where(c => !currentConnections.ContainsKey(c.Key)).ToList();

                        // Check for changed connections states
                        var changedConnections = currentConnections
                            .Where(c => _prevConnections.ContainsKey(c.Key))
                            .Where(c => c.Value != _prevConnections[c.Key])
                            .ToList();

                        // Check for new and removed listeners
                        var newListeners = currentListeners.Where(l => !_prevListeners.Contains(l)).ToList();
                        var oldListeners = _prevListeners.Where(l => !currentListeners.Contains(l)).ToList();

                        newConnections.ForEach(async c => await LogConnectionAsync(EventType.NewSocket, c.Key.local, c.Key.remote, null, c.Value));
                        oldConnections.ForEach(async c => await LogConnectionAsync(EventType.DisconnectedSocket, c.Key.local, c.Key.remote, c.Value, null));
                        changedConnections.ForEach(async c => await LogConnectionAsync(EventType.ChangedSocket, c.Key.local, c.Key.remote, _prevConnections[c.Key], c.Value));
                        newListeners.ForEach(async c => await LogConnectionAsync(EventType.NewListener, c, null, null, null));
                        oldListeners.ForEach(async c => await LogConnectionAsync(EventType.StoppedListener, c, null, null, null));

                        // Replace fields to compare next iteration
                        _prevConnections = currentConnections;
                        _prevListeners = currentListeners;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR: {ex.Message}");
                Environment.Exit(1);
            }
        }

        public void Stop()
        {
            if (!IsRunning())
            {
                throw new InvalidOperationException("Not running");
            }
            _cancellationTokenSource.Cancel();
            _runningTask.Wait();
        }

        private Dictionary<(string local, string remote), TcpState> GetConnections()
        {
            var conns = _ipProp.GetActiveTcpConnections();
            var dict = conns.ToDictionary(k => (k.LocalEndPoint.ToString(), k.RemoteEndPoint.ToString()), v => v.State);
            return dict;
        }

        private HashSet<string> GetListeners()
        {
            var listeners = _ipProp.GetActiveTcpListeners();
            return listeners.Select(l => l.ToString()).ToHashSet();
        }

        private async Task LogConnectionAsync(EventType eventType, string localEndpoint, string remoteEndpoint, TcpState? oldState, TcpState? newState)
        {
            var currentTime = DateTime.Now.ToString("s");
            if (_logToConsole)
            {
                Console.WriteLine($"{currentTime}: {eventType} - [Local: {localEndpoint}] [Remote: {remoteEndpoint}] [Old State: {oldState}] [New State: {newState}]");
            }
            await _logWriter.WriteAsync($"{currentTime},.{eventType},\"{localEndpoint}\",\"{remoteEndpoint}\",{oldState},{newState}");
        }
    }
}
