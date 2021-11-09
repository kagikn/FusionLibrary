﻿using FusionLibrary.Extensions;
using System.Collections.Generic;

namespace FusionLibrary.Other
{
    /// <summary>
    /// Dictionary for usages like torque table with ability to get interpolated value by key.
    /// </summary>
    /// <remarks>
    /// For example in dictionary:
    /// <para>
    /// 1000 - 0.0f
    /// </para>
    /// <para>
    /// 2000 - 0.5f
    /// </para>
    /// <para>
    /// <see cref="GetInterpolatedValue(int)"/> with 1500 passed as key argument
    /// will return 0.25f.
    /// </para>
    /// </remarks>
    internal class InterpolationDictionary : Dictionary<int, float>
    {
        /// <summary>
        /// Creates a new instance of <see cref="InterpolationDictionary"/>.
        /// </summary>
        public InterpolationDictionary()
        {

        }

        /// <summary>
        /// Returns interpolated value by key.
        /// </summary>
        /// <param name="key">Key to look up value by.</param>
        /// <returns>
        /// Interpolated value. If dictionary is empty, returns -1f.
        /// </returns>
        public float GetInterpolatedValue(int key)
        {
            if (Count == 0)
                return -1f;

            int bottomKey = -1;
            float bottomKeyValue = -1;

            KeyValuePair<int, float> previousKeyPair = default;
            foreach (KeyValuePair<int, float> keyValuePair in this)
            {
                int currentKey = keyValuePair.Key;

                if (key >= currentKey)
                {
                    bottomKey = currentKey;
                    bottomKeyValue = keyValuePair.Value;
                    break;
                }
                previousKeyPair = keyValuePair;
            }

            // If key value is higher than highest key of dictionary
            if (bottomKey == -1)
            {
                return bottomKeyValue;
            }
            int topKey = previousKeyPair.Key;
            float topKeyValue = previousKeyPair.Value;

            // if key value is below than lowest key of dictionary
            if (topKey == -1)
            {
                return topKeyValue;
            }

            // Get interpolated value
            return ((float)key).Remap(bottomKey, topKey, bottomKeyValue, topKeyValue);
        }
    }
}