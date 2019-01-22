# OpenMAFF lets you open MAFF (Mozilla Archive Format) files that have been saved with FireFox

![](https://raw.githubusercontent.com/ChrisBertrandDotNet/OpenMAFF/master/Sources/OpenMAFF/OpenMAFF.ico)

## The MAFF format

MAFF (Mozilla Archive Format File) is a file format produced by several FireFox addons:

- [Web ScrapBook](https://addons.mozilla.org/en-US/firefox/addon/web-scrapbook/).
    
- [Maf](http://maf.mozdev.org/), the original, now abandoned.
    

Recent versions of FireFox do not allow addons to open these files.
That is why OpenMAFF has been made.

## OpenMAFF

OpenMAFF can open MAFF files, including files that contain several tabs, then send them to your Web browser.

OpenMAFF runs on Windows only.

## Installation and usage

1.  Download [the latest release on GitHub](https://github.com/ChrisBertrandDotNet/OpenMAFF/releases/latest).  
    The archive file name is `OpenMAFF.x.y.zip`
2.  Extract it in a directory.  
    Suggestion: `C:\Users\_<user name>_\AppData\Local\Programs`
3.  Associate the MAFF file type with OpenMAFF.
    1.  In the File Explorer, right-click on any MAFF file.
    2.  Choose menu "Properties".
    3.  In the panel, in the section "Opens with:", click on the button "Change".
    4.  Scroll down then click on "More apps ?".
    5.  Scroll down then click on "Look for another app on this PC".
    6.  Select `OpenMAFF.exe`, in the directory you extracted it to.
4.  Open the MAFF file.  
    That should silently run OpenMAFF which in turn will run your Web browser.

If no Web browser is associated with the HTML file type, Windows may let you choose one.

## How does it work ?

A MAFF file basically is a Zip archive that contains the actual files that were saved by a Web browser.  
If you want, you can do an experience: copy a MAFF file then rename its extension to Zip, then open the Zip file. You will see, or be able to extract, the files and directories the Zip/Maff file contains.

When `OpenMAFF.exe` runs, it extracts the files the MAFF file contains.  
These files are extracted to the temporary directory (usually `C:\Users\_<user name>_\AppData\Local\Temp\Maff__xxxxxxxxx_`).

Then there are two options:

*   If there is one tab/html only in the MAFF, OpenMAFF tells Windows to open the file `index.html`.  
    Windows will then open your Web browser.
    
*   If there are several tabs/html in the MAFF, OpenMAFF finds what application is your Web browser then runs it writing all the `index.html` files as parameters on the command line.  
    The resulting command line is similar to that:  
    ``FireFox.exe "c:\...\xxx\index.html" "c:\...\yyy\index.html" "c:\...\zzz\index.html"` ``

## Multi-tabs on Microsoft Edge

As of september 2018, Edge does not seem to be able to open several files at once.
Consequently, when you open a multi-tabs MAFF file, only the first tab is displayed by Edge.
A workaround is to associate the HTML file type with another Web browser (I suggest FireFox).

## Release notes

Version 1.5
2019-01-22
- New: A banner at the top of the web page lets you open the original page on its website.
- (programming)
  - Changed: source code is now  translated in English (from French).
  - Changed: the file "CB.Files.INI.cs" is now imported from the open source library "CB Helpers" (https://github.com/ChrisBertrandDotNet/CB-Helpers).

Version 1.4
2018-10-28
- Added: A "Settings.ini" file in the executable directory.
- Added: Automatically removes old temporary "Maff_*" directories at application start. (older than 10 days by default).

Version 1.3
2018-09-08
- Improved: multi-size icon.

Version 1.2
2018-09-06
- Improved: better icon.
- Added: Release notes.txt (as an embedded resource)

Version 1.1
2018-09-05
- Fixed: Can open index.html now.

Version 1.0
2018-09-04



