using System.Runtime.InteropServices;
using System.Text;

namespace NMinify.Interop
{
    public class MarshalEx
    {
        public static unsafe IntPtr StringToHGlobalUTF8(string? s)
        {
            if (s is null)
            {
                return IntPtr.Zero;
            }

            int nb = Encoding.UTF8.GetMaxByteCount(s.Length);

            IntPtr ptr = Marshal.AllocHGlobal(checked(nb + 1));

            byte* pbMem = (byte*)ptr;
            int nbWritten = Encoding.UTF8.GetBytes(s, new Span<byte>(pbMem, nb));
            pbMem[nbWritten] = 0; //\0

            return ptr;
        }

    }
}
