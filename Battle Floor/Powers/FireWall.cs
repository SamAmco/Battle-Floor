using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    public class FireWall : Powers
    {
        int FrameTimer = 0;
        bool Dispose = false;
        int Life = 0;

        public FireWall(Texture2D WallTex, Vector2 Pos, Point CurrentFrame, Point FrameSize, float Scale)
            : base(WallTex, Pos, CurrentFrame, FrameSize, Scale)
        {
        }

        public void Update(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Life += gameTime.ElapsedGameTime.Milliseconds;
            if (Life >= 30000)
                Dispose = true;
            UpdateFrames(gameTime);
            Draw(spriteBatch);
        }

        private void UpdateFrames(GameTime gameTime)
        {
            FrameTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (FrameTimer > 30)
            {
                FrameTimer = 0;
                if (CurrentFrame.X < 5)
                    CurrentFrame.X += 1;
                else if (CurrentFrame.X == 5 && CurrentFrame.Y < 1)
                {
                    CurrentFrame.X = 0;
                    CurrentFrame.Y += 1;
                }
                else if (CurrentFrame.X == 5 && CurrentFrame.Y == 1)
                {
                    CurrentFrame.X = 0;
                    CurrentFrame.Y = 1;
                }
            }
        }

        public bool Destroy()
        {
            return Dispose;
        }
        public Rectangle GetRect()
        {
            return new Rectangle((int)Position.X + 45, (int)Position.Y, 1, 650);
        }
        public void Kill()
        {
            Dispose = true;
        }
    }
}
