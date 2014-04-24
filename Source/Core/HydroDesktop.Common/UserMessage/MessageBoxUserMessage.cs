using System;
using System.Windows.Forms;
using HydroDesktop.Common.Logging;

namespace HydroDesktop.Common.UserMessage
{
    internal class MessageBoxUserMessage : IUserMessage
    {
        private readonly ILog _logger;

        public MessageBoxUserMessage(IExtraLog logger)
        {
            if (logger == null) throw new ArgumentNullException("logger");
            logger.SkipFrames++;
            _logger = logger;
        }

        #region Implementation of IUserMessage

        public void Info(string message)
        {
            _logger.Info(message);
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Warn(string message, Exception exception = null)
        {
            _logger.Warn(message);
            MessageBox.Show(message + (exception != null
                                           ? Environment.NewLine +
                                             "Reason: " + exception.Message
                                           : string.Empty), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void Error(string message, Exception exception = null)
        {
            _logger.Error(message, exception);
            MessageBox.Show(message + (exception != null
                                           ? Environment.NewLine +
                                             "Reason: " + exception.Message
                                           : string.Empty), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}