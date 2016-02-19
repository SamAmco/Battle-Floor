using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Battle_Floor
{
    class BasicEnemy : EnemyBase
    {
        float SpinSpeed = 1;
        Vector2 CharPos;
        Vector2 Velocity = new Vector2(0, 0);
        int MovementTimer = 0;
        
        int Health = 100;
        bool dead = false;
        bool Destroy = false;
        bool Hit = false;
            bool h = false;

        public BasicEnemy(Texture2D Tex, Vector2 Pos, Point FrameSize, Vector2 RC, float RCDeg, Color Col) 
            : base(Tex, Pos, FrameSize, RC, RCDeg, Col, 'L')
        {
        }
        
        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            UpdateRotation(gameTime);
            UpdatePos(gameTime, ClientBounds);
            if (Health <= 0)
            {
                dead = true;
                Hit = false;
            }
        }

        private void UpdatePos(GameTime gameTime, Rectangle ClientBounds)
        {
            #region Horizontal
            CharPos = Game1.mainCharacter.GetPos();

            MovementTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (MovementTimer >= 100)
            {
                MovementTimer = 0;

                if (Pos.X > CharPos.X + 170)
                {
                    Velocity.X -= 1;
                }
                else if (Pos.X <= CharPos.X - 100)
                {
                    Velocity.X += 1;
                }
            }
            if (dead)
            {
                if (CurrentFrame.X < 12)
                {
                    CurrentFrame.X += 1;
                }
                else if (CurrentFrame.X == 12 && CurrentFrame.Y < 1)
                {
                    CurrentFrame.Y += 1;
                    CurrentFrame.X = 0;
                }
                else if (CurrentFrame.X == 12 && CurrentFrame.Y == 1)
                {
                    Destroy = true;
                }
            }
            else if (Hit)
            {
                if (!h)
                {
                    if (CurrentFrame.X < 12)
                    {
                        CurrentFrame.X += 1;
                    }
                    else if (CurrentFrame.X == 12)
                    {
                        h = true;
                    }
                }
                else if (h)
                {
                    if (CurrentFrame.X > 0)
                        CurrentFrame.X -= 1;
                    else if (CurrentFrame.X == 0)
                    {
                        Hit = false;
                        h = false;
                    }
                }
            }

            if (CurrentFrame.X > 12)
                CurrentFrame.X = 12;
            if (CurrentFrame.Y > 1)
                CurrentFrame.Y = 1;

            Pos += Velocity;

            if (Pos.X > ClientBounds.Width)
                Pos.X = ClientBounds.Width;
            else if (Pos.X < 0)
                Pos.X = 0;
            #endregion
            #region Vertical
            if (Velocity.Y < 20)
                Velocity.Y += 1;
            if (Pos.Y < ClientBounds.Height - 35)
                Pos.Y += Velocity.Y;
            if (Pos.Y > ClientBounds.Height - 35)
            {
                Velocity.Y = 0;
                Pos.Y = ClientBounds.Height - 35;
            }
            #endregion
        }

        private void UpdateRotation(GameTime gameTime)
        {
            if (SpinSpeed < 5)
                SpinSpeed += 0.1f;

            RCDeg += MathHelper.ToRadians(SpinSpeed);
            if (RCDeg >= 360)
                RCDeg = 0;
        }
        #region returns
        public Vector2 GetPos()
        {
            return Pos;
        }
        public Rectangle GetRect()
        {
            return new Rectangle((int)Pos.X - 20, (int)Pos.Y - 20, 40, 40);
        }
        public void HitShield()
        {
            Velocity.X *= -1;
            Velocity.Y *= -1;
            Health -= 10;
        }
        public void HitWall()
        {
            Velocity *= -1;
            Hit = true;
            Health -= 10;
        }
        public void HitBall()
        {
            Velocity.X = 0;
            Hit = true;
            Health -= 50;
        }
        public void CombatHit(int Damage, int Xinc)
        {
            Hit = true;
            Velocity.X = Xinc;
            Health -= Damage;
        }
        public bool Dispose()
        {
            if (Destroy)
                Game1.mainCharacter.CreateDust(2, Pos);
            return Destroy;
        }
        public void PowerHit(string Type)
        {
            switch (Type)
            {
                case "Bullet":
                    {
                        Hit = true;
                        Health -= 50;
                        break;
                    }
                case "Rocket":
                    {
                        Hit = true;
                        Health -= 200;
                        break;
                    }
                case "MageBall":
                    {
                        Hit = true;
                        Health -= 100;
                        break;
                    }
                case "MageBlast":
                    {
                        Hit = true;
                        Health -= 200;
                        break;
                    }
                case "IMageBall":
                    {
                        Hit = true;
                        Health -= 200;
                        break;
                    }
                case "IceBlast":
                    {
                        Hit = true;
                        Health -= 400;
                        break;
                    }
            }
        }
        #endregion
    }
}
