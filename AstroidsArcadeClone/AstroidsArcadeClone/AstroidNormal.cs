using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    class AstroidNormal : IEnemyBuilder
    {
        private Enemy enemy;
        public Enemy GetEnemy
        {
            get { return enemy; }
        }

        public AstroidNormal()
        {
            enemy = new Enemy(Vector2.Zero);
        }

        public void BuildTexture(ContentManager content)
        {
            this.enemy.Texture = content.Load<Texture2D>(@"Astroid");
        }
        public void BuildScale()
        {
            this.enemy.Scale = 0.75f;
        }
        public void BuildWeapon()
        {
            this.enemy.Weapon = false;
        }
        public void BuildPosition(Vector2 position)
        {
            this.enemy.Position = position;
        }
        public void BuildType()
        {
            this.enemy.Type = EnemyType.AstroidNormal;
        }
    }
}
