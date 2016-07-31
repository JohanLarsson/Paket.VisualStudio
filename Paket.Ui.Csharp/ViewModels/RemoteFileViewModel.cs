namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;

    public class RemoteFileViewModel : DependencyViewModel
    {
        private static readonly List<RemoteFileViewModel> Cache = new List<RemoteFileViewModel>();
        private readonly RemoteFileReference remoteFileReference;
        private readonly ModuleResolver.UnresolvedSource unresolvedSource;

        private RemoteFileViewModel(RemoteFileReference remoteFileReference)
            : base(remoteFileReference.Name)
        {
            this.remoteFileReference = remoteFileReference;
        }

        private RemoteFileViewModel(ModuleResolver.UnresolvedSource unresolvedSource)
            : base(unresolvedSource.Name)
        {
            this.unresolvedSource = unresolvedSource;
        }

        internal static RemoteFileViewModel GetOrCreate(RemoteFileReference remoteFileReference)
        {
            if (remoteFileReference == null)
            {
                return null;
            }

            var key = remoteFileReference.Name;
            var match = Cache.Find(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                match = new RemoteFileViewModel(remoteFileReference);
                Cache.Add(match);
            }

            return match;
        }

        internal static RemoteFileViewModel GetOrCreate(ModuleResolver.UnresolvedSource unresolvedSource)
        {
            if (unresolvedSource == null)
            {
                return null;
            }

            var key = unresolvedSource.Name;
            var match = Cache.Find(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                match = new RemoteFileViewModel(unresolvedSource);
                Cache.Add(match);
            }

            return match;
        }
    }
}