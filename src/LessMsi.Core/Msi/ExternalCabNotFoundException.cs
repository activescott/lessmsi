using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LessMsi.Msi
{
    /// <summary>
    /// Thrown when the msi file indicates there should be an external .cab file (i.e. not embedded inside the MSI, but saved along side it) but that cab file does not exist.
    /// </summary>
    public class ExternalCabNotFoundException : Exception
    {
        public static ExternalCabNotFoundException CreateFromCabPath(string cabFileName, string expectedLocation)
        {
            var msg = string.Format("This msi file references a CAB file that is not embedded inside of the msi file itself. The msi file is named {0} and was expected to be in the following folder: {1}", cabFileName, expectedLocation);
            return new ExternalCabNotFoundException(msg);
        }

        public ExternalCabNotFoundException()
        {
        }

        public ExternalCabNotFoundException(string message)
            : base(message)
        {
        }

        public ExternalCabNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
