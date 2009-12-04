using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LessMsi.UI
{
    class WinFormsHelper
    {
        /// <summary>
        /// Allows a c# using statement to be used for the <see cref="ISupportInitialize"/> contract.
        /// </summary>
        public static IDisposable BeginUiUpdate(ISupportInitialize control)
        {
            control.BeginInit();
            return new ControlInitializationToken(control);
        }

        private struct ControlInitializationToken : IDisposable
        {
            private ISupportInitialize _control;

            public ControlInitializationToken(ISupportInitialize control)
            {
                _control = control;
            }

            public void Dispose()
            {
                _control.EndInit();
            }
        }
    }
}
