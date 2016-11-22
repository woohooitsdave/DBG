// https://katyscode.wordpress.com/2013/01/18/2d-platform-games-collision-detection-for-dummies/
// http://www.wildbunny.co.uk/blog/2011/12/14/how-to-make-a-2d-platform-game-part-2-collision-detection/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace TKSprites
{
    public abstract class MoveableObject
    {
        public Vector2 position;

        public bool m_HasWorldCollision, m_ApplyGravity, m_ApplyFriction;

        public float accX, decX; //acceleration, deceleration on X direction

        public float speedX, speedY; //amount of movement to apply on next frame
        public float maxSpeedX, maxSpeedY; //maximum speeds

        public float jumpStartSpeedY; //amount of upward force to apply when first jumping

        public float accY; //Y acceleration

        public bool jumping; //true if player is jumping, prevents dbl 
        public bool jumpKeyDown; //true if jump key pressed

        public abstract void Update(float time);

        public abstract bool HasWorldCollision { get; }
        public abstract bool ApplyGravity { get; }
        public abstract bool ApplyFriction { get; }

    }
}
