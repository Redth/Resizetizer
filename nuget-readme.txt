Resizetizer Installation
--------------------------------

1. Create a `resizetizer.yaml` configuration file in your project.  Refer to the example below.
2. Set the build action of your `resizetizer.yaml` file to `ResizetizerConfig`.  You might need to reload your Solution to see this build action type.


Sample resizetizer.yaml file (For more information visit: http://github.com/Redth/Resizetizer):
--------------------------------

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
