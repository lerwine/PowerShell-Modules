#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Quantizers.Uniform
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal struct UniformColorSlot
    {
        private int value;
        private int pixelCount;

        /// <summary>
        /// Adds the value to the slot.
        /// </summary>
        /// <param name="component">The color component value.</param>
        public void AddValue(int component)
        {
            value += component;
            pixelCount++;
        }

        /// <summary>
        /// Gets the average, just simple value divided by pixel presence.
        /// </summary>
        /// <returns>The average color component value.</returns>
        public readonly int GetAverage()
        {
            int result = 0;

            if (pixelCount > 0)
            {
                result = pixelCount == 1 ? value : value / pixelCount;
            }

            return result;
        }
    }
}
