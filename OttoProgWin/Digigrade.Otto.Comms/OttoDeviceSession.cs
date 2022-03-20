using Digigrade.Otto.Comms.Devices;
using Digigrade.Otto.Comms.SerialUtilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Digigrade.Otto.Comms
{
    public class OttoDeviceSession : INotifyPropertyChanged, IDisposable
    {
        private System.Timers.Timer? _comPortUpdateTimer;
        private OttoDevice? _ottoDevice;
        private bool _disposed;
        private List<OttoDevice> _offlineDevices = new List<OttoDevice>();

        public OttoDeviceSession()
        {
            _comPortUpdateTimer = new System.Timers.Timer();
            _comPortUpdateTimer.Elapsed += ComPortUpdateTimer_Elapsed;
            _comPortUpdateTimer.Interval = 500; // in milliseconds
            _comPortUpdateTimer.Start();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The connected device that this session is currently communicating with.
        /// </summary>
        public OttoDevice? OttoDevice
        {
            get => _ottoDevice;
            set
            {
                if (_ottoDevice != null)
                {
                    _ottoDevice.PropertyChanged -= ConnectedDevice_PropertyChanged;
                    _ottoDevice = null;
                }

                _ottoDevice = value;

                if (_ottoDevice != null)
                {
                    _ottoDevice.PropertyChanged += ConnectedDevice_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// List of known offlined devices for auto-fill and hints.
        /// </summary>
        public List<OttoDevice> OfflineDevices { get => _offlineDevices; }

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
            RaisePropertyChangedEvent(nameof(Invalidate));
        }

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

        private void ComPortUpdateTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (OttoDevice is null)
            {
                foreach (var item in OttoSerialPort.AllCompatiblePorts())
                {
                    try 
                    {
                        var ottoDevice = new OttoDevice(item);

                        if (ottoDevice?.ConnectionState != ConnectionState.Failed)
                        {
                            OttoDevice = ottoDevice;
                            return;
                        }
                    }
                    catch { }
                }
            }
            else if (OttoDevice.ConnectionState == ConnectionState.Offline)
            {
                // TODO: Begin device connection sequence
            }
            else if (OttoDevice.ConnectionState == ConnectionState.Connecting)
            {
                // TODO: Wait for connection to succeed or fail.
            }
            else if (OttoDevice.ConnectionState == ConnectionState.ProgramMode)
            {
                // TODO: Update state
            }
            else if (OttoDevice.ConnectionState == ConnectionState.NormalMode)
            {
                // TODO: Update state
            }
            else if (OttoDevice.ConnectionState == ConnectionState.Failed)
            {
                // TODO: Cleanup failed connection
            }
        }

        private void ConnectedDevice_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        }
    }
}