namespace NMinify
{
    public interface IMinifier
    {
        void Dispose();
        Span<byte> MinifyBytes(MinifierMediaType mediaType, ReadOnlySpan<byte> input, Span<byte> output);
        string MinifyFile(string mediatype, string input);
        string MinifyString(MinifierMediaType mediaType, string input);
    }
}