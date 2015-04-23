using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    interface IEnemyBuilder
    {
        Enemy GetEnemy
        {
            get;
        }
        void BuildTexture(ContentManager content);
        void BuildScale();
        void BuildWeapon();
        void BuildPosition(Vector2 position);
        void BuildType();
    }
}
