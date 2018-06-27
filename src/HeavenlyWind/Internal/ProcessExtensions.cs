using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    static class ProcessExtensions
    {
        public static TaskAwaiter<int> GetAwaiter(this Process process)
        {
            var tcs = new TaskCompletionSource<int>();

            if (process.HasExited)
                tcs.TrySetResult(process.ExitCode);
            else
            {
                process.EnableRaisingEvents = true;
                process.Exited += OnProcessExited;
            }

            return tcs.Task.GetAwaiter();

            void OnProcessExited(object sender, EventArgs e)
            {
                process.Exited -= OnProcessExited;

                tcs.TrySetResult(process.ExitCode);
            }
        }
    }
}
