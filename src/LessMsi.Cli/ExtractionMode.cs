namespace LessMsi.Cli
{
    public enum ExtractionMode
    {
        /// <summary>
        /// Default value indicating that no extraction should be performed.
        /// </summary>
        None,
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
        OverwriteFlatExtraction
    }
}