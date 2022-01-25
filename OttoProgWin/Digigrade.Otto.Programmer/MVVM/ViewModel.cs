using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Digigrade.Otto.Programmer.MVVM
{
    /// <summary>
    /// View-model with basic tie-ins for wpf user-controls.
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool _disposed;
        private object? _model;

        /// <summary>
        /// Instantiates a new view-model.
        /// <para>NOTE: The base type already subscribes to the model's PropertyChangedEvent.</para>
        /// </summary>
        /// <param name="model">This is essentially any business-logic object that is being exposed through the view-model.</param>
        public ViewModel(object? model)
        {
            Model = model;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// <para>Gets or sets model that this view-model exposes.  Attaches or detaches to property changed event.</para>
        /// <para>EXAMPLE: <code language="cs">internal new SpecificType Model => base.Model as SpecificType;</code></para>
        /// </summary>
        protected object? Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (_model != null && _model is INotifyPropertyChanged previousModel)
                {
                    previousModel.PropertyChanged -= Model_PropertyChanged;
                }

                _model = value;

                if (_model != null && _model is INotifyPropertyChanged newModel)
                {
                    newModel.PropertyChanged += Model_PropertyChanged;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Notifies any listeners that any state information should be refreshed.
        /// </summary>
        public virtual void Invalidate()
        {
            RaisePropertyChangedEvent(nameof(Invalidate));
        }

        /// <summary>
        /// Override and put any cleanup-code to run before disposing.
        /// <para>NOTE: The dispose base detaches from the model's PropertyChangedEvent.</para>
        /// </summary>
        protected virtual void Cleanup()
        { }

        /// <summary>
        /// Property changed event method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Model_PropertyChanged(object? sender, PropertyChangedEventArgs? e)
        { }

        /// <summary>
        /// Directly raises a property changed event.  If no value is passed, the calling member's name will be used.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChangedEvent([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Releases all resources controlled by this object.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Model = null;
                    Cleanup();
                }

                _disposed = true;
            }
        }
    }
}