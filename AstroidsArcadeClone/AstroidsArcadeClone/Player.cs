using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    class Player : SpriteObject
    {
        private int lives = 3;
        static Player instance;
        private int timer = 0;
        private Vector2 oldVelocity = Vector2.Zero;
        private bool invinsible = false;
        private float invinsibleTimer = 0;
        private SoundEffect effect;
        private SoundEffect effect2;
        private SoundEffect effect3;
        private int soundTimer = 0;

        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }
        public static Player Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Player(new Vector2(500,500));
                }
                return instance;
            }
        }

        private Player(Vector2 position)
            : base(position)
        {

        }
        public override void LoadContent(ContentManager content)
        {
            Frames = 2;
            speed = 2;
            texture = content.Load<Texture2D>(@"Ship");

            CreateAnimation("Idle", 1, 0, 1, 128, 128, Vector2.Zero, 1, texture);
            CreateAnimation("Thrust", 2, 0, 0, 128, 128, Vector2.Zero, 30, texture);
            PlayAnimation("Idle");

            //Laver vores lyd filer
            effect = content.Load<SoundEffect>("fire");
            effect2 = content.Load<SoundEffect>("thrust");
            effect3 = content.Load<SoundEffect>("bangSmall");

            base.LoadContent(content);
        }
        private void HandleInput(KeyboardState keyState)
        {
            if (keyState.IsKeyDown(Keys.Up))
            {
                //Thrust
                PlayAnimation("Thrust");
                velocity += new Vector2((float)Math.Sin(rotation), -(float)Math.Cos(rotation));
                //nogle if sætninger som sørger for at thrust lyden ikke bliver spillet ind over hinanden
                if (soundTimer == 0)
                {
                    effect2.Play();
                    soundTimer++;
                }
                soundTimer++;
                if (soundTimer == 15)
                {
                    soundTimer = 0;
                }
            }
            else
            {
                PlayAnimation("Idle");
            }
            if (keyState.IsKeyDown(Keys.Left))
            {
                //Rotate Left
                rotation -= 0.05f;
            }
            if (keyState.IsKeyDown(Keys.Right))
            {
                //Rotate right
                rotation += 0.05f;
            }
            if (keyState.IsKeyDown(Keys.Space))
            {
                if (timer > 20)
                {
                    effect.Play(); //lyd af skud der bliver affyret
                    Space.AddObjects.Add(new Missile(position + new Vector2(-(float)Math.Sin(rotation), (float)Math.Cos(rotation)), this));
                    timer = 0;
                }
            }
            timer++;
        }
        public override void Update(GameTime gametime)
        {
            velocity.X *= 0.5f;
            velocity.Y *= 0.5f;
            HandleInput(Keyboard.GetState());

            velocity *= speed;

            float deltatime = (float)gametime.ElapsedGameTime.TotalSeconds;

            if (invinsible == true)
            {
                invinsibleTimer += deltatime;
                if (invinsibleTimer > 2)
                {
                    invinsible = false;
                    invinsibleTimer = 0;
                }
            }

            Position += (velocity * deltatime);
            base.Update(gametime);
            oldVelocity = velocity;
            if (lives == 0)
            {
                Space.RemoveObjects.Add(this);
            }
        }
        protected override void HandleCollision()
        {
            foreach (SpriteObject obj in Space.Objects)
            {
                #region enemy
                if (obj != this && obj is Enemy && obj.CollisionRect.Intersects(this.CollisionRect))
                {
                    try
                    {
                        if (PixelCollision(obj))
                        {
                            if (invinsible == false)
                            {
                                effect3.Play(); //eksplosion når spilleren dør
                                lives -= 1;
                                invinsible = true;
                                for (int i = 0; i < 9; i++)
                                {
                                    Space.AddObjects.Add(new Partikle(position));
                                }
                                position = new Vector2(Space.Gamewindow.ClientBounds.Width / 2, Space.Gamewindow.ClientBounds.Height / 2);
                                velocity = Vector2.Zero;
                                Space.RemoveObjects.Add(obj);
                                (obj as Enemy).DeathSpawn(this);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (invinsible == false)
                        {
                            lives -= 1;
                            invinsible = true;
                            for (int i = 0; i < 9; i++)
                            {
                                Space.AddObjects.Add(new Partikle(position));
                            }
                            position = new Vector2(Space.Gamewindow.ClientBounds.Width / 2, Space.Gamewindow.ClientBounds.Height / 2);
                            velocity = Vector2.Zero;
                            Space.RemoveObjects.Add(obj);
                            (obj as Enemy).DeathSpawn(this);
                        }
                    }
                }
                #endregion
                #region missile
                if (obj != this && obj is Missile && obj.CollisionRect.Intersects(this.CollisionRect) && !(obj as Missile).PlayerMissile)
                {
                    try
                    {
                        if (PixelCollision(obj))
                        {
                            if (invinsible == false)
                            {
                                lives -= 1;
                                invinsible = true;
                                for (int i = 0; i < 9; i++)
                                {
                                    Space.AddObjects.Add(new Partikle(position));
                                }
                                position = new Vector2(Space.Gamewindow.ClientBounds.Width / 2, Space.Gamewindow.ClientBounds.Height / 2);
                                velocity = Vector2.Zero;
                                Space.RemoveObjects.Add(obj);
                                (obj as Enemy).DeathSpawn(this);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (invinsible == false)
                        {
                            lives -= 1;
                            invinsible = true;
                            for (int i = 0; i < 9; i++)
                            {
                                Space.AddObjects.Add(new Partikle(position));
                            }
                            position = new Vector2(Space.Gamewindow.ClientBounds.Width / 2, Space.Gamewindow.ClientBounds.Height / 2);
                            velocity = Vector2.Zero;
                            Space.RemoveObjects.Add(obj);
                            (obj as Enemy).DeathSpawn(this);
                        }
                    }
                }
                #endregion

            }
            base.HandleCollision();
        }
    }
}
