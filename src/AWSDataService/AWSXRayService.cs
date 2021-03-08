using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core;
using Services;

namespace AWSDataServices
{
    public class AWSXRayService : IDiagnosticService
    {
        public void Exec(string callingTypeName, Action<IDiagnosticService> action,
            [CallerMemberName] string callerName = "")
        {
            Exec<object>(callingTypeName, diagnosticService =>
            {
                action(diagnosticService);
                return null;
            }, callerName);
        }

        public T Exec<T>(string callingTypeName, Func<IDiagnosticService, T> function,
            [CallerMemberName] string callerName = "")
        {
            AWSXRayRecorder.Instance?.BeginSubsegment($"{callingTypeName}.{callerName}");
            try
            {
                return function(this);
            }
            catch (Exception ex)
            {
                AWSXRayRecorder.Instance?.AddException(ex);
                throw;
            }
            finally
            {
                AWSXRayRecorder.Instance?.EndSubsegment();
            }
        }

        public async Task ExecAsync(string callingTypeName, Func<IDiagnosticService, Task> asyncAction,
            [CallerMemberName] string callerName = "")
        {
            await ExecAsync<object>(callingTypeName, async diagnosticService =>
            {
                await asyncAction(diagnosticService);
                return null;
            }, callerName);
        }

        public async Task<T> ExecAsync<T>(string callingTypeName, Func<IDiagnosticService, Task<T>> asyncFunction,
            [CallerMemberName] string callerName = "")
        {
            AWSXRayRecorder.Instance?.BeginSubsegment($"{callingTypeName}.{callerName}");
            try
            {
                return await asyncFunction(this);
            }
            catch (Exception ex)
            {
                AWSXRayRecorder.Instance?.AddException(ex);
                throw;
            }
            finally
            {
                AWSXRayRecorder.Instance?.EndSubsegment();
            }
        }

    }
}