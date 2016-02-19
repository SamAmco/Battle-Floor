using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class PowerUpBase
    {
        protected Texture2D Tex2;
        protected Texture2D Tex;
        Color Col = Color.White;
        public Vector2 Pos;
        protected Vector2 Tex2Offset = new Vector2(0, 0);
        public Vector2 RC = new Vector2(0, 0);
        public float RCDeg = 0;
        public Point CurrentFrame = new Point(0, 0);
        protected Point FrameSize = new Point(0, 0);
        public Point CurrentFrame2 = new Point(0, 0);
        protected Point FrameSize2 = new Point(0, 0);
        protected char Direction = 'L';
        protected float Scale = 1;


        public PowerUpBase(Texture2D Tex, Texture2D Tex2, Vector2 Pos, Vector2 Tex2Offset,
            Point FrameSize, Point FrameSize2, Point CurrentFrame2)
        {
            this.Pos = Pos;
            this.Tex = Tex;
            this.Tex2 = Tex2;
            this.Tex2Offset = Tex2Offset;
            this.FrameSize = FrameSize;
            this.FrameSize2 = FrameSize2;
            this.CurrentFrame2 = CurrentFrame2;
        }
        public PowerUpBase(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Point CurrentFrame)
        {
            this.Tex = Tex;
            this.Pos = Pos;
            this.FrameSize = FrameSize;
            this.CurrentFrame = CurrentFrame;
        }
        public PowerUpBase(Texture2D Tex, Vector2 Pos, Point FrameSize)
        {
            this.Tex = Tex;
            this.Pos = Pos;
            this.FrameSize = FrameSize;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Direction == 'L')
            {
                spriteBatch.Draw(Tex, Pos,
                    new Rectangle(CurrentFrame.X * FrameSize.X,
                        CurrentFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
                    Col, -RCDeg, RC, Scale, SpriteEffects.None, 0);
            }
            else if (Direction == 'R')
            {
                spriteBatch.Draw(Tex, Pos,
                    new Rectangle(CurrentFrame.X * FrameSize.X,
                        CurrentFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
                    Col, RCDeg, RC, Scale, SpriteEffects.FlipHorizontally, 0);
            }
            if (Tex2 != null)
            {
                spriteBatch.Draw(Tex2, Pos + Tex2Offset,
                    new Rectangle(CurrentFrame2.X * FrameSize2.X,
                    CurrentFrame2.Y * FrameSize2.Y, FrameSize2.X, FrameSize2.Y),
                    Col, -RCDeg, RC, Scale, SpriteEffects.None, 0);
            }
        }
    }
}
