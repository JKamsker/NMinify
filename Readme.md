<p align="center">
  <a href="https://github.com/JKamsker/NMinify">
    <picture>
      <source media="(prefers-color-scheme: dark)" srcset="https://user-images.githubusercontent.com/11245306/213277904-0129ae88-f655-4e03-a723-e5bac4b864e1.png">
      <img src="https://user-images.githubusercontent.com/11245306/213277904-0129ae88-f655-4e03-a723-e5bac4b864e1.png" height="128" alt="NMinify logo">
    </picture>
    <p align="center">Minify HTML, CSS, JS and more</p>
  </a>
</p>

[![NuGet version (NMinify)](https://img.shields.io/nuget/v/NMinify.svg?style=flat-square)](https://www.nuget.org/packages/NMinify)
[![Nuget](https://img.shields.io/nuget/dt/NMinify)](https://www.nuget.org/packages/NMinify)
[![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/JKamsker/NMinify/build-dependency.yaml?branch=master)](https://github.com/JKamsker/NMinify/actions)
[![GitHub license](https://img.shields.io/github/license/JKamsker/NMinify)](https://github.com/JKamsker/NMinify/blob/master/LICENSE.txt)
[![PR](https://img.shields.io/badge/PR-Welcome-blue)](https://github.com/JKamsker/NMinify/pulls)

## Introduction
NMinify is a Wrapper for the [golang library minify](https://github.com/tdewolff/minify). <br/>
Minification is the process of removing bytes from a file (such as whitespace) without changing its output and therefore shrinking its size and speeding up transmission over the internet and possibly parsing. The implemented minifiers are designed for high performance.

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
var minified = minifier.MinifyString(MinifierMediaType.Html, text);


Console.WriteLine(text); // Output: <span style="color:#ff0000;" class="text">Some  text</span>
Console.WriteLine(minified); // Output: <span style=color:red class=text>Some text</span>
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
