using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Amazon.XRay.Recorder.Core;
using Services;

namespace AWSDataServices
{
    public class AWSXRayService : IDiagnosticService
    {
        void IDiagnosticService.Exec(Action<IDiagnosticService> action, [CallerMemberName] string callerName = "")
        {
            AWSXRayRecorder.Instance?.BeginSubsegment(callerName);
            try
            {
                action(this);
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

        T IDiagnosticService.Exec<T>(Func<IDiagnosticService, T> function, [CallerMemberName] string callerName = "")
        {
            AWSXRayRecorder.Instance?.BeginSubsegment(callerName);
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

        async Task IDiagnosticService.ExecAsync(Func<IDiagnosticService, Task> asyncAction, [CallerMemberName] string callerName = "")
        {
            AWSXRayRecorder.Instance?.BeginSubsegment(callerName);
            try
            {
                await asyncAction(this);
            }
            catch (Exception ex)
            {
                AWSXRayRecorder.Instance?.AddException(ex);

            }
            finally
            {
                AWSXRayRecorder.Instance?.EndSubsegment();
            }
        }

        async Task<T> IDiagnosticService.ExecAsync<T>(Func<IDiagnosticService, Task<T>> asyncFunction, [CallerMemberName] string callerName = "")
        {
            AWSXRayRecorder.Instance?.BeginSubsegment(callerName);
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