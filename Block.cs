using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKSprites
{
    public class Block
    {
        public Vector2 Position = new Vector2();
        public Vector2 Scale = new Vector2(1.0f, 1.0f);
        public int TileID = -1;
        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;
        public bool solid = true;

        private float maxDist = 1.0f;

        /// <summary>
        /// Gets or sets the size of this Sprite in pixels
        /// </summary>
        public SizeF Size
        {
            get
            {
                return new SizeF(Scale.X, Scale.Y);
            }
            set
            {
                Scale = new Vector2(value.Width, value.Height);
                maxDist = (float)Math.Sqrt(this.Scale.X * this.Scale.X + this.Scale.Y * this.Scale.Y);
            }
        }

        public void CalculateModelMatrix()
        {
            Vector3 translation = new Vector3();

            translation = new Vector3(Position.X - DBG.MainWindow.ClientSize.Width / 2 - DBG.MainWindow.MainCamera.X, Position.Y - DBG.MainWindow.ClientSize.Height / 2 - DBG.MainWindow.MainCamera.Y, 0.0f);

            ModelMatrix = Matrix4.CreateScale(Scale.X, Scale.Y, 1.0f) * Matrix4.CreateRotationZ(0.0f) * Matrix4.CreateTranslation(translation);
        }

        public Block(int tileID, float x, float y)
        {
            TileID = tileID;
            Position = new Vector2(x, y);
            Size = new Size(32, 32);
        }

        /// <summary>
        /// Determines if this Sprite is visible in the current view
        /// </summary>
        public bool IsVisible
        {
            get
            {
                if (TileID == -1) return false;
                return Position.X + LongestSide > DBG.MainWindow.MainCamera.X && Position.X - LongestSide < DBG.MainWindow.MainCamera.X + DBG.MainWindow.MainCamera.Width && Position.Y + LongestSide > DBG.MainWindow.MainCamera.Y && Position.Y - LongestSide < DBG.MainWindow.MainCamera.Y + DBG.MainWindow.MainCamera.Height;
            }
        }
        
        /// <summary>
        /// Half the width of this Sprite
        /// </summary>
        public float HalfWidth { get { return Scale.X / 2.0f; } }

        /// <summary>
        /// Half the height of this Sprite
        /// </summary>
        public float HalfHeight { get { return Scale.Y / 2.0f; } }
        
        /// <summary>
        /// The length of the longest side of the rectangle drawn for this Sprite
        /// </summary>
        public float LongestSide { get { return Math.Max(Size.Width, Size.Height); } }
        
        /// <summary>
        /// The top-left corner of this Sprite
        /// </summary>
        public Vector2 TopLeft
        {
            get
            {
                return new Vector2((float)((-HalfWidth) * Math.Cos(0.0f) - (-HalfHeight) * Math.Sin(0.0f)), (float)((-HalfWidth) * Math.Sin(0.0f) + (-HalfHeight) * Math.Cos(0.0f))) + Position;
            }
        }

        /// <summary>
        /// The top-right corner of this Sprite
        /// </summary>
        public Vector2 TopRight
        {
            get
            {
                return new Vector2((float)((HalfWidth) * Math.Cos(0.0f) - (-HalfHeight) * Math.Sin(0.0f)), (float)((HalfWidth) * Math.Sin(0.0f) + (-HalfHeight) * Math.Cos(0.0f))) + Position;
            }
        }

        /// <summary>
        /// The bottom-left corner of this Sprite
        /// </summary>
        public Vector2 BottomLeft
        {
            get
            {
                return new Vector2((float)((-HalfWidth) * Math.Cos(0.0f) - (HalfHeight) * Math.Sin(0.0f)), (float)((-HalfWidth) * Math.Sin(0.0f) + (HalfHeight) * Math.Cos(0.0f))) + Position;
            }
        }

        /// <summary>
        /// The bottom-left corner of this Sprite
        /// </summary>
        public Vector2 BottomRight
        {
            get
            {
                return new Vector2((float)((HalfWidth) * Math.Cos(0.0f) - (HalfHeight) * Math.Sin(0.0f)), (float)((HalfWidth) * Math.Sin(0.0f) + (HalfHeight) * Math.Cos(0.0f))) + Position;
            }
        }
        
        /// <summary>
        /// Determine if a point is inside the Sprite's rotated rectangle
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>True if the given point is inside this Sprite's rectangle</returns>
        public bool IsInside(Vector2 point)
        {
            Vector2 AP = point - TopLeft;
            Vector2 AB = TopRight - TopLeft;
            Vector2 AD = BottomLeft - TopLeft;

            // Use the dot products to find if the point is inside or outside the Sprite
            return (0 < Vector2.Dot(AP, AB) && Vector2.Dot(AP, AB) < Vector2.Dot(AB, AB) && 0 < Vector2.Dot(AP, AD) && Vector2.Dot(AP, AD) < Vector2.Dot(AD, AD));
        }
    }

}
