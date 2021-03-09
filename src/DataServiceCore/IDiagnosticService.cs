using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Services
{
    public interface IDiagnosticService
    {
        void Exec(string callingTypeName, Action<IDiagnosticService> action, [CallerMemberName] string callerName = "");
        T Exec<T>(string callingTypeName, Func<IDiagnosticService, T> function,
            [CallerMemberName] string callerName = "");
        Task ExecAsync(string callingTypeName, Func<IDiagnosticService, Task> asyncAction,
            [CallerMemberName] string callerName = "");
        Task<T> ExecAsync<T>(string callingTypeName, Func<IDiagnosticService, Task<T>> asyncFunction,
            [CallerMemberName] string callerName = "");
    }
}