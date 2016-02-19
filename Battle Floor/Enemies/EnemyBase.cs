using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class EnemyBase
    {
        protected Texture2D Tex;
        Color Col = Color.White;
        public Vector2 Pos;
        public Vector2 RC = new Vector2(0, 0);
        public float RCDeg = 0;
        public Point CurrentFrame = new Point(0, 0);
        protected Point FrameSize = new Point(0, 0);
        protected char Direction = 'L';
        protected float Scale = 1;


        public EnemyBase(Texture2D Tex, Vector2 Pos, Point FrameSize, Vector2 RC, float RCDeg,
            Color Col, char Direction)
        {
            this.Col = Col;
            this.RCDeg = RCDeg;
            this.RC = RC;
            this.Pos = Pos;
            this.Tex = Tex;
            this.FrameSize = FrameSize;
            this.Direction = Direction;
        }
        public EnemyBase(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, Color Col, char Direction)
        {
            this.Tex = Tex;
            this.Pos = Pos;
            this.FrameSize = FrameSize;
            this.CurrentFrame = CurrentFrame;
            this.Col = Col;
            this.Direction = Direction;
        }
        public EnemyBase(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, Color Col, char Direction, float Scale)
        {
            this.Tex = Tex;
            this.Pos = Pos;
            this.FrameSize = FrameSize;
            this.CurrentFrame = CurrentFrame;
            this.Col = Col;
            this.Direction = Direction;
            this.Scale = Scale;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Direction == 'L')
            {
                spriteBatch.Draw(Tex, Pos,
                    new Rectangle(CurrentFrame.X * FrameSize.X,
                        CurrentFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
                    Col, -RCDeg, RC, Scale, SpriteEffects.None, 1);
            }
            else if (Direction == 'R')
            {
                spriteBatch.Draw(Tex, Pos,
                    new Rectangle(CurrentFrame.X * FrameSize.X,
                        CurrentFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y),
                    Col, RCDeg, RC, Scale, SpriteEffects.FlipHorizontally, 1);
            }
        }
    }
}
