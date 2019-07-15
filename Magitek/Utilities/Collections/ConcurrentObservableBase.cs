using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Magitek.Utilities.Collections
{
    public abstract class ConcurrentObservableBase<T> : IObservable<NotifyCollectionChangedEventArgs>, INotifyCollectionChanged, IEnumerable<T>, IDisposable
    {
        private readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();
        private readonly ObservableCollection<T> _baseCollection;
        private bool _newSnapshotRequired = false;
        private readonly ReaderWriterLockSlim _snapshotLock = new ReaderWriterLockSlim();
        private ImmutableCollectionBase<T> _baseSnapshot;
        private readonly Dictionary<int, IObserver<NotifyCollectionChangedEventArgs>> _subscribers;
        private int _subscriberKey;
        private bool _isDisposed;
        private ObservableCollectionViewModel<T> _viewModel;

        protected ConcurrentObservableBase() : this(new T[] { }) { }

        protected ConcurrentObservableBase(IEnumerable<T> enumerable)
        {
            _subscribers = new Dictionary<int, IObserver<NotifyCollectionChangedEventArgs>>();

            _baseCollection = new ObservableCollection<T>(enumerable);
            _baseSnapshot = new ImmutableCollection<T>(enumerable);

            _viewModel = new ObservableCollectionViewModel<T>(this);

            _baseCollection.CollectionChanged += HandleBaseCollectionChanged;

            _viewModel.CollectionChanged += (sender, e) => 
            {
                CollectionChanged?.Invoke(sender, e);
            };
        }

        ~ConcurrentObservableBase()
        {
            Dispose(false);
        }

        protected void DoBaseClear(Action action)
        {
            _readWriteLock.TryEnterUpgradeableReadLock(Timeout.Infinite);

            try
            {
                _readWriteLock.TryEnterWriteLock(Timeout.Infinite);
                action();
                while (WriteCollection.Count > 0)
                {
                    _newSnapshotRequired = true;
                    WriteCollection.RemoveAt(WriteCollection.Count - 1);
                }
            }
            finally
            {
                if (_readWriteLock.IsWriteLockHeld)
                {
                    _readWriteLock.ExitWriteLock();
                }
                _readWriteLock.ExitUpgradeableReadLock();
            }
        }

        protected void HandleBaseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            var actionTypeIsOk = e.Action != NotifyCollectionChangedAction.Reset;
            System.Diagnostics.Debug.Assert(actionTypeIsOk, "Reset called on concurrent observable collection. This shouldn't happen");

            OnNext(e);
        }

        private void UpdateSnapshot()
        {
            if (_newSnapshotRequired)
            {
                _snapshotLock.TryEnterWriteLock(Timeout.Infinite);
                if (_newSnapshotRequired)
                {
                    _baseSnapshot = new ImmutableCollection<T>(_baseCollection);
                    _newSnapshotRequired = false;
                }
                _snapshotLock.ExitWriteLock();
            }
        }

        protected void DoBaseRead(Action readFunc)
        {
            DoBaseRead<object>(() => {
                readFunc();
                return null;
            });
        }

        protected TResult DoBaseRead<TResult>(Func<TResult> readFunc)
        {
            if (IsDispatcherThread)
            {
                return readFunc();
            }

            _readWriteLock.TryEnterReadLock(Timeout.Infinite);
            try
            {
                return readFunc();
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        protected TResult DoBaseReadWrite<TResult>(Func<bool> readFuncTest, Func<TResult> readFunc, Func<TResult> writeFunc)
        {
            _readWriteLock.TryEnterUpgradeableReadLock(Timeout.Infinite);
            try
            {
                if (readFuncTest())
                {
                    return readFunc();
                }
                else
                {
                    _readWriteLock.TryEnterWriteLock(Timeout.Infinite);
                    try
                    {
                        _newSnapshotRequired = true;
                        var returnValue = writeFunc();
                        return returnValue;
                    }
                    finally
                    {
                        if (_readWriteLock.IsWriteLockHeld)
                        {
                            _readWriteLock.ExitWriteLock();
                        }
                    }
                }
            }
            finally
            {
                _readWriteLock.ExitUpgradeableReadLock();
            }
        }

        protected TResult DoBaseReadWrite<TResult>(Func<bool> readFuncTest, Func<TResult> readFunc, Action preWriteFunc, Func<TResult> writeFunc)
        {
            _readWriteLock.TryEnterReadLock(Timeout.Infinite);
            try
            {
                if (readFuncTest())
                {
                    return readFunc();
                }
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
            preWriteFunc();
            return DoBaseReadWrite(readFuncTest, readFunc, writeFunc);
        }

        protected void DoBaseWrite(Action writeFunc)
        {
            DoBaseWrite<object>(() => {
                writeFunc();
                return null;
            });
        }

        protected TResult DoBaseWrite<TResult>(Func<TResult> writeFunc)
        {
            _readWriteLock.TryEnterUpgradeableReadLock(Timeout.Infinite);
            try
            {
                _readWriteLock.TryEnterWriteLock(Timeout.Infinite);
                _newSnapshotRequired = true;
                return writeFunc();
            }
            finally
            {
                if (_readWriteLock.IsWriteLockHeld)
                {
                    _readWriteLock.ExitWriteLock();
                }
                _readWriteLock.ExitUpgradeableReadLock();
            }
        }

        public ImmutableCollectionBase<T> Snapshot
        {
            get
            {
                return DoBaseRead(() => {
                    UpdateSnapshot();
                    return _baseSnapshot;
                });
            }
        }

        protected ObservableCollection<T> WriteCollection => _baseCollection;

        protected ObservableCollection<T> ReadCollection => IsDispatcherThread ? ViewModel : WriteCollection;

        protected ObservableCollectionViewModel<T> ViewModel => _viewModel;

        protected static bool IsDispatcherThread => DispatcherQueueProcessor.Instance.IsDispatcherThread;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            OnCompleted();
            _isDisposed = true;
        }

        protected void OnNext(NotifyCollectionChangedEventArgs value)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Observable<T>");
            }

            foreach (var observer in _subscribers.Select(kv => kv.Value))
            {
                observer.OnNext(value);
            }
        }

        protected void OnError(Exception exception)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Observable<T>");
            }

            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            foreach (IObserver<NotifyCollectionChangedEventArgs> observer in _subscribers.Select(kv => kv.Value))
            {
                observer.OnError(exception);
            }
        }

        protected void OnCompleted()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Observable<T>");
            }

            foreach (var observer in _subscribers.Select(kv => kv.Value))
            {
                observer.OnCompleted();
            }
        }

        public IDisposable Subscribe(IObserver<NotifyCollectionChangedEventArgs> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }
            return DoBaseWrite(() => {
                int key = _subscriberKey++;
                _subscribers.Add(key, observer);
                UpdateSnapshot();
                foreach (var item in _baseSnapshot)
                {
                    observer.OnNext(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                }

                return new DoDispose(() => {
                    DoBaseWrite(() => {
                        _subscribers.Remove(key);
                    });
                });
            });
        }

        private class DoDispose : IDisposable
        {
            private readonly Action _doDispose;
            public DoDispose(Action doDispose)
            {
                _doDispose = doDispose;
            }

            public void Dispose()
            {
                _doDispose();
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<T> GetEnumerator()
        {
            return IsDispatcherThread ? _viewModel.GetEnumerator() : Snapshot.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
