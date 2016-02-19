using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Basic_Man_Class : EnemyBase
    {
        #region Variables
        Texture2D StandMove;
        string CurrentState = "Running";
        int FrameTimer = 0;
        Vector2 Velocity = new Vector2(0, 0);
        int Multiplier = 1;
        #endregion

        public Basic_Man_Class(Texture2D StandMove, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, Char Direction, int Multiplier)
            : base(StandMove, Pos, FrameSize, CurrentFrame, Color.White, Direction)
        {
            this.StandMove = StandMove;
            this.Multiplier = Multiplier;
        }

        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            HandleState(ClientBounds, gameTime);
            HandleFrames(gameTime);
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
                            else if (Pos.X - 150 > MainCharPos.X + 35
                                && Rnd.Next(0, 1000) > (1000 - (5 * Multiplier)))
                            {
                                CurrentState = "Running";
                            }
                        }
                        else if (Direction == 'R')
                        {
                            if (MainCharPos.X + 35 < Pos.X + 35)
                            {
                                Direction = 'L';
                            }
                            else if (Pos.X + 220 < MainCharPos.X + 35
                                && Rnd.Next(0, 1000) > (1000 - (5 * Multiplier)))
                            {
                                CurrentState = "Running";
                            }
                        }
                        break;
                    }
                #endregion
            }
        }

        private void HandleFrames(GameTime gameTime)
        {
            FrameTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (FrameTimer >= 30)
            {
                FrameTimer = 0;
                switch (CurrentState)
                {
                    #region Running
                    case "Running":
                        {
                            {
                                if (CurrentFrame.X < 3)
                                    CurrentFrame.X += 1;
                                else if (CurrentFrame.X == 3 && CurrentFrame.Y < 1)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y += 1;
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
                            break;
                        }
                    #endregion
                }
            }
        }

        /*#region Returns
         public Vector2 GetPos()
         {
             return Pos;
         }
         public Rectangle GetARect()
         {
             if (RocketList.Count() > 0)
                 return RocketList.Last<Rocket>().GetARect();
             else
                 return new Rectangle(0, 0, 0, 0);
         }
         public Rectangle GetDRect()
         {
             return new Rectangle((int)Pos.X + 15, (int)Pos.Y + 20, 43, 8);
         }
         public void RocketHit(Vector2 CharPos)
         {
             RocketList.Last<Rocket>().Hit(CharPos);
         }
         public void HitShield()
         {
             if (Direction == 'R')
                 Pos.X -= 36;
             else if (Direction == 'L')
                 Pos.X += 6;
             CurrentState = "Launch";
             Tex = Launch;
             CurrentFrame.X = 0;
             CurrentFrame.Y = 0;
         }
         public void HitFireWall()
         {
             if (!Dying)
             {
                 CurrentState = "DieBack";
                 Tex = DieBack;
                 CurrentFrame.X = 0;
                 CurrentFrame.Y = 0;
                 Dying = true;
             }
         }
         public void CombatHit()
         {
             if (!Dying)
             {
                 if (Direction == 'L')
                 {
                     if (Game1.mainCharacter.GetPos().X < Pos.X)
                     {
                         CurrentState = "DieBack";
                         Tex = DieBack;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         Dying = true;
                     }
                     else
                     {
                         CurrentState = "DieForward";
                         Tex = DieForward;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         Dying = true;
                     }
                 }
                 else
                 {
                     if (Game1.mainCharacter.GetPos().X > Pos.X)
                     {
                         CurrentState = "DieBack";
                         Tex = DieBack;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         Dying = true;
                     }
                     else
                     {
                         CurrentState = "DieForward";
                         Tex = DieForward;
                         CurrentFrame.X = 0;
                         CurrentFrame.Y = 0;
                         Dying = true;
                     }
                 }
             }
         }
         public bool Dispose()
         {
             if (Destroy)
             {
                 Game1.mainCharacter.CreateDust(3,
                    new Vector2(Pos.X + 175, Pos.Y + 250));
             }
             return Destroy;
         }
         #endregion*/
    }
}
