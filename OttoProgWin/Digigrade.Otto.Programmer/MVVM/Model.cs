using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Digigrade.Otto.Programmer.MVVM
{
    /// <summary>
    /// Observable class that can be exposed by a view-model.
    /// </summary>
    public abstract class Model : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

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
    }
}