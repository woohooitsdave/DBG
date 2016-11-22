using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKSprites
{
    public class BlockTextureManager
    {

        public Atlas blockAtlas;
        public float rectHeight, rectWidth;
        public Dictionary<int, Vector2> blockAtlasDefs = new Dictionary<int, Vector2>();

        public BlockTextureManager(int blockAtlasInput)
        {
            blockAtlas = new Atlas(blockAtlasInput, 1, 5);
            rectHeight = 1.0f / blockAtlas.Rows;
            rectWidth = 1.0f / blockAtlas.Columns;

            for(int i = 0; i <= blockAtlas.Columns; i++)
            {
                blockAtlasDefs.Add(i, new Vector2(rectWidth*i, 0.0f));
            }
            
        }

        public int Count { get { return blockAtlas.Columns * blockAtlas.Rows; } }

        public Vector2[] GetTexCoords(int tileID)
        {
            RectangleF TexRect = new RectangleF(blockAtlasDefs[tileID].X, blockAtlasDefs[tileID].Y, rectWidth, rectHeight);
            return new Vector2[] {
                new Vector2(TexRect.Left, TexRect.Bottom),
                new Vector2(TexRect.Left,  TexRect.Top),
                new Vector2(TexRect.Right, TexRect.Top),
                new Vector2(TexRect.Right, TexRect.Bottom)
            };

        }

        /// <summary>
        /// Gets an array of vertices for the quad of this Sprite
        /// </summary>
        /// <returns></returns>
        public static Vector2[] GetVertices()
        {
            return new Vector2[] {
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f,  0.5f),
                new Vector2(0.5f,  0.5f),
                new Vector2(0.5f, -0.5f)
            };
        }

        /// <summary>
        /// Gets the indices to draw this Sprite
        /// </summary>
        /// <param name="offset">Value to offset the indice values by (number of verts before this Sprite)</param>
        /// <returns>Array of indices to draw</returns>
        public static int[] GetIndices(int offset = 0)
        {
            int[] indices = new int[] { 0, 1, 2, 0, 2, 3 };

            if (offset != 0)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] += offset;
                }
            }

            return indices;
        }
    }
}
