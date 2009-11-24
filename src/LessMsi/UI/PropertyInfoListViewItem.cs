// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using System;
using System.Collections;
using System.Windows.Forms;
using LessMsi.Msi;
using Microsoft.Tools.WindowsInstallerXml.Msi;

namespace LessMsi.UI
{
    internal class PropertyInfoListViewItem : ListViewItem//TODO: This should not be ListViewItem!
    {
        private string _name;
        private int _pid;
        private VT _propertyType;
        private string _description;
        private object _value;

        #region msdn doc

        //http://msdn.microsoft.com/library/default.asp?url=/library/en-us/msi/setup/summary_information_stream_property_set.asp
        //Codepage 	PID_CODEPAGE 	1 	VT_I2
        //Title 	PID_TITLE 	2 	VT_LPSTR
        //Subject 	PID_SUBJECT 	3 	VT_LPSTR
        //Author 	PID_AUTHOR 	4 	VT_LPSTR
        //Keywords 	PID_KEYWORDS 	5 	VT_LPSTR
        //Comments 	PID_COMMENTS 	6 	VT_LPSTR
        //Template 	PID_TEMPLATE 	7 	VT_LPSTR
        //Last Saved By 	PID_LASTAUTHOR 	8 	VT_LPSTR
        //Revision Number 	PID_REVNUMBER 	9 	VT_LPSTR
        //Last Printed 	PID_LASTPRINTED 	11 	VT_FILETIME
        //Create Time/Date 	PID_CREATE_DTM 	12 	VT_FILETIME
        //Last Save Time/Date 	PID_LASTSAVE_DTM 	13 	VT_FILETIME
        //Page Count 	PID_PAGECOUNT 	14 	VT_I4
        //Word Count 	PID_WORDCOUNT 	15 	VT_I4
        //Character Count 	PID_CHARCOUNT 	16 	VT_I4
        //Creating Application 	PID_APPNAME 	18 	VT_LPSTR
        //Security 	PID_SECURITY 	19 	VT_I4

        #endregion

        private static readonly PropertyInfoListViewItem[] DefaultPropertySet = new PropertyInfoListViewItem[]
                                                                                    {
                                                                                        new PropertyInfoListViewItem("Codepage", 1, VT.I2, "The ANSI code page used for any strings that are stored in the summary information. Note that this is not the same code page for strings in the installation database. The Codepage Summary property is used to translate the strings in the summary information into Unicode when calling the Unicode API functions."),
                                                                                        new PropertyInfoListViewItem("Title", 2, VT.LPSTR, "Breifly describes the installer."),
                                                                                        new PropertyInfoListViewItem("Subject", 3, VT.LPSTR, "Describes what can be installed using the installer."),
                                                                                        new PropertyInfoListViewItem("Author", 4, VT.LPSTR, "The manufacturer of the installer"),
                                                                                        new PropertyInfoListViewItem("Keywords", 5, VT.LPSTR, "Keywords that permit the database file to be found in a keyword search"),
                                                                                        new PropertyInfoListViewItem("Comments", 6, VT.LPSTR, "Used to describe the general purpose of the installer."),
                                                                                        new PropertyInfoListViewItem("Template", 7, VT.LPSTR, "The platform and languages supported by the installer (syntax:[platform property][,platform property][,...];[language id][,language id][,...].)."),
                                                                                        new PropertyInfoListViewItem("Last Saved By ", 8, VT.LPSTR, "The installer sets the Last Saved by Summary Property to the value of the LogonUser property during an administrative installation."),
                                                                                        new PropertyInfoListViewItem("Revision Number ", 9, VT.LPSTR, "Unique identifier of the installer package. In patch packages authored for Windows Installer version 2.0 this can be followed by a list of patch code GUIDs for obsolete patches that are removed when this patch is applied. The patch codes are concatenated with no delimiters separating GUIDs in the list. Windows Installer version 3.0 can install these earlier package versions and remove the obsolete patches. Windows Installer version 3.0 ignores the list of obsolete patches in the Revision Number Summary property if there is sequencing information present in the MsiPatchSequence table."),
                                                                                        new PropertyInfoListViewItem("Last Printed", 11, VT.FILETIME, "The date and time during an administrative installation to record when the administrative image was created. For non-administrative installations this property is the same as the Create Time/Date Summary property."),
                                                                                        new PropertyInfoListViewItem("Create Time/Date", 12, VT.FILETIME, "When the installer database was created."),
                                                                                        new PropertyInfoListViewItem("Last Save Time/Date", 13, VT.FILETIME, "When the last time the installer database was modified. Each time a user changes an installation the value for this summary property is updated to the current system time/date at the time the installer database was saved. Initially the value for this summary property is set to null to indicate that no changes have yet been made."),
                                                                                        new PropertyInfoListViewItem("Page Count", 14, VT.I4, "The minimum installer version required. \r\nFor Windows Installer version 1.0, this property must be set to the integer 100. For 64-bit Windows Installer Packages, this property must be set to the integer 200. For a transform package, the Page Count Summary property contains minimum installer version required to process the transform. Set to the greater of the two Page Count Summary property values belonging to the databases used to generate the transform. Set to Null in patch packages.", "0x{0:x}"),
                                                                                        new PropertyInfoListViewItem("Word Count", 15, VT.I4, "Indicates the type of source file image.", "0x{0:x}"),
                                                                                        new PropertyInfoListViewItem("Character Count", 16, VT.I4, "Two 16-bit words. The upper word contains the \"transform validation flags\" (used to verify that a transform can be applied to the database). The lower word contains the \"transform error condition flags\" (used to flag the error conditions of a transform)."),
                                                                                        new PropertyInfoListViewItem("Creating Application", 18, VT.LPSTR, "The software used to author the installation."),
                                                                                        new PropertyInfoListViewItem("Security", 19, VT.I4, "Indicates if the package should be opened as read-only:\r\n 0: No restriction \r\n2:Read-only recommended\r\n 4: Read-only enforced")
                                                                                    };

        private string _valueFormatString = "{0}";

        public static PropertyInfoListViewItem[] GetPropertiesFromDatabase(Database msidb)
        {
            int[] standardIDs = new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 18, 19};

            ArrayList properties = new ArrayList();
            using (SummaryInformation summaryInfo = new SummaryInformation(msidb))
            {
                foreach (int propID in standardIDs)
                {
                    bool failed = false;
                    object propValue = null;
                    try
                    {
                        propValue = summaryInfo.GetProperty(propID);
                    }
                    catch
                    {
                        failed = true;
                    }
                    if (!failed)
                        properties.Add(new PropertyInfoListViewItem(propID, propValue));
                }
            }
            return (PropertyInfoListViewItem[]) properties.ToArray(typeof (PropertyInfoListViewItem));
        }

        public static void InitListViewColumns(ListView lv)
        {
            lv.Columns.Add("Name", 200, HorizontalAlignment.Left);
            lv.Columns.Add("Value", 225, HorizontalAlignment.Left);
            lv.Columns.Add("ID", 60, HorizontalAlignment.Left);
            lv.Columns.Add("Type", 60, HorizontalAlignment.Left);
            lv.AllowColumnReorder = true;
        }

        private PropertyInfoListViewItem(int id, object value)
        {
            _pid = id;
            _value = value;

            PropertyInfoListViewItem prototype = GetPropertyInfoByID(id);
            if (prototype != null)
            {
                _name = prototype.Name;
                _propertyType = prototype._propertyType;
                _description = prototype.Description;
                _valueFormatString = prototype._valueFormatString;
                switch(_propertyType)
                {
                    case VT.FILETIME:
                        //everything is coming from wix as a string, need to submit patch to wix:
                        // _value = DateTime.FromFileTime((long)_value);
                        break;
                    case VT.I2:
                    case VT.I4:
                        if (_value is string && _value != null && ((string)_value).Length > 0)
                        {
                            try
                            {
                                _value = Int32.Parse((string)_value);
                            }
                            catch (FormatException )
                            {}
                        }
                        break;
                }
					
            }
            else
            {
                _name = "non-standard";
                _propertyType = VT.EMPTY;
                _description = "Unknown.";
            }
            InitListViewItem();
        }

        private PropertyInfoListViewItem(string name, int pid, VT propertyType, string description)
            : this(name, pid, propertyType, description, "{0}")
        {
        }

        private PropertyInfoListViewItem(string name, int pid, VT propertyType, string description, string valueFormatString)
        {
            _name = name;
            _pid = pid;
            _propertyType = propertyType;
            _description = description;
            _value = null;
            _valueFormatString = valueFormatString;
            InitListViewItem();
				
        }

        private void InitListViewItem()
        {
            this.Text = _name;
            this.SubItems.AddRange(new string[] {ValueString, Convert.ToString(_pid), Convert.ToString(_propertyType)});
        }

        /// <summary>
        /// Returns a <see cref="PropertyInfo"/> with the specified <see cref="PropertyInfo.ID"/> or null if the ID is unknown.
        /// </summary>
        public static PropertyInfoListViewItem GetPropertyInfoByID(int id)
        {
            foreach (PropertyInfoListViewItem info in DefaultPropertySet)
            {
                if (info.ID == id)
                    return info;
            }
            return null;
        }

        public string Name
        {
            get { return _name; }
        }

        public int ID
        {
            get { return _pid; }
        }

        public string Description
        {
            get { return _description; }
        }

        public object Value
        {
            get { return _value; }
        }

        public string ValueFormatString
        {
            get { return _valueFormatString; }
        }

        public string ValueString
        {
            get
            {
                return String.Format(this.ValueFormatString, this.Value);
            }
        }

    }

    internal enum VT : uint
    {
        EMPTY = 0,
        NULL = 1,
        I2 = 2,
        I4 = 3,
        LPSTR = 30,
        FILETIME = 0x40,
    }
}