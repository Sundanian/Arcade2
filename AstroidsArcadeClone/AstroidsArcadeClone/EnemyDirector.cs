using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    class EnemyDirector
    {
        private readonly IEnemyBuilder enemyBuilder;
        private ContentManager content;
        private Vector2 position;

        public Enemy GetEnemy
        {
            get { return enemyBuilder.GetEnemy; }
        }

        public EnemyDirector(IEnemyBuilder enemyBuilder, ContentManager content, Vector2 position)
        {
            this.enemyBuilder = enemyBuilder;
            this.content = content;
            this.position = position;
        }

        public void BuildEnemy()
        {
            enemyBuilder.BuildScale();
            enemyBuilder.BuildTexture(content);
            enemyBuilder.BuildWeapon();
            enemyBuilder.BuildPosition(position);
            enemyBuilder.BuildType();
        }
    }
}
