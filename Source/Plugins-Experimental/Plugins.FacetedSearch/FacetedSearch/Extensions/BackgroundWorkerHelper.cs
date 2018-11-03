using System.ComponentModel;

namespace FacetedSearch3.Extensions
{
    static class BackgroundWorkerHelper
    {
        internal const string OPERATION_CANCELLED = "Operation Cancelled";

        /// <summary>
        /// Checking for CancellationPending and sets OPERATION_CANCELLED result
        /// </summary>
        /// <param name="worker">Instance of BackgroundWorker</param>
        /// <param name="e">Args to set result</param>
        public static void CheckForCancel(this BackgroundWorker worker, DoWorkEventArgs e)
        {
            // Check for cancel
            if (worker == null || e == null) return;
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                e.Result = OPERATION_CANCELLED;
            }
        }
    }
}
