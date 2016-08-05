namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;

    public class RemoteFileViewModel : DependencyViewModel
    {
        private static readonly List<RemoteFileViewModel> Cache = new List<RemoteFileViewModel>();

        private RemoteFileViewModel(string name)
            : base(name)
        {
        }

        public override string Version { get; } = "Not implemented";

        public override bool IsFavorite { get; set; }

        internal static RemoteFileViewModel GetOrCreate(RemoteFileReference remoteFileReference)
        {
            return GetOrCreate(remoteFileReference?.Name);
        }

        internal static RemoteFileViewModel GetOrCreate(ModuleResolver.UnresolvedSource unresolvedSource)
        {
            return GetOrCreate(unresolvedSource?.Name);
        }

        private static RemoteFileViewModel GetOrCreate(string name)
        {
            if (name == null)
            {
                return null;
            }

            var match = Cache.Find(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                match = new RemoteFileViewModel(name);
                Cache.Add(match);
            }

            return match;
        }
    }
}