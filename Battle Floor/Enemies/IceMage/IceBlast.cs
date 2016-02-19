using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class IceBlast : EnemyBase
    {
        bool Destroy = false;
        Rectangle ARect;
        int FrameTimer = 0;
        bool Decline = false;

        public IceBlast(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, char Direction)
            : base(Tex, Pos, FrameSize, CurrentFrame, Color.White, Direction)
        {
        }

        public bool Update(GameTime gameTime)
        {
            UpdateFrames(gameTime);
            return Destroy;
        }

        private void UpdateFrames(GameTime gameTime)
        {
            #region Frames
            FrameTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (FrameTimer > 10)
            {
                FrameTimer = 0;
                if (!Decline)
                {
                    if (CurrentFrame.Y < 7)
                    {
                        CurrentFrame.Y += 1;
                    }
                    else if (CurrentFrame.Y == 7)
                    {
                        Decline = true;
                    }
                }
                else if (Decline)
                {
                    Destroy = true;
                    if (CurrentFrame.Y > 0)
                    {
                        CurrentFrame.Y -= 1;
                    }
                    else if (CurrentFrame.Y == 0)
                    {
                        Destroy = true;
                    }
                }
            }
            #endregion
            #region ColRect
            if (Direction == 'L')
            {
                switch (CurrentFrame.Y)
                {
                    case 0:
                        ARect = new Rectangle((int)Pos.X + 710, (int)Pos.Y + 15, 90, 15);
                        break;
                    case 1:
                        ARect = new Rectangle((int)Pos.X + 630, (int)Pos.Y + 15, 170, 15);
                        break;
                    case 2:
                        ARect = new Rectangle((int)Pos.X + 555, (int)Pos.Y + 15, 245, 15);
                        break;
                    case 3:
                        ARect = new Rectangle((int)Pos.X + 460, (int)Pos.Y + 15, 340, 15);
                        break;
                    case 4:
                        ARect = new Rectangle((int)Pos.X + 385, (int)Pos.Y + 15, 415, 15);
                        break;
                    case 5:
                        ARect = new Rectangle((int)Pos.X + 330, (int)Pos.Y + 15, 470, 15);
                        break;
                    case 6:
                        ARect = new Rectangle((int)Pos.X + 145, (int)Pos.Y + 15, 655, 15);
                        break;
                    case 7:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 15, 15);
                        break;
                }
            }
            else
            {
                switch (CurrentFrame.Y)
                {
                    case 0:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 90, 15);
                        break;
                    case 1:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 170, 15);
                        break;
                    case 2:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 245, 15);
                        break;
                    case 3:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 340, 15);
                        break;
                    case 4:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 415, 15);
                        break;
                    case 5:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 470, 15);
                        break;
                    case 6:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 655, 15);
                        break;
                    case 7:
                        ARect = new Rectangle((int)Pos.X, (int)Pos.Y + 15, 800, 15);
                        break;
                }
            }
            #endregion
        }
        public Rectangle GetARect()
        {
            if (!Decline)
                return ARect;
            else
                return new Rectangle(0, 0, 0, 0);
        }
        public void Hit()
        {
            Decline = true;
        }
        public bool IsInclined()
        {
            if (Decline)
                return false;
            else return true;
        }
        public bool Dispose()
        {
            return Destroy;
        }
    }
}
