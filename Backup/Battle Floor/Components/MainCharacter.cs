using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Battle_Floor
{
    public class MainCharacter : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Variables
        KeyboardState PrevKeyState;
        KeyboardState CurrentKeyboardState;
        SpriteBatch spriteBatch;
        Texture2D Character;

        Texture2D Test;

        Texture2D HealthBack;
        Texture2D PowerBack;
        Texture2D Power;
        Texture2D Health;
        Texture2D Sights;
        Texture2D BulletTex;
        Texture2D RocketTex;
        Texture2D DetonationTex;
        Vector2 PowerPos = new Vector2(0, 0);
        Vector2 HealthPos = new Vector2(0, 0);
        bool GameOver = false;
        bool Dying = false;
        float CurrentXP = 0;
        int MaxXP = 0;
        int Healthi = 100;
        int Lives = 0;
        bool CanTakeHit = true;
        int HitRegenTimer = 0;
        int HealthTimer = 0;
        int XPCounter;
        SpriteFont spriteFont;

        string CurrentSpriteSheet = "StandMoveLR";
        Point CurrentFrame = new Point(0, 0);
        Point FrameSize = new Point(70, 50);
        Vector2 Speed = new Vector2(0, 10);
        Vector2 CharacterPosition;
        char Direction = 'R';
        bool FrameIncrement = false;

        int Gravity = 20;
        bool Jumping = false;
        Random MRnd = new Random();
        int CtrlMove;
        int RunningFR = 0;

        int IceTimer = 0;
        Color color = Color.White;
        bool IceHit = false;

        Rectangle ARect = new Rectangle(0, 0, 0, 0);
        Rectangle DRect = new Rectangle(0, 0, 0, 0);
        Rectangle BRect = new Rectangle(0, 0, 0, 0);
        int Damage = 0;

        public List<Shield> ShieldList = new List<Shield>();
        public List<FireWall> WallList = new List<FireWall>();
        public List<FireBall> BallList = new List<FireBall>();
        List<GoldDust> DustList = new List<GoldDust>();
        List<Explosion> ExpList = new List<Explosion>();
        List<PowerBall> PowList = new List<PowerBall>();

        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        Cue Running;
        Cue FireCrackle;
        Cue Music;
        bool StickHasHit = false;

        //For each new mode or weapon edit here 1 2 3 4 5
        enum CharacterMode {FireMage, HandGunner, AKGunner, Launcher, Mage, IceMage};
        CharacterMode Mode = CharacterMode.FireMage;

        //here 1 2 3 4 5
        public int HGAmmo { get; set; }
        public int AKAmmo { get; set; }
        public int LAmmo { get; set; }
        MCArms arms;
        MCMShield MShield;
        List<Bullet> BulletList = new List<Bullet>();
        List<MCRocket> RocketList = new List<MCRocket>();
        List<MageBall> MBallList = new List<MageBall>();
        List<MageBlast> MBlastList = new List<MageBlast>();
        List<MageBall> IMBallList = new List<MageBall>();
        List<IceBlast> IMBlastList = new List<IceBlast>();
        int FireRate = 500;
        int FireTimer = 500;
        public bool HasMageHat { get; set; }
        public bool HasIMageHat { get; set; }
        int CastMReps = 0;
        public struct Powers
        {
            public Rectangle ARect;
            public string type;
            public int index;
        }

        int Rain = 0;
        int Invincible = 0;

        int DeflectionTimer = 0;
        bool Deflection = false;
        #endregion
        ///<summary>
        ///mage and ice mage are overpowered, think about giving each hat a timer
        ///or an amount of power that can be used before a hat wares out
        ///these changes will affect PowerUp under ammo, and probably others...
        ///</summary>
        public MainCharacter(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        protected override void LoadContent()
        {
            Test = Game.Content.Load<Texture2D>(@"MainCharacter\untitled");
            Sights = Game.Content.Load<Texture2D>(@"MainCharacter\Sights");
            BulletTex = Game.Content.Load<Texture2D>(@"Enemies\Gunner1\Bullet");

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            spriteFont = Game.Content.Load<SpriteFont>("SpriteFont");

            CharacterPosition = new Vector2((Game.Window.ClientBounds.Width / 2) - 35,
                (Game.Window.ClientBounds.Height) - 60);
            
            Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
            HealthBack = Game.Content.Load<Texture2D>(@"MainCharacter\Stats\HealthBack");
            PowerBack = Game.Content.Load<Texture2D>(@"MainCharacter\Stats\PowerBack");
            Power = Game.Content.Load<Texture2D>(@"MainCharacter\Stats\Power");
            Health = Game.Content.Load<Texture2D>(@"MainCharacter\Stats\Health");
            RocketTex = Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\Rocket");
            DetonationTex = Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\Explosion");

            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            Music = soundBank.GetCue("Track01");

            base.LoadContent();
        }

        // here 1 2 3 4 5
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            if (!(HGAmmo > 0))
                HGAmmo = 0;
            if (!(AKAmmo > 0))
                AKAmmo = 0;
            if (!(LAmmo > 0))
                LAmmo = 0;
            if (!HasIMageHat)
                HasIMageHat = false;
            if (!HasMageHat)
                HasMageHat = false;
            PrevKeyState = Keyboard.GetState();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            PrevKeyState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            CheckInput(gameTime);
            UpdateCharacter(gameTime);
            HandelPowers(gameTime);
            UpdateBars(gameTime);
            PowerUps(gameTime);
            UpdateMusic();
            UpdateFrames(gameTime);
            audioEngine.Update();
            base.Update(gameTime);
        }

        private void UpdateMusic()
        {
            if (!Music.IsPlaying)
            {
                //Music.Play();
                //Music.Stop(AudioStopOptions.Immediate);
                //Music.Dispose();
            }
        }

        //here at the bottom 1 2 3 4 5
        private void UpdateBars(GameTime gameTime)
        {
            XPCounter += gameTime.ElapsedGameTime.Milliseconds;
            if (CurrentXP < MaxXP && XPCounter >= 100)
            {
                XPCounter = 0;
                if (MaxXP / 100 < 1)
                    CurrentXP += 1;
                else
                    CurrentXP += MaxXP / 100;
            }
            if (CurrentXP < 2)
            {
                CurrentXP = 1;
            }

            PowerPos.X = 500 - (((CurrentXP / MaxXP) *100) * 5);

            HealthTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (HealthTimer >= 1000)
            {
                HealthTimer = 0;
                if (Healthi < 100)
                {
                    Healthi += 1;
                }
            }
            if (Healthi > 100)
                Healthi = 100;
            HealthPos.X = 500 - (Healthi * 5);

            if (Healthi <= 0)
            {
                Dying = true;
                CurrentFrame.X = 0;
                CurrentFrame.Y = 0;
                CurrentSpriteSheet = "Death";
                Speed.X = 0;
                switch (Mode)
                {
                    case CharacterMode.FireMage:
                        {
                            Character = Game.Content.Load<Texture2D>(@"MainCharacter\Death");
                            break;
                        }
                    case CharacterMode.HandGunner:
                        {
                            Character = Game.Content.Load<Texture2D>(@"MainCharacter\GDeath");
                            break;
                        }
                    case CharacterMode.AKGunner:
                        {
                            Character = Game.Content.Load<Texture2D>(@"MainCharacter\AKDeath");
                            break;
                        }
                    case CharacterMode.Launcher:
                        {
                            Character = Game.Content.Load<Texture2D>(@"MainCharacter\LDeath");
                            break;
                        }
                    case CharacterMode.Mage:
                        {
                            Character = Game.Content.Load<Texture2D>(@"MainCharacter\MDeath");
                            break;
                        }
                    case CharacterMode.IceMage:
                        {
                            Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMDeath");
                            break;
                        }
                }
            }
        }

        //here DISPOSAL 1 2 3 4 5
        private void HandelPowers(GameTime gameTime)
        {
            #region Disposal
            for (int i = 0; i < ShieldList.Count(); i++)
            {
                bool Dispose = false;
                Dispose = ShieldList[i].Destroy();
                if (Dispose)
                {
                    ShieldList.Remove(ShieldList[i]);
                    if (FireCrackle != null)
                    {
                        if (FireCrackle.IsPlaying && WallList.Count() == 0)
                        {
                            FireCrackle.Stop(AudioStopOptions.AsAuthored);
                            FireCrackle = null;
                        }
                    }
                }
            }
            for (int i = 0; i < WallList.Count(); i++)
            {
                bool Dispose = false;
                Dispose = WallList[i].Destroy();
                if (Dispose)
                {
                    WallList.Remove(WallList[i]);
                    if (FireCrackle != null)
                    {
                        if (FireCrackle.IsPlaying && ShieldList.Count() == 0 && WallList.Count() == 0)
                        {
                            FireCrackle.Stop(AudioStopOptions.AsAuthored);
                            FireCrackle = null;
                        }
                    }
                }
            }
            for (int i = 0; i < ExpList.Count(); i++)
            {
                bool Dispose = false;
                Dispose = ExpList[i].Destroy();
                if (Dispose)
                    ExpList.Remove(ExpList[i]);
            }
            for (int i = 0; i < BallList.Count(); i++)
            {
                bool Dispose = false;
                Dispose = BallList[i].Destroy();
                if (Dispose)
                    BallList.Remove(BallList[i]);
            }
            for (int i = 0; i < PowList.Count(); i++)
            {
                bool Dispose = false;
                Dispose = PowList[i].Destroy();
                if (Dispose)
                    PowList.Remove(PowList[i]);
            }
            for (int i = 0; i < BulletList.Count(); i++)
            {
                bool Dispose = false;
                Dispose = BulletList[i].Dispose();
                if (Dispose)
                    BulletList.Remove(BulletList[i]);
            }
            for (int i = 0; i < RocketList.Count(); i++)
            {
                bool Dispose = false;
                Dispose = RocketList[i].Dispose();
                if (Dispose)
                    RocketList.Remove(RocketList[i]);
            }
            for (int i = 0; i < MBallList.Count(); i++)
            {
                bool Dispose = MBallList[i].Dispose();
                if (Dispose)
                    MBallList.Remove(MBallList[i]);
            }
            for (int i = 0; i < MBlastList.Count(); i++)
            {
                bool Dispose = MBlastList[i].Dispose();
                if (Dispose)
                    MBlastList.Remove(MBlastList[i]);
            }
            if (MShield != null)
            {
                if (MShield.Update(Game.Window.ClientBounds, gameTime, CharacterPosition))
                {
                    MShield = null;
                }
            }
            for (int i = 0; i < IMBallList.Count(); i++)
            {
                bool Dispose = IMBallList[i].Dispose();
                if (Dispose)
                    IMBallList.Remove(IMBallList[i]);
            }
            for (int i = 0; i < IMBlastList.Count(); i++)
            {
                bool Dispose = IMBlastList[i].Dispose();
                if (Dispose)
                    IMBlastList.Remove(IMBlastList[i]);
            }
            #endregion
            if (!Dying)
            {
                #region GoldDust
                for (int i = 0; i < DustList.Count(); i++)
                {
                    Rectangle Rect = new Rectangle((int)CharacterPosition.X + 10, (int)CharacterPosition.Y + 5, 50, 40);
                    Rectangle DustRect = new Rectangle((int)DustList[i].GetPos().X, (int)DustList[i].GetPos().Y, 10, 10);
                    if (DustRect.Intersects(Rect))
                    {
                        DustList.Remove(DustList[i]);
                        MaxXP += 1;
                        Healthi += 1;
                    }
                }
                #endregion
            }
        }

        //here 1 2 3 4 5
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);
            DrawCharacter();
            DrawBars();
            #region DrawPowers
            foreach (GoldDust g in DustList)
            {
                g.Update(spriteBatch, Game.Window.ClientBounds);
            }
            foreach (Shield s in ShieldList)
            {
                s.Update(spriteBatch, gameTime);
            }
            foreach (FireWall f in WallList)
            {
                f.Update(spriteBatch, gameTime);
            }
            foreach (FireBall f in BallList)
            {
                f.Update(spriteBatch, Game.Window.ClientBounds);
            }
            foreach (Explosion e in ExpList)
            {
                e.Update(spriteBatch, gameTime);
            }
            foreach (PowerBall p in PowList)
            {
                p.Update(spriteBatch, gameTime, CharacterPosition);
            }
            foreach (Bullet b in BulletList)
            {
                b.Update(Game.Window.ClientBounds, gameTime);
                b.Draw(spriteBatch);
            }
            foreach (MCRocket r in RocketList)
            {
                r.Update(Game.Window.ClientBounds, gameTime);
                r.Draw(spriteBatch);
            }
            foreach (MageBall m in MBallList)
            {
                m.Update(Game.Window.ClientBounds, gameTime);
                m.Draw(spriteBatch);
            }
            foreach (MageBlast m in MBlastList)
            {
                m.Update(gameTime);
                m.Draw(spriteBatch);
            }
            foreach (MageBall m in IMBallList)
            {
                m.Update(Game.Window.ClientBounds, gameTime);
                m.Draw(spriteBatch);
            }
            foreach (IceBlast m in IMBlastList)
            {
                m.Update(gameTime);
                m.Draw(spriteBatch);
            }
            #endregion
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //here 1 2 3 4 5
        private void DrawBars()
        {
            #region Standard
            spriteBatch.DrawString(spriteFont, "XP: " + MaxXP, new Vector2(520, 10),
                Color.BlanchedAlmond, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
            spriteBatch.DrawString(spriteFont, "Lives: " + Lives, new Vector2(520, 30),
                Color.BlanchedAlmond, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
            if (IceHit)
                spriteBatch.DrawString(spriteFont, "Frozen: " + (IceTimer / 1000), new Vector2(640, 30),
                    Color.BlanchedAlmond, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
            if (Invincible > 0)
                spriteBatch.DrawString(spriteFont, "Invincible: " + (Invincible / 1000), new Vector2(640, 10),
                    Color.BlanchedAlmond, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);

            spriteBatch.Draw(HealthBack, new Vector2(10, 30), null, Color.White, 0,
                Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(PowerBack, new Vector2(10, 10), null, Color.White, 0,
                Vector2.Zero, 1, SpriteEffects.None, 0);

            int PowerInc = 500;
            if ((int)(((CurrentXP / MaxXP) * 100) * 5) < 500)
            {
                PowerInc = (int)(((CurrentXP / MaxXP) * 100) * 5);
            }
            spriteBatch.Draw(Power, new Vector2(10, 10), new Rectangle(0, 0, PowerInc, 20),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(Health, new Vector2(10, 30), new Rectangle(0, 0, (Healthi * 5), 20),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            #endregion
            #region Modes
            Color FireMageCol = Color.BlanchedAlmond;
            Color HandGunnerCol = Color.BlanchedAlmond;
            Color AKGunnerCol = Color.BlanchedAlmond;
            Color LauncherCol = Color.BlanchedAlmond;
            Color MageCol = Color.BlanchedAlmond;
            Color IMageCol = Color.BlanchedAlmond;
            switch (Mode)
            {
                case CharacterMode.FireMage:
                    {
                        FireMageCol = Color.LightGreen;
                        break;
                    }
                case CharacterMode.HandGunner:
                    {
                        HandGunnerCol = Color.LightGreen;
                        spriteBatch.DrawString(spriteFont, "Ammo: " + HGAmmo,
                            new Vector2(640, 10), Color.BlanchedAlmond, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
                        break;
                    }
                case CharacterMode.AKGunner:
                    {
                        AKGunnerCol = Color.LightGreen;
                        spriteBatch.DrawString(spriteFont, "Ammo: " + AKAmmo,
                            new Vector2(640, 10), Color.BlanchedAlmond, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
                        break;
                    }
                case CharacterMode.Launcher:
                    {
                        LauncherCol = Color.LightGreen;
                        spriteBatch.DrawString(spriteFont, "Ammo: " + LAmmo,
                            new Vector2(640, 10), Color.BlanchedAlmond, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
                        break;
                    }
                case CharacterMode.Mage:
                    {
                        MageCol = Color.LightGreen;
                        break;
                    }
                case CharacterMode.IceMage:
                    {
                        IMageCol = Color.LightGreen;
                        break;
                    }
            }
            
            if (HGAmmo > 0 || AKAmmo > 0 || LAmmo > 0 || HasMageHat || HasIMageHat)
            {
                spriteBatch.DrawString(spriteFont, "1",
                    new Vector2(10, 50), FireMageCol,
                    0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);

                if (HGAmmo > 0)
                    spriteBatch.DrawString(spriteFont, "2",
                        new Vector2(30, 50), HandGunnerCol,
                        0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
                
                if (AKAmmo > 0)
                    spriteBatch.DrawString(spriteFont, "4",
                        new Vector2(70, 50), AKGunnerCol,
                        0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);

                if (LAmmo > 0)
                    spriteBatch.DrawString(spriteFont, "3",
                        new Vector2(50, 50), LauncherCol,
                        0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);

                if (HasMageHat)
                    spriteBatch.DrawString(spriteFont, "5",
                        new Vector2(90, 50), MageCol,
                        0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);

                if (HasIMageHat)
                    spriteBatch.DrawString(spriteFont, "6",
                        new Vector2(110, 50), IMageCol,
                        0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);

            }
            #endregion
            //spriteBatch.DrawString(spriteFont, "" + MBallList.Count(),
              //  new Vector2(100, 300), Color.BlanchedAlmond);
            //spriteBatch.Draw(Test, DRect, Color.White);
            //spriteBatch.Draw(Test, ARect, Color.Blue);
            //spriteBatch.Draw(Test, BRect, Color.Lavender);
            /*foreach (Bullet b in BulletList)
            {
                spriteBatch.Draw(Test, b.GetARect(), Color.White);
            }*/
        }

        private void UpdateCharacter(GameTime gameTime)
        {
            CharacterPosition.Y += Gravity;
            CharacterPosition.X += Speed.X;

            if (!CanTakeHit)
            {
                HitRegenTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (HitRegenTimer >= 50)
                {
                    HitRegenTimer = 0;
                    CanTakeHit = true;
                }
            }

            if (CharacterPosition.X < 0)
            {
                CharacterPosition.X = 0;
            }
            else if (CharacterPosition.X > Game.Window.ClientBounds.Width - 70)
            {
                CharacterPosition.X = Game.Window.ClientBounds.Width - 70;
            }
            if (CharacterPosition.Y < 0)
            {
                CharacterPosition.Y = 0;
            }
            else if (CharacterPosition.Y > Game.Window.ClientBounds.Height - 60)
            {
                CharacterPosition.Y = Game.Window.ClientBounds.Height - 60;
            }

            if (Jumping)
            {
                if (Speed.Y > 0)
                {
                    Speed.Y -= 1;
                }
                else
                {
                    Jumping = false;
                }

                CharacterPosition.Y -= Speed.Y;
            }

            if (arms != null)
                arms.Update(gameTime, Direction);
        }

        //In here
        private void UpdateFrames(GameTime gameTime)
        {
            switch (CurrentSpriteSheet)
            {
                #region MoveLR
                case "StandMoveLR":
                    {
                        ARect = new Rectangle(0, 0, 0, 0);
                        DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        if (FrameIncrement)
                        {
                            RunningFR += gameTime.ElapsedGameTime.Milliseconds;
                            if (RunningFR >= 20)
                            {
                                RunningFR = 0;
                                if (CurrentFrame.X < 6)
                                {
                                    CurrentFrame.X += 1;
                                }
                                else if (CurrentFrame.X == 6 && CurrentFrame.Y < 2)
                                {
                                    CurrentFrame.Y += 1;
                                    CurrentFrame.X = 0;
                                }
                                else if (CurrentFrame.X == 6 && CurrentFrame.Y == 2)
                                {
                                    CurrentFrame.X = 1;
                                    CurrentFrame.Y = 0;
                                }
                            }
                        }
                        

                        if (CurrentFrame.X > 6)
                            CurrentFrame.X = 6;
                    }
                    break;
                #endregion
                #region Jump
                case "Jump":
                    {
                        ARect = new Rectangle(0, 0, 0, 0);
                        DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                    
                        if (CharacterPosition.Y <= Game.Window.ClientBounds.Height - 120)
                        {
                            if (CurrentFrame.X == 4 && CurrentFrame.Y < 1)
                            {
                                CurrentFrame.Y += 1;
                                CurrentFrame.X = 0;
                            }
                            else if(CurrentFrame.X < 4)
                                CurrentFrame.X += 1;
                        }
                        else if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 120)
                        {
                            if (CurrentFrame.X == 0 && CurrentFrame.Y > 0)
                            {
                                CurrentFrame.Y -= 1;
                                CurrentFrame.X = 4;
                            }
                            else if (CurrentFrame.X > 0)
                            {
                                CurrentFrame.X -= 1;
                            }
                            if (CurrentFrame.Y == 1 && CurrentFrame.X == 3)
                                soundBank.PlayCue("JumpLanding");
                            else if (!Jumping)
                            {
                                Speed.X = 0;
                            }
                        }
                        if (CurrentFrame.X > 4)
                            CurrentFrame.X = 4;
                        if (CurrentFrame.Y > 1)
                            CurrentFrame.Y = 1;
                    }
                break;
                #endregion
                //here 1 2 3 4 5
                #region DiveRoll
                case "DiveRoll":
                {
                    ARect = new Rectangle(0, 0, 0, 0);
                    DRect = new Rectangle((int)CharacterPosition.X + 20, (int)CharacterPosition.Y + 35, 20, 5);
                    if (CurrentFrame.X < 8)
                        CurrentFrame.X += 1;
                    else if (CurrentFrame.X == 8 && CurrentFrame.Y < 2)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y += 1;
                    }
                    if (CurrentFrame.X == 0 && CurrentFrame.Y == 1)
                    {
                        if (Direction == 'L')
                            Speed.X = -15;
                        else if (Direction == 'R')
                            Speed.X = 15;
                    }
                    else if (CurrentFrame.X == 8 && CurrentFrame.Y == 2)
                    {
                        Speed.X = 0;
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        FrameIncrement = false;
                        CurrentSpriteSheet = "StandMoveLR";
                        switch (Mode)
                        {
                            case CharacterMode.FireMage:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                                    break;
                                }
                            case CharacterMode.HandGunner:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    break;
                                }
                            case CharacterMode.AKGunner:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    break;
                                }
                            case CharacterMode.Launcher:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    break;
                                }
                            case CharacterMode.Mage:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                    break;
                                }
                            case CharacterMode.IceMage:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                    break;
                                }
                        }
                    }
                    break;
                }
                #endregion
                #region BasicAttack
                case "BasicAttack":
                {
                    if (PowList.Count() > 0)
                    {
                        Damage = 2;
                    }
                    else
                        Damage = 1;

                    if (FrameIncrement)
                    {
                        if (Direction == 'L')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 10, (int)CharacterPosition.Y + 10, 25, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                        else if (Direction == 'R')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 35, (int)CharacterPosition.Y + 10, 20, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                    }

                    if (FrameIncrement)
                    {
                        if (CurrentFrame.X < 7)
                        {
                            CurrentFrame.X += 1;
                        }
                        else if (CurrentFrame.X == 7 && CurrentFrame.Y < 1)
                        {
                            if (StickHasHit)
                            {
                                soundBank.PlayCue("StickHit");
                                StickHasHit = false;
                            }
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 1;
                        }
                        else if (CurrentFrame.X == 7 && CurrentFrame.Y == 1)
                        {
                            FrameIncrement = false;
                        }
                    }
                    else if (!FrameIncrement)
                    {
                        if (CurrentFrame.X > 0)
                        {
                            CurrentFrame.X -= 1;
                        }
                        if (CurrentFrame.Y == 1 && CurrentFrame.X == 0)
                        {
                            CurrentFrame.Y -= 1;
                            CurrentFrame.X = 7;
                        }
                        else if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                        {
                            Speed.X = 0;
                            ARect = new Rectangle(0, 0, 0, 0);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                    }
                    if (CurrentFrame.X > 7)
                        CurrentFrame.X = 7;
                    if (CurrentFrame.Y > 1)
                        CurrentFrame.Y = 1;
                    
                    break;
                }
                #endregion
                #region BasicAttack2
                case "BasicAttack2":
                {
                    if (PowList.Count() > 0)
                    {
                        Damage = 2;
                    }
                    else
                        Damage = 1;

                    if (FrameIncrement)
                    {
                        if (Direction == 'L')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 10, (int)CharacterPosition.Y + 10, 25, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                        if (Direction == 'R')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 35, (int)CharacterPosition.Y + 10, 20, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                    }

                    if (FrameIncrement)
                    {
                        if (CurrentFrame.X < 8)
                            CurrentFrame.X += 1;

                        else if (CurrentFrame.X == 8 && CurrentFrame.Y < 3)
                        {
                            if (StickHasHit)
                            {
                                soundBank.PlayCue("StickHit");
                                StickHasHit = false;
                            }
                            CurrentFrame.X = 0;
                            CurrentFrame.Y += 1;
                        }

                        else if (CurrentFrame.X == 8 && CurrentFrame.Y == 3)
                        {
                            FrameIncrement = false;
                        }
                    }
                    else if (!FrameIncrement)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Speed.X = 0;
                        ARect = new Rectangle(0, 0, 0, 0);
                        DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                    }
                    if (CurrentFrame.X > 8)
                        CurrentFrame.X = 8;
                    if (CurrentFrame.Y > 3)
                        CurrentFrame.Y = 3;

                }
                break;
                #endregion
                #region BasicAttack3
                case "BasicAttack3":
                {
                    if (PowList.Count() > 0)
                    {
                        Damage = 2;
                    }
                    else
                        Damage = 1;
                    if (FrameIncrement)
                    {
                        if (Direction == 'L')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 10, (int)CharacterPosition.Y + 10, 25, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                        if (Direction == 'R')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 35, (int)CharacterPosition.Y + 10, 20, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                    }

                    if (FrameIncrement)
                    {
                        
                        if (CurrentFrame.X < 3)
                            CurrentFrame.X += 1;

                        else if (CurrentFrame.X == 3 && CurrentFrame.Y < 2)
                        {
                            if (StickHasHit)
                            {
                                soundBank.PlayCue("StickHit");
                                StickHasHit = false;
                            }
                            CurrentFrame.Y += 1;
                            CurrentFrame.X = 0;
                        }
                        else if (CurrentFrame.X == 3 && CurrentFrame.Y == 2)
                        {
                            FrameIncrement = false;
                        }
                    }
                    else if (!FrameIncrement)
                    {
                        if (CurrentFrame.X > 0)
                            CurrentFrame.X -= 1;
                            
                        else if (CurrentFrame.X == 0 && CurrentFrame.Y > 0)
                        {
                            CurrentFrame.Y -= 1;
                            CurrentFrame.X = 3;
                        }
                        else if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                        {
                            ARect = new Rectangle(0, 0, 0, 0);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                            Speed.X = 0;
                        }
                    }
                    if (CurrentFrame.X > 3)
                        CurrentFrame.X = 3;
                    if (CurrentFrame.Y > 2)
                        CurrentFrame.Y = 2;
                }
                break;
                #endregion
                #region ChargeAttack
                case "ChargeAttack":
                {
                    DeflectionTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (PowList.Count() > 0)
                    {
                        Damage = 10;
                    }
                    else
                        Damage = 5;
                    if (CurrentFrame.Y == 4 || CurrentFrame.Y == 5 && CurrentFrame.X != 9)
                    {
                        if (Direction == 'L')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 10, (int)CharacterPosition.Y + 10, 25, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                        else if (Direction == 'R')
                        {
                            ARect = new Rectangle((int)CharacterPosition.X + 35, (int)CharacterPosition.Y + 10, 20, 40);
                            DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                        }
                    }
                    else
                    {
                        ARect = new Rectangle(0, 0, 0, 0);
                        DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                    }
                    if (FrameIncrement)
                    {
                        Speed.X = 0;
                        if (CurrentFrame.X < 9)
                        {
                            CurrentFrame.X += 1;
                        }
                        else if (CurrentFrame.X == 9 && CurrentFrame.Y < 3)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y += 1;
                        }
                        else if (CurrentFrame.X == 9 && CurrentFrame.Y == 3)
                        {
                            soundBank.PlayCue("Swing2");
                            if (StickHasHit)
                            {
                                soundBank.PlayCue("StickHit2");
                                StickHasHit = false;
                            }
                            if (Direction == 'L')
                                Speed.X = -9;
                            else
                                Speed.X = 9;

                            FrameIncrement = false;
                        }
                    }
                    else
                    {
                        if (CurrentFrame.X < 9)
                        {
                            CurrentFrame.X += 1;
                        }
                        else if (CurrentFrame.Y < 5 && CurrentFrame.X == 9)
                        {
                            CurrentFrame.Y += 1;
                            CurrentFrame.X = 0;
                        }
                        if (CurrentFrame.Y == 5 && CurrentFrame.X == 9)
                        {
                            Speed.X = 0;
                        }
                    }
                    if (CurrentFrame.X > 9)
                        CurrentFrame.X = 9;
                    if (CurrentFrame.Y > 5)
                        CurrentFrame.Y = 5;
                }
                break;
                #endregion
                #region FireBallM
                case "FireBallM":
                {
                    ARect = new Rectangle(0, 0, 0, 0);
                    DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                    if (FrameIncrement)
                    {
                        if (CurrentFrame.X < 4)
                            CurrentFrame.X += 1;
                        else if (CurrentFrame.X == 4 && CurrentFrame.Y < 2)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y += 1;
                        }
                        else if (CurrentFrame.X == 4 && CurrentFrame.Y == 2)
                            FrameIncrement = false;
                    }
                    else if (!FrameIncrement)
                    {
                        if (CurrentFrame.X > 0)
                            CurrentFrame.X -= 1;
                        else if (CurrentFrame.X == 0 && CurrentFrame.Y > 0)
                        {
                            CurrentFrame.X = 4;
                            CurrentFrame.Y -= 1;
                        }
                        else if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            Speed.X = 0;
                    }
                  
                    if (CurrentFrame.X > 4)
                        CurrentFrame.X = 4;
                    if (CurrentFrame.Y > 2)
                        CurrentFrame.Y = 2;
                }
                break;
                #endregion
                #region ShieldM
                case "ShieldM":
                {
                    ARect = new Rectangle(0, 0, 0, 0);
                    DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                    if (FrameIncrement)
                    {
                        if (CurrentFrame.X < 4)
                            CurrentFrame.X += 1;
                        else if (CurrentFrame.X == 4 && CurrentFrame.Y < 3)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y += 1;
                        }
                        else if (CurrentFrame.X == 4 && CurrentFrame.Y == 3)
                        {
                            FrameIncrement = false;
                        }
                    }
                    else if (!FrameIncrement)
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
                            Speed.X = 0;
                        }
                    }
                    if (CurrentFrame.X > 4)
                        CurrentFrame.X = 4;
                    if (CurrentFrame.Y > 3)
                        CurrentFrame.Y = 3;
                }
                break;
                #endregion
                //here 1 2 3 4 5
                #region BehindHit1
                case "BehindHit1":
                {
                    ARect = new Rectangle(0, 0, 0, 0);
                    DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                    
                    if (FrameIncrement)
                    {
                        if (CurrentFrame.X < 9)
                            CurrentFrame.X += 1;
                        else if (CurrentFrame.X == 9 && CurrentFrame.Y < 2)
                        {
                            Speed.X = 0;
                            CurrentFrame.X = 0;
                            CurrentFrame.Y += 1;
                        }
                        else if (CurrentFrame.X == 9 && CurrentFrame.Y == 2)
                        {
                            switch (Mode)
                            {
                                case CharacterMode.FireMage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                                        break;
                                    }
                                case CharacterMode.HandGunner:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.AKGunner:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.Launcher:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.Mage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.IceMage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                        break;
                                    }
                            }
                            FrameIncrement = false;
                            CurrentSpriteSheet = "StandMoveLR";
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                        }
                    }

                    if (CurrentFrame.X > 9)
                        CurrentFrame.X = 9;
                    if (CurrentFrame.Y > 2)
                        CurrentFrame.Y = 2;

                    break;
                }
                #endregion
                //here 1 2 3 4 5
                #region FrontHit
                case "FrontHit":
                {
                    ARect = new Rectangle(0, 0, 0, 0);
                    DRect = new Rectangle((int)CharacterPosition.X + 32, (int)CharacterPosition.Y + 20, 5, 20);
                   
                    if (FrameIncrement)
                    {
                        if (CurrentFrame.X < 4)
                            CurrentFrame.X += 1;
                        else if (CurrentFrame.X == 4 && CurrentFrame.Y < 4)
                        {
                            Speed.X = 0;
                            CurrentFrame.X = 0;
                            CurrentFrame.Y += 1;
                        }
                        else if (CurrentFrame.X == 4 && CurrentFrame.Y == 4)
                        {
                            switch (Mode)
                            {
                                case CharacterMode.FireMage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                                        break;
                                    }
                                case CharacterMode.HandGunner:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.AKGunner:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.Launcher:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.Mage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.IceMage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                        break;
                                    }
                            }
                            FrameIncrement = false;
                            CurrentSpriteSheet = "StandMoveLR";
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                        }
                    }
                    if (CurrentFrame.X > 4)
                        CurrentFrame.X = 4;
                    if (CurrentFrame.Y > 4)
                        CurrentFrame.Y = 4;
                break;
                }
                #endregion
                //here 1 2 3 4 5
                #region Death
                case "Death":
                {
                    if (CurrentFrame.X < 9)
                        CurrentFrame.X += 1;
                    else if (CurrentFrame.X == 9 && CurrentFrame.Y < 5)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y += 1;
                    }
                    else if (CurrentFrame.X == 9 && CurrentFrame.Y == 5)
                    {
                        if (Lives > 0)
                        {
                            Lives -= 1;
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            FrameIncrement = false;
                            Healthi = 100;
                            CurrentSpriteSheet = "StandMoveLR";
                            switch (Mode)
                            {
                                case CharacterMode.FireMage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                                        break;
                                    }
                                case CharacterMode.HandGunner:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.AKGunner:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.Launcher:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.Mage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.IceMage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                        break;
                                    }
                            }
                            Dying = false;
                        }
                        else
                            GameOver = true;
                    }
                    break;
                }
                #endregion
                #region CastBall
                case "CastBall":
                {
                    if (CurrentFrame.X < 7)
                        CurrentFrame.X += 1;
                    else if (CurrentFrame.X == 7 && CurrentFrame.Y < 3)
                    {
                        CurrentFrame.X = 0;
                        CurrentFrame.Y += 1;
                    }
                    if (CurrentFrame.X == 0 && CurrentFrame.Y == 3)
                    {
                        if (Mode == CharacterMode.Mage)
                        {
                            MBallList.Add(new MageBall(Game.Content.Load<Texture2D>(@"Enemies\Mage\Ball"),
                                CharacterPosition, new Point(70, 50),
                                new Point(0, 0), Direction));
                            Game1.enemies.PlaySound("MageBall");
                        }
                        else if (Mode == CharacterMode.IceMage)
                        {
                            IMBallList.Add(new MageBall(Game.Content.Load<Texture2D>(@"Enemies\IceMage\Ball"),
                                CharacterPosition, new Point(70, 50),
                                new Point(0, 0), Direction));
                            Game1.enemies.PlaySound("IMageBall");
                        }
                    }
                    else if (CurrentFrame.X == 7 && CurrentFrame.Y == 3)
                    {
                        switch (Mode)
                        {
                            case CharacterMode.Mage:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                    break;
                                }
                            case CharacterMode.IceMage:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                    break;
                                }
                        }
                        FrameIncrement = false;
                        CurrentSpriteSheet = "StandMoveLR";
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Speed.X = 0;
                    }
                    break;
               }
               #endregion
                #region CastBlast
                case "CastBlast":
                {
                    if (CurrentFrame.X < 7)
                        CurrentFrame.X += 1;
                    else if (CastMReps < 4 && CurrentFrame.Y == 1)
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
                            if (Mode == CharacterMode.Mage)
                            {
                                MBlastList.Add(new MageBlast(Game.Content.Load<Texture2D>(@"Enemies\Mage\Blast"),
                                    new Vector2(CharacterPosition.X - 780, CharacterPosition.Y),
                                    new Point(800, 50),
                                    new Point(0, 0), Direction));
                                Game1.enemies.PlaySound("MageBlast");
                            }
                            else if (Mode == CharacterMode.IceMage)
                            {
                                IMBlastList.Add(new IceBlast(Game.Content.Load<Texture2D>(@"Enemies\IceMage\Blast"),
                                    new Vector2(CharacterPosition.X - 780, CharacterPosition.Y),
                                    new Point(800, 50),
                                    new Point(0, 0), Direction));
                                Game1.enemies.PlaySound("IMageBlast");
                            }
                        }
                        else
                        {
                            if (Mode == CharacterMode.Mage)
                            {
                                MBlastList.Add(new MageBlast(Game.Content.Load<Texture2D>(@"Enemies\Mage\Blast"),
                                    new Vector2(CharacterPosition.X + 50,
                                    CharacterPosition.Y),
                                    new Point(800, 50),
                                    new Point(0, 0), Direction));
                                Game1.enemies.PlaySound("MageBlast");
                            }
                            else if (Mode == CharacterMode.IceMage)
                            {
                                IMBlastList.Add(new IceBlast(Game.Content.Load<Texture2D>(@"Enemies\IceMage\Blast"),
                                    new Vector2(CharacterPosition.X + 50, CharacterPosition.Y),
                                    new Point(800, 50),
                                    new Point(0, 0), Direction));
                                Game1.enemies.PlaySound("IMageBlast");
                            }
                        }
                    }
                    else if (CurrentFrame.X == 7 && CurrentFrame.Y == 3)
                    {
                        switch (Mode)
                        {
                            case CharacterMode.Mage:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                    break;
                                }
                            case CharacterMode.IceMage:
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                    break;
                                }
                        }
                        FrameIncrement = false;
                        CurrentSpriteSheet = "StandMoveLR";
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Speed.X = 0;
                    }
                    break;
                }
                #endregion
                #region MageShield
                case "MShield":
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
                            if (Mode == CharacterMode.Mage)
                            {
                                MShield = new MCMShield(Game.Content.Load<Texture2D>(@"Enemies\Mage\Shield"),
                                    CharacterPosition, new Point(45, 60), new Point(0, 0), 800);
                                soundBank.PlayCue("MageCastShield");
                            }
                            else if (Mode == CharacterMode.IceMage)
                            {
                                MShield = new MCMShield(Game.Content.Load<Texture2D>(@"Enemies\IceMage\Shield"),
                                   CharacterPosition, new Point(45, 60), new Point(0, 0), 1000);
                                soundBank.PlayCue("IMageCastShield");
                            }
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
                            switch (Mode)
                            {
                                case CharacterMode.Mage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                        break;
                                    }
                                case CharacterMode.IceMage:
                                    {
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                        break;
                                    }
                            }
                            FrameIncrement = false;
                            CurrentSpriteSheet = "StandMoveLR";
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            Speed.X = 0;

                        }
                    }
                    break;
                }
                #endregion
                #region Deflection
                case "Deflection":
                {
                    if (PowList.Count() > 0)
                    {
                        Damage = 11;
                    }
                    else
                        Damage = 6;

                    if (FrameIncrement && !Deflection)
                    {
                        ARect = new Rectangle(0, 0, 0, 0);
                        DRect = new Rectangle((int)CharacterPosition.X + 32,
                            (int)CharacterPosition.Y + 20, 5, 20);
                        if (CurrentFrame.X < 3)
                        {
                            CurrentFrame.X += 1;
                        }
                        else if (CurrentFrame.X == 3 && CurrentFrame.Y < 1)
                        {
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 1;
                        }
                        else if (CurrentFrame.X == 3 && CurrentFrame.Y == 1)
                        {
                            FrameIncrement = false;
                            Deflection = true;
                            if (Direction == 'L')
                            {
                                BRect = new Rectangle((int)CharacterPosition.X + 10,
                                    (int)CharacterPosition.Y + 10, 25, 40);
                                DRect = new Rectangle((int)CharacterPosition.X + 32,
                                    (int)CharacterPosition.Y + 20, 5, 20);
                            }
                            else if (Direction == 'R')
                            {
                                BRect = new Rectangle((int)CharacterPosition.X + 35,
                                    (int)CharacterPosition.Y + 10, 20, 40);
                                DRect = new Rectangle((int)CharacterPosition.X + 32,
                                    (int)CharacterPosition.Y + 20, 5, 20);
                            }
                        }
                    }
                    else if (!FrameIncrement && Deflection)
                    {
                        DeflectionTimer -= gameTime.ElapsedGameTime.Milliseconds;
                        if (DeflectionTimer <= 0)
                        {
                            Deflection = false;
                        }
                    }
                    else if (!Deflection && !FrameIncrement)
                    {
                        if (CurrentFrame.X > 0)
                            CurrentFrame.X -= 1;
                        else
                        {
                            Speed.X = 0;
                            ARect = new Rectangle(0, 0, 0, 0);
                            BRect = new Rectangle(0, 0, 0, 0);
                            DRect = new Rectangle((int)CharacterPosition.X + 32,
                                (int)CharacterPosition.Y + 20, 5, 20);
                            Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                            CurrentFrame.X = 0;
                            CurrentFrame.Y = 0;
                            FrameIncrement = false;
                            CurrentSpriteSheet = "StandMoveLR";
                        }
                    }
                    if (CurrentFrame.X > 3)
                        CurrentFrame.X = 3;
                    if (CurrentFrame.Y > 1)
                        CurrentFrame.Y = 1;

                    break;
                }
                #endregion
            }
        }

        private void PowerUps(GameTime gameTime)
        {
            #region IceHit
            if (IceHit)
            {
                IceTimer -= gameTime.ElapsedGameTime.Milliseconds;
                if (IceTimer <= 0)
                {
                    IceTimer = 0;
                    IceHit = false;
                    color = Color.White;
                }
            }
            #endregion
            #region Rain
            if (Rain > 0)
            {
                Rain -= gameTime.ElapsedGameTime.Milliseconds;
                if ((Rain % 500) < 100)
                {
                    Vector2 Pos = new Vector2(MRnd.Next(0, 790), 0);
                    RocketList.Add(new MCRocket(RocketTex, DetonationTex,
                                            Pos,
                                            new Point(10, 30), Vector2.Zero, 0,
                                            'L', new Vector2(Pos.X, Pos.Y + 600),
                                            new Vector2(0, 0)));
                    if ((Rain % 500) < 10)
                        soundBank.PlayCue("RocketLaunch");
                }
            }
            #endregion
            #region Invincible
            if (Invincible > 0)
                Invincible -= gameTime.ElapsedGameTime.Milliseconds;
            #endregion
        }
        //In here
        private void CheckInput(GameTime gameTime)
        {
            //You must edit switch mode for each mode 1 2 3 4 5
            switch (Mode)
            {
                #region FireMage
                case CharacterMode.FireMage:
                    {
                        if (CurrentSpriteSheet != "Deflection")
                        {
                            Deflection = false;
                            BRect = new Rectangle(0, 0, 0, 0);
                        }
                        if (!Dying)
                        {
                            #region MoveLR
                            if (CurrentKeyboardState.IsKeyDown(Keys.Left))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.Left))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                                    Direction = 'L';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = -9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            if (CurrentKeyboardState.IsKeyDown(Keys.Right))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.Right))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                                    Direction = 'R';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = 9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            else if (CurrentSpriteSheet == "StandMoveLR" && CurrentKeyboardState.IsKeyUp(Keys.Left) && CurrentKeyboardState.IsKeyUp(Keys.Right))
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Speed.X = 0;
                                FrameIncrement = false;
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region Jump
                            if (CurrentKeyboardState.IsKeyDown(Keys.Up) && !PrevKeyState.IsKeyDown(Keys.Up))
                            {
                                soundBank.PlayCue("Jump");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\Jump");
                                CurrentSpriteSheet = "Jump";

                                if (CharacterPosition.Y == Game.Window.ClientBounds.Height - 60)
                                {
                                    Speed.Y = 40;
                                    Jumping = true;
                                }
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region DiveRoll
                            if (CurrentKeyboardState.IsKeyDown(Keys.Down)
                                && !PrevKeyState.IsKeyDown(Keys.Down) && CurrentSpriteSheet != "DiveRoll")
                            {
                                soundBank.PlayCue("DiveRoll");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\DiveRoll");
                                CurrentSpriteSheet = "DiveRoll";
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region SwitchMode
                            if (Keyboard.GetState().IsKeyDown(Keys.D2)
                            && !PrevKeyState.IsKeyDown(Keys.D2) && HGAmmo > 0)
                            {
                                SetMode("HandGunner", 0);
                                soundBank.PlayCue("Reload");
                                Deflection = false;
                                BRect = new Rectangle(0, 0, 0, 0);
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D4)
                                && !PrevKeyState.IsKeyDown(Keys.D4) && AKAmmo > 0)
                            {
                                SetMode("AKGunner", 0);
                                soundBank.PlayCue("Reload");
                                Deflection = false;
                                BRect = new Rectangle(0, 0, 0, 0);
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D3)
                                && !PrevKeyState.IsKeyDown(Keys.D3) && LAmmo > 0)
                            {
                                SetMode("Launcher", 0);
                                soundBank.PlayCue("Reload");
                                Deflection = false;
                                BRect = new Rectangle(0, 0, 0, 0);
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D5)
                                && !PrevKeyState.IsKeyDown(Keys.D5) && HasMageHat)
                            {
                                SetMode("Mage", 0);
                                soundBank.PlayCue("MageBall");
                                Deflection = false;
                                BRect = new Rectangle(0, 0, 0, 0);
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D6)
                                && !PrevKeyState.IsKeyDown(Keys.D6) && HasIMageHat)
                            {
                                SetMode("IceMage", 0);
                                soundBank.PlayCue("IMageBall");
                                Deflection = false;
                                BRect = new Rectangle(0, 0, 0, 0);
                            }
                            #endregion
                            #region BasicAttacks
                            if (CurrentKeyboardState.IsKeyDown(Keys.Q) && !PrevKeyState.IsKeyDown(Keys.Q))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                soundBank.PlayCue("Swing");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                CtrlMove = MRnd.Next(0, 3);
                                if (DeflectionTimer > 0 && CurrentSpriteSheet == "ChargeAttack")
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\DeflectionMove");
                                    CurrentSpriteSheet = "Deflection";
                                    FrameIncrement = true;
                                    Speed.X = 0;
                                    DeflectionTimer *= 2;
                                }
                                else if (CtrlMove == 0)
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\BasicAttack");
                                    CurrentSpriteSheet = "BasicAttack";
                                    FrameIncrement = true;
                                    if (Direction == 'L')
                                    {
                                        Speed.X = -5;
                                    }
                                    else
                                    {
                                        Speed.X = 5;
                                    }
                                }
                                else if (CtrlMove == 1)
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\BasicAttack2");
                                    CurrentSpriteSheet = "BasicAttack2";
                                    FrameIncrement = true;
                                    if (Direction == 'L')
                                    {
                                        Speed.X = -5;
                                    }
                                    else
                                    {
                                        Speed.X = 5;
                                    }
                                }
                                else if (CtrlMove == 2)
                                {
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\BasicAttack3");
                                    CurrentSpriteSheet = "BasicAttack3";
                                    FrameIncrement = true;
                                    if (Direction == 'L')
                                    {
                                        Speed.X = -5;
                                    }
                                    else
                                    {
                                        Speed.X = 5;
                                    }
                                }
                            }
                            #endregion
                            #region ChargeAttack
                            else if (CurrentKeyboardState.IsKeyDown(Keys.W) && !PrevKeyState.IsKeyDown(Keys.W))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\ChargeAttack");
                                CurrentSpriteSheet = "ChargeAttack";
                                FrameIncrement = true;
                                DeflectionTimer = 0;
                            }
                            else if (CurrentKeyboardState.IsKeyUp(Keys.W) && CurrentSpriteSheet == "ChargeAttack")
                            {
                                Speed.X = 0;
                            }
                            #endregion
                            if (!IceHit)
                            {
                                #region FireBall
                                if (Keyboard.GetState().IsKeyDown(Keys.R) && !PrevKeyState.IsKeyDown(Keys.R))
                                {
                                    if (Running != null)
                                    {
                                        if (Running.IsPlaying)
                                            Running.Stop(AudioStopOptions.Immediate);
                                        Running = null;
                                    }
                                    if (CurrentXP >= 100)
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        CurrentXP -= 100;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\PowerMoves\FireBallM");
                                        CurrentSpriteSheet = "FireBallM";
                                        FrameIncrement = true;
                                        soundBank.PlayCue("FireBall");

                                        Point Frame = new Point(0, 0);
                                        int Xoffset = 0;
                                        if (Direction == 'R')
                                        {
                                            Xoffset = -40;
                                            Frame.Y = 5;
                                        }
                                        else if (Direction == 'L')
                                        {
                                            Xoffset = 30;
                                        }

                                        BallList.Add(new FireBall(Game.Content.Load<Texture2D>(@"Powers\FireBall"),
                                        new Vector2(CharacterPosition.X - Xoffset, CharacterPosition.Y),
                                        Frame,
                                        new Point(70, 50),
                                        1,
                                        Direction));
                                    }
                                }
                                #endregion
                                #region PowerBull
                                if (Keyboard.GetState().IsKeyDown(Keys.E) && !PrevKeyState.IsKeyDown(Keys.E) && PowList.Count() == 0)
                                {
                                    if (Running != null)
                                    {
                                        if (Running.IsPlaying)
                                            Running.Stop(AudioStopOptions.Immediate);
                                        Running = null;
                                    }
                                    
                                    if (CurrentXP >= 50)
                                    {
                                        soundBank.PlayCue("PowerBull");
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        CurrentXP -= 50;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\PowerMoves\FireBallM");
                                        CurrentSpriteSheet = "FireBallM";
                                        FrameIncrement = true;

                                        int YOffset = -50;

                                        PowList.Add(new PowerBall(Game.Content.Load<Texture2D>(@"Powers\PowerBall"),
                                        new Vector2(CharacterPosition.X, CharacterPosition.Y + YOffset),
                                        new Point(0, 0),
                                        new Point(70, 50),
                                        0.5f));
                                    }
                                }
                                #endregion
                                #region FireWall
                                if (Keyboard.GetState().IsKeyDown(Keys.A) && !PrevKeyState.IsKeyDown(Keys.A))
                                {
                                    if (Running != null)
                                    {
                                        if (Running.IsPlaying)
                                            Running.Stop(AudioStopOptions.Immediate);
                                        Running = null;
                                    }
                                    
                                    if (CurrentXP >= 200)
                                    {
                                        soundBank.PlayCue("FireWall");
                                        if (FireCrackle == null)
                                        {
                                            FireCrackle = soundBank.GetCue("FireCrackle");
                                            FireCrackle.Play();
                                        }
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        CurrentXP -= 200;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\PowerMoves\FireBallM");
                                        CurrentSpriteSheet = "FireBallM";
                                        FrameIncrement = true;

                                        int XOffset = 0;
                                        if (Direction == 'R')
                                        {
                                            XOffset = 40;
                                        }
                                        else if (Direction == 'L')
                                        {
                                            XOffset = -60;
                                        }

                                        WallList.Add(new FireWall(Game.Content.Load<Texture2D>(@"Powers\FireWall"),
                                        new Vector2(CharacterPosition.X + XOffset, -60),
                                        new Point(0, 0),
                                        new Point(100, 650),
                                        1));
                                    }
                                }
                                #endregion
                                #region Shield
                                if (Keyboard.GetState().IsKeyDown(Keys.S) && !PrevKeyState.IsKeyDown(Keys.S) && ShieldList.Count() == 0)
                                {
                                    if (Running != null)
                                    {
                                        if (Running.IsPlaying)
                                            Running.Stop(AudioStopOptions.Immediate);
                                        Running = null;
                                    }
                                    
                                    if (CurrentXP >= 300)
                                    {
                                        if (FireCrackle == null)
                                        {
                                            FireCrackle = soundBank.GetCue("FireCrackle");
                                            FireCrackle.Play();
                                        }
                                        soundBank.PlayCue("Shield");
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        CurrentXP -= 300;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\PowerMoves\ShieldM");
                                        CurrentSpriteSheet = "ShieldM";
                                        FrameIncrement = true;

                                        int XOffset = 0;
                                        if (Direction == 'R')
                                        {
                                            XOffset = -60;
                                        }
                                        else if (Direction == 'L')
                                        {
                                            XOffset = -60;
                                        }

                                        ShieldList.Add(new Shield(Game.Content.Load<Texture2D>(@"Powers\Shield"),
                                        new Vector2(CharacterPosition.X + XOffset, Game.Window.ClientBounds.Height - 190),
                                        new Point(0, 0),
                                        new Point(90, 90),
                                        2));
                                    }
                                }
                                #endregion
                                #region Explosion
                                if (Keyboard.GetState().IsKeyDown(Keys.D) && !PrevKeyState.IsKeyDown(Keys.D))
                                {
                                    if (Running != null)
                                    {
                                        if (Running.IsPlaying)
                                            Running.Stop(AudioStopOptions.Immediate);
                                        Running = null;
                                    }
                                    if (CurrentXP >= 1000 && Lives > 0)
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        CurrentXP -= 1000;
                                        Lives -= 1;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\PowerMoves\ShieldM");
                                        CurrentSpriteSheet = "ShieldM";
                                        soundBank.PlayCue("Demolition");
                                        FrameIncrement = true;

                                        ExpList.Add(new Explosion(Game.Content.Load<Texture2D>(@"Powers\Explosion"),
                                        new Vector2(0, -15),
                                        new Point(0, 0),
                                        new Point(395, 295),
                                        2.05f));
                                    }
                                }
                                #endregion
                            }
                        }
                        break;
                    }
                #endregion
                #region HandGunner
                case CharacterMode.HandGunner:
                    {
                        if (!Dying)
                        {
                            #region MoveLR
                            if (CurrentKeyboardState.IsKeyDown(Keys.A))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.A))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    Direction = 'L';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = -9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            if (CurrentKeyboardState.IsKeyDown(Keys.D))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.D))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    Direction = 'R';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = 9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            else if (CurrentSpriteSheet == "StandMoveLR" && CurrentKeyboardState.IsKeyUp(Keys.A) && CurrentKeyboardState.IsKeyUp(Keys.D))
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Speed.X = 0;
                                FrameIncrement = false;
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region Jump
                            if (CurrentKeyboardState.IsKeyDown(Keys.W) && !PrevKeyState.IsKeyDown(Keys.W))
                            {
                                soundBank.PlayCue("Jump");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\GJump");
                                CurrentSpriteSheet = "Jump";

                                if (CharacterPosition.Y == Game.Window.ClientBounds.Height - 60)
                                {
                                    Speed.Y = 40;
                                    Jumping = true;
                                }
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region DiveRoll
                            if (CurrentKeyboardState.IsKeyDown(Keys.S)
                                && !PrevKeyState.IsKeyDown(Keys.S) && CurrentSpriteSheet != "DiveRoll")
                            {
                                soundBank.PlayCue("DiveRoll");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\GDiveRoll");
                                CurrentSpriteSheet = "DiveRoll";
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region MouseInput
                            if ((CurrentSpriteSheet == "StandMoveLR" || CurrentSpriteSheet == "Jump")
                                && CurrentFrame.X == 0 && CurrentFrame.Y == 0 && Speed.X == 0)
                            {
                                if (Direction == 'L')
                                {
                                    if (Mouse.GetState().X > CharacterPosition.X + 35)
                                        Direction = 'R';
                                }
                                else
                                {
                                    if (Mouse.GetState().X < CharacterPosition.X + 35)
                                        Direction = 'L';
                                }
                            }
                            if (CurrentSpriteSheet == "StandMoveLR" || CurrentSpriteSheet == "Jump")
                            {
                                if (FireTimer < FireRate)
                                    FireTimer += gameTime.ElapsedGameTime.Milliseconds;
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && FireTimer >= FireRate
                                    && HGAmmo > 0)
                                {
                                    FireTimer = 0;
                                    HGAmmo -= 1;
                                    BulletList.Add(new Bullet(BulletTex,
                                        new Vector2(CharacterPosition.X + 35, CharacterPosition.Y + 19),
                                            new Point(6, 20), new Vector2(2, 7), arms.RCDeg, Direction,
                                            arms.GetVelocity()));
                                    soundBank.PlayCue("GunShot");
                                }
                            }
                            #endregion
                            #region SwitchMode
                            if (Keyboard.GetState().IsKeyDown(Keys.D1)
                                && !PrevKeyState.IsKeyDown(Keys.D1))
                            {
                                SetMode("FireMage", 0);
                                soundBank.PlayCue("FireBall");
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.D4)
                                && !PrevKeyState.IsKeyDown(Keys.D4) && AKAmmo > 0)
                            {
                                SetMode("AKGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D3)
                                && !PrevKeyState.IsKeyDown(Keys.D3) && LAmmo > 0)
                            {
                                SetMode("Launcher", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D5)
                                && !PrevKeyState.IsKeyDown(Keys.D5) && HasMageHat)
                            {
                                SetMode("Mage", 0);
                                soundBank.PlayCue("MageBall");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D6)
                                && !PrevKeyState.IsKeyDown(Keys.D6) && HasIMageHat)
                            {
                                SetMode("IceMage", 0);
                                soundBank.PlayCue("IMageBall");
                            }
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region AKGunner
                case CharacterMode.AKGunner:
                    {
                        if (!Dying)
                        {
                            #region MoveLR
                            if (CurrentKeyboardState.IsKeyDown(Keys.A))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.A))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    Direction = 'L';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = -9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            if (CurrentKeyboardState.IsKeyDown(Keys.D))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.D))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    Direction = 'R';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = 9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            else if (CurrentSpriteSheet == "StandMoveLR" && CurrentKeyboardState.IsKeyUp(Keys.A) && CurrentKeyboardState.IsKeyUp(Keys.D))
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Speed.X = 0;
                                FrameIncrement = false;
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region Jump
                            if (CurrentKeyboardState.IsKeyDown(Keys.W) && !PrevKeyState.IsKeyDown(Keys.W))
                            {
                                soundBank.PlayCue("Jump");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\GJump");
                                CurrentSpriteSheet = "Jump";

                                if (CharacterPosition.Y == Game.Window.ClientBounds.Height - 60)
                                {
                                    Speed.Y = 40;
                                    Jumping = true;
                                }
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region DiveRoll
                            if (CurrentKeyboardState.IsKeyDown(Keys.S)
                                && !PrevKeyState.IsKeyDown(Keys.S) && CurrentSpriteSheet != "DiveRoll")
                            {
                                soundBank.PlayCue("DiveRoll");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\AKDiveRoll");
                                CurrentSpriteSheet = "DiveRoll";
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region MouseInput
                            if ((CurrentSpriteSheet == "StandMoveLR" || CurrentSpriteSheet == "Jump")
                                && CurrentFrame.X == 0 && CurrentFrame.Y == 0 && Speed.X == 0)
                            {
                                if (Direction == 'L')
                                {
                                    if (Mouse.GetState().X > CharacterPosition.X + 35)
                                        Direction = 'R';
                                }
                                else
                                {
                                    if (Mouse.GetState().X < CharacterPosition.X + 35)
                                        Direction = 'L';
                                }
                            }
                            if (CurrentSpriteSheet == "StandMoveLR" || CurrentSpriteSheet == "Jump")
                            {
                                if (FireTimer < FireRate)
                                    FireTimer += gameTime.ElapsedGameTime.Milliseconds;
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && FireTimer >= FireRate
                                    && AKAmmo > 0)
                                {
                                    FireTimer = 0;
                                    AKAmmo -= 1;
                                    BulletList.Add(new Bullet(BulletTex,
                                        new Vector2(CharacterPosition.X + 35, CharacterPosition.Y + 19),
                                            new Point(6, 20), new Vector2(2, 7), arms.RCDeg, Direction,
                                            arms.GetVelocity()));
                                    soundBank.PlayCue("GunShot");
                                }
                            }
                            #endregion
                            #region SwitchMode
                            if (Keyboard.GetState().IsKeyDown(Keys.D1)
                                && !PrevKeyState.IsKeyDown(Keys.D1))
                            {
                                SetMode("FireMage", 0);
                                soundBank.PlayCue("FireBall");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D2)
                                && !PrevKeyState.IsKeyDown(Keys.D2) && HGAmmo > 0)
                            {
                                SetMode("HandGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D3)
                                && !PrevKeyState.IsKeyDown(Keys.D3) && LAmmo > 0)
                            {
                                SetMode("Launcher", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D5)
                                && !PrevKeyState.IsKeyDown(Keys.D5) && HasMageHat)
                            {
                                SetMode("Mage", 0);
                                soundBank.PlayCue("MageBall");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D6)
                                && !PrevKeyState.IsKeyDown(Keys.D6) && HasIMageHat)
                            {
                                SetMode("IceMage", 0);
                                soundBank.PlayCue("IMageBall");
                            }
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Launcher
                case CharacterMode.Launcher:
                    {
                        if (!Dying)
                        {
                            #region MoveLR
                            if (CurrentKeyboardState.IsKeyDown(Keys.A))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.A))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    Direction = 'L';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = -9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            if (CurrentKeyboardState.IsKeyDown(Keys.D))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.D))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                                    Direction = 'R';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = 9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            else if (CurrentSpriteSheet == "StandMoveLR" && CurrentKeyboardState.IsKeyUp(Keys.A) && CurrentKeyboardState.IsKeyUp(Keys.D))
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Speed.X = 0;
                                FrameIncrement = false;
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region Jump
                            if (CurrentKeyboardState.IsKeyDown(Keys.W) && !PrevKeyState.IsKeyDown(Keys.W))
                            {
                                soundBank.PlayCue("Jump");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\GJump");
                                CurrentSpriteSheet = "Jump";

                                if (CharacterPosition.Y == Game.Window.ClientBounds.Height - 60)
                                {
                                    Speed.Y = 40;
                                    Jumping = true;
                                }
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region DiveRoll
                            if (CurrentKeyboardState.IsKeyDown(Keys.S)
                                && !PrevKeyState.IsKeyDown(Keys.S) && CurrentSpriteSheet != "DiveRoll")
                            {
                                soundBank.PlayCue("DiveRoll");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\LDiveRoll");
                                CurrentSpriteSheet = "DiveRoll";
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region MouseInput
                            if ((CurrentSpriteSheet == "StandMoveLR" || CurrentSpriteSheet == "Jump")
                                && CurrentFrame.X == 0 && CurrentFrame.Y == 0 && Speed.X == 0)
                            {
                                if (Direction == 'L')
                                {
                                    if (Mouse.GetState().X > CharacterPosition.X + 35)
                                        Direction = 'R';
                                }
                                else
                                {
                                    if (Mouse.GetState().X < CharacterPosition.X + 35)
                                        Direction = 'L';
                                }
                            }
                            if (CurrentSpriteSheet == "StandMoveLR" || CurrentSpriteSheet == "Jump")
                            {
                                if (FireTimer < FireRate)
                                    FireTimer += gameTime.ElapsedGameTime.Milliseconds;
                                if (Mouse.GetState().LeftButton == ButtonState.Pressed && FireTimer >= FireRate
                                    && LAmmo > 0)
                                {
                                    FireTimer = 0;
                                    LAmmo -= 1;
                                    RocketList.Add(new MCRocket(RocketTex, DetonationTex, 
                                            new Vector2(CharacterPosition.X + 33 + arms.GetArmsEndPos().X, 
                                            CharacterPosition.Y + 19 + arms.GetArmsEndPos().Y),
                                            new Point(10, 30), new Vector2(4, 13), arms.RCDeg,
                                            Direction, new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                                            arms.GetVelocity()));
                                    soundBank.PlayCue("RocketLaunch");
                                }
                            }
                            #endregion
                            #region SwitchMode
                            if (Keyboard.GetState().IsKeyDown(Keys.D1)
                                && !PrevKeyState.IsKeyDown(Keys.D1))
                            {
                                SetMode("FireMage", 0);
                                soundBank.PlayCue("FireBall");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D2)
                                && !PrevKeyState.IsKeyDown(Keys.D2) && HGAmmo > 0)
                            {
                                SetMode("HandGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D4)
                                && !PrevKeyState.IsKeyDown(Keys.D4) && AKAmmo > 0)
                            {
                                SetMode("AKGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D5)
                                && !PrevKeyState.IsKeyDown(Keys.D5) && HasMageHat)
                            {
                                SetMode("Mage", 0);
                                soundBank.PlayCue("MageBall");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D6)
                                && !PrevKeyState.IsKeyDown(Keys.D6) && HasIMageHat)
                            {
                                SetMode("IceMage", 0);
                                soundBank.PlayCue("IMageBall");
                            }
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region Mage
                case CharacterMode.Mage:
                    {
                        if (!Dying)
                        {
                            #region MoveLR
                            if (CurrentKeyboardState.IsKeyDown(Keys.Left))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.Left))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                    Direction = 'L';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = -9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            if (CurrentKeyboardState.IsKeyDown(Keys.Right))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80 && !PrevKeyState.IsKeyDown(Keys.Right))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                                    Direction = 'R';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = 9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            else if (CurrentSpriteSheet == "StandMoveLR" && CurrentKeyboardState.IsKeyUp(Keys.Left) && CurrentKeyboardState.IsKeyUp(Keys.Right))
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Speed.X = 0;
                                FrameIncrement = false;
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region Jump
                            if (CurrentKeyboardState.IsKeyDown(Keys.Up) && !PrevKeyState.IsKeyDown(Keys.Up))
                            {
                                soundBank.PlayCue("Jump");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\MJump");
                                CurrentSpriteSheet = "Jump";

                                if (CharacterPosition.Y == Game.Window.ClientBounds.Height - 60)
                                {
                                    Speed.Y = 40;
                                    Jumping = true;
                                }
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region MageBall
                            if (Keyboard.GetState().IsKeyDown(Keys.Q) && !PrevKeyState.IsKeyDown(Keys.Q))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                if (CurrentXP >= 100)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                    CurrentXP -= 100;
                                    Character = Game.Content.Load<Texture2D>(@"Enemies\Mage\CastBall");
                                    CurrentSpriteSheet = "CastBall";
                                    soundBank.PlayCue("MageBallCharge");
                                }
                            }
                            #endregion
                            #region MageBlast
                            if (Keyboard.GetState().IsKeyDown(Keys.W) && !PrevKeyState.IsKeyDown(Keys.W))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                if (CurrentXP >= 200)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                    CurrentXP -= 200;
                                    CastMReps = 0;
                                    Character = Game.Content.Load<Texture2D>(@"Enemies\Mage\CastBall");
                                    CurrentSpriteSheet = "CastBlast";
                                    soundBank.PlayCue("MageBlastCharge");
                                }
                            }
                            #endregion
                            #region Shield
                            if (Keyboard.GetState().IsKeyDown(Keys.E) && !PrevKeyState.IsKeyDown(Keys.E))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                if (CurrentXP >= 300)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                    CurrentXP -= 300;
                                    FrameIncrement = true;
                                    Character = Game.Content.Load<Texture2D>(@"Enemies\Mage\ShieldM");
                                    CurrentSpriteSheet = "MShield";
                                    Speed.X = 0;
                                }
                            }
                            #endregion
                            #region SwitchMode
                            if (Keyboard.GetState().IsKeyDown(Keys.D1)
                                && !PrevKeyState.IsKeyDown(Keys.D1))
                            {
                                SetMode("FireMage", 0);
                                soundBank.PlayCue("FireBall");
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.D2)
                            && !PrevKeyState.IsKeyDown(Keys.D2) && HGAmmo > 0)
                            {
                                SetMode("HandGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D4)
                                && !PrevKeyState.IsKeyDown(Keys.D4) && AKAmmo > 0)
                            {
                                SetMode("AKGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D3)
                                && !PrevKeyState.IsKeyDown(Keys.D3) && LAmmo > 0)
                            {
                                SetMode("Launcher", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D6)
                                && !PrevKeyState.IsKeyDown(Keys.D6) && HasIMageHat)
                            {
                                SetMode("IceMage", 0);
                                soundBank.PlayCue("IMageBall");
                            }
                            #endregion
                        }
                        break;
                    }
                #endregion
                #region IceMage
                case CharacterMode.IceMage:
                    {
                        if (!Dying)
                        {
                            #region MoveLR
                            if (CurrentKeyboardState.IsKeyDown(Keys.Left))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80
                                    && !PrevKeyState.IsKeyDown(Keys.Left))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                    Direction = 'L';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = -9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            if (CurrentKeyboardState.IsKeyDown(Keys.Right))
                            {

                                if (CharacterPosition.Y >= Game.Window.ClientBounds.Height - 80
                                    && !PrevKeyState.IsKeyDown(Keys.Right))
                                {
                                    CurrentSpriteSheet = "StandMoveLR";
                                    Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                                    Direction = 'R';
                                    CurrentFrame.Y = 0;
                                    CurrentFrame.X = 1;
                                    FrameIncrement = true;
                                    Speed.X = 9;
                                    if (Running == null)
                                    {
                                        Running = soundBank.GetCue("Running");
                                        Running.Play();
                                    }
                                }
                            }
                            else if (CurrentSpriteSheet == "StandMoveLR"
                                && CurrentKeyboardState.IsKeyUp(Keys.Left)
                                && CurrentKeyboardState.IsKeyUp(Keys.Right))
                            {
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Speed.X = 0;
                                FrameIncrement = false;
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region Jump
                            if (CurrentKeyboardState.IsKeyDown(Keys.Up) && !PrevKeyState.IsKeyDown(Keys.Up))
                            {
                                soundBank.PlayCue("Jump");
                                CurrentFrame.X = 0;
                                CurrentFrame.Y = 0;
                                Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMJump");
                                CurrentSpriteSheet = "Jump";

                                if (CharacterPosition.Y == Game.Window.ClientBounds.Height - 60)
                                {
                                    Speed.Y = 40;
                                    Jumping = true;
                                }
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                            }
                            #endregion
                            #region MageBall
                            if (Keyboard.GetState().IsKeyDown(Keys.Q) && !PrevKeyState.IsKeyDown(Keys.Q))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                if (CurrentXP >= 100)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                    CurrentXP -= 100;
                                    Character = Game.Content.Load<Texture2D>(@"Enemies\IceMage\CastBall");
                                    CurrentSpriteSheet = "CastBall";
                                    soundBank.PlayCue("IMageBallCharge");
                                }
                            }
                            #endregion
                            #region MageBlast
                            if (Keyboard.GetState().IsKeyDown(Keys.W) && !PrevKeyState.IsKeyDown(Keys.W))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                if (CurrentXP >= 200)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                    CurrentXP -= 200;
                                    CastMReps = 0;
                                    Character = Game.Content.Load<Texture2D>(@"Enemies\IceMage\CastBall");
                                    CurrentSpriteSheet = "CastBlast";
                                    soundBank.PlayCue("IMageBlastCharge");
                                }
                            }
                            #endregion
                            #region Shield
                            if (Keyboard.GetState().IsKeyDown(Keys.E) && !PrevKeyState.IsKeyDown(Keys.E))
                            {
                                if (Running != null)
                                {
                                    if (Running.IsPlaying)
                                        Running.Stop(AudioStopOptions.Immediate);
                                    Running = null;
                                }
                                if (CurrentXP >= 300)
                                {
                                    CurrentFrame.X = 0;
                                    CurrentFrame.Y = 0;
                                    CurrentXP -= 300;
                                    FrameIncrement = true;
                                    Character = Game.Content.Load<Texture2D>(@"Enemies\IceMage\ShieldM");
                                    CurrentSpriteSheet = "MShield";
                                    Speed.X = 0;
                                }
                            }
                            #endregion
                            #region SwitchMode
                            if (Keyboard.GetState().IsKeyDown(Keys.D1)
                                && !PrevKeyState.IsKeyDown(Keys.D1))
                            {
                                SetMode("FireMage", 0);
                                soundBank.PlayCue("FireBall");
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.D2)
                            && !PrevKeyState.IsKeyDown(Keys.D2) && HGAmmo > 0)
                            {
                                SetMode("HandGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D4)
                                && !PrevKeyState.IsKeyDown(Keys.D4) && AKAmmo > 0)
                            {
                                SetMode("AKGunner", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D3)
                                && !PrevKeyState.IsKeyDown(Keys.D3) && LAmmo > 0)
                            {
                                SetMode("Launcher", 0);
                                soundBank.PlayCue("Reload");
                            }
                            else if (Keyboard.GetState().IsKeyDown(Keys.D5)
                                && !PrevKeyState.IsKeyDown(Keys.D5) && HasMageHat)
                            {
                                SetMode("Mage", 0);
                                soundBank.PlayCue("MageBall");
                            }
                            #endregion
                        }
                        break;
                    }
                #endregion
            }
        }
        //here  1 2 3 4 5
        private void DrawCharacter()
        {
            #region CharacterBody
            if (Direction == 'L')
            {
                spriteBatch.Draw(Character, CharacterPosition,
                    new Rectangle(CurrentFrame.X * FrameSize.X,
                        CurrentFrame.Y * FrameSize.Y,
                        FrameSize.X,
                        FrameSize.Y),
                    color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }
            else if (Direction == 'R')
            {
                spriteBatch.Draw(Character, CharacterPosition,
                    new Rectangle(CurrentFrame.X * FrameSize.X,
                        CurrentFrame.Y * FrameSize.Y,
                        FrameSize.X,
                        FrameSize.Y),
                    color, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            }
            #endregion
            #region HandGunner
            if (Mode == CharacterMode.HandGunner)
            {
                if (CurrentSpriteSheet == "StandMoveLR")
                {
                    if (Direction == 'L')
                    {
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            arms.SetOffset(new Vector2(33, 19));
                        else
                            arms.SetOffset(new Vector2(30, 20));
                    }
                    else
                    {
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            arms.SetOffset(new Vector2(35, 19));
                        else
                            arms.SetOffset(new Vector2(40, 20));
                    }
                    arms.Draw(spriteBatch);
                }
                else if (CurrentSpriteSheet == "Jump")
                {
                    if (Direction == 'L')
                    {
                        arms.SetOffset(new Vector2(33, 18));
                    }
                    else
                    {
                        arms.SetOffset(new Vector2(36, 18));
                    }
                    arms.Draw(spriteBatch);
                }
                spriteBatch.Draw(Sights, new Vector2(Mouse.GetState().X - 15, Mouse.GetState().Y - 14),
                    Color.White);
            }
            #endregion
            #region AKGunner
            else if (Mode == CharacterMode.AKGunner)
            {
                if (CurrentSpriteSheet == "StandMoveLR")
                {
                    if (Direction == 'L')
                    {
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            arms.SetOffset(new Vector2(33, 19));
                        else
                            arms.SetOffset(new Vector2(30, 20));
                    }
                    else
                    {
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            arms.SetOffset(new Vector2(35, 19));
                        else
                            arms.SetOffset(new Vector2(40, 20));
                    }
                    arms.Draw(spriteBatch);
                }
                else if (CurrentSpriteSheet == "Jump")
                {
                    if (Direction == 'L')
                    {
                        arms.SetOffset(new Vector2(33, 18));
                    }
                    else
                    {
                        arms.SetOffset(new Vector2(36, 18));
                    }
                    arms.Draw(spriteBatch);
                }
                spriteBatch.Draw(Sights, new Vector2(Mouse.GetState().X - 15, Mouse.GetState().Y - 14),
                    Color.White);
            }
            #endregion
            #region Launcher
            else if (Mode == CharacterMode.Launcher)
            {
                if (CurrentSpriteSheet == "StandMoveLR")
                {
                    if (Direction == 'L')
                    {
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            arms.SetOffset(new Vector2(33, 19));
                        else
                            arms.SetOffset(new Vector2(30, 20));
                    }
                    else
                    {
                        if (CurrentFrame.X == 0 && CurrentFrame.Y == 0)
                            arms.SetOffset(new Vector2(35, 19));
                        else
                            arms.SetOffset(new Vector2(40, 20));
                    }
                    arms.Draw(spriteBatch);
                }
                else if (CurrentSpriteSheet == "Jump")
                {
                    if (Direction == 'L')
                    {
                        arms.SetOffset(new Vector2(33, 18));
                    }
                    else
                    {
                        arms.SetOffset(new Vector2(36, 18));
                    }
                    arms.Draw(spriteBatch);
                }
                spriteBatch.Draw(Sights, new Vector2(Mouse.GetState().X - 15, Mouse.GetState().Y - 14),
                    Color.White);
            }
            #endregion
            #region Mage
            if (Mode == CharacterMode.Mage || Mode == CharacterMode.IceMage)
            {
                if (MShield != null)
                    MShield.Draw(spriteBatch);
            }
            #endregion
        }
        
        //In here
        #region returns
        public Vector2 GetPos()
        {
            return CharacterPosition;
        }
        public Rectangle GetARect()
        {
            return ARect;
        }
        public Rectangle GetBRect()
        {
            return BRect;
        }
        public Rectangle GetDRect()
        {
            return DRect;
        }
        public int GetDamage()
        {
            return Damage;
        }
        public Char GetDirection()
        {
            return Direction;
        }
        public bool Demolition()
        {
            if (ExpList.Count() > 0)
            {
                return true;
            }
            else
                return false;
        }
        //here 1 2 3 4 5
        public void Hit(Vector2 Pos, int D, string SoundName)
        {
            if (Running != null)
            {
                if (Running.IsPlaying)
                    Running.Stop(AudioStopOptions.Immediate);
                Running = null;
            }
            if (!Dying && Invincible <= 0)
            {
                #region HandleHit
                if (CanTakeHit)
                {
                    if (SoundName != "null")
                    {
                        soundBank.PlayCue(SoundName);
                    }
                    if (Healthi > 0)
                    {
                        CanTakeHit = false;
                        if (MShield != null)
                            MShield.Hit(D);
                        else
                            Healthi -= D;
                        if (Healthi < 0)
                            Healthi = 0;
                    }
                    switch (Mode)
                    {
                        #region FireMage
                        case CharacterMode.FireMage:
                            {
                                if (Pos.X > CharacterPosition.X + 35)
                                {
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\FrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\BehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
                                    }
                                }
                                if (Pos.X < CharacterPosition.X + 35)
                                {
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\FrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\BehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region HandGunner
                        case CharacterMode.HandGunner:
                            {
                                if (Pos.X > CharacterPosition.X + 35)
                                {
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GFrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GBehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
                                    }
                                }
                                if (Pos.X < CharacterPosition.X + 35)
                                {
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GFrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GBehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region AKGunner
                        case CharacterMode.AKGunner:
                            {
                                if (Pos.X > CharacterPosition.X + 35)
                                {
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\AKFrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\AKBehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
                                    }
                                }
                                if (Pos.X < CharacterPosition.X + 35)
                                {
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\AKFrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\AKBehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
                                    }
                                }
                                break;
                            }
                        #endregion
                        #region Launcher
                        case CharacterMode.Launcher:
                            {
                                if (Pos.X > CharacterPosition.X + 35)
                                {
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\LFrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\LBehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
                                    }
                                }
                                if (Pos.X < CharacterPosition.X + 35)
                                {
                                    if (Direction == 'L')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\LFrontHit");
                                        CurrentSpriteSheet = "FrontHit";
                                        FrameIncrement = true;
                                    }
                                    if (Direction == 'R')
                                    {
                                        CurrentFrame.X = 0;
                                        CurrentFrame.Y = 0;
                                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\LBehindHit1");
                                        CurrentSpriteSheet = "BehindHit1";
                                        FrameIncrement = true;
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
        public void CreateDust(int Amount, Vector2 Pos)
        {
            Random Rnd = new Random();
            soundBank.PlayCue("Kerching");
            for (int i = 0; i < Amount; i++)
            {
                DustList.Add(new GoldDust(Game.Content.Load<Texture2D>(@"Powers\GoldDust"),
                    Pos, new Point(0, 0), new Point(17, 16),
                    new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Red));
            }
        }
        public int GetXPlevel()
        {
            return MaxXP;
        }
        public bool CheckGameOver()
        {
            return GameOver;
        }
        public void SetLives(int Lives)
        {
            this.Lives = Lives;
        }
        public int GetLives()
        {
            return Lives;
        }
        public void RandomisePos()
        {
            CharacterPosition.X = MRnd.Next(0, Game.Window.ClientBounds.Width - 70);
            CharacterPosition.Y = MRnd.Next(0, Game.Window.ClientBounds.Height - 60);
        }
        public void IceBallHit(int Time)
        {
            IceTimer = Time;
            color = Color.Aqua;
            IceHit = true;
        }
        public void SetXPLevel(int XPLevel)
        {
            CurrentXP = XPLevel;
            MaxXP = XPLevel;
        }
        public void StopAllSound()
        {
            if (Running != null)
            {
                if (Running.IsPlaying)
                    Running.Stop(AudioStopOptions.Immediate);
                Running = null;
            }
            if (FireCrackle != null)
            {
                if (FireCrackle.IsPlaying && WallList.Count() == 0)
                {
                    FireCrackle.Stop(AudioStopOptions.Immediate);
                    FireCrackle = null;
                }
            }
        }
        public void StickHit()
        {
            StickHasHit = true;
        }
        //here 1 2 3 4 5
        public void SetMode(string ChangeMode, int Bullets)
        {
            switch (ChangeMode)
            {
                case "FireMage" :
                    {
                        Mode = CharacterMode.FireMage;
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\StandMoveLR");
                        CurrentSpriteSheet = "StandMoveLR";
                        Speed = new Vector2(0, 0);
                        FrameIncrement = false;
                        MShield = null;
                        break;
                    }
                case "HandGunner":
                    {
                        Mode = CharacterMode.HandGunner;
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                        CurrentSpriteSheet = "StandMoveLR";
                        Speed = new Vector2(0, 0);
                        FrameIncrement = false;
                        HGAmmo += Bullets;
                        FireRate = 500;
                        FireRate = 500;
                        arms = new MCArms(Game.Content.Load<Texture2D>(@"Enemies\Gunner1\Arms"),
                          new Vector2(CharacterPosition.X + 35, CharacterPosition.Y + 19),
                          new Point(70, 50), new Vector2(35, 19), 0f, Direction);
                        MShield = null;
                        break;
                    }
                case "AKGunner":
                    {
                        Mode = CharacterMode.AKGunner;
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                        CurrentSpriteSheet = "StandMoveLR";
                        Speed = new Vector2(0, 0);
                        FrameIncrement = false;
                        AKAmmo += Bullets;
                        FireRate = 83;
                        FireTimer = 83;
                        arms = new MCArms(Game.Content.Load<Texture2D>(@"Enemies\Gunner2\Arms"),
                          new Vector2(CharacterPosition.X + 35, CharacterPosition.Y + 19),
                          new Point(70, 50), new Vector2(35, 19), 0f, Direction);
                        MShield = null;
                        break;
                    }
                case "Launcher":
                    {
                        Mode = CharacterMode.Launcher;
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\GStandMoveLR");
                        CurrentSpriteSheet = "StandMoveLR";
                        Speed = new Vector2(0, 0);
                        FrameIncrement = false;
                        LAmmo += Bullets;
                        FireRate = 1500;
                        FireTimer = 1500;
                        arms = new MCArms(Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\Arms"),
                          new Vector2(CharacterPosition.X + 35, CharacterPosition.Y + 19),
                          new Point(70, 50), new Vector2(35, 19), 0f, Direction);
                        MShield = null;
                        break;
                    }
                case "Mage":
                    {
                        Mode = CharacterMode.Mage;
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\MStandMoveLR");
                        CurrentSpriteSheet = "StandMoveLR";
                        Speed = new Vector2(0, 0);
                        FrameIncrement = false;
                        HasMageHat = true;
                        break;
                    }
                case "IceMage":
                    {
                        Mode = CharacterMode.IceMage;
                        CurrentFrame.X = 0;
                        CurrentFrame.Y = 0;
                        Character = Game.Content.Load<Texture2D>(@"MainCharacter\IMStandMoveLR");
                        CurrentSpriteSheet = "StandMoveLR";
                        Speed = new Vector2(0, 0);
                        FrameIncrement = false;
                        HasIMageHat = true;
                        break;
                    }
            }
        }
        //here 1 2 3 4 5
        public List<Powers> GetColRects()
        {
            List<Powers> ColRects = new List<Powers>();
            for (int i = 0; i < BulletList.Count(); i++)
            {
                Powers BulletInfo = new Powers();
                BulletInfo.ARect = BulletList[i].GetARect();
                BulletInfo.index = i;
                BulletInfo.type = "Bullet";
                ColRects.Add(BulletInfo);
            }
            for (int i = 0; i < RocketList.Count(); i++)
            {
                Powers RocketInfo = new Powers();
                RocketInfo.ARect = RocketList[i].GetARect();
                RocketInfo.index = i;
                RocketInfo.type = "Rocket";
                ColRects.Add(RocketInfo);
            }
            for (int i = 0; i < MBallList.Count(); i++)
            {
                Powers BallInfo = new Powers();
                BallInfo.ARect = MBallList[i].GetARect();
                BallInfo.index = i;
                BallInfo.type = "MageBall";
                ColRects.Add(BallInfo);
            }
            for (int i = 0; i < MBlastList.Count(); i++)
            {
                Powers BlastInfo = new Powers();
                BlastInfo.ARect = MBlastList[i].GetARect();
                BlastInfo.index = i;
                BlastInfo.type = "MageBlast";
                ColRects.Add(BlastInfo);
            }
            for (int i = 0; i < IMBallList.Count(); i++)
            {
                Powers BallInfo = new Powers();
                BallInfo.ARect = IMBallList[i].GetARect();
                BallInfo.index = i;
                BallInfo.type = "IMageBall";
                ColRects.Add(BallInfo);
            }
            for (int i = 0; i < IMBlastList.Count(); i++)
            {
                Powers BlastInfo = new Powers();
                BlastInfo.ARect = IMBlastList[i].GetARect();
                BlastInfo.index = i;
                BlastInfo.type = "IceBlast";
                ColRects.Add(BlastInfo);
            }
            return ColRects;
        }
        //here 1 2 3 4 5
        public void PowerHit(string Type, int Index)
        {
            switch (Type)
            {
                case "Bullet":
                    {
                        BulletList.Remove(BulletList[Index]);
                        break;
                    }
                case "Rocket":
                    {
                        RocketList[Index].Hit();
                        break;
                    }
                case "MageBall":
                    {
                        MBallList[Index].Hit(Vector2.Zero);
                        break;
                    }
                case "MageBlast":
                    {
                        MBlastList[Index].Hit();
                        break;
                    }
                case "IMageBall":
                    {
                        IMBallList[Index].Hit(Vector2.Zero);
                        break;
                    }
                case "IceBlast":
                    {
                        IMBlastList[Index].Hit();
                        break;
                    }
            }
        }
        public void PowerUp(char type)
        {
            switch (type)
            {
                #region Ammo
                case 'A':
                    soundBank.PlayCue("Reload");
                    switch (Mode)
                    {
                        case CharacterMode.FireMage:
                            {
                                MaxXP += (MaxXP / 10);
                                break;
                            }
                        case CharacterMode.HandGunner:
                            {
                                HGAmmo += 100;
                                break;
                            }
                        case CharacterMode.AKGunner:
                            {
                                AKAmmo += 500;
                                break;
                            }
                        case CharacterMode.Launcher:
                            {
                                LAmmo += 50;
                                break;
                            }
                        case CharacterMode.Mage:
                            {
                                break;
                            }
                        case CharacterMode.IceMage:
                            {
                                break;
                            }
                    }
                    break;
                #endregion
                #region Rain
                case 'R':
                    {
                        soundBank.PlayCue("Reload");
                        Rain += 8000;
                    }
                    break;
                #endregion
                #region ExtraLives
                case '+':
                    Lives += 1;
                    break;
                #endregion
                #region Invincible
                case 'I':
                    Invincible = 20000;
                    soundBank.PlayCue("PowerBull");
                    break;
                #endregion
                #region Demolition
                case 'D':
                    ExpList.Add(new Explosion(Game.Content.Load<Texture2D>(@"Powers\Explosion"),
                                        new Vector2(0, -15),
                                        new Point(0, 0),
                                        new Point(395, 295),
                                        2.05f));
                    soundBank.PlayCue("Demolition");
                    break;
                #endregion
            }
        }
        public void Deflect(string Type)
        {
            switch (Type)
            {
                #region Bullet
                case "Bullet":
                    {
                        BulletList.Add(new Bullet(BulletTex,
                                       new Vector2(CharacterPosition.X + 35, CharacterPosition.Y + 19),
                                       new Point(6, 20), new Vector2(2, 7), MathHelper.ToRadians(90f),
                                       Direction, new Vector2(30, 0)));
                        break;
                    }
                #endregion
                #region Ball
                case "Ball":
                    {
                        Point Frame = new Point(0, 0);
                        int Xoffset = 0;
                        if (Direction == 'R')
                        {
                            Xoffset = -40;
                            Frame.Y = 5;
                        }
                        else if (Direction == 'L')
                        {
                            Xoffset = 30;
                        }

                        BallList.Add(new FireBall(Game.Content.Load<Texture2D>(@"Powers\FireBall"),
                                        new Vector2(CharacterPosition.X - Xoffset, CharacterPosition.Y),
                                        Frame, new Point(70, 50), 1, Direction));
                        break;
                    }
                #endregion
            }
        }
        #endregion
    }
}
