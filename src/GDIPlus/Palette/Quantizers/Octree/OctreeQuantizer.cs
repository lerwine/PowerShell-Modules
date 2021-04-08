using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Erwine.Leonard.T.GDIPlus.Palette.Helpers;
using Erwine.Leonard.T.GDIPlus.Collections.Synchronized;

namespace Erwine.Leonard.T.GDIPlus.Palette.Quantizers.Octree
{
    /// <summary>
    /// The idea here is to build a tree structure containing always a maximum of K different 
    /// colors. If a further color is to be added to the tree structure, its color value has 
    /// to be merged with the most likely one that is already in the tree. The both values are 
    /// substituted by their mean. 
    ///
    /// The most important data structure are the nodes of the octree. Each inner node of the 
    /// octree contain a maximum of eight successors, the leave nodes keep information for the 
    /// color value (colorvalue), the color index (colorindex), and a counter (colorcount) for 
    /// the pixel that are already mapped to a particular leave. Because each of the red, green 
    /// and blue value is between 0 and 255 the maximum depth of the tree is eight. In level i 
    /// Bit i of RGB is used as selector for the successors. 
    ///
    /// The octree is constructed during reading the image that is to be quantized. Only that 
    /// parts of the octree are created that are really needed. Initially the first K values 
    /// are represented exactly (in level eight). When the number of leaves nodes (currentK) 
    /// exceeds K, the tree has to reduced. That would mean that leaves at the largest depth 
    /// are substituted by their predecessor.
    /// </summary>
    public class OctreeQuantizer : BaseColorQuantizer
    {
        #region | Fields |

        private OctreeNode _root = null;
        private Int32 lastColorCount;
        private SynchronizedReadOnlyList<NonNullSynchronizedList<OctreeNode>> _levels = null;

        #endregion

        #region | Calculated properties |

        private object _syncRootForLevels = new object();

        private OctreeNode Root
        {
            get
            {
                bool justCreated = false;

                lock (this._syncRootForLevels)
                {
                    justCreated = this._root == null;
                    if (justCreated)
                        this._root = new OctreeNode(0, this);
                }

                // If this was a new root node, we'll give other threads a chance to do what they need to do before we return the octree node to the caller.
                // Previously, a System.NullReference exception was being thrown shortly after this was called, due to null nodes in
                // one of the the Levels collections. This, combined with using thread-safe collections seems to have fixed it.
                if (justCreated) 
                    System.Threading.Thread.Sleep(10);

                return this._root;
            }
            set
            {
                lock (this._syncRootForLevels)
                    this._root = value;
            }
        }

        private SynchronizedReadOnlyList<NonNullSynchronizedList<OctreeNode>> Levels
        {
            get
            {
                lock (this._syncRootForLevels)
                {
                    if (this._levels == null)
                        this._levels = new SynchronizedReadOnlyList<NonNullSynchronizedList<OctreeNode>>(7);
                }

                return this._levels;
            }
            set
            {
                lock (this._syncRootForLevels)
                    this._levels = value;
            }
        }

        /// <summary>
        /// Gets the leaf nodes only (recursively).
        /// </summary>
        /// <value>All the tree leaves.</value>
        internal IEnumerable<OctreeNode> Leaves
        {
            get { return this.Root.ActiveNodes.Where(node => node.IsLeaf); }
        }

        #endregion

        #region | Methods |

        /// <summary>
        /// Adds the node to a level node list.
        /// </summary>
        /// <param name="level">The depth level.</param>
        /// <param name="octreeNode">The octree node to be added.</param>
        internal void AddLevelNode(Int32 level, OctreeNode octreeNode)
        {
            if (octreeNode == null)
                throw new ArgumentNullException("octreeNode");

            this.Levels[level].Add(octreeNode);
        }

        #endregion

        #region << BaseColorQuantizer >>

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnPrepare"/> for more details.
        /// </summary>
        protected override void OnPrepare(ImageBuffer image)
        {
            base.OnPrepare(image);

            // TODO: See if this is no longer necessary
            OnFinish();
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnAddColor"/> for more details.
        /// </summary>
        protected override void OnAddColor(Color color, Int32 key, Int32 x, Int32 y)
        {
            this.Root.AddColor(color, 0, this);
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnGetPalette"/> for more details.
        /// </summary>
        protected override List<Color> OnGetPalette(Int32 colorCount)
        {
            // use optimized palette, if any
            List<Color> optimizedPalette = base.OnGetPalette(colorCount);
            if (optimizedPalette != null) return optimizedPalette;

            // otherwise let's get to build one
            List<Color> result = new List<Color>();
            Int32 leafCount = Leaves.Count();
            lastColorCount = leafCount;
            Int32 paletteIndex = 0;

            // goes thru all the levels starting at the deepest, and goes upto a root level
            for (Int32 level = 6; level >= 0; level--)
            {
                // if level contains any node
                if (this.Levels[level].Count > 0)
                {
                    // orders the level node list by pixel presence (those with least pixels are at the top)
                    IEnumerable<OctreeNode> sortedNodeList = this.Levels[level].OrderBy(node => node.ActiveNodesPixelCount);

                    // removes the nodes unless the count of the leaves is lower or equal than our requested color count
                    foreach (OctreeNode node in sortedNodeList)
                    {
                        // removes a node
                        leafCount -= node.RemoveLeaves(level, leafCount, colorCount, this);

                        // if the count of leaves is lower then our requested count terminate the loop
                        if (leafCount <= colorCount) break;
                    }

                    // if the count of leaves is lower then our requested count terminate the level loop as well
                    if (leafCount <= colorCount) break;

                    // otherwise clear whole level, as it is not needed anymore
                    this.Levels[level].Clear();
                }
            }

            // goes through all the leaves that are left in the tree (there should now be less or equal than requested)
            foreach (OctreeNode node in Leaves.OrderByDescending(node => node.ActiveNodesPixelCount))
            {
                if (paletteIndex >= colorCount) break;

                // adds the leaf color to a palette
                if (node.IsLeaf)
                {
                    result.Add(node.Color);
                }

                // and marks the node with a palette index
                node.SetPaletteIndex(paletteIndex++);
            }

            // we're unable to reduce the Octree with enough precision, and the leaf count is zero
            if (result.Count == 0)
            {
                throw new NotSupportedException("The Octree contains after the reduction 0 colors, it may happen for 1-16 colors because it reduces by 1-8 nodes at time. Should be used on 8 or above to ensure the correct functioning.");
            }

            // returns the palette
            return result;
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnGetPaletteIndex"/> for more details.
        /// </summary>
        protected override void OnGetPaletteIndex(Color color, Int32 key, Int32 x, Int32 y, out Int32 paletteIndex)
        {
            // retrieves a palette index
            paletteIndex = this.Root.GetPaletteIndex(color, 0);
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnGetColorCount"/> for more details.
        /// </summary>
        protected override Int32 OnGetColorCount()
        {
            // calculates the number of leaves, by parsing the whole tree
            return lastColorCount;
        }

        /// <summary>
        /// See <see cref="BaseColorQuantizer.OnFinish"/> for more details.
        /// </summary>
        protected override void OnFinish()
        {
            base.OnFinish();

            // Set Levels and Root to null. The next time it they needed, created will be created again and initialized with new octree level lists
            this.Levels = null;
            this.Root = null;
        }

        #endregion

        #region << IColorQuantizer >>

        /// <summary>
        /// See <see cref="IColorQuantizer.AllowParallel"/> for more details.
        /// </summary>
        public override Boolean AllowParallel
        {
            get { return false; }
        }

        #endregion
    }
}


