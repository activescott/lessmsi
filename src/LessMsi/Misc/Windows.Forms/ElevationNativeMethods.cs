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

namespace LessMsi.Misc.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class NativeMethods
	{
        public const int BS_COMMANDLINK = 0x0000000E;
		public const int BCM_SETNOTE = 0x00001609;
		const int BCM_FIRST = 0x1600;
        private const int BCM_SETSHIELD = (BCM_FIRST + 0x000C);

        /// <summary>
        /// Sets the elevation required state for a specified button or command link to display an elevated icon.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="show"></param>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/bb761865(VS.85).aspx
        /// 
        /// #define BCM_FIRST               0x1600      // Button control messages
        /// // Macro to use on a button or command link to display an elevated icon
        /// #define BCM_SETSHIELD            (BCM_FIRST + 0x000C)
        /// #define Button_SetElevationRequiredState(hwnd, fRequired) \
        /// (LRESULT)SNDMSG((hwnd), BCM_SETSHIELD, 0, (LPARAM)fRequired)
        /// </remarks>
        public static void Button_SetElevationRequiredState(ButtonBase button, bool show)
		{
			if (button == null)
				throw new ArgumentNullException("button");

			button.FlatStyle = FlatStyle.System;

			var buttonHandle = new HandleRef(button, button.Handle);
			Trace.WriteLine("handle:" + buttonHandle.Handle);

			var lParam = new IntPtr(Int32.MaxValue - 1);
			var ret = SendMessage(buttonHandle, BCM_SETSHIELD, show ? new IntPtr(1) : IntPtr.Zero, ref lParam);
			Trace.WriteLine(ret.ToInt64());
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		private static extern IntPtr SendMessage(HandleRef hWnd, Int32 Msg, IntPtr wParam, ref IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		public static extern IntPtr SendMessage(HandleRef hWnd, Int32 Msg, IntPtr wParam, string lParam);
	}
}
