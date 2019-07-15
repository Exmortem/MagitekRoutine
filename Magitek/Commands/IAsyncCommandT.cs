using System.Threading.Tasks;
using System.Windows.Input;

namespace Magitek.Commands
{
    public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
    {
        ICommand Command { get; }
        Task ExecuteAsync(T obj);
        bool CanExecute(object obj);
    }
}