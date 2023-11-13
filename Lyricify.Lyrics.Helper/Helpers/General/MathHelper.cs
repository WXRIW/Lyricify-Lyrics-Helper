namespace Lyricify.Lyrics.Helpers.General
{
    public static class MathHelper
    {
        /// <summary>
        /// Returns the smaller of two 32-bit signed integers.
        /// </summary>
        /// <param name="val1">The first of two 32-bit signed integers to compare.</param>
        /// <param name="val2">The second of two 32-bit signed integers to compare.</param>
        /// <returns>Parameter val1 or val2, whichever is smaller.</returns>
        public static int? Min(int? val1, int? val2)
        {
            if (val1.HasValue && val2.HasValue) return Math.Min(val1.Value, val2.Value);
            if (val1.HasValue) return val1.Value;
            if (val2.HasValue) return val2.Value;
            return null;
        }

        /// <summary>
        /// Returns the larger of two 32-bit signed integers.
        /// </summary>
        /// <param name="val1">The first of two 32-bit signed integers to compare.</param>
        /// <param name="val2">The second of two 32-bit signed integers to compare.</param>
        /// <returns>Parameter val1 or val2, whichever is larger.</returns>
        public static int? Max(int? val1, int? val2)
        {
            if (val1.HasValue && val2.HasValue) return Math.Max(val1.Value, val2.Value);
            if (val1.HasValue) return val1.Value;
            if (val2.HasValue) return val2.Value;
            return null;
        }
    }
}
