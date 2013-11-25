﻿// Permission is hereby granted, free of charge, to any person obtaining
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
// Copyright (c) 2010 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LessMsi.UI
{

	/// <summary>
	/// This is a simple MRU file manager for a <see cref="System.Windows.Forms.ToolStripMenuItem"/>.
	/// </summary>
	internal class MruMenuStripManager
	{
		private readonly List<MruItem> _items = new List<MruItem>();
		private readonly ToolStripMenuItem _placeHolderItem;

		/// <summary>
		/// Initializes a new instance of the MRU manager with the specified item to be used as a placeholder for where the MRU items will be placed.
		/// </summary>
		/// <param name="placeHolderItem">This should be a <see cref="ToolStripMenuItem"/> on the menu where you want the MRU items to be placed.</param>
		public MruMenuStripManager(ToolStripMenuItem placeHolderItem)
		{
			if (placeHolderItem == null)
				throw new ArgumentNullException("placeHolderItem");
			_placeHolderItem = placeHolderItem;	
		}

		private ToolStrip PlaceHolderOwner
		{
			get { return _placeHolderItem.Owner; }
		}

		private int PlaceHolderIndexInOwner
		{
			get { return _placeHolderItem.Owner.Items.IndexOf(_placeHolderItem); }
		}

		public void UsedFile(string filePath)
		{
			var item = _items.Find((compareItem) => string.Equals(compareItem.FilePathName, filePath, StringComparison.InvariantCultureIgnoreCase));
			if (item == null) {
				item = new MruItem(this, filePath);
				_items.Add(item);
				//since this might be the first item, make sure the placeholder is now invisible:
				this._placeHolderItem.Visible = false;
			}
			else {
				//remove it because below we are going to add it to the top again:
				item.Owner.Items.Remove(item);
			}
			PlaceHolderOwner.Items.Insert(PlaceHolderIndexInOwner + 1, item);
			// enumerate items and make sure that they have the proper number in front of their caption & proper keyboard shortcut
			FixupMenuItems();
		}

		private void FixupMenuItems()
		{
			var placeHolderIndex = PlaceHolderIndexInOwner;
			var shortcutKeyMap = new[] { Keys.Control | Keys.D0, Keys.Control | Keys.D1, Keys.Control | Keys.D2, Keys.Control | Keys.D3, Keys.Control | Keys.D4, Keys.Control | Keys.D5, Keys.Control | Keys.D6, Keys.Control | Keys.D7, Keys.Control | Keys.D8, Keys.Control | Keys.D9 };
			
			foreach (var item in this._items) {
				var itemIndex = item.Owner.Items.IndexOf(item);
				var number = itemIndex - placeHolderIndex;
				var text = item.Text;
				text = text.TrimStart('&', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ');
				text = '&' + number.ToString() + ' ' + text;
				item.Text = text;
				item.ShortcutKeys = shortcutKeyMap[number];
				item.ShowShortcutKeys = true;
			}
		}

		private void ToolStripItemClicked(MruItem item)
		{
			if (MruItemClicked != null)
				MruItemClicked(item.FilePathName);
		}

		public event MruItemClickedHandler MruItemClicked;

		public delegate void MruItemClickedHandler(string filePathNameClicked);

	    private sealed class MruItem : ToolStripMenuItem
		{
			public MruItem(MruMenuStripManager manager, string filePathName)
			{
				Manager = manager;
				FilePathName = filePathName;
				Text = filePathName;
			}

			protected override void OnClick(EventArgs e)
			{
				base.OnClick(e);
				Manager.ToolStripItemClicked(this);
			}

	        private MruMenuStripManager Manager { get; set; }
			public string FilePathName { get; private set; }
		}
	}
}
