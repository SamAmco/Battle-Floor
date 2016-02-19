using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class IceMage : EnemyBase
    {
        #region Variables
        Texture2D StandMove;
        Texture2D CastBall;
        Texture2D MageBallTex;
        Texture2D MageBlastTex;
        Texture2D Shield;
        Texture2D ShieldM;
        string CurrentState = "Running";
        int FrameTimer = 0;
        Vector2 Velocity = new Vector2(0, 0);
        int Multiplier = 1;
        bool FrameIncrement = true;
        int Health = 800;
        bool Destroy = false;
        int CastMReps = 0;

        int ShieldTimer = 20000;

        List<IceBlast> BlastList = new List<IceBlast>();
        List<MageBall> BallList = new List<MageBall>();
        IceMageShield mageShield;
        #endregion

        public IceMage(Texture2D StandMove, Texture2D CastBall, Texture2D MageBallTex,
            Texture2D MageBlastTex, Texture2D ShieldM, Texture2D Shield, Vector2 Pos,
            Point FrameSize, Point CurrentFrame, Char Direction, int Multiplier)
            : base(StandMove, Pos, FrameSize, CurrentFrame, Color.White, Direction)
        {
            this.StandMove = StandMove;
            this.Multiplier = Multiplier;
            this.CastBall = CastBall;
            this.MageBallTex = MageBallTex;
            this.Shield = Shield;
            this.ShieldM = ShieldM;
            this.MageBlastTex = MageBlastTex;
        }

        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            if (Health <= 0)
                Destroy = true;
            HandleState(ClientBounds, gameTime);
            HandleFrames(gameTime);
            HandlePowers(ClientBounds, gameTime);
        }

        private void HandlePowers(Rectangle ClientBounds, GameTime gameTime)
        {
            #region MageBall
            for (int i = 0; i < BallList.Count(); i++)
            {
                if (BallList[i].Update(ClientBounds, gameTime))
                    BallList.Remove(BallList[i]);

            }
            #endregion
            #region Shield
            ShieldTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (ShieldTimer >= 20000)
            {
                if (Pos.X > 0 && Pos.X < ClientBounds.Width - 70)
                {
                    ShieldTimer = 0;
                    CurrentFrame.X = 0;
                    CurrentFrame.Y = 0;
                    Tex = ShieldM;
                    CurrentState = "Shield";
                    FrameIncrement = true;
                }
            }
            if (mageShield != null)
            {
                if (mageShield.Update(ClientBounds, gameTime))
                {
                    mageShield = null;
                }
            }
            #endregion
            #region MageBlast
            for (int i = 0; i < BlastList.Count(); i++)
            {
                if (BlastList[i].Update(gameTime))
                    BlastList.Remove(BlastList[i]);

            }
            #endregion
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
                        if (Rnd.Next(0, 1000) < 10 * Multiplier)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Tex = CastBall;
                            CurrentState = "CastBall";
                        }
                        else if (Rnd.Next(0, 1000) > (1000 - (10 * Multiplier)))
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Tex = CastBall;
                            CurrentState = "CastBlast";
                            CastMReps = 0;
                        }
                        if (mageShield == null)
                        {
                            if (Direction == 'L')
                                Direction = 'R';
                            else
                                Direction = 'L';
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Tex = StandMove;
                            CurrentState = "Running";
                        }
                        break;
                    }
                #endregion
                #region CastBall
                case "CastBall":
                {
                    break;
                }
                #endregion
                #region Shield
                case "Shield":
                {

                    break;
                }
                #endregion
                #region CastBlast
                case "CastBlast":
                {
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
                            break;
                        }
                    #endregion
                    #region CastBall
                    case "CastBall":
                        {
                            if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            {
                                Game1.enemies.PlaySound("IMageBallCharge");
                            }
                            if (CurrentFrame.X < 7)
                                CurrentFrame.X += 1;
                            else if (CurrentFrame.X == 7 && CurrentFrame.Y < 3)
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y += 1;
                            }
                            if (CurrentFrame.X == 0 && CurrentFrame.Y == 3)
                            {
                                BallList.Add(new MageBall(MageBallTex, Pos, new Point(70, 50),
                                    new Point(0, 0), Direction));
                                Game1.enemies.PlaySound("IMageBall");
                            }
                            else if (CurrentFrame.X == 7 && CurrentFrame.Y == 3)
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                CurrentState = "Standing";
                                Tex = StandMove;
                            }
                            break;
                        }
                    #endregion
                    #region Shield
                    case "Shield":
                    {
                        if (FrameIncrement)
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
                                FrameIncrement = false;
                                mageShield = new IceMageShield(Shield, Pos, new Point(45, 60), new Point(0, 0),
                                    this);
                                Game1.enemies.PlaySound("IMageCastShield");
                            }
                        }
                        else if (!FrameIncrement)
                        {
                            if (CurrentFrame.X > 0)
                                CurrentFrame.X -= 1;
                            else if (CurrentFrame.X == 0 && CurrentFrame.Y > 0)
                            {
                                CurrentFrame.X = 3;
                                CurrentFrame.Y -= 1;
                            }
                            else if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            {
                                FrameIncrement = true;
                                CurrentState = "Standing";
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Tex = StandMove;

                            }
                        }
                        break;
                    }
                    #endregion
                    #region CastBlast
                    case "CastBlast":
                    {
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            Game1.enemies.PlaySound("IMageBlastCharge");
                        if (CurrentFrame.X < 7)
                            CurrentFrame.X += 1;
                        else if (CastMReps < 3 && CurrentFrame.Y == 1)
                        {
                            CastMReps += 1;
                            CurrentFrame.X = 0;
                        }
                        else if (CurrentFrame.X == 7 && CurrentFrame.Y < 3)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y += 1;
                        }
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 3)
                        {
                            if (Direction == 'L')
                            {
                                BlastList.Add(new IceBlast(MageBlastTex, new Vector2(Pos.X - 800, Pos.Y),
                                    new Point(800, 50),
                                    new Point(0, 0), Direction));
                                Game1.enemies.PlaySound("IMageBlast");
                            }
                            else
                            {
                                BlastList.Add(new IceBlast(MageBlastTex, new Vector2(Pos.X + 70, Pos.Y),
                                    new Point(800, 50),
                                    new Point(0, 0), Direction));
                                Game1.enemies.PlaySound("IMageBlast");
                            }
                        }
                        else if (CurrentFrame.X == 7 && CurrentFrame.Y == 3)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            CurrentState = "Standing";
                            Tex = StandMove;
                        }
                        break;
                    }
                    #endregion
                }
            }
        }

        public void DrawMageBalls(SpriteBatch spriteBatch)
        {
            foreach (MageBall m in BallList)
            {
                m.Draw(spriteBatch);
            }
        }

        public void DrawShield(SpriteBatch spriteBatch)
        {
            if (mageShield != null)
                mageShield.Draw(spriteBatch);
        }

        public void DrawMageBlast(SpriteBatch spriteBatch)
        {
            foreach (IceBlast i in BlastList)
            {
                i.Draw(spriteBatch);
            }
        }

        #region Returns
        public Vector2 GetPos()
        {
            return Pos;
        }
        public List<Rectangle> GetARect()
        {
            if (BallList.Count() > 0)
            {
                List<Rectangle> A = new List<Rectangle>();
                foreach (MageBall m in BallList)
                {
                     A.Add(m.GetARect());
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
        public Rectangle GetBRect()
        {
            if (BlastList.Count() > 0)
                return BlastList.Last<IceBlast>().GetARect();
            else
                return new Rectangle(0, 0, 0, 0);
        }
        public Rectangle GetDRect()
        {
            return new Rectangle((int)Pos.X + 15, (int)Pos.Y + 15, 30, 18);
        }
        public void BallHit(Vector2 CharPos, int i)
        {
            BallList[i].Hit(CharPos);
        }
        public void BlastHit()
        {
            BlastList.Last<IceBlast>().Hit();
        }
        public bool ColDet(int i)
        {
            if (BallList[i].CurrentFrame.X == 0)
                return true;
            else
                return false;
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
            if (Direction == 'L')
                Pos.X += 10;
            else
                Pos.X -= 10;
        }
        public void CombatHit(int Damage, string AttackType)
        {
            if (AttackType == "FireBall")
            {
                if (mageShield != null)
                {
                    Vector2 MainCharPos = Game1.mainCharacter.GetPos();
                    if (MainCharPos.X + 35 > Pos.X + 35)
                    {
                        BallList.Add(new MageBall(MageBallTex, Pos, new Point(70, 50),
                                    new Point(0, 0), 'R'));
                    }
                    if (MainCharPos.X + 35 < Pos.X + 35)
                    {
                        BallList.Add(new MageBall(MageBallTex, Pos, new Point(70, 50),
                                    new Point(0, 0), 'L'));
                    }
                    mageShield.Hit(Damage / 2);
                }
                else
                    Health -= Damage;
            }
            else if (AttackType == "Demolition")
            {
                Health -= Damage;
            }
            //Temporary
            else
            {
                if (mageShield != null)
                    mageShield.Hit(Damage);
                else
                    Health -= Damage;
            }
        }
        public bool Dispose()
        {
            if (Destroy)
            {
                Game1.mainCharacter.CreateDust(35,
                   new Vector2(Pos.X + 35, Pos.Y + 25));
            }
            return Destroy;
        }
        public bool BlastIsInclined()
        {
            if (BlastList.Last<IceBlast>().IsInclined())
                return true;
            else
                return false;
        }
        public bool HasSheild()
        {
            if (mageShield != null)
                return true;
            else
                return false;
        }
        public void PowerHit(string Type)
        {
            switch (Type)
            {
                case "Bullet":
                    {
                        if (mageShield != null)
                            mageShield.Hit(10);
                        else
                            Health -= 10;
                        break;
                    }
                case "Rocket":
                    {
                        if (mageShield != null)
                            mageShield.Hit(250);
                        else
                            Health -= 250;
                        break;
                    }
                case "MageBall":
                    {
                        if (mageShield != null)
                            mageShield.Hit(100);
                        else
                            Health -= 250;
                        break;
                    }
                case "MageBlast":
                    {
                        if (mageShield != null)
                            mageShield.Hit(200);
                        else
                            Health -= 250;
                        break;
                    }
                case "IMageBall":
                    {
                        if (mageShield != null)
                            mageShield.Hit(200);
                        else
                            Health -= 200;
                        break;
                    }
                case "IceBlast":
                    {
                        if (mageShield != null)
                            mageShield.Hit(300);
                        else
                            Health -= 300;
                        break;
                    }
            }
        }
        #endregion
    }
}
