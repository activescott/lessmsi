namespace LessMsi
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class OptionException : Exception {
        public OptionException ()
        {
        }

        public OptionException (string message, string optionName)
            : base (message)
        {
            OptionName = optionName;
        }

        public OptionException (string message, string optionName, Exception innerException)
            : base (message, innerException)
        {
            OptionName = optionName;
        }

        protected OptionException (SerializationInfo info, StreamingContext context)
            : base (info, context)
        {
            OptionName = info.GetString ("OptionName");
        }

        private string OptionName { get; set; }

        [SecurityPermission (SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData (SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData (info, context);
            info.AddValue ("OptionName", OptionName);
        }
    }
}