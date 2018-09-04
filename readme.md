# OpenMAFF lets you open MAFF (Mozilla Archive Format) files that have been saved with FireFox

## The MAFF format

MAFF (Mozilla Archive Format File) is a file format produced by several FireFox addons:

*   [Maf](http://maf.mozdev.org/), the original, now abandoned.
    
*   [Web ScrapBook](https://addons.mozilla.org/en-US/firefox/addon/web-scrapbook/).
    

The problem is new versions of FireFox do not allow addons to open these files.

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
