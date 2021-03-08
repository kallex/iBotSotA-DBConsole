using System;
using System.Threading.Tasks;
using Services;

namespace AWSDataServices
{
    public class NoOpDiagnosticService : IDiagnosticService
    {
        public void Exec(Action<IDiagnosticService> action, string callerName = "")
        {
            action(this);
        }

        public T Exec<T>(Func<IDiagnosticService, T> function, string callerName = "")
        {
            return function(this);
        }

        public async Task ExecAsync(Func<IDiagnosticService, Task> asyncAction, string callerName = "")
        {
            await asyncAction(this);
        }

        public async Task<T> ExecAsync<T>(Func<IDiagnosticService, Task<T>> asyncFunction, string callerName = "")
        {
            return await asyncFunction(this);
        }
    }
}