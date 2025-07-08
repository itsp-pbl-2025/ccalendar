#nullable enable
using System;
using System.Threading.Tasks;

namespace AppCore.Utilities
{
    public static class TaskExtensions
    {
        public static void Forget(this Task task, Action<Exception>? onException = null)
        {
            _ = ForgetInternal(task, onException);
        }

        private static async Task ForgetInternal(Task task, Action<Exception>? onException)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onException?.Invoke(ex);
            }
        }
    }
}