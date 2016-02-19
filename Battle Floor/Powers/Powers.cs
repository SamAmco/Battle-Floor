using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    public class Powers
    {
        Texture2D Texture;
        protected Vector2 Position;
        protected Point CurrentFrame;
        protected Point FrameSize;
        float Scale = 1;
        Rectangle Frame;
        Color color = Color.White;

        public Powers(Texture2D Texture, Vector2 Position, Point CurrentFrame, Point FrameSize, float Scale, Color color)
        {
            this.Texture = Texture;
            this.Position = Position;
            this.CurrentFrame = CurrentFrame;
            this.FrameSize = FrameSize;
            this.Scale = Scale;
            this.color = color;
        }

        public Powers(Texture2D Texture, Vector2 Position, Point CurrentFrame, Point FrameSize, float Scale)
        {
            this.Texture = Texture;
            this.Position = Position;
            this.CurrentFrame = CurrentFrame;
            this.FrameSize = FrameSize;
            this.Scale = Scale;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Frame = new Rectangle(CurrentFrame.X * FrameSize.X, CurrentFrame.Y * FrameSize.Y, FrameSize.X, FrameSize.Y);
            spriteBatch.Draw(Texture, Position, Frame, color, 0, Vector2.Zero, Scale, SpriteEffects.None, 1);
        }
    }
}
