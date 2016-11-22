using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;

namespace TKSprites
{

    public class Map
    {
        public Block[,] blocks = new Block[200, 50];

        public Dictionary<string, int> tileNames = new Dictionary<string, int>();

        public Map()
        {
            tileNames.Add("bedrock", 0);
            tileNames.Add("sand", 1);
            tileNames.Add("dirt", 2);
            tileNames.Add("stone", 3);
            tileNames.Add("cloud", 4);
        }

        public void Generate()
        {
            int tileSize = Properties.Settings.Default.TileSize;
            float locX = 0, locY = 0;
            Random r = new Random();
            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                for (int x = 0; x < blocks.GetLength(0); x++)
                {
                    int tileID = -1;
                    if (y >= 40) tileID = tileNames["cloud"];
                    else if (y == 0) tileID = tileNames["bedrock"];
                    else if (y > 0 && y < 15) tileID = r.Next(1, 4);
                    
                    // Assign random texture
                    int texval = -1;
                    if(tileID >= 0 && tileID < DBG.MainWindow.TextureMgr.Count) texval = tileID;

                    Block t = new Block(-1, locX, locY); //new Block(texval, locX, locY);
                    t.solid = false;// (texval != -1);
                    
                    blocks[x, y] = t;
                    locX += tileSize;
                }
                locX = 0;
                locY += tileSize;
            }

            blocks[0, 0] = new Block(0, 50 * Properties.Settings.Default.TileSize-32, 0);
            blocks[0, 0].solid = true;

        }

        public bool TestSolid(int x, int y)
        {
            if (x < 0 || y < 0 || x > Width-1 || y > Height-1) return true;
            return (blocks[x, y].solid);
        }

        public Block[,] Blocks { get { return blocks; } }
        public int Width { get { return blocks.GetLength(0); } }
        public int Height { get { return blocks.GetLength(1); } }
        public int ActualWidth { get { return blocks.GetLength(0) * (int)Properties.Settings.Default.TileSize; } }
        public int ActualHeight { get { return blocks.GetLength(1) * (int)Properties.Settings.Default.TileSize; } }
    }
}

                    // Transform sprite randomly
                    //t.Position = ;//new Vector2(r.Next(-8000, 8000), r.Next(-6000, 6000));
                    //float scale = 16f;// 300.0f * (float) r.NextDouble() + 0.5f;
                    //t.Size = new SizeF(scale, scale);
                    //s.Rotation = (float) r.NextDouble() * 2.0f * 3.141f;