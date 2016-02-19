using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Gunner : EnemyBase
    {
        #region Variables
        Texture2D StandMove;
        Texture2D BulletTex;
        Texture2D TakeHitTex;
        string CurrentState = "Running";
        int FrameTimer = 0;
        Vector2 Velocity = new Vector2(0, 0);
        int Multiplier = 1;
        int FireRate = 0;
        bool Destroy;
        int Health;
        bool FrameInc = true;
        int FireTimer = 0;
        int Dust;

        Arms arms;
        List<Bullet> BulletList = new List<Bullet>();
        #endregion

        public Gunner(Texture2D StandMove, Texture2D ArmsTex, Texture2D TakeHitTex, Texture2D BulletTex,
            Vector2 Pos, Point FrameSize, Point CurrentFrame, Char Direction, int Multiplier,
            int FireRate, int Dust, int Health)
            : base(StandMove, Pos, FrameSize, CurrentFrame, Color.White, Direction)
        {
            this.StandMove = StandMove;
            this.Multiplier = Multiplier;
            this.BulletTex = BulletTex;
            this.TakeHitTex = TakeHitTex;
            this.FireRate = FireRate;
            this.Dust = Dust;
            this.Health = Health;
            arms = new Arms(ArmsTex, new Vector2(Pos.X + 35, Pos.Y + 19), new Point(70, 50),
                new Vector2(35, 19), 0f, Direction, this);
        }

        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            if (Health <= 0 && CurrentState != "TakeHit")
            {
                CurrentFrame.X = 0;
                CurrentFrame.Y = 0;
                CurrentState = "TakeHit";
                Tex = TakeHitTex;
                FrameInc = true;
            }
            if (Health < -10)
                Destroy = true;
                
            HandleState(ClientBounds, gameTime);
            arms.Update(gameTime, Direction);
            HandleFrames(gameTime);

            for (int i = 0; i < BulletList.Count(); i++)
            {
                bool Dispose = BulletList[i].Update(ClientBounds, gameTime);
                if (Dispose)
                {
                    BulletList.Remove(BulletList[i]);
                }
            }
        }

        private void HandleState(Rectangle ClientBounds, GameTime gameTime)
        {
            switch (CurrentState)
            {
                #region Running
                case "Running":
                    {
                        Random Rnd = new Random();
                        if (Direction == 'R')
                        {
                            Pos.X += Velocity.X;

                            if (Pos.X + 70 >= ClientBounds.Width)
                            {
                                Pos.X = ClientBounds.Width - 70;
                                Direction = 'L';
                            }
                            else if (Rnd.Next(0, 1000) < 5 * Multiplier
                                && Pos.X + 70 <= ClientBounds.Width
                                && Pos.X > 0)
                            {
                                Direction = 'L';
                                CurrentState = "Standing";
                            }
                        }
                        else if (Direction == 'L')
                        {
                            Pos.X -= Velocity.X;

                            if (Pos.X <= 0)
                            {
                                Pos.X = 0;
                                Direction = 'R';
                            }
                            else if (Rnd.Next(0, 1000) < 5 * Multiplier
                                && Pos.X + 70 <= ClientBounds.Width
                                && Pos.X > 0)
                            {
                                Direction = 'R';
                                CurrentState = "Standing";
                            }
                        }
                        break;
                    }
                #endregion
                #region Standing
                case "Standing":
                    {
                        Random Rnd = new Random();
                        Vector2 MainCharPos = Game1.mainCharacter.GetPos();
                        if (Direction == 'L')
                        {
                            if (MainCharPos.X + 35 > Pos.X + 35)
                            {
                                Direction = 'R';
                            }
                            else if (Pos.X - 150 < MainCharPos.X + 35
                                && Rnd.Next(0, 1000) > (1000 - (5 * Multiplier)))
                            {
                                CurrentState = "Running";
                                Direction = 'R';
                            }
                        }
                        else if (Direction == 'R')
                        {
                            if (MainCharPos.X + 35 < Pos.X + 35)
                            {
                                Direction = 'L';
                            }
                            else if (Pos.X + 220 > MainCharPos.X + 35
                                && Rnd.Next(0, 1000) > (1000 - (5 * Multiplier)))
                            {
                                CurrentState = "Running";
                                Direction = 'L';
                            }
                        }
                        break;
                    }
                #endregion
            }
        }

        private void HandleFrames(GameTime gameTime)
        {
            switch (CurrentState)
            {
                #region Running
                    case "Running":
                        {
                            FrameTimer += gameTime.ElapsedGameTime.Milliseconds;
                            if (FrameTimer >= 30)
                            {
                                FrameTimer = 0;
                                if (CurrentFrame.X < 3)
                                    CurrentFrame.X += 1;
                                else if (CurrentFrame.X == 3 && CurrentFrame.Y < 1)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y += 1;
                                    Game1.enemies.PlaySound("Step");
                                }
                                else if (CurrentFrame.X == 3 && CurrentFrame.Y == 1)
                                {
                                    CurrentFrame.X = 1;
                                    CurrentFrame.Y = 0;
                                }

                                if (CurrentFrame.X > 3)
                                    CurrentFrame.X = 3;
                                if (CurrentFrame.Y > 1)
                                    CurrentFrame.Y = 1;

                                if (Multiplier > 3)
                                    Velocity.X = 6;
                                else if (Multiplier <= 3)
                                    Velocity.X = 4;
                            }
                            break;
                        }
                    #endregion
                #region Standing
                    case "Standing":
                        {
                            Velocity.X = 0;
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;

                            FireTimer += gameTime.ElapsedGameTime.Milliseconds;
                            if (FireTimer >= FireRate)
                            {
                                FireTimer = 0;
                                BulletList.Add(new Bullet(BulletTex, new Vector2(Pos.X + 35, Pos.Y + 19),
                                    new Point(6, 20), new Vector2(2, 7), arms.RCDeg, Direction, arms.GetVelocity()));
                                Game1.enemies.PlaySound("GunShot");
                            }
                            break;
                        }
                    #endregion
                #region TakeHit
                    case "TakeHit":
                        {
                            if (FrameInc)
                            {
                                if (CurrentFrame.X < 4)
                                    CurrentFrame.X += 1;
                                else if (CurrentFrame.X == 4 && CurrentFrame.Y < 2)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y += 1;
                                }
                                else if (CurrentFrame.X == 4 && CurrentFrame.Y == 2)
                                {
                                    if (Health <= 0)
                                        Destroy = true;
                                    else
                                        FrameInc = false;
                                }
                            }
                            else
                            {
                                if (CurrentFrame.X > 0)
                                    CurrentFrame.X -= 1;
                                else if (CurrentFrame.X == 0 && CurrentFrame.Y > 0)
                                {
                                    CurrentFrame.X = 4;
                                    CurrentFrame.Y -= 1; 
                                }
                                else if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                                {
                                    CurrentState = "Standing";
                                    Tex = StandMove;
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                }
                            }

                            if (CurrentFrame.X > 4)
                                CurrentFrame.X = 4;
                            if (CurrentFrame.Y > 2)
                                CurrentFrame.Y = 2;

                            break;
                        }
                    #endregion
            }
        }

        public void DrawArms(SpriteBatch spriteBatch)
        {
            if (CurrentState == "Standing")
                arms.Draw(spriteBatch);
        }

        public void DrawBullets(SpriteBatch spriteBatch)
        {
            foreach (Bullet b in BulletList)
            {
                b.Draw(spriteBatch);
            }
        }

         #region Returns
         public Vector2 GetPos()
         {
             return Pos;
         }
         public List<Rectangle> GetARect()
         {
             if (BulletList.Count() > 0)
             {
                 List<Rectangle> A = new List<Rectangle>();
                 foreach (Bullet b in BulletList)
                 {
                     A.Add(b.GetARect());
                 }
                 return A;
             }
             else
             {
                 List<Rectangle> A = new List<Rectangle>();
                 A.Add(new Rectangle(0, 0, 0, 0));
                 return A;
             }
         }
         public Rectangle GetDRect()
         {
             return new Rectangle((int)Pos.X + 15, (int)Pos.Y + 10, 30, 30);
         }
         public void BulletHit(int i)
         {
             BulletList[i].Hit();
         }
         public void HitShield()
         {
             if (Direction == 'R')
                 Pos.X -= 10;
             else if (Direction == 'L')
                 Pos.X += 10;
         }
         public void HitFireWall()
         {
             Health -= 1;
             CurrentFrame.X = 0;
             CurrentFrame.Y = 0;
             CurrentState = "TakeHit";
             Tex = TakeHitTex;
             FrameInc = true;
             if (Direction == 'R')
                 Pos.X -= 10;
             else if (Direction == 'L')
                 Pos.X += 10;
         }
         public void CombatHit(int Damage)
         {
             Health -= Damage;
             CurrentFrame.X = 0;
             CurrentFrame.Y = 0;
             CurrentState = "TakeHit";
             Tex = TakeHitTex;
             FrameInc = true;
         }
         public bool Dispose()
         {
             if (Destroy)
             {
                 Game1.mainCharacter.CreateDust(Dust,
                    new Vector2(Pos.X + 175, Pos.Y + 250));
             }
             return Destroy;
         }
         public char GetDirection()
         {
             return Direction;
         }
         public void PowerHit(string Type)
         {
             switch (Type)
             {
                 case "Bullet":
                     {
                         Health -= 20;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         CurrentState = "TakeHit";
                         Tex = TakeHitTex;
                         FrameInc = true;
                         break;
                     }
                 case "Rocket":
                     {
                         Health -= 250;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         CurrentState = "TakeHit";
                         Tex = TakeHitTex;
                         FrameInc = true;
                         break;
                     }
                 case "MageBall":
                     {
                         Health -= 100;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         CurrentState = "TakeHit";
                         Tex = TakeHitTex;
                         FrameInc = true;
                         break;
                     }
                 case "MageBlast":
                     {
                         Health -= 200;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         CurrentState = "TakeHit";
                         Tex = TakeHitTex;
                         FrameInc = true;
                         break;
                     }
                 case "IMageBall":
                     {
                         Health -= 200;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         CurrentState = "TakeHit";
                         Tex = TakeHitTex;
                         FrameInc = true;
                         break;
                     }
                 case "IceBlast":
                     {
                         Health -= 300;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         CurrentState = "TakeHit";
                         Tex = TakeHitTex;
                         FrameInc = true;
                         break;
                     }
             }
         }
         #endregion
    }
}
