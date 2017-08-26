using System;

namespace Erwine.Leonard.T.GDIPlus.Palette.Ditherers.ErrorDiffusion
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class JarvisJudiceNinkeDitherer : BaseErrorDistributionDitherer
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// See <see cref="BaseColorDitherer.CreateCoeficientMatrix"/> for more details.
        /// </summary>
        protected override Byte[,] CreateCoeficientMatrix()
        {
            return new Byte[,]
            {
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0 },
                { 0, 0, 0, 7, 5 },
                { 3, 5, 7, 5, 3 },
                { 1, 3, 5, 3, 1 }
            };
        }

        /// <summary>
        /// See <see cref="BaseErrorDistributionDitherer.MatrixSideWidth"/> for more details.
        /// </summary>
        protected override Int32 MatrixSideWidth
        {
            get { return 2; }
        }

        /// <summary>
        /// See <see cref="BaseErrorDistributionDitherer.MatrixSideHeight"/> for more details.
        /// </summary>
        protected override Int32 MatrixSideHeight
        {
            get { return 2; }
        }
    }
}
