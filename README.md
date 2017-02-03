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


## Configuration Reference

 - **platform**: *Optional* platform enum (`Android` | `iOS` | `UWP`).  If you specify this, you can use the `autoAddPlatformSizes: true` setting to automatically include some basic `outputs` for the given platform.
 - **outputBasePath**: Relative path (relative to the location of the config file) to specify the base location where outputs should be saved to.
 - **assets**: Set of source asset files to be resized to the given set of `outputs`.
    - **file**: Filename of asset source (relative to the `outputBasePath`).
    - **size**: The nominal dimension (widthxheight) which all `outputs` ratios are calculated from.  For example, if the `size` is *100x100*, even if the actual source asset file is *497x497*, all ratios for resizing outputs will be calculated as *100* * *ratio*.  This makes it easier to use known ratios for calculating output sizes (e.g.: *drawable-mdpi* is the nominal size, and *drawable-hdpi* is a 1.5 * the nominal size, or *150x150*).
 - **outputs**: Set of output configurations (ratio, filename, etc) to resize all the `assets` in the configuration to.
   - **path**: *Optional* output path for the resized image (relative to the `outputBasePath`.
   - **ratio**: The multiplier for determine resized width and height.  This value is multiplied against the `asset`'s `size` to determine the output size.
   - **filePrefix**: *Optional* value to add to the beginning of the output resized filename.
   - **fileSuffix**: *Optional* value to add to the end of the resized output filename.  This is useful for iOS images especially (e.g.: `fileSuffix: @2x` will cause the asset *happy.png* to be output as *happy@2x.png*.


## What's next?

**Automatically add generated assets to Xamarin.iOS and Xamarin.Android projects**

Currently Resizetizer does not automatically add resized assets to your app projects (e.g.: it will not add images inside a `drawable-hdpi` folder to your Xamarin.Android app project).  You will still have to manually add them as references in your projects the after first time they are generated.  One idea is to have this all automated for Xamarin.Android and Xamarin.iOS projects.

**PDF Asset support**

It's somewhat common for iOS developers to use PDF files in their projects.  It would be nice for Resizetizer to support this format... However it should.  More research needed.
