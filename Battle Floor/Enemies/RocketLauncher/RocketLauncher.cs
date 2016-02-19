using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class RocketLauncher : EnemyBase
    {
        #region Variables
        Texture2D StandMove;
        Texture2D Launch;
        Texture2D RocketTex;
        Texture2D DieForward;
        Texture2D DieBack;
        Texture2D Explosion;
        bool Dying;
        bool Destroy = false;
        int DeathTimer = 0;
        string CurrentState = "Running";
        int FrameTimer = 0;
        Vector2 Velocity = new Vector2(0, 0);
        bool LaunchDec = false;
        int Multiplier = 1;
        Rectangle ClientBounds;

        List<Rocket> RocketList = new List<Rocket>();
        #endregion

        public RocketLauncher(Texture2D StandMove, Texture2D Launch, Texture2D RocketTex,
            Texture2D DieBack, Texture2D DieForward, Texture2D Explosion, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, Char Direction, int Multiplier)
            : base(StandMove, Pos, FrameSize, CurrentFrame, Color.White, Direction)
        {
            this.StandMove = StandMove;
            this.Launch = Launch;
            this.RocketTex = RocketTex;
            this.Multiplier = Multiplier;
            this.DieBack = DieBack;
            this.DieForward = DieForward;
            this.Explosion = Explosion;
        }

        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            HandleState(ClientBounds, gameTime);
            HandleFrames(gameTime);
            for (int i = 0; i < RocketList.Count(); i++)
            {
                if (RocketList[i].Update(ClientBounds, gameTime))
                    RocketList.Remove(RocketList[i]);
            }
            if (Dying)
            {
                DeathTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (DeathTimer >= 5000)
                {
                    Destroy = true;
                }
            }
            this.ClientBounds = ClientBounds;
        }

        //Collision Detection is handled here!!!!! P.S- Coheed and Cambria- The Suffering is a good song
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
                        if (Rnd.Next(0, 1000) < 10 * Multiplier)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Tex = Launch;
                            CurrentState = "Launch";
                        }
                        break;
                    }
                #endregion
                #region Launch
                case "Launch":
                    {
                        Random Rnd = new Random();
                        Vector2 MainCharPos = Game1.mainCharacter.GetPos();
                        if (Direction == 'L')
                        {
                            if (Rnd.Next(0, 10000) > 10000 - Math.Pow(Multiplier - 5, 2))
                            {
                                Tex = StandMove;
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                CurrentState = "Standing";
                                LaunchDec = false;
                            }
                            if (MainCharPos.X + 35 > Pos.X + 35)
                            {
                                Direction = 'R';
                            }
                        }
                        else if (Direction == 'R')
                        {
                            if (Rnd.Next(0, 10000) > 10000 - Math.Pow(Multiplier - 5, 2))
                            {
                                Tex = StandMove;
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                CurrentState = "Standing";
                                LaunchDec = false;
                            }
                            if (MainCharPos.X + 35 < Pos.X + 35)
                            {
                                Direction = 'L';
                            }
                        }
                        break;
                    }
                #endregion
                #region Dying
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
                                if (CurrentFrame.X < 4)
                                    CurrentFrame.X += 1;
                                else if (CurrentFrame.X == 4 && CurrentFrame.Y < 1)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y += 1;
                                    Game1.enemies.PlaySound("Step");
                                }
                                else if (CurrentFrame.X == 4 && CurrentFrame.Y == 1)
                                {
                                    CurrentFrame.X = 1;
                                    CurrentFrame.Y = 0;
                                }

                                if (CurrentFrame.X > 4)
                                    CurrentFrame.X = 4;
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
                    #region Launch
                    case "Launch":
                        {
                            if (!LaunchDec)
                            {
                                if (CurrentFrame.X < 6)
                                    CurrentFrame.X += 1;
                                else if (CurrentFrame.X == 6 && CurrentFrame.Y < 4)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y += 1;
                                }
                                if (CurrentFrame.X == 1 && CurrentFrame.Y == 4)
                                {
                                    if (Direction == 'L')
                                    {
                                        RocketList.Add(new Rocket(RocketTex, Explosion, new Vector2(Pos.X + 20, Pos.Y + 15),
                                            new Point(10, 30), new Vector2(15, 5), MathHelper.ToRadians(90), Direction));
                                        Game1.enemies.PlaySound("RocketLaunch");
                                    }
                                    else
                                    {
                                        RocketList.Add(new Rocket(RocketTex, Explosion, new Vector2(Pos.X + 20, Pos.Y + 35),
                                            new Point(10, 30), new Vector2(15, 5), MathHelper.ToRadians(90), Direction));
                                        Game1.enemies.PlaySound("RocketLaunch");
                                    }
                                }
                                else if (CurrentFrame.X == 6 && CurrentFrame.Y == 4)
                                {
                                    LaunchDec = true;
                                }
                            }
                            else
                            {
                                if (CurrentFrame.X > 0)
                                    CurrentFrame.X -= 1;
                                else if (CurrentFrame.X == 0 && CurrentFrame.Y > 2)
                                {
                                    CurrentFrame.X = 6;
                                    CurrentFrame.Y -= 1;
                                }
                                else if (CurrentFrame.X == 0 && CurrentFrame.Y == 2)
                                {
                                    LaunchDec = false;
                                }
                            }
                            break;
                        }
                    #endregion
                    #region Dying
                    case "DieBack":
                        {
                            if (CurrentFrame.X < 4)
                                CurrentFrame.X += 1;
                            else if (CurrentFrame.X == 4 && CurrentFrame.Y < 2)
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y += 1;
                            }
                            else if (CurrentFrame.X == 4 && CurrentFrame.Y == 2)
                                Destroy = true;
                            break;
                        }
                    case "DieForward":
                        {
                            if (CurrentFrame.X < 3)
                                CurrentFrame.X += 1;
                            else if (CurrentFrame.X == 3 && CurrentFrame.Y < 2)
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y += 1;
                            }
                            else if (CurrentFrame.X == 3 && CurrentFrame.Y == 2)
                                Destroy = true;
                            break;
                        }
                    #endregion
                }
            }
        }

        public void DrawRockets(SpriteBatch spriteBatch)
        {
            foreach (Rocket r in RocketList)
            {
                r.Draw(spriteBatch);
            }
        }

       #region Returns
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
            if (!Dying)
                return new Rectangle((int)Pos.X + 15, (int)Pos.Y + 15, 43, 18);
            else
                return new Rectangle(0, 0, 0, 0);
        }
        public bool ColDet()
        {
            if (RocketList.Last<Rocket>().currentState == "Flight")
                return true;
            else
                return false;
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
                Pos.X += 36;
            
            if (Pos.X + 70 <= ClientBounds.Width
               && Pos.X > 0)
            {
                CurrentState = "Launch";
                Tex = Launch;
                CurrentFrame.X = 0;
                CurrentFrame.Y = 0;
            }
        }
        public void HitFireWall()
        {
            if (CurrentState == "DieBack" || CurrentState == "DieForward")
                Destroy = true;
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
            if (CurrentState == "DieBack" || CurrentState == "DieForward")
                Destroy = true;
            else if (Direction == 'L')
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
        public bool Dispose()
        {
            if (Destroy)
            {
                Game1.mainCharacter.CreateDust(2,
                   new Vector2(Pos.X + 35, Pos.Y + 25));
            }
            return Destroy;
        }
        public void PowerHit(string Type)
        {
            switch (Type)
            {
                #region Bullet
                case "Bullet":
                    {
                        if (CurrentState == "DieBack" || CurrentState == "DieForward")
                            Destroy = true;
                        else if (Direction == 'L')
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
                        break;
                    }
                #endregion
                #region Rocket
                case "Rocket":
                    {
                        if (CurrentState == "DieBack" || CurrentState == "DieForward")
                            Destroy = true;
                        else if (Direction == 'L')
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
                        break;
                    }
                #endregion
                #region MageBall
                case "MageBall":
                    {
                        if (CurrentState == "DieBack" || CurrentState == "DieForward")
                            Destroy = true;
                        else if (Direction == 'L')
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
                        break;
                    }
                #endregion
                #region MageBlast
                case "MageBlast":
                    {
                        if (CurrentState == "DieBack" || CurrentState == "DieForward")
                            Destroy = true;
                        else if (Direction == 'L')
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
                        break;
                    }
                #endregion
                #region IceBlast
                case "IceBlast":
                    {
                        if (CurrentState == "DieBack" || CurrentState == "DieForward")
                            Destroy = true;
                        else if (Direction == 'L')
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
                        break;
                    }
                #endregion
                #region IMageBall
                case "IMageBall":
                    {
                        if (CurrentState == "DieBack" || CurrentState == "DieForward")
                            Destroy = true;
                        else if (Direction == 'L')
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
                        break;
                    }
                #endregion
            }
        }
        #endregion
    }
}