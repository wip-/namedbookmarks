﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;


namespace Wil.NamedBookmarks
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidNamedBookmarksPkgString)]
    public sealed class NamedBookmarksPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public NamedBookmarksPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidNamedBookmarksCmdSet, (int)PkgCmdIDList.cmdidNamedBookmark);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }
        #endregion

        /// <summary>
        /// Special characters for the SendKeys.Send command (excluding curly braces, a special case)
        /// </summary>
        private static readonly string[] SpecialStrings = new string[] { "+", "^", "%", "~", "(", ")", "[", "]"};

        /// <summary>
        /// Replace special characters for the SendKeys.Send command
        /// </summary>
        private string ReplaceSpecialCharacters(string str)
        {
            // replace special characters
            foreach (string special in SpecialStrings)
            {
                str = str.Replace(special, "{" + special + "}");
            }

            // store closing curly braces locations (as "%")
            str = str.Replace("}", "%");

            // replace opening curly braces
            str = str.Replace("{", "{{}");

            // replace closing curly braces (stored as "%")
            str = str.Replace("%", "{}}");

            return str;
        }


        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            DTE2 _applicationObject = (DTE2)GetService(typeof(DTE));

            TextSelection selection = _applicationObject.ActiveDocument.Selection as TextSelection;
            Window currentWindow = _applicationObject.ActiveWindow;

            if (null != selection)
            {
                bool keepEditMode = false;


                // Get caption
                string caption = selection.Text;
                if (caption == "")
                {
                    // get current line content
                    selection.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
                    selection.EndOfLine(true);
                    selection = _applicationObject.ActiveDocument.Selection as TextSelection;
                    caption = selection.Text;
                    keepEditMode = true;
                }
                caption = ReplaceSpecialCharacters(caption);


                // Get window
                _applicationObject.ExecuteCommand("View.BookmarkWindow");
                Window bookmarkWindow = null;
                foreach(Window window in _applicationObject.Windows)
                {
                    if(window.Caption == "Bookmarks")
                    {
                        bookmarkWindow = window;
                        break;
                    }
                }


                // Set bookmark
                selection.SetBookmark();
                //_applicationObject.ExecuteCommand("Format.InsertBookmark");
                

                // Name bookmark
                if (caption != "")
                {
                    // enter bookmark caption edit mode
                    bookmarkWindow.Activate();
                    _applicationObject.ExecuteCommand("OtherContextMenus.BookmarkWindow.Rename");

                    SendKeys.Send(caption);     // hate to use SendKeys.Send().. but BookmarkWindow.Rename does not accept parameters
                    SendKeys.Send("{ENTER}");

                    _applicationObject.ExecuteCommand("File.SaveAll");

                    if (keepEditMode)
                    {
                        bookmarkWindow.Activate();
                        SendKeys.Send("{F2}");  // hate to use SendKeys.Send().. but BookmarkWindow.Rename does not work in that case
                    }
                    else
                    {
                        //currentWindow.Activate();
                        SendKeys.Send("^{TAB}");    // hate to use SendKeys.Send().. but currentWindow.Activate() breaks bookmarks name edition
                        // TODO test SendKeys.SendWait()
                    }
                }               
            }

            // other ideas : 
            // http://stackoverflow.com/questions/54052/tool-to-view-the-contents-of-the-solution-user-options-file-suo
            // http://msdn.microsoft.com/en-us/library/vstudio/163ba701.aspx
        }

    }
}