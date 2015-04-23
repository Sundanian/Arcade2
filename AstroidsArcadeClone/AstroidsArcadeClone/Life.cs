using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    class Life : SpriteObject
    {
        public Life(Vector2 position)
            : base(position)
        {

        }
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            texture = content.Load<Texture2D>(@"Life");

            CreateAnimation("Idle", 1, 0, 0, 128, 128, Vector2.Zero, 1, texture);
            PlayAnimation("Idle");

            base.LoadContent(content);
        }
    }
}
