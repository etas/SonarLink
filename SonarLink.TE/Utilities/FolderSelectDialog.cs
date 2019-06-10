// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Microsoft.WindowsAPICodePack.Dialogs;
using System;

namespace SonarLink.TE.Utilities
{
    /// <summary>
    /// UI modal dialog which allows selection of directories
    /// </summary>
    public class FolderSelectDialog : IDisposable
    {
        /// <summary>
        /// 'Base' file dialog
        /// </summary>
        private readonly CommonOpenFileDialog dialog = null;
    
        /// <summary>
        /// Constructor
        /// </summary>
        public FolderSelectDialog()
        {
            dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select project local path"
            };
        }
    
        /// <summary>
        /// Directory path selected by users
        /// </summary>
        public string SelectedPath
        {
            get { return dialog?.FileName; }
        }
    
        /// <summary>
        /// Displays the modal dialog
        /// </summary>
        /// <returns>true if a folder was selected; false otherwise (dialog dismissed)</returns>
        public bool ShowDialog()
        {
            return (dialog.ShowDialog() == CommonFileDialogResult.Ok);
        }
    
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
    
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dialog?.Dispose();
                }
                disposedValue = true;
            }
        }
    
        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    
    }
}
