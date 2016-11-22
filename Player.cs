using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TKSprites
{
    public class Player : MoveableObject
    {

        public Sprite sprite;
        public bool facingRight = true;


        public Player(float x, float y)
        {

            // Set acceleration and speed
            float mScale = 60.0f;

            accX = 1.0f * mScale;
            decX = 1.5f * mScale;
            maxSpeedX = 5.0f * mScale;
            maxSpeedY = 10.0f * mScale;

            speedX = 0.0f;
            speedY = 0.0f;

            // Set jump and gravity forces
            jumpStartSpeedY = 8.0f * mScale;
            accY = 0.2f * mScale;

            jumping = false;
            jumpKeyDown = false;

            position = new Vector2(x, y);

            sprite = new Sprite(DBG.MainWindow.loadImage(@"player\player.png"), 32, 64);
            sprite.Position = position;
            sprite.TexRect = new RectangleF(0.0f, 0.0f, 0.5f, 1.0f);
        }

        //public Player() : this(0, 0) { }
        
        public override void Update(float time)
        {

            position.X += speedX * time;
            position.Y += speedY * time;


            if (position.Y < 0)
            {
                position.Y = 0;
                jumping = false;
            }

            UpdateSprite();
        }


        public bool MoveX(float offset)
        {
           
            return true;
        }

        public bool MoveY(float offset)
        {
            

            //else position.Y = 0;
            return UpdateSprite();
        }


        public bool UpdateSprite()
        {
            sprite.Position.X = position.X;// + (sprite.Size.Width / 2);
            sprite.Position.Y = position.Y;// + (sprite.Size.Height / 2);
            return true;
        }

        public bool SolidAt(float atY, float atX)
        {
            int leftOf = ((int)atX / Properties.Settings.Default.TileSize);
            int heightOf = ((int)atY / Properties.Settings.Default.TileSize);
            return (DBG.MainWindow.world.TestSolid(leftOf, heightOf));
        }

        public bool SolidBelow()
        {
            int leftOf = ((int)Left / Properties.Settings.Default.TileSize);
            int heightOf = ((int)Top / Properties.Settings.Default.TileSize);
            return ((DBG.MainWindow.world.TestSolid(leftOf, heightOf)) || (DBG.MainWindow.world.TestSolid(leftOf+1, heightOf)));
        }

        public Vector2 Position { get { return position;} }
        //public Vector2 LastPosition { get { return lastPosition; } }
        public float Left { get { return position.X; } }
        public float Right { get { return position.X+sprite.Size.Width; } }
        public float Top { get { return position.Y; } }
        public float Bottom { get { return position.Y + sprite.Size.Height; } }
        public float Width { get { return sprite.Size.Width; } }
        public float Height { get { return sprite.Size.Height; } }

        public override bool HasWorldCollision
        {
            get
            {
                return true;
            }
        }

        public override bool ApplyGravity
        {
            get
            {
                return true;
            }
        }

        public override bool ApplyFriction
        {
            get
            {
                return true;
            }
        }
    }
}
