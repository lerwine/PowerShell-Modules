using System;

namespace Erwine.Leonard.T.GDIPlus.Palette.Ditherers.Ordered
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class BayerDitherer4 : BaseOrderedDitherer
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// See <see cref="BaseColorDitherer.CreateCoeficientMatrix"/> for more details.
        /// </summary>
        protected override Byte[,] CreateCoeficientMatrix()
        {
            return new Byte[,] 
            {
        		{  1,  9,  3, 11 },
			    { 13,  5, 15,  7 },
			    {  4, 12,  2, 10 },
			    { 16,  8, 14,  6 }
            };
        }

        /// <summary>
        /// See <see cref="BaseOrderedDitherer.MatrixWidth"/> for more details.
        /// </summary>
        protected override Byte MatrixWidth
        {
            get { return 4; }
        }

        /// <summary>
        /// See <see cref="BaseOrderedDitherer.MatrixHeight"/> for more details.
        /// </summary>
        protected override Byte MatrixHeight
        {
            get { return 4; }
        }
    }
}
