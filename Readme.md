<p align="center">
  <a href="https://github.com/JKamsker/NMinify">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="http://i.epvpimg.com/EmTMaab.png">
      <img src="http://i.epvpimg.com/EmTMaab.png" height="128">
    </picture>
    <h1 align="center">NMinify</h1>
    <p align="center">A .NET wrapper for the golang minify library</p>
  </a>
</p>

 <p align="center">
[![NuGet version (NMinify)](https://img.shields.io/nuget/v/NMinify.svg?style=flat-square)](https://www.nuget.org/packages/NMinify)
[![Nuget](https://img.shields.io/nuget/dt/NMinify)](https://www.nuget.org/packages/NMinify)
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/JKamsker/NMinify/.NET)](https://github.com/JKamsker/NMinify/actions)
[![GitHub license](https://img.shields.io/github/license/JKamsker/NMinify)](https://github.com/JKamsker/NMinify/blob/master/LICENSE.txt)
[![PR](https://img.shields.io/badge/PR-Welcome-blue)](https://github.com/JKamsker/NMinify/pulls)
</p>

## Installation

To install NMinify, run the following command in the Package Manager Console:

```
Install-Package NMinify
```

You can also install NMinify via the .NET CLI by running:
```
dotnet add package NMinify
```

Alternatively, you can add NMinify as a dependency in your project's `.csproj` file:

```xaml
<ItemGroup>
  <PackageReference Include="NMinify" Version="x.x.x" />
</ItemGroup>
``` 

Make sure to replace `x.x.x` with the latest version available on [NuGet](https://www.nuget.org/packages/NMinify/).

Once NMinify is installed, you can start using it in your project by adding `using NMinify;` to the top of your file.

## Basic Usage

```csharp
var text = "<span style=\"color:#ff0000;\" class=\"text\">Some  text</span>";
var minifier = new Minifier();
var minified = minifier.MinifyString("text/html", text);

Console.WriteLine(text);
Console.WriteLine(minified);
```

## Available Methods
```csharp
Span<byte> MinifyBytes(MinifierMediaType mediaType, ReadOnlySpan<byte> input, Span<byte> output);
string MinifyFile(string mediatype, string input);
string MinifyString(MinifierMediaType mediaType, string input);
```

# License

NMinify is released under the [MIT License](https://opensource.org/licenses/MIT). This means that you are free to use, modify, and distribute this software as long as you include the original copyright and license notice in your distribution.

Please note that the underlying minify library by tdewolff is also released under the MIT License, and its copyright and license must also be included in any distribution of NMinify.

By using NMinify, you are agreeing to the terms of the MIT License. If you do not agree to these terms, you should not use this software.

----------

<p align="center">Made with <a href="https://stackedit.io/">stackedit.io</a>, <a href="https://github.com/tdewolff/minify">minify</a> and lots of ❤️ in Austria</p>
