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
// Copyright (c) 2010 Scott Willeke (http://scott.willeke.com)
//
// Authors:
//	Scott Willeke (scott@willeke.com)
//
using LessMsi.Gui.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;

namespace LessMsi.Gui
{

    /// <summary>
    /// This is a simple MRU file manager for a <see cref="System.Windows.Forms.ToolStripMenuItem"/>.
    /// </summary>
    internal class MruMenuStripManager
    {
        private readonly List<MruItem> _items = new List<MruItem>();
        private readonly ToolStripMenuItem _placeHolderItem;
        private readonly Keys[] _shortcutKeyMap =
        {
            Keys.Control | Keys.D0,
            Keys.Control | Keys.D1,
            Keys.Control | Keys.D2,
            Keys.Control | Keys.D3,
            Keys.Control | Keys.D4,
            Keys.Control | Keys.D5,
            Keys.Control | Keys.D6,
            Keys.Control | Keys.D7,
            Keys.Control | Keys.D8,
            Keys.Control | Keys.D9
        };

        /// <summary>
        /// Initializes a new instance of the MRU manager with the specified item to be used as a placeholder for where the MRU items will be placed.
        /// </summary>
        /// <param name="placeHolderItem">This should be a <see cref="ToolStripMenuItem"/> on the menu where you want the MRU items to be placed.</param>
        public MruMenuStripManager(ToolStripMenuItem placeHolderItem)
        {
            if (placeHolderItem == null)
                throw new ArgumentNullException("placeHolderItem");
            _placeHolderItem = placeHolderItem;
            LoadPreferences();
        }

        private void LoadPreferences()
        {
            var recentFiles = ApplicationSettings.Default.RecentFiles;
            if (recentFiles == null)
                return;
            var paths = new string[recentFiles.Count];
            recentFiles.CopyTo(paths, 0);

            _items.Clear();
            _items.AddRange(paths.Select(path => new MruItem(this, path)));
            for (var index = 0; index < _items.Count; index++)
            {
                // insert each one begining right after the placeholder
                PlaceHolderOwner.Items.Insert(PlaceHolderIndexInOwner + index + 1, _items[index]);
            }
            OnItemsChanged();
        }

        public void SavePreferences()
        {
            PruneItems();
			ApplicationSettings.Default.RecentFiles.Clear();
            _items.ForEach((item) => ApplicationSettings.Default.RecentFiles.Add(item.FilePathName));
			ApplicationSettings.Default.Save();
        }

        /// <summary>
        /// The collection of menu items changed.
        /// </summary>
        private void OnItemsChanged()
        {
            PruneItems();
            // enumerate items and make sure that they have the proper number in front of their caption & proper keyboard shortcut
            FixupMenuItems();
            //since this might be the first item, make sure the placeholder is now invisible:
            this._placeHolderItem.Visible = PlaceHolderOwner.Items.Count == 1;
        }

        private ToolStrip PlaceHolderOwner
        {
            get { return _placeHolderItem.Owner; }
        }

        private int PlaceHolderIndexInOwner
        {
            get { return _placeHolderItem.Owner.Items.IndexOf(_placeHolderItem); }
        }

        private int MaxItems => _shortcutKeyMap.Length - 1;

        public void UsedFile(string filePath)
        {
            var item = _items.Find((compareItem) => string.Equals(compareItem.FilePathName, filePath, StringComparison.InvariantCultureIgnoreCase));
            if (item == null)
            {
                item = new MruItem(this, filePath);
            }
            else
            {
                //remove it because below we are going to add it to the top again:
                item.Owner.Items.Remove(item);
                _items.Remove(item);
            }
            _items.Insert(0, item);
            PlaceHolderOwner.Items.Insert(PlaceHolderIndexInOwner + 1, item);
            OnItemsChanged();
        }

        private void FixupMenuItems()
        {
            int placeHolderIndex = PlaceHolderIndexInOwner;

            foreach (MruItem item in this._items)
            {
                var itemIndex = item.Owner.Items.IndexOf(item);
                var number = itemIndex - placeHolderIndex;
                var text = item.Text;
                text = text.TrimStart('&', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ');
                text = '&' + number.ToString() + ' ' + text;
                item.Text = text;
                item.ShortcutKeys = _shortcutKeyMap[number];
                item.ShowShortcutKeys = true;
            }
        }

        private void PruneItems()
        {
            for (var i = _items.Count - 1; i >= MaxItems; i--)
            {
                PlaceHolderOwner.Items.Remove(_items[i]);
                _items.RemoveAt(i);
            }
        }

        private void ToolStripItemClicked(MruItem item)
        {
            if (MruItemClicked != null)
                MruItemClicked(item.FilePathName);
        }

        public event MruItemClickedHandler MruItemClicked;

        public delegate void MruItemClickedHandler(string filePathNameClicked);

        public sealed class MruItem : ToolStripMenuItem
        {
            public MruItem(MruMenuStripManager manager, string filePathName)
            {
                this.Manager = manager;
                this.FilePathName = filePathName;
                this.Text = filePathName;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                Manager.ToolStripItemClicked(this);
            }

            public MruMenuStripManager Manager { get; set; }
            public string FilePathName { get; set; }
        }
    }
}
