using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AstroidsArcadeClone
{
    class Animation
    {
        private Vector2 offset;
        private float fps;
        private Rectangle[] rectangles;
        private Color[][] colors;

        public Color[][] Colors
        {
            get { return colors; }
            set { colors = value; }
        }
        public Vector2 Offset
        {
            get { return offset; }
        }
        public Rectangle[] Rectangles
        {
            get { return rectangles; }
        }
        public float Fps
        {
            get { return fps; }
        }

        public Animation(int frames, int yPos, int xStartFrame, int width, int height, Vector2 offset, float fps, Texture2D texture)
        {
            rectangles = new Rectangle[frames];

            colors = new Color[frames][];

            for (int i = 0; i < frames; i++)
            {
                colors[i] = new Color[width * height];
                rectangles[i] = new Rectangle((i + xStartFrame) * width, yPos, width, height);
                texture.GetData<Color>(0, rectangles[i], colors[i], 0, width * height);
            }

            this.fps = fps;

            this.offset = offset;
        }
    }
}
