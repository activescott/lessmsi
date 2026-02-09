using System.Globalization;

namespace LessMsi.Gui.Extensions
{
    public static class CultureInfoExtensions
    {
        public static bool BelongsTo(this CultureInfo culture, CultureInfo target)
        {
            if (culture == null || target == null)
            {
                return false;
            }

            var current = culture;

            while (!current.Equals(CultureInfo.InvariantCulture))
            {
                if (current.Equals(target))
                {
                    return true;
                }

                current = current.Parent;
            }

            return false;
        }
    }
}
