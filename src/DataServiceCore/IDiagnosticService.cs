using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Services
{
    public interface IDiagnosticService
    {
        void Exec(Action<IDiagnosticService> action, [CallerMemberName] string callerName = "");
        T Exec<T>(Func<IDiagnosticService, T> function, [CallerMemberName] string callerName = "");
        Task ExecAsync(Func<IDiagnosticService, Task> asyncAction, [CallerMemberName] string callerName = "");
        Task<T> ExecAsync<T>(Func<IDiagnosticService, Task<T>> asyncFunction, [CallerMemberName] string callerName = "");
    }
}