using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    public class FireBall : Powers
    {
        char Direction;
        bool Dispose = false;
        int Speed = 15;
        bool Dying = false;
            bool d = true;

        public FireBall(Texture2D BallTex, Vector2 Pos, Point CurrentFrame,
            Point FrameSize, float Scale, char Direction)
            : base(BallTex, Pos, CurrentFrame, FrameSize, Scale)
        {
            this.Direction = Direction;
        }

        public void Update(SpriteBatch spriteBatch, Rectangle ClientBounds)
        {
            UpdateFrames();
            UpdatePos(ClientBounds);
            Draw(spriteBatch);
        }

        private void UpdatePos(Rectangle ClientBounds)
        {
            if (Direction == 'R')
                Position.X += Speed;
            else
                Position.X -= Speed;

            if (Position.X < -20 || Position.X > ClientBounds.Width)
            {
                Dying = true;
            }
        }

        private void UpdateFrames()
        {
            if (Direction == 'L')
            {
                if (!Dying)
                {
                    if (CurrentFrame.X < 3)
                    {
                        CurrentFrame.X += 1;
                    }
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y < 4)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y += 1;
                    }
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y == 4)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 1;
                    }
                }
                else if (Dying)
                {
                    if (d)
                    {
                        CurrentFrame.Y = 10;
                        CurrentFrame.X = 0;
                        d = false;
                    }
                    if (CurrentFrame.X < 3)
                        CurrentFrame.X += 1;
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y < 11)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y += 1;
                    }
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y == 11)
                    {
                        Dispose = true;
                    }
                }
            }
            else if (Direction == 'R')
            {
                if (!Dying)
                {
                    if (CurrentFrame.X < 3)
                    {
                        CurrentFrame.X += 1;
                    }
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y < 9)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y += 1;
                    }
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y == 9)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 6;
                    }
                }
                else if (Dying)
                {
                    if (d)
                    {
                        CurrentFrame.Y = 12;
                        CurrentFrame.X = 0;
                        d = false;
                    }
                    if (CurrentFrame.X < 3)
                        CurrentFrame.X += 1;
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y < 13)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y += 1;
                    }
                    else if (CurrentFrame.X == 3 && CurrentFrame.Y == 13)
                    {
                        Dispose = true;
                    }
                }
            }
        }

        public bool Destroy()
        {
            return Dispose;
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)Position.X + 6, (int)Position.Y, 58, 43);
        }
        public void Kill()
        {
            Dying = true;
        }
        public void KillNow()
        {
            Dispose = true;
        }
    }

}
