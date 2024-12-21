namespace LessMsi.Msi
{
    public enum ExtractionMode
    {
        /// <summary>
        /// Default value indicating that a regular extraction should be performed.
        /// </summary>
        Default,
        /// <summary>
        /// Value indicating that a file extraction preserving directories should be performed.
        /// </summary>
        PreserveDirectoriesExtraction,
        /// <summary>
        /// Value indicating that a file extraction renaming identical files should be performed.
        /// </summary>
        RenameFlatExtraction,
        /// <summary>
        /// Value indicating that a file extraction overwriting identical files should be performed.
        /// </summary>
        OverwriteFlatExtraction,
        /// <summary>
        /// Value indicating that a file extraction overwriting identical files should be performed.
        /// While preserving the directories structures
        /// </summary>
        OverwriteExtraction
    }
}