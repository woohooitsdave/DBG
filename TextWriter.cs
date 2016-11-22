using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKSprites
{
    public class TextWriter
    {
        public Dictionary<char, RectangleF> Glyphs = new Dictionary<char, RectangleF>();
        public int charAtlas = -1;
        public int glyphCount = 36;
        public float glyphHeight = 1.0f, glyphWidth;

        public TextWriter(int texureID)
        {
            glyphWidth = (1.0f / (glyphCount + 1));
            charAtlas = texureID;
            
            float gT = 0.0f, gH = glyphHeight, gW = glyphWidth;
            //float gL = 0.0f;
            Glyphs.Add('0', new RectangleF(0.0f, gT, gW, gH));
            Glyphs.Add('1', new RectangleF(gW, gT, gW, gH));
            Glyphs.Add('2', new RectangleF(gW*Glyphs.Count, gT, gW, gH));
            Glyphs.Add('3', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('4', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('5', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('6', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('7', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('8', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('9', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('a', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
            Glyphs.Add('b', new RectangleF(gW * Glyphs.Count, gT, gW, gH));
        }

        public TextSprite GetGlyph(char c, float x, float y)
        {
            if (charAtlas < 0) throw new Exception("Glyph Atlas Not Loaded");

            TextSprite charSprite = new TextSprite(charAtlas, 32, 32);

            charSprite.Position = new Vector2(x, y);

            charSprite.Scale = new Vector2(40, 40);

            charSprite.TexRect = Glyphs[c];

            return charSprite;
        }
    }
}
