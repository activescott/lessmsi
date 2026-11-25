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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LessMsi.Gui.Windows.Forms
{
    /// <summary>
    /// Applies a light or a dark color scheme to a WinForms control tree. WinForms has no built in
    /// theming, so this walks the control tree and colors each control type explicitly.
    /// </summary>
    internal static class DarkTheme
    {
        #region Palette

        public static readonly Color WindowBackground = Color.FromArgb(30, 30, 30);
        public static readonly Color SurfaceBackground = Color.FromArgb(37, 37, 38);
        public static readonly Color ControlBackground = Color.FromArgb(51, 51, 55);
        public static readonly Color AlternatingRowBackground = Color.FromArgb(42, 42, 45);
        public static readonly Color HeaderBackground = Color.FromArgb(45, 45, 48);
        public static readonly Color BorderColor = Color.FromArgb(63, 63, 70);
        public static readonly Color Foreground = Color.FromArgb(240, 240, 240);
        public static readonly Color MutedForeground = Color.FromArgb(170, 170, 170);
        public static readonly Color SelectionBackground = Color.FromArgb(0, 95, 184);
        public static readonly Color AccentColor = Color.FromArgb(0, 122, 204);
        public static readonly Color MenuHover = Color.FromArgb(62, 62, 66);

        #endregion

        /// <summary>
        /// True while the dark scheme is active. Dialogs read this so they can match the main window.
        /// </summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>
        /// Controls (tab controls and menu strips) that we already attached handlers to, so repeated
        /// calls to <see cref="Apply"/> do not attach them twice.
        /// </summary>
        private static readonly HashSet<Control> HookedControls = new HashSet<Control>();

        /// <summary>
        /// Asks the DWM to draw the window title bar dark. Ignored on Windows versions that do not
        /// support the attribute, which simply leaves the title bar as the user configured it.
        /// </summary>
        public static void ApplyTitleBar(Form form, bool dark)
        {
            if (form == null || !form.IsHandleCreated)
                return;

            const int DwmwaUseImmersiveDarkMode = 20;
            const int DwmwaUseImmersiveDarkModeBefore20H1 = 19;
            var enabled = dark ? 1 : 0;
            if (DwmSetWindowAttribute(form.Handle, DwmwaUseImmersiveDarkMode, ref enabled, sizeof(int)) != 0)
                DwmSetWindowAttribute(form.Handle, DwmwaUseImmersiveDarkModeBefore20H1, ref enabled, sizeof(int));

            // Repaint the non client area so the change is visible immediately.
            form.Invalidate();
            form.Update();
        }

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        /// <summary>
        /// Applies the requested scheme to <paramref name="root"/> and all of its children.
        /// </summary>
        public static void Apply(Control root, bool dark)
        {
            if (root == null)
                return;

            IsEnabled = dark;
            ApplyTo(root, dark);
        }

        private static void ApplyTo(Control control, bool dark)
        {
            var tabPage = control as TabPage;
            if (tabPage != null)
            {
                // Tab pages need their color set before other styles so children inherit it.
                tabPage.BackColor = dark ? SurfaceBackground : SystemColors.Control;
                tabPage.ForeColor = dark ? Foreground : SystemColors.ControlText;
            }

            var dataGrid = control as DataGridView;
            if (dataGrid != null)
                StyleDataGridView(dataGrid, dark);

            var menuStrip = control as MenuStrip;
            if (menuStrip != null)
                StyleMenuStrip(menuStrip, dark);

            var statusStrip = control as StatusStrip;
            if (statusStrip != null)
                StyleStatusStrip(statusStrip, dark);

            var tabControl = control as TabControl;
            if (tabControl != null)
                StyleTabControl(tabControl, dark);

            if (!(control is DataGridView))
            {
                // DataGridView cell styles above are enough; everything else gets flat colors.
                if (control is TextBox || control is ComboBox || control is ListBox)
                {
                    if (dark)
                    {
                        control.BackColor = ControlBackground;
                        control.ForeColor = Foreground;
                    }
                    else
                    {
                        control.BackColor = SystemColors.Window;
                        control.ForeColor = SystemColors.WindowText;
                    }

                    var comboBox = control as ComboBox;
                    if (comboBox != null)
                        comboBox.FlatStyle = dark ? FlatStyle.Flat : FlatStyle.Standard;
                }
                else if (control is Button)
                {
                    StyleButton((Button)control, dark);
                }
                else if (control is TableLayoutPanel || control is Panel || control is GroupBox ||
                         control is StatusStrip || control is TabControl)
                {
                    control.BackColor = dark ? SurfaceBackground : SystemColors.Control;
                    control.ForeColor = dark ? Foreground : SystemColors.ControlText;
                }
                else if (control is Label || control is LinkLabel || control is CheckBox || control is RadioButton)
                {
                    control.ForeColor = dark ? Foreground : SystemColors.ControlText;
                    control.BackColor = Color.Transparent;
                }
            }

            if (control is Form || control is GroupBox || control is TableLayoutPanel || control is Panel)
            {
                // Forms paint their own background; containers stay transparent to the parent.
                if (control is Form)
                {
                    control.BackColor = dark ? WindowBackground : SystemColors.Control;
                    control.ForeColor = dark ? Foreground : SystemColors.ControlText;
                }
            }

            foreach (Control child in control.Controls)
                ApplyTo(child, dark);
        }

        private static void StyleDataGridView(DataGridView grid, bool dark)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.BorderStyle = BorderStyle.FixedSingle;

            if (dark)
            {
                grid.BackgroundColor = SurfaceBackground;
                grid.GridColor = BorderColor;

                grid.DefaultCellStyle.BackColor = SurfaceBackground;
                grid.DefaultCellStyle.ForeColor = Foreground;
                grid.DefaultCellStyle.SelectionBackColor = SelectionBackground;
                grid.DefaultCellStyle.SelectionForeColor = Foreground;

                grid.AlternatingRowsDefaultCellStyle.BackColor = AlternatingRowBackground;
                grid.AlternatingRowsDefaultCellStyle.ForeColor = Foreground;
                grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = SelectionBackground;
                grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = Foreground;

                grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderBackground;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Foreground;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = HeaderBackground;
                grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Foreground;

                grid.RowHeadersDefaultCellStyle.BackColor = HeaderBackground;
                grid.RowHeadersDefaultCellStyle.ForeColor = Foreground;
            }
            else
            {
                grid.BackgroundColor = SystemColors.AppWorkspace;
                grid.GridColor = SystemColors.ControlDark;

                grid.DefaultCellStyle.BackColor = SystemColors.Window;
                grid.DefaultCellStyle.ForeColor = SystemColors.WindowText;
                grid.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                grid.DefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
                grid.AlternatingRowsDefaultCellStyle.ForeColor = SystemColors.WindowText;
                grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
                grid.AlternatingRowsDefaultCellStyle.SelectionForeColor = SystemColors.HighlightText;

                grid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = SystemColors.Control;
                grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.ControlText;

                grid.RowHeadersDefaultCellStyle.BackColor = SystemColors.Control;
                grid.RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            }
        }

        private static void StyleButton(Button button, bool dark)
        {
            if (dark)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = BorderColor;
                button.FlatAppearance.MouseOverBackColor = MenuHover;
                button.FlatAppearance.MouseDownBackColor = SelectionBackground;
                button.BackColor = ControlBackground;
                button.ForeColor = Foreground;
                button.UseVisualStyleBackColor = false;
            }
            else
            {
                button.FlatStyle = FlatStyle.System;
                button.BackColor = SystemColors.Control;
                button.ForeColor = SystemColors.ControlText;
                button.UseVisualStyleBackColor = true;
            }
        }

        private static void StyleMenuStrip(MenuStrip menu, bool dark)
        {
            if (dark)
            {
                menu.Renderer = new ToolStripProfessionalRenderer(new DarkColorTable()) { RoundedEdges = false };
                menu.BackColor = WindowBackground;
                menu.ForeColor = Foreground;
            }
            else
            {
                menu.Renderer = null; // restores the inherited (light) renderer
                menu.BackColor = SystemColors.Control;
                menu.ForeColor = SystemColors.ControlText;
            }

            foreach (ToolStripItem item in menu.Items)
                StyleMenuItem(item, dark);
        }

        private static void StyleMenuItem(ToolStripItem item, bool dark)
        {
            item.ForeColor = dark ? Foreground : SystemColors.ControlText;
            var dropDownItem = item as ToolStripMenuItem;
            if (dropDownItem == null)
                return;
            foreach (ToolStripItem child in dropDownItem.DropDownItems)
                StyleMenuItem(child, dark);
        }

        private static void StyleStatusStrip(StatusStrip strip, bool dark)
        {
            strip.BackColor = dark ? WindowBackground : SystemColors.Control;
            strip.ForeColor = dark ? MutedForeground : SystemColors.ControlText;
            foreach (ToolStripItem item in strip.Items)
            {
                item.ForeColor = dark ? MutedForeground : SystemColors.ControlText;
                item.BackColor = dark ? WindowBackground : SystemColors.Control;
            }
        }

        private static void StyleTabControl(TabControl tabControl, bool dark)
        {
            if (dark)
            {
                tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
                tabControl.Padding = new Point(12, 5);
                if (HookedControls.Add(tabControl))
                    tabControl.DrawItem += DrawDarkTab;
            }
            else
            {
                if (HookedControls.Remove(tabControl))
                    tabControl.DrawItem -= DrawDarkTab;
                tabControl.DrawMode = TabDrawMode.Normal;
                tabControl.Padding = new Point(6, 3);
            }
        }

        private static void DrawDarkTab(object sender, DrawItemEventArgs e)
        {
            var tabControl = (TabControl)sender;
            if (e.Index < 0 || e.Index >= tabControl.TabPages.Count)
                return;

            var page = tabControl.TabPages[e.Index];
            var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var background = isSelected ? ControlBackground : WindowBackground;

            using (var brush = new SolidBrush(background))
                e.Graphics.FillRectangle(brush, e.Bounds);

            using (var pen = new Pen(isSelected ? AccentColor : BorderColor))
                e.Graphics.DrawRectangle(pen, e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1);

            var textColor = isSelected ? Foreground : MutedForeground;
            TextRenderer.DrawText(e.Graphics, page.Text, e.Font, e.Bounds, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            // The system paints the remainder of the tab strip, which would stay light. Fill it here.
            if (e.Index == tabControl.TabPages.Count - 1)
            {
                var stripStart = e.Bounds.Right;
                var stripWidth = tabControl.ClientRectangle.Width - stripStart;
                if (stripWidth > 0)
                {
                    var stripRect = new Rectangle(stripStart, e.Bounds.Top, stripWidth, e.Bounds.Height);
                    using (var brush = new SolidBrush(WindowBackground))
                        e.Graphics.FillRectangle(brush, stripRect);
                }
            }
        }

        /// <summary>
        /// Color table used to render drop down menus in the dark scheme.
        /// </summary>
        private sealed class DarkColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground { get { return SurfaceBackground; } }
            public override Color ImageMarginGradientBegin { get { return SurfaceBackground; } }
            public override Color ImageMarginGradientMiddle { get { return SurfaceBackground; } }
            public override Color ImageMarginGradientEnd { get { return SurfaceBackground; } }
            public override Color MenuBorder { get { return BorderColor; } }
            public override Color MenuItemBorder { get { return SelectionBackground; } }
            public override Color MenuItemSelected { get { return MenuHover; } }
            public override Color MenuItemSelectedGradientBegin { get { return MenuHover; } }
            public override Color MenuItemSelectedGradientEnd { get { return MenuHover; } }
            public override Color MenuItemPressedGradientBegin { get { return ControlBackground; } }
            public override Color MenuItemPressedGradientEnd { get { return ControlBackground; } }
            public override Color MenuStripGradientBegin { get { return WindowBackground; } }
            public override Color MenuStripGradientEnd { get { return WindowBackground; } }
            public override Color SeparatorDark { get { return BorderColor; } }
            public override Color SeparatorLight { get { return BorderColor; } }
            public override Color CheckBackground { get { return SelectionBackground; } }
            public override Color CheckSelectedBackground { get { return SelectionBackground; } }
            public override Color CheckPressedBackground { get { return SelectionBackground; } }
        }
    }
}
