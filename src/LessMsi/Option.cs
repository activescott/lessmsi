namespace LessMsi
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public abstract class Option {
        readonly string _prototype;
        readonly string _description;
        readonly OptionValueType type;
        readonly int _count;

        protected Option (string prototype, string description, int maxValueCount = 1)
        {
            if (prototype == null)
                throw new ArgumentNullException ("prototype");
            if (prototype.Length == 0)
                throw new ArgumentException ("Cannot be the empty string.", "prototype");
            if (maxValueCount < 0)
                throw new ArgumentOutOfRangeException ("maxValueCount");

            _prototype   = prototype;
            Names       = prototype.Split ('|');
            _description = description;
            _count       = maxValueCount;
            type        = ParsePrototype ();

            if (_count == 0 && type != OptionValueType.None)
                throw new ArgumentException (
                    "Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
                    "OptionValueType.Optional.",
                    "maxValueCount");
            if (type == OptionValueType.None && maxValueCount > 1)
                throw new ArgumentException (
                    string.Format ("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
                    "maxValueCount");
            if (Array.IndexOf (Names, "<>") >= 0 && 
                ((Names.Length == 1 && type != OptionValueType.None) ||
                 (Names.Length > 1 && MaxValueCount > 1)))
                throw new ArgumentException (
                    "The default option handler '<>' cannot require values.",
                    "prototype");
        }

        private string           Prototype       {get {return _prototype;}}
        public string           Description     {get {return _description;}}
        public OptionValueType  OptionValueType {get {return type;}}
        public int              MaxValueCount   {get {return _count;}}

        public string[] GetNames ()
        {
            return (string[]) Names.Clone ();
        }

        public string[] GetValueSeparators ()
        {
            if (ValueSeparators == null)
                return new string [0];
            return (string[]) ValueSeparators.Clone ();
        }

        protected static T Parse<T> (string value, OptionContext c)
        {
            var conv = TypeDescriptor.GetConverter (typeof (T));
            var t = default (T);
            try {
                if (value != null)
                    t = (T) conv.ConvertFromString (value);
            }
            catch (Exception e) {
                throw new OptionException (
                    string.Format (
                        c.OptionSet.MessageLocalizer ("Could not convert string `{0}' to type {1} for option `{2}'."),
                        value, typeof (T).Name, c.OptionName),
                    c.OptionName, e);
            }
            return t;
        }

        internal string[] Names { get; private set; }
        internal string[] ValueSeparators { get; private set; }

        static readonly char[] NameTerminator = new[]{'=', ':'};

        private OptionValueType ParsePrototype ()
        {
            var type = '\0';
            var seps = new List<string> ();
            for (var i = 0; i < Names.Length; ++i) {
                var name = Names [i];
                if (name.Length == 0)
                    throw new ArgumentException ("Empty option names are not supported.", "prototype");

                var end = name.IndexOfAny (NameTerminator);
                if (end == -1)
                    continue;
                Names [i] = name.Substring (0, end);
                if (type == '\0' || type == name [end])
                    type = name [end];
                else 
                    throw new ArgumentException (
                        string.Format ("Conflicting option types: '{0}' vs. '{1}'.", type, name [end]),
                        "prototype");
                AddSeparators (name, end, seps);
            }

            if (type == '\0')
                return OptionValueType.None;

            if (_count <= 1 && seps.Count != 0)
                throw new ArgumentException (
                    string.Format ("Cannot provide key/value separators for Options taking {0} value(s).", _count),
                    "prototype");
            if (_count > 1) {
                if (seps.Count == 0)
                    ValueSeparators = new string[]{":", "="};
                else if (seps.Count == 1 && seps [0].Length == 0)
                    ValueSeparators = null;
                else
                    ValueSeparators = seps.ToArray ();
            }

            return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
        }

        private static void AddSeparators (string name, int end, ICollection<string> seps)
        {
            var start = -1;
            for (var i = end+1; i < name.Length; ++i) {
                switch (name [i]) {
                    case '{':
                        if (start != -1)
                            throw new ArgumentException (
                                string.Format ("Ill-formed name/value separator found in \"{0}\".", name),
                                "prototype");
                        start = i+1;
                        break;
                    case '}':
                        if (start == -1)
                            throw new ArgumentException (
                                string.Format ("Ill-formed name/value separator found in \"{0}\".", name),
                                "prototype");
                        seps.Add (name.Substring (start, i-start));
                        start = -1;
                        break;
                    default:
                        if (start == -1)
                            seps.Add (name [i].ToString ());
                        break;
                }
            }
            if (start != -1)
                throw new ArgumentException (
                    string.Format ("Ill-formed name/value separator found in \"{0}\".", name),
                    "prototype");
        }

        public void Invoke (OptionContext c)
        {
            OnParseComplete (c);
            c.OptionName  = null;
            c.Option      = null;
            c.OptionValues.Clear ();
        }

        protected abstract void OnParseComplete (OptionContext c);

        public override string ToString ()
        {
            return Prototype;
        }
    }
}