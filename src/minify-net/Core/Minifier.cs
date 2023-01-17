using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using NMinify.Interop;

namespace NMinify;

public class Minifier : IDisposable, IMinifier
{
    static Minifier()
    {
        LibraryResolver.Register();
    }

    private UIntPtr _minifierPtr;
    private bool _disposed;

    public Minifier(Dictionary<string, string>? config = null)
    {
        config ??= new Dictionary<string, string>()
        {
            { "html-keep-comments", "false" }
            , { "html-keep-conditional-comments", "false" }
            , { "html-keep-default-attr-vals", "false" }
            , { "html-keep-document-tags", "false" }
            , { "html-keep-end-tags", "false" }
            , { "html-keep-whitespace", "false" }
            , { "html-keep-quotes", "false" }
            , { "js-precision", "0" }
            , { "js-keep-var-names", "false" }
            , { "js-no-nullish-operator", "false" }
            , { "json-precision", "0" }
            , { "json-keep-numbers", "false" }
            , { "svg-keep-comments", "false" }
            , { "svg-precision", "0" }
            , { "xml-keep-whitespace", "false" }
            , { "css-precision", "0" }
        };

        // Allocate a new minifier object
        _minifierPtr = NativeMinifierWrapper.allocateMinifier();
        Configure(config);
    }


    private void Configure(Dictionary<string, string> config)
    {
        Configure(config.Keys.ToArray(), config.Values.ToArray());
    }

    private void Configure(string[] keys, string[] values)
    {
        ThrowIfDisposed();

        // Configure the minifier object with the provided keys and values
        NativeMinifierWrapper.configureMinifier(_minifierPtr, keys, values, keys.Length, out var errorMessage);
        if (!string.IsNullOrEmpty(errorMessage))
        {
            throw new ArgumentException(errorMessage);
        }
    }



    public string MinifyFile(string mediatype, string input)
    {
        ThrowIfDisposed();
        // Minify a file
        var coutput = string.Empty;
        NativeMinifierWrapper.minifyFile(_minifierPtr, mediatype, input, out coutput);
        return coutput;
    }

    public unsafe string MinifyString(MinifierMediaType mediaType, string input)
    {
        ThrowIfDisposed();
        return NativeMinifierWrapper.MinifyString(_minifierPtr, mediaType, input);
    }

    public unsafe Span<byte> MinifyBytes(MinifierMediaType mediaType, ReadOnlySpan<byte> input, Span<byte> output)
    {
        ThrowIfDisposed();
        return NativeMinifierWrapper.MinifyBytes(_minifierPtr, mediaType, input, output);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(Minifier));
        }
    }

    private bool IsDisposed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _disposed || _minifierPtr == UIntPtr.Zero;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
        }


        _disposed = true;
        NativeMinifierWrapper.freeMinifier(_minifierPtr);
        _minifierPtr = UIntPtr.Zero;
    }

    ~Minifier()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}