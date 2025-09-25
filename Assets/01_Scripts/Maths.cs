using UnityEngine;

namespace John
{
    /// <summary>
    /// A collection of maths functions written by John Bullock for the SAE Institute Games Programming
    /// module 4FSC0PE001.
    /// </summary>
    static class Maths
    {

        /// <summary>
        /// Clamps <paramref name="value"/> between <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        /// <param name="value">The float being clamped.</param>
        /// <param name="min">The minimum value to be clamped to.</param>
        /// <param name="max">The maximum value to be clamped to.</param>
        /// <returns>A float between min and max.</returns>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            else return value;
        }

        /// <summary>
        /// Performs a linear interpolation between two floats.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="t">The interpolation factor, clamped between 0 and 1./param>
        /// <returns></returns>
        public static float Lerp(float start, float end, float t)
        {
            float factor = Clamp(t, 0f, 1f);
            return start + (end - start) * factor;
        }

        /// <summary>
        /// Performs a linear interpolation between two Vector3s.
        /// </summary>
        /// <param name="start">The start vector</param>
        /// <param name="end">The end vector</param>
        /// <param name="t">The interpolation factor, clamped between 0 and 1.</param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 start, Vector3 end, float t)
        {
            Vector3 result = new()
            {
                x = Lerp(start.x, end.x, t),
                y = Lerp(start.y, end.y, t),
                z = Lerp(start.z, end.z, t)
            };
            return result;
        }

        /// <summary>
        /// Rounds a float to the nearest whole number.
        /// </summary>
        /// <param name="value">The float being rounded.</param>
        /// <returns>An integer value.</returns>
        public static int RoundToInt(float value)
        {
            // Casting a float to an int removes the decimal places (ie, 1.8943 -> 1)
            int castedInt = (int)value;
            float remainder = value - castedInt;
            if (remainder < 0.5f) return castedInt;
            else return castedInt + 1;
        }

    }
}
