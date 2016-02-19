using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class MageHat : EnemyBase
    {
        bool FrameIncrement = true;
        int Timer;
        bool Destroy = false;

        public MageHat(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, char Direction, float Scale)
            : base(Tex, Pos, FrameSize, CurrentFrame, Color.White, Direction, Scale)
        {
        }

        public void Update(GameTime gameTime)
        {
            UpdateFrames();
            DisposeCountDown(gameTime);
        }
        private void DisposeCountDown(GameTime gameTime)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            if (Timer >= 10000)
            {
                Destroy = true;
            }
        }
        public bool Dispose()
        {
            return Destroy;
        }
        private void UpdateFrames()
        {
            if (FrameIncrement)
            {
                if (CurrentFrame.X < 5)
                    CurrentFrame.X += 1;
                else if (CurrentFrame.X == 5 && CurrentFrame.Y < 1)
                {
                    CurrentFrame.X = 0;
                    CurrentFrame.Y += 1;
                }
                else if (CurrentFrame.X == 5 && CurrentFrame.Y == 1)
                {
                    FrameIncrement = false;
                }
            }
            else
            {
                if (CurrentFrame.X > 0)
                    CurrentFrame.X -= 1;
                else if (CurrentFrame.X == 0 && CurrentFrame.Y > 0)
                {
                    CurrentFrame.X = 5;
                    CurrentFrame.Y -= 1;
                }
                else if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                {
                    FrameIncrement = true;
                }
            }
        }
        public Rectangle GetRect()
        {
            return new Rectangle((int)Pos.X, (int)Pos.Y,
                FrameSize.X, FrameSize.Y);
        }
        public void Kill()
        {
            Destroy = true;
        }
        public void PowerHit(string Type)
        {
            Game1.mainCharacter.CreateDust(50, Pos);
            Destroy = true;
        }
    }
}

