using System.Collections.Concurrent;
using NMinify.Interop;

namespace NMinify
{
    public readonly struct MinifierMediaType
    {
        public static readonly MinifierMediaType Html;
        public static readonly MinifierMediaType Css;
        public static readonly MinifierMediaType JavaScript;
        public static readonly MinifierMediaType Xml;

        static MinifierMediaType()
        {
            _cache = new();

            Html = new("text/html");
            Css = new("text/css");
            JavaScript = new("application/javascript");
            Xml = new("text/xml");

            var initMatrix = new[]
            {
                "text/css",
                "text/html",
                "image/svg+xml",
                "application/javascript",
                "text/javascript",
                "application/x-javascript",
                "text/ecmascript",
                "application/ecmascript",
                "application/json",
                "application/x-json",
                "text/json",
                "text/x-json",
                "application/xml",
                "text/xml",
                "application/x-xml",
                "text/x-xml"
            };

            foreach (var item in initMatrix)
            {
                _ = new MinifierMediaType(item);
            }
        }

        private static ConcurrentDictionary<string, nint> _cache;

        public readonly string MediaType;
        public readonly nint NativePointer;

        public MinifierMediaType(string mediaTypeString, bool useCache = true)
        {
            NativePointer = useCache
                ? _cache.GetOrAdd(mediaTypeString, x => MarshalEx.StringToHGlobalUTF8(x))
                : MarshalEx.StringToHGlobalUTF8(mediaTypeString);

            MediaType = mediaTypeString;
        }

        public static implicit operator MinifierMediaType(string mediaTypeString)
        {
            return new(mediaTypeString);
        }
    }
}
