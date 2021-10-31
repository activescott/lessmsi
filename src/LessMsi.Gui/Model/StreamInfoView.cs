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
// Copyright (c) 2017 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
namespace LessMsi.Gui.Model
{
	/// <summary>
	/// Used to model a OLE structured storage stream in the UI.
	/// </summary>
	internal sealed class StreamInfoView
	{
		private readonly string _name;
		private readonly string _displayName;

		public static StreamInfoView FromStream(System.IO.Packaging.StreamInfo si)
		{
			return new StreamInfoView(si.Name, OleStorage.OleStorageFile.IsCabStream(si));
		}

		private StreamInfoView(string name, bool isCabStream)
		{
			_name = name;
			_displayName = OleStorage.OleStorageFile.DecodeName(name);
			IsCabStream = isCabStream;
		}

		public string Name
		{
			get { return _name; }
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public bool IsCabStream { get; private set; }

		public string Label
		{
			get { return this.IsCabStream ? this.DisplayName + " (CAB)" : this.DisplayName; }
		}
	}
}
