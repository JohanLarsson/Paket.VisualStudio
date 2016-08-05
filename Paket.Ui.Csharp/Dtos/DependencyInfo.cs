namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;

    public abstract class DependencyInfo
    {
        protected DependencyInfo(string id, IReadOnlyList<string> authors, string version, string description, string summary, string title, string iconUrl)
        {
            this.Id = id;
            this.Authors = authors;
            this.Version = version;
            this.Description = description;
            this.Summary = summary;
            this.Title = title;
            this.IconUrl = iconUrl;
        }

        public string Id { get; }

        public IReadOnlyList<string> Authors { get; }

        public string Version { get; }

        public string Description { get; }

        public string Summary { get; }

        public string Title { get; }

        public string IconUrl { get; }
    }
}