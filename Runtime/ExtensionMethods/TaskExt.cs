using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UniKit.Async
{
    public static class TaskExt
    {
        public const string AsyncConsoleFilter = "#Async #";
        public static async Task WaitUntil(Func<bool> predicate, int checkDelayInMilliSeconds, CancellationToken token)
        {
            if (predicate == null || checkDelayInMilliSeconds == 0 || token == null)
                throw new NullReferenceException();

            while (!predicate.Invoke())
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(checkDelayInMilliSeconds);
            }
        }

        public static void ThrowIfNotValid(this CancellationToken token)
        {
            if (token == null)
                throw new OperationCanceledException();

            token.ThrowIfCancellationRequested();
        }
        public static void DebugAsyncLog(this OperationCanceledException e)
            => DebugAsyncLog(e as Exception);

        public static void DebugAsyncLog(Exception e)
        {
            DebugAsyncLog(e.Message);
        }
        public static void DebugAsyncLog(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            Debug.Log(AsyncConsoleFilter + message);
        }
    }
}
