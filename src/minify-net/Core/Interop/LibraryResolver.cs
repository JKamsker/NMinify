using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMinify.Interop;

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

public static class LibraryResolver
{
    public static Func<IEnumerable<string>> LibraryDirectoryResolver = GetLibraryDirectories;

    static Dictionary<(Assembly assembly, DllImportSearchPath? searchPath), nint> _loadedLibraries
        = new();

    private static bool _registered;

    public static void Register()
    {
        if (_registered)
        {
            return;
        }
        _registered = true;
        NativeLibrary.SetDllImportResolver(typeof(LibraryResolver).Assembly, Resolve);
        //NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), Resolve);

    }

    private static nint Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (!string.Equals("minifier", libraryName, StringComparison.OrdinalIgnoreCase))
        {
            return nint.Zero;
        }

        var key = (assembly, searchPath);
        if (_loadedLibraries.TryGetValue(key, out var ptr))
        {
            return ptr;
        }

        return _loadedLibraries[key] = ResolveMinifier(assembly, searchPath);
    }

    private static nint ResolveMinifier(Assembly assembly, DllImportSearchPath? searchPath)
    {
        var lib = ResolveLibraryPath(LibraryDirectoryResolver());
        if (lib == null)
        {
            return nint.Zero;
        }

// #if DEBUG
//         Console.WriteLine($"Loading {lib}");
// #endif
        return NativeLibrary.Load(lib, assembly, searchPath);
    }

    private static IEnumerable<string> GetLibraryDirectories()
    {
        var directories = GetDirectories
        (
            Assembly.GetEntryAssembly()?.Location,
            Assembly.GetCallingAssembly()?.Location,
            Assembly.GetExecutingAssembly()?.Location,
            Assembly.GetAssembly(typeof(LibraryResolver))?.Location
        )
        .Prepend(Environment.CurrentDirectory)
        .Distinct();

        return directories.Concat(directories.Select(n =>
        {
            var root = GetRootPath(n);
            if (!string.IsNullOrEmpty(root))
            {
                return Path.Combine(root, ".bin\\go\\lib\\");
            }

            return Path.Combine(n, "..\\..\\..\\..\\..\\..\\..\\.bin\\go\\lib\\");
        }));

        static IEnumerable<string> GetDirectories(params string?[] files)
        {
            return files?
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => Path.GetDirectoryName(x))
                .OfType<string>()
                ?? Enumerable.Empty<string>();
        }
    }

    // Root is marked by a .gitignore file, a .bin file and a .build file. Go up until those files are there.
    private static string? GetRootPath(string someSubChild)
    {
        var root = Path.GetFullPath(someSubChild);
        while (!File.Exists(Path.Combine(root, ".gitignore"))
            || !Directory.Exists(Path.Combine(root, ".bin"))
            || !Directory.Exists(Path.Combine(root, ".build")))
        {
            root = Path.GetDirectoryName(root);
            if (string.IsNullOrEmpty(root))
            {
                return null;
            }
        }
        return root;
    }



    public static string? ResolveLibraryPath(IEnumerable<string> basePaths)
    {
        string relativeLibraryPath = GetRelativeLibraryPath();

        var paths = Enumerable.Concat
        (
            basePaths.Select(x => Path.Combine(x, relativeLibraryPath)),
            basePaths.Select(x => Path.Combine(x, "runtimes", relativeLibraryPath))
        );

        return paths
            .FirstOrDefault(x => File.Exists(x));
    }


    private static string GetRelativeLibraryPath()
    {
        // Determine the operating system and bit system
        string os = GetOperatingSystem();
        string bitSystem = GetBitSystem();

        //var folderName = ;
        // Construct the library file name based on the naming scheme
        string fileName = $"minifier-bindings-{os}-{bitSystem}";

        // Set the library file extension based on the operating system
        switch (os)
        {
            case "darwin":
                fileName += ".dylib";
                break;
            case "linux":
                fileName += ".so";
                break;
            case "windows":
                fileName += ".dll";
                break;
            default:
                throw new Exception("Unsupported operating system");
        }

        return Path.Combine($"{os}-{bitSystem}", "native", fileName);
    }

    // Returns the name of the current operating system
    private static string GetOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "darwin";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "windows";
        }
        else
        {
            throw new Exception("Unsupported operating system");
        }
    }

    // Returns the bit system of the current operating system (e.g. "amd64" or "arm64")
    private static string GetBitSystem()
    {
        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ||
            RuntimeInformation.ProcessArchitecture == Architecture.X64)
        {
            return "amd64";
        }
        else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm)
        {
            return "arm64";
        }
        else if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
        {
            return "386";
        }
        else
        {
            throw new Exception("Unsupported bit system");
        }
    }
}

