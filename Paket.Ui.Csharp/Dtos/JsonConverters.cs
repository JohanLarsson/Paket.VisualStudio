namespace Paket.Ui.Csharp
{
    using Newtonsoft.Json;

    internal static class JsonConverters
    {
        public static readonly JsonConverter[] Default =
        {
            PackageInfoConverter.Default,
        };
    }
}
