using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{

    class Partikle : SpriteObject
    {
        private int velocityX;
        private int velocityY;
        private static Random r = new Random();
        private float lifetime = 1;
        public Partikle(Vector2 position) : base(position)
        {
            
        }
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            scale = 0.1f;
            texture = content.Load<Texture2D>(@"Missile");
            CreateAnimation("Idle", 1, 0, 1, 16, 16, Vector2.Zero, 1, texture);
            PlayAnimation("Idle");

            velocityX = r.Next(-1, 2);
            velocityY = r.Next(-1, 2);
            if (velocityX == 0 && velocityY == 0)
            {
                velocityX = r.Next(1, 3);
                if (velocityX == 2)
                {
                    velocityX = -1;
                }
                velocityY = r.Next(1, 3);
                if (velocityY == 2)
                {
                    velocityY = -1;
                }
            }

            base.LoadContent(content);
        }
        public override void Update(GameTime gametime)
        {
            velocity = Vector2.Zero;
            velocity += new Vector2(velocityX, velocityY);
            velocity *= speed;
            float deltatime = (float)gametime.ElapsedGameTime.TotalSeconds;
            position += (velocity * deltatime);

            lifetime -= (float)gametime.ElapsedGameTime.TotalSeconds;
            if (lifetime < 0)
            {
                Space.RemoveObjects.Add(this);
            }

            base.Update(gametime);
        }
    }
}
