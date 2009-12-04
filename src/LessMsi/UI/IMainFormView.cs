using System.IO;

namespace LessMsi.UI
{
    internal interface IMainFormView
    {
        FileInfo GetSelectedMsiFile();
    }
}