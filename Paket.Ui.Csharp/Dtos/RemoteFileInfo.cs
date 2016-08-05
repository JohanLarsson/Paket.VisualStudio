namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;

    public class RemoteFileInfo : DependencyInfo
    {
        public RemoteFileInfo(string id, IReadOnlyList<string> authors, string version, string description,
            string summary, string title, string iconUrl)
            : base(id, authors, version, description, summary, title, iconUrl)
        {
        }
    }
}
