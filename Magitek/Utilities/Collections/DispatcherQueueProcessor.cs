using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Magitek.Utilities.Collections
{
    internal class DispatcherQueueProcessor
    {
        private static readonly Lazy<DispatcherQueueProcessor> _instance = new Lazy<DispatcherQueueProcessor>(() => new DispatcherQueueProcessor(), true);
        private readonly BlockingCollection<Action> _actionQueue;
        private ConcurrentDictionary<WeakReference, object> _subscriberQueue;
        private Dispatcher _dispatcher;
        private Action _actionWaiting;
        private readonly Semaphore _actionWaitingSemaphore = new Semaphore(0, 1);

        private readonly object _startQueueLock = new object();

        private DispatcherQueueProcessor()
        {
            _actionQueue = new BlockingCollection<Action>();
            _subscriberQueue = new ConcurrentDictionary<WeakReference, object>();

            if (CheckIfDispatcherCreated())
                return;

            var timer = new System.Timers.Timer(100);
            timer.Elapsed += (sender, e) => {
                if (CheckIfDispatcherCreated())
                {
                    timer.Enabled = false;
                }
            };
            timer.Enabled = true;
        }

        public void Add(Action action)
        {

            _actionQueue.Add(action);

            if (!IsDispatcherThread)
                return;

            _actionWaitingSemaphore.WaitOne();

            if (_actionWaiting != null)
            {
                _actionWaiting();
                _actionWaiting = null;
            }

            Action nextCommand;
            while (_actionQueue.TryTake(out nextCommand))
            {
                nextCommand();
            }
            _actionWaitingSemaphore.Release(1);
        }

        private class DoDispose : IDisposable
        {
            private Action _subscribeAction = null;
            private Action _doDispose = null;

            public DoDispose()
            {
            }

            public DoDispose(Action subscribeAction, Action doDispose)
            {
                _subscribeAction = subscribeAction;
                _doDispose = doDispose;
            }

            public void Dispose()
            {
                _doDispose?.Invoke();
                _doDispose = null;
            }
        }


        public IDisposable QueueSubscribe(Action subscribeAction)
        {

            if (_subscriberQueue != null)
            {
                try
                {
                    WeakReference weakRef = new WeakReference(subscribeAction);
                    _subscriberQueue[weakRef] = null;

                    return new DoDispose(subscribeAction, () => {
                        var subscriberQueue = _subscriberQueue;

                        if (subscriberQueue == null)
                            return;

                        subscriberQueue.TryRemove(weakRef, out object dummy);
                    });

                }
                catch
                {
                    if (_subscriberQueue == null)
                    {
                        subscribeAction();
                    }
                }
            }
            else
            {
                subscribeAction();
            }

            return new DoDispose();
        }

        private bool CheckIfDispatcherCreated()
        {
            if (_dispatcher != null)
            {
                return true;
            }
            else
            {
                lock (_startQueueLock)
                {
                    if (_dispatcher != null)
                        return _dispatcher != null;

                    if (Application.Current == null)
                        return _dispatcher != null;

                    _dispatcher = Application.Current.Dispatcher;
                    if (_dispatcher != null)
                    {
                        StartQueueProcessing();
                    }
                    return _dispatcher != null;
                }
            }
        }

        private void StartQueueProcessing()
        {
            var keys = _subscriberQueue.Keys;
            _subscriberQueue = null;

            foreach (var subscribeRef in keys)
            {
                var subscribe = subscribeRef.Target as Action;
                subscribe?.Invoke();
            }

            var actionThread = new Thread(() =>
            {
                try
                {
                    foreach (var action in _actionQueue.GetConsumingEnumerable())
                    {
                        _actionWaiting = action;
                        _actionWaitingSemaphore.Release(1);


                        _dispatcher.Invoke((Action) (() =>
                        {
                            if (_actionWaiting != null)
                            {
                                _actionWaiting();
                                _actionWaiting = null;
                            }

                            var countDown = 100;
                            Action nextCommand;

                            while (countDown > 0 && _actionQueue.TryTake(out nextCommand))
                            {
                                --countDown;
                                nextCommand();
                            }
                        }));
                        _actionWaitingSemaphore.WaitOne();
                    }
                }
                catch (Exception)
                {
                }
            }) {IsBackground = true};
            actionThread.Start();
        }

        public static DispatcherQueueProcessor Instance => _instance.Value;

        public bool IsDispatcherThread
        {
            get
            {
                if (!CheckIfDispatcherCreated())
                {
                    return false;
                }
                else
                {
                    return _dispatcher.Thread == Thread.CurrentThread;
                }
            }
        }

    }
}
