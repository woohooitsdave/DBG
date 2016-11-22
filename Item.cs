using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKSprites
{
    public class Item : MoveableObject
    {
        public Sprite sprite;
        public bool facingRight = true;

        public Item(float x, float y)
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

        public bool UpdateSprite()
        {
            sprite.Position.X = position.X;// + (sprite.Size.Width / 2);
            sprite.Position.Y = position.Y;// + (sprite.Size.Height / 2);
            return true;
        }



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
                return false;
            }
        }
    }
}
