using System;

namespace LessMsi.Tests
{
    public struct CompareEntriesResult : IEquatable<CompareEntriesResult>
    {
        public bool AreEntriesEqual { get; private set; }
        public string ErrorMessage { get; private set; }

        public CompareEntriesResult(bool areEntriesEqual, string errorMessage)
        {
            AreEntriesEqual = areEntriesEqual;
            ErrorMessage = errorMessage;
        }

        public bool Equals(CompareEntriesResult other)
        {
            return other.AreEntriesEqual == this.AreEntriesEqual && String.Compare(other.ErrorMessage, this.ErrorMessage, StringComparison.InvariantCulture) == 0;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is CompareEntriesResult))
            {
                return false;
            }

            return this.Equals((CompareEntriesResult)obj);
        }

        public override int GetHashCode()
        {
            var code = this.AreEntriesEqual.GetHashCode();
            code ^= this.ErrorMessage?.GetHashCode() ?? 0;
            return code;
        }

        public static bool operator ==(CompareEntriesResult a, CompareEntriesResult b)
        {
            if (((object)a) == null || ((object)b) == null)
            {
                return Object.Equals(a, b);
            }

            return a.Equals(b);
        }

        public static bool operator !=(CompareEntriesResult a, CompareEntriesResult b)
        {
            return !(a == b);
        }
    }
}