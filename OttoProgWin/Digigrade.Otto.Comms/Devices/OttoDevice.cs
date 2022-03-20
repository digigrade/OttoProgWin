using Digigrade.Otto.Comms.Commands;
using Digigrade.Otto.Comms.SerialUtilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Digigrade.Otto.Comms
{
    public class OttoDevice : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;
        private OttoSerialPort? _ottoSerialPort;
        private ConnectionState _connectionState;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initialize a new instance of OttoDevice.
        /// </summary>
        public OttoDevice()
        {

        }

        /// <summary>
        /// Initialize a new instance of OttDevice and imediately attempt to connect with passed in serial port.
        /// </summary>
        /// <param name="ottoSerialPort"></param>
        public OttoDevice(OttoSerialPort ottoSerialPort)
        {
            Connect(ottoSerialPort);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Notifies any listeners that state information should be refreshed.
        /// </summary>
        public virtual void Invalidate()
        {
            RaisePropertyChangedEvent();
        }

        /// <summary>
        /// Stops any communication and cleans up any managed objects. 
        /// </summary>
        /// <param name="forceUnsafe"></param>
        public void Shutdown(bool forceUnsafe = false)
        {
            if (_ottoSerialPort != null)
            {
                _ottoSerialPort.PropertyChanged -= OttoSerialPort_PropertyChanged;
                _ottoSerialPort.Shutdown(forceUnsafe);
                _ottoSerialPort = null;
            }
        }

        /// <summary>
        /// Attempts to esablish communication through serial port.
        /// </summary>
        /// <param name="ottoSerialPort"></param>
        public void Connect(OttoSerialPort ottoSerialPort)
        {
            if (_ottoSerialPort != null)
            {
                _ottoSerialPort.PropertyChanged -= OttoSerialPort_PropertyChanged;
                _ottoSerialPort.Shutdown();
                _ottoSerialPort = null;
            }

            _ottoSerialPort = ottoSerialPort;

            if (_ottoSerialPort != null)
            {
                _ottoSerialPort.PropertyChanged += OttoSerialPort_PropertyChanged;
                _ottoSerialPort.ProcessCommandDelegate = new DeviceCommand.ProcessCommandDelegate(ProcessPayload);
                _ottoSerialPort.Connect();
            }
        }

        protected bool ProcessPayload(string payload)
        {
            var command = DeviceCommand.Unpack(payload);

            if (command.CommandHeader == CommandHeaders.BeginProgramMode)
            {
                return ProcessBeginProgramModeCommand(this, command);
            }
            else
            {
                return false;
            }
        }

        private static bool ProcessBeginProgramModeCommand(OttoDevice ottoDevice, DeviceCommand command)
        {
            if (!string.IsNullOrWhiteSpace(command?.Message))
            {
                try
                {
                    //var file = JsonSerializer.Deserialize<HubFileSyncObject>(command.Message);
                    //if (file != null)
                    //{
                    //    syncSession._isSyncronizing = true;
                    //    syncSession.SyncronizedHubFiles.Add(file);
                    //    syncSession._isSyncronizing = false;
                    //    syncSession.ZeroMqClientSession.ReplyToRequest(ZeroMqCommands.Acknowledge);
                    //    return true;
                    //}
                }
                catch { }
            }

            //syncSession.ZeroMqClientSession.ReplyToRequest(ZeroMqCommands.ErrorParameters);
            return true;
        }

        private void OttoSerialPort_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OttoSerialPort.AttemptingToConnect) && (_ottoSerialPort?.AttemptingToConnect ?? false))
            {
                ConnectionState = ConnectionState.Connecting;
            }
        }

        public ConnectionState ConnectionState
        {
            get { return _connectionState; }
            set
            {
                if (_connectionState != value)
                {
                    _connectionState = value;
                    RaisePropertyChangedEvent();
                    RaisePropertyChangedEvent(nameof(IsConnected));
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                if (ConnectionState != ConnectionState.Offline
                    && ConnectionState != ConnectionState.Failed)
                {
                    return true;
                }
                return false;
            }
        }

        public string? SerialPortFullName { get => SerialPortNumber.ToString() + " - " + SerialPortName; }

        public string? SerialPortName { get; }

        public int SerialPortNumber { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        /// <summary>
        /// Directly raises a property changed event.  If no value is passed, the calling member's name will be used.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}