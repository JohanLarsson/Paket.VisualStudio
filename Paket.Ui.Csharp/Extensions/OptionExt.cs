namespace Paket.Ui.Csharp
{
    using Microsoft.FSharp.Core;

    internal static class OptionExt
    {
        internal static T ValueOrNull<T>(this FSharpOption<T> option)
            where T : class 
        { 
            if (FSharpOption<T>.get_IsSome(option))
            {
                return option.Value;
            }

            return null;
        }
    }
}
