#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Ditherers.Ordered
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class ClusteredDotDitherer : BaseOrderedDitherer
    {
        /// <summary>
        /// See <see cref="BaseColorDitherer.CreateCoeficientMatrix"/> for more details.
        /// </summary>
        protected override byte[,] CreateCoeficientMatrix() => new byte[,]
            {
                { 13,  5, 12, 16 },
                {  6,  0,  4, 11 },
                {  7,  2,  3, 10 },
                { 14,  8,  9, 15 }
            };

        /// <summary>
        /// See <see cref="BaseOrderedDitherer.MatrixWidth"/> for more details.
        /// </summary>
        protected override byte MatrixWidth => 4;

        /// <summary>
        /// See <see cref="BaseOrderedDitherer.MatrixHeight"/> for more details.
        /// </summary>
        protected override byte MatrixHeight => 4;
    }
}
