using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Explosion : Powers
    {
        bool Increase = true;
        bool Dispose = false;

        public Explosion(Texture2D ExpTex, Vector2 Pos, Point CurrentFrame, Point FrameSize, float Scale)
            : base(ExpTex, Pos, CurrentFrame, FrameSize, Scale)
        {
        }

        public void Update(SpriteBatch spriteBatch, GameTime gameTime)
        {
            UpdateFrames(gameTime);
            Draw(spriteBatch);
        }

        private void UpdateFrames(GameTime gameTime)
        {
            if (Increase)
            {
                if (CurrentFrame.X < 3)
                    CurrentFrame.X += 1;
                else if (CurrentFrame.Y < 7 && CurrentFrame.X == 3)
                {
                    CurrentFrame.Y += 1;
                    CurrentFrame.X = 0;
                }
                else if (CurrentFrame.X == 3 && CurrentFrame.Y == 7)
                    Increase = false;
            }
            else
            {
                if (CurrentFrame.X > 0)
                    CurrentFrame.X -= 1;
                else if (CurrentFrame.X == 0 && CurrentFrame.Y > 4)
                {
                    CurrentFrame.X = 3;
                    CurrentFrame.Y -= 1;
                }
                else if (CurrentFrame.X == 0 && CurrentFrame.Y == 4)
                {
                    Dispose = true;
                }
            }
        }

        public bool Destroy()
        {
            return Dispose;
        }
    }
}
