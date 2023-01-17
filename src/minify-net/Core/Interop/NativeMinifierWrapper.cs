using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NMinify.Interop
{
    public class NativeMinifierWrapper
    {
        [DllImport("minifier", EntryPoint = "allocateMinifier", CallingConvention = CallingConvention.Cdecl)]
        public static extern UIntPtr allocateMinifier();

        [DllImport("minifier", EntryPoint = "freeMinifier", CallingConvention = CallingConvention.Cdecl)]
        public static extern void freeMinifier(UIntPtr minifierPtr);

        [DllImport("minifier", EntryPoint = "configureMinifier")]
        public static extern IntPtr configureMinifier(UIntPtr minifierPtr, string[] ckeys, string[] cvals, long length, out string output_errorMessage);

        [DllImport("minifier", EntryPoint = "minifyFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr minifyString(UIntPtr minifierPtr, string cmediatype, string cinput, long input_length, out string coutput, out long output_length);

        [DllImport("minifier", EntryPoint = "minifyString", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr minifyFile(UIntPtr minifierPtr, string cmediatype, string cinput, out string coutput);

        [DllImport("minifier", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static unsafe extern IntPtr minifyString(UIntPtr minifierPtr, byte* cmediatype, byte* cinput, long input_length, byte* coutput, long* output_length);


        public unsafe static string MinifyString(UIntPtr minifierPtr, MinifierMediaType mediaType, string input)
        {
            const int BYTE_LIMIT = 512;

            int maxByteCount = Encoding.UTF8.GetMaxByteCount(input.Length);

            var inputPooled = maxByteCount > BYTE_LIMIT;

            var inputArray = inputPooled ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
            byte[]? outputArray = null;
            try
            {
                Span<byte> inputSpan = inputArray ?? stackalloc byte[maxByteCount];
                var actualByteCount = Encoding.UTF8.GetBytes(input, inputSpan);

                var outputPooled = actualByteCount > BYTE_LIMIT;
                outputArray = outputPooled ? ArrayPool<byte>.Shared.Rent(actualByteCount) : null;
                Span<byte> outputSpan = outputArray ?? stackalloc byte[actualByteCount];

                inputSpan = inputSpan[..actualByteCount];

                var output = MinifyBytes(minifierPtr, mediaType, inputSpan, outputSpan);
                return Encoding.UTF8.GetString(output);
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
            finally
            {
                if (inputArray is not null)
                {
                    ArrayPool<byte>.Shared.Return(inputArray);
                }

                if (outputArray is not null)
                {
                    ArrayPool<byte>.Shared.Return(outputArray);
                }
            }
        }

        public unsafe static Span<byte> MinifyBytes(UIntPtr minifierPtr, MinifierMediaType mediaType, ReadOnlySpan<byte> input, Span<byte> output)
        {
            long output_length = 0;
            fixed (byte* inputPtr = input, outputPtr = output)
            {
                var error = minifyString
                (
                    minifierPtr,
                    (byte*)mediaType.NativePointer.ToPointer(),
                    inputPtr, input.Length,
                    outputPtr, &output_length
                );

                if (error != nint.Zero)
                {
                    var errorStr = Marshal.PtrToStringUTF8(error);

                    //Marshal.FreeCoTaskMem(error);
                    Console.WriteLine($"Error! {errorStr}");
                }
            }
            return output[..(int)output_length];
        }
    }



}
