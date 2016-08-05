namespace Paket.Ui.Csharp
{
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Id}")]
    public class PackageInfo : DependencyInfo
    {
        public PackageInfo(
            string type,
            string registration,
            string id,
            string version,
            string description,
            string summary,
            string title,
            string iconUrl,
            string licenseUrl,
            string projectUrl,
            string[] tags,
            string[] authors,
            int totalDownloads,
            VersionInfo[] versions)
            : base(id, authors, version, description, summary, title, iconUrl)
        {
            this.Type = type;
            this.Registration = registration;
            this.LicenseUrl = licenseUrl;
            this.ProjectUrl = projectUrl;
            this.Tags = tags;
            this.TotalDownloads = totalDownloads;
            this.Versions = versions;
        }

        public string Type { get; }

        public string Registration { get; }

        public string LicenseUrl { get; }

        public string ProjectUrl { get; }

        public IReadOnlyList<string> Tags { get; }

        public int TotalDownloads { get; }

        public IReadOnlyList<VersionInfo> Versions { get; }

        public static bool operator ==(PackageInfo left, PackageInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PackageInfo left, PackageInfo right)
        {
            return !Equals(left, right);
        }

        protected bool Equals(PackageInfo other)
        {
            return string.Equals(this.Id, other.Id) && string.Equals(this.Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((PackageInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Id.GetHashCode() * 397) ^ this.Version.GetHashCode();
            }
        }
    }
}