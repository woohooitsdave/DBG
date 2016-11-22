using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKSprites
{
    public struct BlockTemplate
    {

        public int tileID;
        public bool solid;

        public BlockTemplate(int id) { tileID = id; solid = true; }
        public BlockTemplate(int id, bool sol) { tileID = id;  solid = sol; }
    }

    public class ObjectLibrary
    {
        public const bool SOLID = true, NONSOLID = false;


        public Dictionary<int, BlockTemplate> blocks = new Dictionary<int, BlockTemplate>();

        public ObjectLibrary()
        {
            blocks.Add(-1, new BlockTemplate(-1, NONSOLID));
            blocks.Add(0, new BlockTemplate(0));
            blocks.Add(1, new BlockTemplate(1));
            blocks.Add(2, new BlockTemplate(2));
            blocks.Add(3, new BlockTemplate(3));
            blocks.Add(4, new BlockTemplate(4, NONSOLID));
        }

        public Block FetchBlock(int id, float x, float y)
        {
            BlockTemplate t = blocks[id];
            Block b = new TKSprites.Block(id, x, y);
            b.solid = t.solid;
            return b;
        }
    }
}
