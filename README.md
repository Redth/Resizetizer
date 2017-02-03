# Resizetizer

[![NuGet](https://img.shields.io/nuget/v/Resizetizer.svg)](https://www.nuget.org/packages/Resizetizer/) 

Have you ever wanted an easier way to manage all those different sizes of images in your app? Resizetizer is a more sane way to automate this process, and is friendly to developers who suck at design (like myself!).

You an use SVG vectors or PNG/BMP/JPG graphics as source assets, and with a bit of simple configuration, have them automatically resized to every resolution your app needs!


## Easy Installation

1. Add the `Resizetizer` NuGet package to one of your projects.
2. Create a `resizetizer.yaml` configuration file in your project with your assets and output ratio/files you want.
3. Set the build action of your `resizetizer.yaml` file to `ResizetizerConfig`.
3. Build your project and watch the magic!


## How does it work?

`Resizetizer` itself is a simple library for resizing SVG or PNG (and more in the future) source assets to various size ratio outputs (fully customizable).

The Resizetizer NuGet package contains a custom MSBuild task which will run before the `PrepareForBuild` target in the project you add it to when you compile.

The MSBuild task will look for YAML or JSON config files with a build action of `ResizetizerConfig` to figure out what images to resize, and how to resize them.

A *Resizetizer.stamps* file is created in your project's immediates output directory after assets are resized with modify timestamps from the source asset file listed for each output to ensure assets aren't resized unless the source has actually changed.  If you find any issues with this, just delete this file and rebuild.

## Configuring Resizetizer
Your config file should look something like this:

```yaml
- platform: Android
  outputBasePath: "./MyProj.Droid/Resources/"
  assets:
   - file: "../input/happy.svg"
     size: 100x100
  outputs:
   - path: "./drawable-mdpi"
     ratio: 1.0
   - path: "./drawable-hdpi"
     ratio: 1.5
   - path: "./drawable-xhdpi"
     ratio: 2.0
   - path: "./drawable-xxhdpi"
     ratio: 3.0
   - path: "./drawable-xxxhdpi"
     ratio: 4.0
- platform: iOS
  outputBasePath: "./MyProj.iOS/Images/"
  assets:
   - file: "../input/happy.svg"
     size: 36x36
  outputs:
   - ratio: 1.0
   - fileSuffix: "@2x"
     ratio: 2.0
   - fileSuffix: "@3x"
     ratio: 3.0
```

As you can see above, you can specify multiple configurations in a single file (you could have multiple files if you wish too!).  Each configuration requires an `outputBasePath` to be declared, which is relative to the location of the config file itself.


## Optimization

**NOTE:** It turns out, skia already does a pretty good job of optimizing PNG output.  I'm still looking at other options (options are good afterall), but I wouldn't be too concerned about letting skia handle this for now.

PNG Output Optimization will be enabled by default.  You can disable it by setting `optimize: false` on the config, or on an individual output item.

For now, optimization is a no-op.  I am looking at different options to run optimizations on.

## External Commands

It's possible to specify external commands to run on the config or an individual output item.  You can use the following tags which will be substituted for you in your commands:


 - `{outputFile}`  This is a full path to the output file.
 - `{width}` This is the width of the outputFile.
 - `{height}` This is the height of the outputFile.
  
Here's an example:

```yaml
- platform: iOS
  outputBasePath: "./MyProj.iOS/Images/"
  externalCommands:
   - /usr/local/bin/optipng -o7 "{outputFile}"
  assets:
   - file: "../input/happy.svg"
     size: 36x36
  outputs:
   - ratio: 1.0
```

## SVG Fill Color

You can also change The fill color of an SVG on your output specification.  For example:

```yaml
- platform: iOS
  outputBasePath: "./MyProj.iOS/Images/"
  assets:
   - file: "../input/happy.svg"
     size: 36x36
  outputs:
   - ratio: 1.0
     fillColor: #000000
   - fileSuffix: "@2x"
     ratio: 2.0
     fillColor: #000000
```

This will change every path in the source SVG to be black (#000000) in the resulting output file.  This only works for SVG assets.


## Configuration Reference

 - **platform**: *Optional* platform enum (`Android` | `iOS` | `UWP`).  If you specify this, you can use the `autoAddPlatformSizes: true` setting to automatically include some basic `outputs` for the given platform.
 - **optimize**: *Optional* `true` by default.  Whether or not optimize output PNG's after they are resized.  Currently this is not actually implemented.
 - **optimizer**: *Optional* Change which PNG optimization engine is used.  Currently the only possible value is *ImageSharp* however this is not yet implemented.
 - **outputBasePath**: Relative path (relative to the location of the config file) to specify the base location where outputs should be saved to.
 - **externalCommands**: *Optional* set of commands to execute for every output in the config.  The placeholders *{outputFile}*, *{width}*, *{height}* will be substituted in the specified string.
 - **assets**: Set of source asset files to be resized to the given set of `outputs`.
    - **file**: Filename of asset source (relative to the `outputBasePath`).
    - **size**: The nominal dimension (widthxheight) which all `outputs` ratios are calculated from.  For example, if the `size` is *100x100*, even if the actual source asset file is *497x497*, all ratios for resizing outputs will be calculated as *100* * *ratio*.  This makes it easier to use known ratios for calculating output sizes (e.g.: *drawable-mdpi* is the nominal size, and *drawable-hdpi* is a 1.5 * the nominal size, or *150x150*).
 - **outputs**: Set of output configurations (ratio, filename, etc) to resize all the `assets` in the configuration to.
   - **path**: *Optional* output path for the resized image (relative to the `outputBasePath`.
   - **ratio**: The multiplier for determine resized width and height.  This value is multiplied against the `asset`'s `size` to determine the output size.
   - **filePrefix**: *Optional* value to add to the beginning of the output resized filename.
   - **fileSuffix**: *Optional* value to add to the end of the resized output filename.  This is useful for iOS images especially (e.g.: `fileSuffix: @2x` will cause the asset *happy.png* to be output as *happy@2x.png*.

    - **fillColor**: *Optional* for SVG assets, if specified, all *paths* in the source SVG file will have their fill color set to this value.
    - **optimize**: *Optional* `true` by default.  Overrides config setting.  Whether or not optimize output PNG's after they are resized.  Currently this is not actually implemented.
    - **optimizer**: *Optional* Change which PNG optimization engine is used.  Overrides config setting.  Currently the only possible value is *ImageSharp* however this is not yet implemented.
   - **externalCommands**: *Optional* set of commands to execute for every output in the config.  The placeholders *{outputFile}*, *{width}*, *{height}* will be substituted in the specified string.  These will run in addition to the commands specified at the config level.


## What's next?

**Automatically add generated assets to Xamarin.iOS and Xamarin.Android projects**

Currently Resizetizer does not automatically add resized assets to your app projects (e.g.: it will not add images inside a `drawable-hdpi` folder to your Xamarin.Android app project).  You will still have to manually add them as references in your projects the after first time they are generated.  One idea is to have this all automated for Xamarin.Android and Xamarin.iOS projects.

**PDF Asset support**

It's somewhat common for iOS developers to use PDF files in their projects.  It would be nice for Resizetizer to support this format... However it should.  More research needed.
