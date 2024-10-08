using System.Collections.Concurrent;
using Erwine.Leonard.T.GDIPlus.Palette.ColorCaches;
using Erwine.Leonard.T.GDIPlus.Palette.Helpers;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus.Palette.Quantizers.MedianCut
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// The premise behind median cut algorithms is to have every entry in the color map represent 
    /// the same number of pixels in the original image. In contrast to uniform sub-division, these 
    /// algorithms divide the color space based on the distribution of the original colors. The 
    /// process is as follows[2]: 
    /// 
    /// 1. Find the smallest box which contains all the colors in the image.
    /// 2. Sort the enclosed colors along the longest axis of the box.
    /// 3. Split the box into 2 regions at median of the sorted list.
    /// 4. Repeat the above process until the original color space has been divided into 256 regions.
    /// 5. The algorithm then divides the color space in a manner depicted below.
    /// 6. The representative colors are found by averaging the colors in each box, and the appropriate color map index assigned to each color in that box.
    /// 
    /// Because these algorithms use image information in dividing the color space this class of 
    /// algorithms consistently produce good results while having memory and time complexity no 
    /// worse than popularity algorithms[1].
    /// </summary>
    public class MedianCutQuantizer : BaseColorCacheQuantizer
    {
        #region | Fields |

        private ConcurrentBag<MedianCutCube> cubeList;

        #endregion

        #region | Methods |

        /// <summary>
        /// Splits all the cubes on the list.
        /// </summary>
        /// <param name="colorCount">The color count.</param>
        private void SplitCubes(int colorCount)
        {
            // creates a holder for newly added cubes
            List<MedianCutCube> newCubes = [];

            foreach (MedianCutCube cube in cubeList)
            {
                // if another new cubes should be over the top; don't do it and just stop here
                if (newCubes.Count >= colorCount) break;
                
                MedianCutCube newMedianCutCubeA, newMedianCutCubeB;

                // splits the cube along the red axis
                if (cube.RedSize >= cube.GreenSize && cube.RedSize >= cube.BlueSize)
                {
                    cube.SplitAtMedian(0, out newMedianCutCubeA, out newMedianCutCubeB);
                }
                else if (cube.GreenSize >= cube.BlueSize) // splits the cube along the green axis
                {
                    cube.SplitAtMedian(1, out newMedianCutCubeA, out newMedianCutCubeB);
                }
                else // splits the cube along the blue axis
                {
                    cube.SplitAtMedian(2, out newMedianCutCubeA, out newMedianCutCubeB);
                }

                // adds newly created cubes to our list; but one by one and if there's enough cubes stops the process
                newCubes.Add(newMedianCutCubeA);
                if (newCubes.Count >= colorCount) break;
                newCubes.Add(newMedianCutCubeB);
            }

            // clears the old cubes
            cubeList =
            [
                // adds the new cubes to the official cube list
                .. newCubes,
            ];
        }

        #endregion

        #region << BaseColorCacheQuantizer >>

        /// <summary>
        /// See <see cref="IColorQuantizer.Prepare"/> for more details.
        /// </summary>
        protected override void OnPrepare(ImageBuffer image)
        {
            base.OnPrepare(image);

            OnFinish();
        }

        /// <summary>
        /// See <see cref="BaseColorCacheQuantizer.OnCreateDefaultCache"/> for more details.
        /// </summary>
        protected override IColorCache OnCreateDefaultCache()
        {
            // use native median cut palette index method; if there's no 
            return null;
        }

        /// <summary>
        /// See <see cref="BaseColorCacheQuantizer.OnGetPaletteToCache"/> for more details.
        /// </summary>
        protected override List<Color> OnGetPaletteToCache(int colorCount)
        {
            // creates the initial cube covering all the pixels in the image
            MedianCutCube initalMedianCutCube = new(UniqueColors.Keys);
            cubeList.Add(initalMedianCutCube);

            // finds the minimum iterations needed to achieve the cube count (color count) we need
            int iterationCount = 1;
            while ((1 << iterationCount) < colorCount) { iterationCount++; }

            for (int iteration = 0; iteration < iterationCount; iteration++)
            {
                SplitCubes(colorCount);
            }

            // initializes the result palette
            List<Color> result = [];
            int paletteIndex = 0;

            // adds all the cubes' colors to the palette, and mark that cube with palette index for later use
            foreach (MedianCutCube cube in cubeList)
            {
                result.Add(cube.Color);
                cube.SetPaletteIndex(paletteIndex++);
            }

            // returns the palette (should contain <= ColorCount colors)
            return result;
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnFinish"/> for more details.
        /// </summary>
        protected override void OnFinish()
        {
            base.OnFinish();

            cubeList = [];
        }

        #endregion

        #region << IColorQuantizer >>

        /// <summary>
        /// See <see cref="IColorQuantizer.AllowParallel"/> for more details.
        /// </summary>
        public override bool AllowParallel => true;

        /// <summary>
        /// See <see cref="IColorQuantizer.GetPaletteIndex"/> for more details.
        /// </summary>
        public void GetPaletteIndex(Color color, out int paletteIndex)
        {
            paletteIndex = 0;
            color = QuantizationHelper.ConvertAlpha(color);

            foreach (MedianCutCube cube in cubeList)
            {
                if (cube.IsColorIn(color))
                {
                    paletteIndex = cube.PaletteIndex;
                    break;
                }
            }
        }

        #endregion
    }
}
