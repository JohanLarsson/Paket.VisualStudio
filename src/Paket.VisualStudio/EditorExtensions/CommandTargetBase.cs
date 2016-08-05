// taken from WebEssentials2015 by Mads Kristensen https://github.com/madskristensen/WebEssentials2015
// todo make this a proper attribution

namespace Paket.VisualStudio.EditorExtensions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows.Threading;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.TextManager.Interop;

    internal abstract class CommandTargetBase<TCommandEnum> : IOleCommandTarget where TCommandEnum : struct, IComparable
    {
        private IOleCommandTarget _nextCommandTarget;
        protected readonly IWpfTextView TextView;

        public Guid CommandGroup { get; set; }
        public ReadOnlyCollection<uint> CommandIds { get; private set; }

        public CommandTargetBase(IVsTextView adapter, IWpfTextView textView, params TCommandEnum[] commandIds)
            : this(adapter, textView, typeof(TCommandEnum).GUID, Array.ConvertAll(commandIds, e => Convert.ToUInt32(e, CultureInfo.InvariantCulture)))
        { }
        public CommandTargetBase(IVsTextView adapter, IWpfTextView textView, Guid commandGroup, params uint[] commandIds)
        {
            this.CommandGroup = commandGroup;
            this.CommandIds = new ReadOnlyCollection<uint>(commandIds);
            this.TextView = textView;

            Dispatcher.CurrentDispatcher.InvokeAsync(() =>
            {
                // Add the target later to make sure it makes it in before other command handlers
                ErrorHandler.ThrowOnFailure(adapter.AddCommandFilter(this, out this._nextCommandTarget));
            }, DispatcherPriority.ApplicationIdle);
        }

        protected abstract bool IsEnabled();
        protected abstract bool Execute(TCommandEnum commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut);

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == this.CommandGroup && this.CommandIds.Contains(nCmdID))
            {
                bool result = this.Execute((TCommandEnum)(object)(int)nCmdID, nCmdexecopt, pvaIn, pvaOut);

                if (result)
                {
                    return VSConstants.S_OK;
                }
            }

            return this._nextCommandTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup != this.CommandGroup)
                return this._nextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

            for (int i = 0; i < cCmds; i++)
            {
                if (this.CommandIds.Contains(prgCmds[i].cmdID))
                {
                    if (this.IsEnabled())
                    {
                        prgCmds[i].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
                        return VSConstants.S_OK;
                    }

                    prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_SUPPORTED;
                }
            }

            return this._nextCommandTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
