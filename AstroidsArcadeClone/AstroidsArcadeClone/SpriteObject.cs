using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    public abstract class SpriteObject
    {
        protected Texture2D texture;
        protected Rectangle[] rectangles;
        protected Vector2 position;
        protected Vector2 origin = Vector2.Zero;
        private float layer = 0;
        protected float scale = 1;
        private Color color = Color.White;
        private SpriteEffects effect = SpriteEffects.None;
        protected float rotation = 0;
        protected Vector2 velocity = Vector2.Zero;
        protected float speed = 50;
        private int currentIndex;
        private float timeElapsed;
        private float fps = 10;
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        protected Vector2 offset = Vector2.Zero;
        private Texture2D boxTexture;
        private string name;
        protected int frames = 1;

        public int Frames
        {
            get { return frames; }
            set { frames = value; }
        }
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle
                (
                    (int)(position.X + offset.X - texture.Width / frames / 2),
                    (int)(position.Y + offset.Y - texture.Height / 2),
                    rectangles[0].Width, rectangles[0].Height
                );
            }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public SpriteObject(Vector2 position)
        {
            this.position = position;
        }
        public virtual void LoadContent(ContentManager content)
        {
            origin = new Vector2(texture.Width / frames / 2, texture.Height / 2);

            boxTexture = content.Load<Texture2D>(@"CollisionTexture");
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position + offset, rectangles[currentIndex], color, rotation, origin, scale, effect, layer);

#if DEBUG
            Rectangle topLine = new Rectangle(CollisionRect.X, CollisionRect.Y, CollisionRect.Width, 1);
            Rectangle bottomLine = new Rectangle(CollisionRect.X, CollisionRect.Y + CollisionRect.Height, CollisionRect.Width, 1);
            Rectangle rightLine = new Rectangle(CollisionRect.X + CollisionRect.Width, CollisionRect.Y, 1, CollisionRect.Height);
            Rectangle leftLine = new Rectangle(CollisionRect.X, CollisionRect.Y, 1, CollisionRect.Height);

            spriteBatch.Draw(boxTexture, topLine, Color.Red);
            spriteBatch.Draw(boxTexture, bottomLine, Color.Red);
            spriteBatch.Draw(boxTexture, rightLine, Color.Red);
            spriteBatch.Draw(boxTexture, leftLine, Color.Red);
#endif
        }
        public virtual void Update(GameTime gametime)
        {
            timeElapsed += (float)gametime.ElapsedGameTime.TotalSeconds;

            currentIndex = (int)(timeElapsed * fps);

            if (currentIndex > rectangles.Length - 1)
            {
                timeElapsed = 0;
                currentIndex = 0;
            }
            HandleCollision();

            //ScreenWrap
            if (Position.X + Texture.Width / Frames / 2 < 0)
            {
                Position = new Vector2(Space.Gamewindow.ClientBounds.Width + Texture.Width / Frames / 2, Position.Y);
            }
            if (Position.Y + Texture.Height / 2 < 0)
            {
                Position = new Vector2(Position.X, Space.Gamewindow.ClientBounds.Height + Texture.Height / 2);
            }
            if (Position.X - Texture.Width / Frames / 2 > Space.Gamewindow.ClientBounds.Width)
            {
                Position = new Vector2(0 - Texture.Width / Frames / 2, Position.Y);
            }
            if (Position.Y - Texture.Height / 2 > Space.Gamewindow.ClientBounds.Height)
            {
                Position = new Vector2(Position.X, 0 - Texture.Height / 2);
            }
        }
        protected void CreateAnimation(string name, int frames, int yPos, int xStartFrame, int width, int height, Vector2 offset, float fps, Texture2D texture)
        {
            animations.Add(name, new Animation(frames, yPos, xStartFrame, width, height, offset, fps, texture));
        }
        protected void PlayAnimation(string name)
        {
            this.name = name;
            rectangles = animations[name].Rectangles;
            offset = animations[name].Offset;
            fps = animations[name].Fps;
        }
        protected bool PixelCollision(SpriteObject other)
        {
            int top = Math.Max(this.CollisionRect.Top, other.CollisionRect.Top);
            int bottom = Math.Min(this.CollisionRect.Bottom, other.CollisionRect.Bottom);
            int left = Math.Max(this.CollisionRect.Left, other.CollisionRect.Left);
            int right = Math.Min(this.CollisionRect.Right, other.CollisionRect.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorA = animations[name].Colors[currentIndex]
                    [(x - CollisionRect.Left) + (y - CollisionRect.Top) * CollisionRect.Width];
                    Color colorB = animations[other.name].Colors[other.currentIndex]
                    [(x - CollisionRect.Left) + (y - CollisionRect.Top) * CollisionRect.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        protected virtual void HandleCollision()
        {

        }
    }
}
