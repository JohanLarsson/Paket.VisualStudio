namespace Paket.Ui.Csharp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using JetBrains.Annotations;
    using Newtonsoft.Json;

    public sealed class BrowseViewModel : INotifyPropertyChanged
    {
        private string searchText;
        private IEnumerable<string> autoCompletes;
        private Exception exception;
        private static readonly string CacheFile = System.IO.Path.Combine(Paket.Constants.NuGetCacheFolder, "defaultSearch.paket");
        private bool isIncludingPreRelease;
        private bool hasUpdatedDefaultSearch;

        public BrowseViewModel()
        {
#pragma warning disable 4014 ctor intentional fire & forget
            this.UpdateWithEmptySearch();
#pragma warning restore 4014
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string SearchText
        {
            get { return this.searchText; }
            set
            {
                if (value == this.searchText) return;
                this.searchText = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsIncludingPreRelease
        {
            get
            {
                return this.isIncludingPreRelease;
            }
            set
            {
                if (value == this.isIncludingPreRelease) return;
                this.isIncludingPreRelease = value;
                this.OnPropertyChanged();
            }
        }

        public IEnumerable<string> AutoCompletes
        {
            get { return this.autoCompletes; }
            private set
            {
                if (Equals(value, this.autoCompletes)) return;
                this.autoCompletes = value;
                this.OnPropertyChanged();
            }
        }

        public RefreshingCollection<DependencyViewModel> Packages { get; } = new RefreshingCollection<DependencyViewModel>();

        public Exception Exception
        {
            get { return this.exception; }
            private set
            {
                if (Equals(value, this.exception)) return;
                this.exception = value;
                this.OnPropertyChanged();
            }
        }

        public object RefreshCommand => null;

        internal async Task FetchMorePackagesAsync()
        {
            try
            {
                var query = this.searchText;
                var results = await Nuget.GetMorePackagesAsync(this.SearchText)
                                         .ConfigureAwait(false);
                if (query == this.searchText)
                {
                    this.Packages.UnionWith(results.Select(PackageViewModel.GetOrCreate));
                }

                this.Exception = null;
            }
            catch (Exception e)
            {
                this.Exception = e;
            }
        }

        [NotifyPropertyChangedInvocator]
        private async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(this.SearchText))
            {
                var query = this.searchText;
                var ints = await Task.WhenAll(
                    this.UpdateResults(),
                    this.UpdateAutoComplete())
                                     .ConfigureAwait(false);

                if (query == this.searchText && ints[0] < 20)
                {
                    await this.AppendAutoCompleteResults(query).ConfigureAwait(false);
                }
            }
        }

        private async Task UpdateWithEmptySearch()
        {
            try
            {
                this.Packages.Clear();
                var favorites = await Favorites.GetPackagesAsync().ConfigureAwait(false);
                this.Packages.UnionWith(favorites.Select(PackageViewModel.GetOrCreate));

                if (System.IO.File.Exists(CacheFile))
                {
                    var json = await File.ReadAllTextAsync(CacheFile).ConfigureAwait(false);
                    var packageInfos = JsonConvert.DeserializeObject<QueryResponse>(json, JsonConverters.Default).Data;
                    this.Packages.UnionWith(packageInfos.Select(PackageViewModel.GetOrCreate));
                }

                if (!this.hasUpdatedDefaultSearch)
                {
                    Nuget.ReceivedRespose += this.OnReceivedResponse;
                }

                var tasks = favorites.Select(x => Nuget.GetPackageInfosAsync($"id:{x.Id}"))
                                     .ToList();
                tasks.Add(Nuget.GetPackageInfosAsync(this.SearchText));
                await Task.WhenAll(tasks).ConfigureAwait(false);
                if (string.IsNullOrEmpty(this.searchText))
                {
                    this.Packages.RefreshWith(tasks.SelectMany(x => x.Result).Select(PackageViewModel.GetOrCreate));
                }
            }
            catch (Exception e)
            {
                this.Exception = e;
                Nuget.ReceivedRespose -= this.OnReceivedResponse;
            }
        }

        private async void OnReceivedResponse(object sender, ReceivedResposeEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.Searchtext))
            {
                return;
            }

            this.hasUpdatedDefaultSearch = true;
            try
            {
                await File.WriteAllTextAsync(CacheFile, args.Json).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                this.Exception = e;
            }
        }

        private async Task AppendAutoCompleteResults(string query)
        {
            var names = this.autoCompletes;
            if (names.Any())
            {
                try
                {
                    var tasks = names.Select(name => Nuget.GetPackageInfosAsync(name));
                    foreach (var task in tasks)
                    {
                        var results = await task.ConfigureAwait(false);
                        if (this.searchText != query)
                        {
                            // awaiting here so we can log exception if any.
                            await Task.WhenAll(tasks).ConfigureAwait(false);
                            break;
                        }

                        this.Packages.UnionWith(results.Select(PackageViewModel.GetOrCreate));
                    }

                    this.Exception = null;
                }
                catch (Exception e)
                {
                    this.Exception = e;
                }
            }
        }

        private async Task<int> UpdateAutoComplete()
        {
            if (this.autoCompletes?.Contains(this.searchText) == true)
            {
                return 0;
            }

            this.autoCompletes = Enumerable.Empty<string>();
            if (string.IsNullOrWhiteSpace(this.searchText))
            {
                return 0;
            }

            try
            {
                var query = this.searchText;
                this.AutoCompletes = await Nuget.GetAutoCompletesAsync(query)
                                                .ConfigureAwait(false);
                this.Exception = null;
                return this.autoCompletes?.Count() ?? 0;
            }
            catch (Exception e)
            {
                this.Exception = e;
            }

            return 0;
        }

        private async Task<int> UpdateResults(int? take = null)
        {
            if (string.IsNullOrEmpty(this.searchText))
            {
                await this.UpdateWithEmptySearch().ConfigureAwait(false);
                return this.Packages.Count;
            }
            try
            {
                var query = this.searchText;
                var results = await Nuget.GetPackageInfosAsync(this.SearchText, take)
                                         .ConfigureAwait(false);

                if (query == this.searchText)
                {
                    this.Packages.RefreshWith(results.Select(PackageViewModel.GetOrCreate));
                    return results.Count;
                }

                this.Exception = null;
                return int.MaxValue;
            }
            catch (Exception e)
            {
                this.Exception = e;
                return 0;
            }
        }
    }
}
