using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;

namespace TKSprites
{
    public class UIManager
    {
        private List<Sprite> elements = new List<Sprite>();


        public void AddString(Vector2 pos, string text)
        {
            elements.Add(new GLText(pos, new Font(FontFamily.GenericSansSerif, 12), Brushes.Firebrick, text));
        }

        public Sprite[] Elements { get { return elements.ToArray(); } }
        public int Count {  get { return elements.Count; } }      

    }
}
