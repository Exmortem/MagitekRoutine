using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Magitek.Utilities.Collections
{
    public class ObservableCollectionViewModel<T> : ObservableCollection<T>, IObserver<NotifyCollectionChangedEventArgs>, IDisposable
    {

        private IDisposable _unsubscribeToken;
        private readonly IDisposable _subscriptionActionToken;

        public ObservableCollectionViewModel(IObservable<NotifyCollectionChangedEventArgs> observable)
        {

            _subscriptionActionToken = DispatcherQueueProcessor.Instance.QueueSubscribe(() =>
            {
                _unsubscribeToken = observable.Subscribe(this);
            });
        }

        ~ObservableCollectionViewModel()
        {
            Dispose(false);
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(NotifyCollectionChangedEventArgs value)
        {
            DispatcherQueueProcessor.Instance.Add(() =>
            {
                ProcessCommand(value);
            });
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void ProcessCommand(NotifyCollectionChangedEventArgs command)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (command.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var startIndex = command.NewStartingIndex;
                        if (startIndex > -1)
                        {
                            foreach (var item in command.NewItems)
                            {
                                InsertItem(startIndex, (T)item);
                                ++startIndex;
                            }
                        }
                        else
                        {
                            foreach (var item in command.NewItems)
                            {
                                Add((T)item);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var startIndex = command.OldStartingIndex;
                        foreach (var item in command.OldItems)
                        {
                            RemoveAt(startIndex);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        var startIndex = command.OldStartingIndex;
                        foreach (var item in command.NewItems)
                        {
                            this[startIndex] = (T)item;
                            ++startIndex;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
            }
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                GC.SuppressFinalize(this);
            }
            _subscriptionActionToken?.Dispose();
            _unsubscribeToken?.Dispose();
        }

    }
}
