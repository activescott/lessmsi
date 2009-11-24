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
namespace LessMsi.Msi
{
    /// <summary>
    /// FYI: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/msi/setup/column_definition_format.asp
    /// </summary>
    class ColumnInfo
    {
        public ColumnInfo(string name, string typeID)
        {
            this.Name = name;
            this.TypeID = typeID;
        }

        public string Name;

        /// <summary>
        /// s? 	String, variable length (?=1-255)
        /// s0 	String, variable length
        /// i2 	Short integer
        /// i4 	Long integer
        /// v0 	Binary Stream
        /// g? 	Temporary string (?=0-255)
        /// j? 	Temporary integer (?=0,1,2,4)
        /// An uppercase letter indicates that null values are allowed in the column.
        /// </summary>
        public string TypeID;


        public bool IsString
        {
            get
            {
                return
                    TypeID[0] == 's' || TypeID[0] == 'S'
                    || TypeID[0] == 'g' || TypeID[0] == 'G'
                    || TypeID[0] == 'l' || TypeID[0] == 'L';
            }
        }

        public bool IsInteger
        {
            get
            {
                return
                    TypeID[0] == 'i' || TypeID[0] == 'I'
                    || TypeID[0] == 'j' || TypeID[0] == 'J'
                    ;
            }
        }

        public bool IsStream
        {
            get
            {
                return
                    TypeID[0] == 'v' || TypeID[0] == 'V';
            }
        }

        public int Size
        {
            get
            {
                return int.Parse(TypeID.Substring(1));
            }
        }
    }
}