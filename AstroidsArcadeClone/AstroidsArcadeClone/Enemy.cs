using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    enum EnemyType { AstroidBig, AstroidNormal, AstroidSmall, UFONormal, UFOSmall };
    class Enemy : SpriteObject
    {
        private bool weapon;
        private int velocityX;
        private int velocityY;
        private static Random r = new Random();
        private EnemyType type;
        private int timer = 0;
        private SoundEffect effect;
        private SoundEffect effect2;
        private SoundEffect effect3;
        private SoundEffect effect4;
        private SoundEffect effect5;
        private int soundTimer = 0;
        private int soundTimer2 = 0;


        public EnemyType Type
        {
            get { return type; }
            set { type = value; }
        }
        public bool Weapon
        {
            get { return weapon; }
            set { weapon = value; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Enemy(Vector2 position)
            : base(position)
        {

        }
        public override void Update(GameTime gametime)
        {
            velocity = Vector2.Zero;
            velocity += new Vector2(velocityX, velocityY);
            velocity *= speed;
            float deltatime = (float)gametime.ElapsedGameTime.TotalSeconds;
            position += (velocity * deltatime);

            Move();

            //Vores metode som laver Ufo lyde, og sørger for de ikke spiller ind over hinanden.
            if (type == EnemyType.UFONormal)
            {
                if (soundTimer == 0)
                {
                    effect4.Play();
                    soundTimer++;
                }
                soundTimer++;
                if (soundTimer == 12)
                {
                    soundTimer = 0;
                }
            }
            else if (type == EnemyType.UFOSmall)
            {
                if (soundTimer2 == 0)
                {
                    effect5.Play();
                    soundTimer2++;
                }
                soundTimer2++;
                if (soundTimer2 == 12)
                {
                    soundTimer2 = 0;
                }

            }

            base.Update(gametime);
        }
        public override void LoadContent(ContentManager content)
        {
            //LAver vores lyd filer
            effect = content.Load<SoundEffect>("bangLarge");
            effect2 = content.Load<SoundEffect>("bangMedium");
            effect3 = content.Load<SoundEffect>("bangSmall");
            effect4 = content.Load<SoundEffect>("saucerBig");
            effect5 = content.Load<SoundEffect>("saucerSmall");

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

            //Sørger for at UFO altid bevæger sig ad yaksen. x bliver bestemt i move()
            if (this.Type == EnemyType.UFONormal || this.Type == EnemyType.UFOSmall)
            {
                if (velocityX == 0)
                {
                    velocityX = r.Next(1, 3);
                    if (velocityX == 2)
                    {
                        velocityX = -1;
                    }
                }
            }

            CreateAnimation("Idle", 1, 0, 0, Texture.Width, Texture.Height, Vector2.Zero, 1, texture);
            PlayAnimation("Idle");
            base.LoadContent(content);
        }
        public virtual void Move()
        {
            if (this.Type == EnemyType.UFONormal || this.Type == EnemyType.UFOSmall)
            {
                if (timer == 0)
                {   
                    switch (r.Next(0, 10))
                    {
                        case 0:
                            this.velocityY = -1;
                            timer++;
                            break;
                        case 1:
                            this.velocityY = 1;
                            timer++;
                            break;
                        case 2:
                            //Skud

                            //Finder to "punkter"
                            Vector2 p = Player.Instance.Position;
                            Vector2 q = position;
                            //Finder en vektor imellem
                            Vector2 v = q - p;
                            //Finder den faktor der skal ganges med for at længen af v = 1
                            double z = 1 / (Math.Sqrt(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2)));
                            //Bruger faktoren, således at vores vektor er 1 lang
                            Vector2 v1 = new Vector2(v.X * (float)z, v.Y * (float)z);
                            //Laver et missile med en position en fra UFOen i retning imod playeren.
                            Space.AddObjects.Add(new Missile(position + v1, this));

                            timer++;
                            break;
                        default:
                            timer++;
                            break;
                    }
                }
                timer++;
            }
            if (timer == 30)
            {
                timer = 0;
            }
        }
        public void DeathSpawn(SpriteObject killer)
        {
            if (!(killer is Missile && !(killer as Missile).PlayerMissile))
            {
                //Score
                switch (type)
                {
                    case EnemyType.AstroidBig:
                        Space.Score += 20;
                        break;
                    case EnemyType.AstroidNormal:
                        Space.Score += 50;
                        break;
                    case EnemyType.AstroidSmall:
                        Space.Score += 100;
                        break;
                    case EnemyType.UFONormal:
                        Space.Score += 200;
                        break;
                    case EnemyType.UFOSmall:
                        Space.Score += 1000;
                        break;
                    default:
                        break;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (this.type == EnemyType.AstroidBig)
                {
                    EnemyDirector director = new EnemyDirector(new AstroidNormal(), Space.ContentMan, position);
                    director.BuildEnemy();
                    Space.AddObjects.Add(director.GetEnemy);
                }
                if (this.type == EnemyType.AstroidNormal)
                {
                    EnemyDirector director = new EnemyDirector(new AstroidSmall(), Space.ContentMan, position);
                    director.BuildEnemy();
                    Space.AddObjects.Add(director.GetEnemy);
                }
                for (int j = 0; j < 3; j++)
                {
                    Space.AddObjects.Add(new Partikle(position));
                }
            }
        }
        protected override void HandleCollision()
        {
            foreach (SpriteObject obj in Space.Objects)
            {
                if (obj != this && obj is Missile && obj.CollisionRect.Intersects(this.CollisionRect))
                {
                    if (PixelCollision(obj))
                    {
                        //vores if sætninger som tjekker hvilken type enemy der dør og spiller den lyd som passer til
                        if (this.type == EnemyType.AstroidBig)
                        {
                            effect.Play();
                        }
                        else if (this.type == EnemyType.AstroidNormal || this.type == EnemyType.UFONormal)
                        {
                            effect2.Play();
                        }
                        else if (this.type == EnemyType.AstroidSmall || this.type == EnemyType.UFOSmall)
                        {
                            effect3.Play();
                        }


                        if (!(type == EnemyType.UFONormal || type == EnemyType.UFOSmall))
                        {
                            DeathSpawn(obj);
                            Space.RemoveObjects.Add(this);
                            Space.RemoveObjects.Add(obj);
                        }
                        else if ((obj as Missile).PlayerMissile)
                        {
                            DeathSpawn(obj);
                            Space.RemoveObjects.Add(this);
                            Space.RemoveObjects.Add(obj);
                        }
                    }
                }
            }
        }
    }
}
