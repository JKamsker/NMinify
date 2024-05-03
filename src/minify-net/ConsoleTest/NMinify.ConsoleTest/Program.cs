using NMinify.Interop;

using System.Reflection;

namespace NMinify.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var text = "<span style=\"color:#ff0000;\" class=\"text\">Some  text</span>";
            var minifier = new Minifier();
            var minified = minifier.MinifyString(MinifierMediaType.Html, text);

            Console.WriteLine(text);
            Console.WriteLine(minified);
        }
    }
}