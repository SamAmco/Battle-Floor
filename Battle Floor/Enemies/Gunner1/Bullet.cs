using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Bullet : EnemyBase
    {
        Vector2 Velocity;
        Rectangle ARect;
        bool Destroy = false;

        public Bullet(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Vector2 RC, float RCDeg, char Direction, Vector2 Velocity)
            : base (Tex, Pos, FrameSize, RC, RCDeg, Color.White, Direction)
        {
            this.Velocity = Velocity;
        }

        public bool Update(Rectangle ClientBounds, GameTime gameTime)
        {
            UpdatePos(ClientBounds);
            UpdateRotation();
            return Destroy;
        }

        private void UpdatePos(Rectangle ClientBounds)
        {
            Pos.Y += Velocity.Y;
            if (Direction == 'R')
                Pos.X += Velocity.X;
            else if (Direction == 'L') 
                Pos.X -= Velocity.X;

            #region CheckBoundries
            if (Pos.X > ClientBounds.Width)
            {
                Destroy = true;
            }
            else if (Pos.X < 0)
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

        private void UpdateRotation()
        {
            float X = Velocity.X;
            float Y = Velocity.Y;
            ARect = new Rectangle(((int)RC.X + (int)Pos.X) - 10, ((int)RC.Y + (int)Pos.Y) - 5, 15, 10);
            if (Direction == 'L')
            {
                RCDeg = MathHelper.ToRadians((float)(90 +
                    MathHelper.ToDegrees((float)Math.Atan(Y / X))));
            }
            else if (Direction == 'R')
            {
                RCDeg = MathHelper.ToRadians((float)(90 +
                    MathHelper.ToDegrees((float)Math.Atan(Y / X))));
            }
        }

        public Rectangle GetARect()
        {
            return ARect;
        }
        public void Hit()
        {
            Destroy = true;
        }
        public bool Dispose()
        {
            return Destroy;
        }
    }
}
