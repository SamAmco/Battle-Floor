using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class IceGiant : EnemyBase
    {
        #region Variables
        Texture2D HitWall;
        Texture2D Running;
        Texture2D TakeHit;
        Texture2D Dying;
        string CurrentState = "Running";
        int RunningTimer = 0;
        Vector2 Velocity = new Vector2(0, 0);
        bool FrameIncrement = true;
        int DownTime = 0;
        int HitTimer = 0;
        int Timer = 0;
        Rectangle DRect = new Rectangle(0, 0, 0, 0);
        Rectangle ARect;
        bool TakenHit = false;
        int Health = 1100;
        int TakeHealth = 0;
        bool Destroy = false;
        bool CanTakeHit = true;
        bool MidField = false;
        bool PlayGiantSound = true;
        #endregion

        public IceGiant(Texture2D Tex, Texture2D HitWall,
            Texture2D TakeHit, Texture2D Dying, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, Char Direction) 
           : base (Tex, Pos, FrameSize, CurrentFrame, Color.Aqua, Direction)
        {
            this.HitWall = HitWall;
            this.Running = Tex;
            this.TakeHit = TakeHit;
            this.Dying = Dying;
            ARect = new Rectangle((int)Pos.X + 200, (int)Pos.Y + 100, 20, 150);
        }

        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            PlayGiantSound = true;
            HandleState(ClientBounds, gameTime);
            HandleFrames(gameTime);
        }

        //Collision Detection is handled here!!!!! P.S- Coheed and Cambria- The Suffering is a good song
        private void HandleState(Rectangle ClientBounds, GameTime gameTime)
        {
            if (TakenHit)
            {
                Health -= TakeHealth;
                TakeHealth = 0;
            }
            if (!CanTakeHit)
            {
                HitTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (HitTimer >= 1000)
                {
                    CanTakeHit = true;
                    HitTimer = 0;
                }
            }
            
            switch (CurrentState)
            {
                #region Running
                case "Running" :
                    {
                        if (Health <= 0)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Tex = Dying;
                            CurrentState = "Dying";
                        }
                        CanTakeHit = false;
                        if (Direction == 'R')
                        {
                            DRect = new Rectangle(0, 0, 0, 0);
                            ARect = new Rectangle((int)Pos.X + 150, (int)Pos.Y + 100, 70, 150);
                            Pos.X += Velocity.X;
                            if (Pos.X + 210 >= ClientBounds.Width)
                            {
                                Pos.X = ClientBounds.Width - 205;
                                CurrentState = "HitWall";
                                Tex = HitWall;
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Game1.enemies.PlaySound("GiantHeadCrash");
                            }
                        }
                        else if (Direction == 'L')
                        {
                            DRect = new Rectangle(0, 0, 0, 0);
                            ARect = new Rectangle((int)Pos.X + 130, (int)Pos.Y + 100, 70, 150);
                            Pos.X -= Velocity.X;
                            if (Pos.X + 135 <= 0)
                            {
                                Pos.X = -142;
                                CurrentState = "HitWall";
                                Tex = HitWall;
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Game1.enemies.PlaySound("GiantHeadCrash");
                            }
                        }
                        break;
                    }
                #endregion
                #region HitWall
                case "HitWall":
                    {
                        if (Health <= 0)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Tex = Dying;
                            CurrentState = "Dying";
                        }
                        if (Direction == 'L')
                        {
                            DRect = new Rectangle((int)Pos.X + 150, (int)Pos.Y + 105, 45, 145);
                            ARect = new Rectangle(0, 0, 0, 0);
                        }
                        else if (Direction == 'R')
                        {
                            DRect = new Rectangle((int)Pos.X + 150, (int)Pos.Y + 105, 45, 145);
                            ARect = new Rectangle(0, 0, 0, 0);
                        }
                        
                        if (!FrameIncrement)
                        {
                            Timer += gameTime.ElapsedGameTime.Milliseconds;
                            if (Timer >= DownTime)
                            {
                                Timer = 0;
                                DownTime = 0;
                                FrameIncrement = true;
                            }
                            else if (TakenHit && CanTakeHit)
                            {
                                CanTakeHit = false;
                                Tex = TakeHit;
                                CurrentState = "TakeHit";
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                            }
                        }
                        break;
                    }
                #endregion
                #region TakeHit
                case "TakeHit":
                    {
                        if (Health <= 0)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Tex = Dying;
                            CurrentState = "Dying";
                        }
                        CanTakeHit = false;
                        if (Direction == 'L')
                        {
                            DRect = new Rectangle(0, 0, 0, 0);
                            ARect = new Rectangle(0, 0, 0, 0);
                        }
                        else if (Direction == 'R')
                        {
                            DRect = new Rectangle(0, 0, 0, 0);
                            ARect = new Rectangle(0, 0, 0, 0);
                        }
                        
                        break;
                    }
                #endregion
                #region Dying
                case "Dying":
                    {

                        break;
                    }
                #endregion
            }
        }

        private void HandleFrames(GameTime gameTime)
        {
            RunningTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (RunningTimer >= 20)
            {
                RunningTimer = 0;
                switch (CurrentState)
                {
                    #region Running
                    case "Running":
                        {
                            {
                                if (CurrentFrame.X == 5)
                                    Game1.enemies.PlaySound("GiantStep");
                                if (CurrentFrame.X < 7)
                                    CurrentFrame.X += 1;
                                else if (CurrentFrame.X == 7 && CurrentFrame.Y < 1)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y += 1;
                                }
                                else if (CurrentFrame.X == 7 && CurrentFrame.Y == 1)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                }

                                if (CurrentFrame.X > 7)
                                    CurrentFrame.X = 7;
                                if (CurrentFrame.Y > 1)
                                    CurrentFrame.Y = 1;

                                Velocity.X = 20;
                            }
                            break;
                        }
                    #endregion
                    #region HitWall
                    case "HitWall":
                        {
                            if (FrameIncrement)
                            {
                                if (CurrentFrame.X < 4)
                                    CurrentFrame.X += 1;
                                else if (CurrentFrame.X == 4 && CurrentFrame.Y < 3)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y += 1;
                                }
                                if (CurrentFrame.X == 0 && CurrentFrame.Y == 2)
                                {
                                    FrameIncrement = false;
                                    Random Rnd = new Random();
                                    DownTime = (Rnd.Next(2, 5)) * 1000;
                                    DRect = new Rectangle((int)Pos.X + 140, (int)Pos.Y + 100, 10, 150);
                                    ARect = new Rectangle(0, 0, 0, 0);
                                }
                                else if (CurrentFrame.X == 4 && CurrentFrame.Y == 3)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                    if (Direction == 'L')
                                    {
                                        if (MidField)
                                            MidField = false;
                                        else
                                            Direction = 'R';
                                    }
                                    else if (Direction == 'R')
                                    {
                                        if (MidField)
                                            MidField = false;
                                        else
                                            Direction = 'L';
                                    }
                                    Tex = Running;
                                    CurrentState = "Running";
                                }
                            }
                            break;
                        }
                    #endregion
                    #region TakeHit
                    case "TakeHit":
                        {
                            if (CurrentFrame.X < 4)
                                CurrentFrame.X += 1;
                            else if (CurrentFrame.X == 4 && CurrentFrame.Y < 1)
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y += 1;
                            }
                            else if (CurrentFrame.X == 4 && CurrentFrame.Y == 1)
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 2;
                                TakenHit = false;
                                CurrentState = "HitWall";
                                Tex = HitWall;
                            }
                            break;
                        }
                    #endregion
                    #region Dying
                    case "Dying":
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
                                Destroy = true;
                            }
                            break;
                        }
                    #endregion
                }
            }
        }

        #region Returns
        public Vector2 GetPos()
        {
            return Pos;
        }
        public Rectangle GetARect()
        {
            return ARect;
        }
        public Rectangle GetDRect()
        {
            return DRect;
        }
        public void HitShield()
        {
            if (Direction == 'R')
                Pos.X -= 50;
            else if (Direction == 'L')
                Pos.X += 40;
            CurrentState = "HitWall";
            Tex = HitWall;
            CurrentFrame.X = 0;
            CurrentFrame.Y = 0;
            MidField = true;
            if (PlayGiantSound)
            {
                Game1.enemies.PlaySound("GiantHeadCrash");
                Game1.enemies.PlaySound("Shield");
                PlayGiantSound = false;
            }
        }
        public void HitFireWall()
        {
            if (Direction == 'R')
                Pos.X -= 50;
            else if (Direction == 'L')
                Pos.X += 50;
            CurrentState = "HitWall";
            Tex = HitWall;
            CurrentFrame.X = 0;
            CurrentFrame.Y = 0;
            TakeHealth = 10;
            MidField = true;
            if (PlayGiantSound)
            {
                Game1.enemies.PlaySound("GiantHeadCrash");
                Game1.enemies.PlaySound("Shield");
                PlayGiantSound = false;
            }
        }
        public void CombatHit(int Damage)
        {
            TakenHit = true;
            TakeHealth = Damage;
        }
        public bool Dispose()
        {
            if (Destroy)
            {
                Game1.mainCharacter.CreateDust(20,
                   new Vector2(Pos.X + 175, Pos.Y + 250));
            }
            return Destroy;
        }
        public void PowerHit(string Type)
        {
            switch (Type)
            {
                case "Bullet":
                    {
                        TakenHit = true;
                        TakeHealth = 15;
                        break;
                    }
                case "Rocket":
                    {
                        TakenHit = true;
                        TakeHealth = 250;
                        break;
                    }
                case "MageBall":
                    {
                        TakenHit = true;
                        TakeHealth = 100;
                        break;
                    }
                case "MageBlast":
                    {
                        TakenHit = true;
                        TakeHealth = 200;
                        break;
                    }
                case "IMageBall":
                    {
                        TakenHit = true;
                        TakeHealth = 200;
                        break;
                    }
                case "IceBlast":
                    {
                        TakenHit = true;
                        TakeHealth = 300;
                        break;
                    }
            }
        }
        #endregion
    }
}
