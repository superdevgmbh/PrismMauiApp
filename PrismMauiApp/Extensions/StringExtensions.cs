namespace PrismMauiApp.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceLastOccurrence(this string source, string find, string replace)
        {
            if (source == null)
            {
                return source;
            }

            var place = source.LastIndexOf(find);
            if (place == -1)
            {
                return source;
            }

            return source.Remove(place, find.Length).Insert(place, replace);
        }
    }
}
