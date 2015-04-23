using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    class Missile : SpriteObject
    {
        private Vector2 senderSpawnPos;
        private Vector2 spawn;
        private float lifetime = 2;
        private bool playerMissile = false;

        public bool PlayerMissile
        {
            get { return playerMissile; }
            set { playerMissile = value; }
        }

        public Missile(Vector2 position, SpriteObject sender) : base(position)
        {
            if (sender is Player)
            {
                playerMissile = true;
            }
            senderSpawnPos = sender.Position;
            spawn = position;
        }
        public override void LoadContent(ContentManager content)
        {
            speed = 500;
            Texture = content.Load<Texture2D>(@"Missile");
            CreateAnimation("Idle", 1, 0, 1, 16, 16, Vector2.Zero, 1, texture);
            PlayAnimation("Idle");

            base.LoadContent(content);
        }
        public override void Update(GameTime gametime)
        {
            velocity = Vector2.Zero;
            velocity += senderSpawnPos - spawn;
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
