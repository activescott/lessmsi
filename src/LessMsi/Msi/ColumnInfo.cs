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

namespace LessMsi.Msi
{
    /// <summary>
    /// FYI: http://msdn.microsoft.com/library/default.asp?url=/library/en-us/msi/setup/column_definition_format.asp
    /// </summary>
    class ColumnInfo
    {
        public ColumnInfo(string name, string typeId)
        {
            Name = name;
            TypeId = typeId;
        }

        public readonly string Name;

        /// <summary>
        /// s? 	String, variable length (?=1-255)
        /// s0 	String, variable length
        /// i2 	Short integer
        /// i4 	Long integer
        /// v0 	Binary Stream
        /// g? 	Temporary string (?=0-255)
        /// j? 	Temporary integer (?=0,1,2,4)
        /// O0	Temporary object
        /// An uppercase letter indicates that null values are allowed in the column.
        /// </summary>
        public readonly string TypeId;


        public bool IsString
        {
            get
            {
                return
                    TypeId[0] == 's' || TypeId[0] == 'S'
                    || TypeId[0] == 'g' || TypeId[0] == 'G'
                    || TypeId[0] == 'l' || TypeId[0] == 'L';
            }
        }

        public bool IsInteger
        {
            get
            {
                return
                    TypeId[0] == 'i' || TypeId[0] == 'I'
                    || TypeId[0] == 'j' || TypeId[0] == 'J'
                    ;
            }
        }

        public bool IsStream
        {
            get
            {
                return
                    TypeId[0] == 'v' || TypeId[0] == 'V';
            }
        }

        public bool IsObject
        {
            get { return string.Equals("O0", TypeId, StringComparison.InvariantCultureIgnoreCase); }
        }

        public int Size
        {
            get
            {
                return int.Parse(TypeId.Substring(1));
            }
        }
    }
}