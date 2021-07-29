namespace Badaboom.Core.Extension
{
    public static class StringExtension
    {
        public static string ReplaceAll(this string @base, string[] oldValues, string newValue)
        {
            foreach (var value in oldValues)
            {
                @base = @base.Replace(value, newValue);
            }

            return @base;
        }
    }
}
