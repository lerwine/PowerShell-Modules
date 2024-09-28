#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Ditherers.Ordered
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class BayerDitherer4 : BaseOrderedDitherer
    {
        /// <summary>
        /// See <see cref="BaseColorDitherer.CreateCoeficientMatrix"/> for more details.
        /// </summary>
        protected override byte[,] CreateCoeficientMatrix() => new byte[,]
            {
                {  1,  9,  3, 11 },
                { 13,  5, 15,  7 },
                {  4, 12,  2, 10 },
                { 16,  8, 14,  6 }
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
