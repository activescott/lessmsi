namespace LessMsi.Tests
{
    public struct CompareEntriesResult
    {
        public bool AreEntriesEqual { get; private set; }
        public string ErrorMessge { get; private set; }

        public CompareEntriesResult(bool areEntriesEqual, string errorMessge)
        {
            AreEntriesEqual = areEntriesEqual;
            ErrorMessge = errorMessge;
        }
    }
}