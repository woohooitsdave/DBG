using OpenTK;
using System.Drawing;

namespace TKSprites
{
    public struct Camera
    {
        //public RectangleF CurrentView;
        //public float X, Y, Width, Height;
        public Vector2 position;
        public SizeF size;

        public Camera(float x, float y, float w, float h)
        {
            //CurrentView = new RectangleF(x, y, w, h);
            position = new Vector2(x, y);
            size = new SizeF(w, h);
        }

        public float X { get { return position.X; } set { position.X = value; } }
        public float Y { get { return position.Y; } set { position.Y = value; } }
        public float Width { get { return size.Width; } set { size.Width = value; } }
        public float Height { get { return size.Height; } set { size.Height = value; } }
        public SizeF Size { get { return size; } set { size = value; } }
        public Vector2 Position { get { return position; } }

        public float Left { get { return position.X; } }
        public float Right { get { return position.X + size.Width; } }
        public float Top { get { return position.Y; } }
        public float Bottom { get { return position.Y + size.Height; } }


        public bool MoveX(float offset)
        {
            float testX = Left + offset;
            if(testX > -Properties.Settings.Default.TileSize && testX < DBG.MainWindow.world.Width)
                position.X += offset;
            return true;
        }

        public bool CenterOnTarget(Vector2 targ)
        {
            float left = targ.X - (size.Width / 2);
            float top = targ.Y - (size.Height / 3);
            position.X = left;
            position.Y = top;
            return true;
        }
    }
}
