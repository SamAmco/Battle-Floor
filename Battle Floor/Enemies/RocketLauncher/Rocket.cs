using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Rocket : EnemyBase
    {
        Vector2 Velocity = new Vector2(15, 0);
        bool Destroy = false;
        string CurrentState = "Flight";
        public string currentState
        {
            get
            {
                return CurrentState;
            }
        }

        Texture2D Explosion;
        Rectangle ARect;
        int FrameTimer = 0;

        public Rocket(Texture2D Tex, Texture2D Explosion, Vector2 Pos, Point FrameSize,
            Vector2 RC, float Rotation, char Direction)
            : base (Tex, Pos, FrameSize, RC, Rotation, Color.White, Direction)
        {
            this.Explosion = Explosion;
        }


        public bool Update(Rectangle ClientBounds, GameTime gameTime)
        {
            if (CurrentState == "Flight")
            {
                UpdatePos(ClientBounds);
                UpdateRotation();
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
                if (CurrentFrame.X < 5)
                    CurrentFrame.X += 1;
                else if (CurrentFrame.X == 5 && CurrentFrame.Y < 2)
                {
                    CurrentFrame.X = 0;
                    CurrentFrame.Y += 1;
                }
                else if (CurrentFrame.X == 5 && CurrentFrame.Y == 2)
                {
                    Destroy = true;
                }
            }
        }

        private void UpdateRotation()
        {
            float X = Velocity.X;
            float Y = Velocity.Y;
            if (Direction == 'L')
            {
                ARect = new Rectangle((int)Pos.X - 5, (int)Pos.Y + 10, 20, 10);
                RCDeg = MathHelper.ToRadians((float)(90 +
                    MathHelper.ToDegrees((float)Math.Atan(Y / X))));
            }
            else if (Direction == 'R')
            {
                ARect = new Rectangle((int)Pos.X + 5, (int)Pos.Y + 10, 20, 10);
                RCDeg = MathHelper.ToRadians((float)(90 +
                    MathHelper.ToDegrees((float)Math.Atan(Y / X))));
            }
        }

        private void UpdatePos(Rectangle ClientBounds)
        {
            Pos.Y += Velocity.Y;
            if (Direction == 'R')
                Pos.X += Velocity.X;
            else if (Direction == 'L') 
                Pos.X -= Velocity.X;

            if (Game1.mainCharacter.GetPos().Y + 20 > Pos.Y)
                Velocity.Y += 2f;
            else if (Game1.mainCharacter.GetPos().Y + 20 < Pos.Y)
                Velocity.Y -= 2f;

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
            else if (CharPos == Vector2.Zero && Direction == 'R')
            {
                Pos.Y -= 25;
                Pos.X -= 30;
            }
            else if (CharPos == Vector2.Zero && Direction == 'L')
            {
                Pos.Y -= 25;
            }

            RCDeg = MathHelper.ToRadians(0f);
            CurrentState = "Hit";
            Tex = Explosion;
            FrameSize = new Point(80, 80);
            ARect = new Rectangle(0, 0, 0, 0);
            Game1.enemies.PlaySound("RocketHit");
        }
    }
}
