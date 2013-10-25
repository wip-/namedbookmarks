# Named Bookmarks #

Custom Visual Studio 2012 command to create automatically named bookmarks.
  
Use it from the Visual Studio 2012 Tools menu.  
Alternately, you can set a keyboard shortcut to call it (**not yet working when called from shortcut**).

Use at your own risk!

------------------------------------------

To use from a keyboard shortcut (**warning! command not yet working in that case**):

* Open the [Options Dialog Box][1].
* Select the **Environment/Keyboard** page
* In the **"Show commands containing:"** box, search for the **Tools.NamedBookmarks** command
* In the **"Press shortcut keys:"** box, type the shortcut you like (you can use CTRL+K, CTRL+K to replace the default bookmark shortcut).
* Be careful to select **Text Editor** in the "Use new shortcut in:" field.
* Press **OK**

------------------------------------------

Notes:

* I didn't implement the bookmark deletion feature, that usually happens when calling the default **ToggleBookmark** command on an already bookmarked line.
* I had to use a lot of **SendKeys.Send()** commands, as a result the add-in might have some strange behavior if not used in the Text Editor
* Installer available on http://visualstudiogallery.msdn.microsoft.com/d082caf9-6df1-4dc7-b239-bf7f01981361
* Code available on https://github.com/wip-/namedbookmarks  
* [From source code] To run in debug mode: in **Properties/Debug**, set Visual Studio 2012 (11.0) devenv.exe path in the **Start external program** box, and **/rootsuffix Exp** in the **Command line arguments** box.

[1]: http://msdn.microsoft.com/en-us/library/7f33da8d(v=vs.110).aspx 

