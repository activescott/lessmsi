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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LessMsi.UI
{
	internal class CommandLink : Button
	{
		public CommandLink()
		{
			this.FlatStyle = FlatStyle.System;
			_textNote = "noteText";
		}
		
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style |= NativeMethods.BS_COMMANDLINK;
				return (cp);
			}
		}

		private string _textNote="";
		public string TextNote
		{
			get { return _textNote; }
			set
			{
				_textNote = value;
				if (this.IsHandleCreated)
					NativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.BCM_SETNOTE, IntPtr.Zero, _textNote);
			}
		}

		bool _showElevationShield;
		public bool ShowElevationShield
		{
			get
			{
				return _showElevationShield;
			}
			set
			{
				_showElevationShield = value;
				NativeMethods.Button_SetElevationRequiredState(this, _showElevationShield);
			}
		}
	}
}
