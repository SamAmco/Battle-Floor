using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class MageBall : EnemyBase
    {
        Vector2 Velocity = new Vector2(15, 0);
        bool Destroy = false;
        string CurrentState = "Flight";
        Rectangle ARect;
        int FrameTimer = 0;

        public MageBall(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, char Direction)
            : base (Tex, Pos, FrameSize, CurrentFrame, Color.White, Direction)
        {
        }

        public bool Update(Rectangle ClientBounds, GameTime gameTime)
        {
            if (CurrentState == "Flight")
            {
                UpdatePos(ClientBounds);
            }
            else if (CurrentState == "Hit")
            {
                UpdateFrames(gameTime);
            }
            return Destroy;
        }

        private void UpdateFrames(GameTime gameTime)
        {
            FrameTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (FrameTimer > 0)
            {
                FrameTimer = 0;
                if (CurrentFrame.X < 3)
                    CurrentFrame.X += 1;
                else if (CurrentFrame.X == 3 && CurrentFrame.Y < 1)
                {
                    CurrentFrame.X = 0;
                    CurrentFrame.Y += 1;
                }
                else if (CurrentFrame.X == 3 && CurrentFrame.Y == 1)
                {
                    Destroy = true;
                }
            }
        }

        private void UpdatePos(Rectangle ClientBounds)
        {
            ARect = new Rectangle((int)Pos.X + 10, (int)Pos.Y + 10, 35, 30);
            if (Direction == 'R')
                Pos.X += Velocity.X;
            else if (Direction == 'L') 
                Pos.X -= Velocity.X;

            #region CheckBoundries
            if (Pos.X > ClientBounds.Width)
            {
                Destroy = true;
            }
            else if (Pos.X < -70)
            {
                Destroy = true;
            }
            if (Pos.Y < 0)
            {
                Destroy = true;
            }
            else if (Pos.Y > ClientBounds.Height)
            {
                Destroy = true;
            }
            #endregion

        }

        public Rectangle GetARect()
        {
            return ARect;
        }
        public void Hit(Vector2 CharPos)
        {
            Velocity.X = 0;
            Velocity.Y = 0;
            if (CharPos != Vector2.Zero)
                Pos = CharPos;
            CurrentState = "Hit";
            ARect = new Rectangle(0, 0, 0, 0);
        }
        public bool Dispose()
        {
            return Destroy;
        }
    }
}
