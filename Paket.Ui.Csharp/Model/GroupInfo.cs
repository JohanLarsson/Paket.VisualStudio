using System.Collections.Generic;

namespace Paket.Ui.Csharp
{
    using System.Linq;

    public class GroupInfo
    {
        private KeyValuePair<Domain.GroupName, DependenciesGroup> group;

        public GroupInfo(KeyValuePair<Domain.GroupName, DependenciesGroup> group)
        {
            this.group = group;
            this.Packages = group.Value.Packages.Select(x => new DependencyInfo(x.Name.ToString())).ToArray();
        }

        public string Name => this.group.Key.ToString();

        public IReadOnlyList<DependencyInfo> Packages { get; }
    }
}