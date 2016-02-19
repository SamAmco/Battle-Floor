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
    public class Enemies : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region variables
        SpriteBatch spriteBatch;
        Texture2D Test;
        SpriteFont spriteFont;
        int Level = 0;
        Random Rnd = new Random();
        int LevelGapTimer = 10000;
        bool LevelIsChanging;
        bool CanLevel = false;
        bool gameComplete = false;
        string DrawHelpLine = "";
        //For each enemy/object added, edit where comented below,
        //including here. 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16
        #region LevelArray
        /* Enemy Index
         * 1 = Basic Enemy(2)
         * 2 = Blind Giant(20)Take on with 50XP
         * 3 = Rocket Launcher(2)
         * 4 = Mage(30)Take on with 400XP
         * 5 = IceMage(35)
         * 6 = IceGiant(25)
         * 7 = Standard Gunner1(2)
         * 8 = AKGunner1 (5)
         */
        int[,] LevelInfo = new int[40, 10]// Level  / Cumulative frequency XP
        #region Phase1 BE's
        {{7, 0, 0, 0, 0, 0, 0, 0, 0, 0},//1   
        {7, 7, 7, 0, 0, 0, 0, 0, 0, 0},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {1, 7, 0, 0, 0, 0, 0, 0, 0, 0},
        {1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 7},
        {0, 0, 0, 0, 0, 1, 1, 1, 7, 7},
        {7, 0, 0, 1, 1, 1, 1, 0, 7, 7},//8    
        #endregion
        #region Phase2 BlindGiant's
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 2},//9       
        {0, 0, 0, 0, 7, 7, 0, 0, 0, 2},
        {1, 1, 1, 7, 7, 7, 7, 0, 0, 2},
        {1, 1, 1, 1, 1, 0, 0, 0, 0, 2},
        {1, 1, 1, 7, 7, 7, 0, 0, 2, 2},
        {1, 1, 1, 1, 2, 2, 7, 7, 7, 2},
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},//16              
        #endregion
        #region Phase3 Military
        {3, 0, 0, 0, 0, 0, 0, 0, 0, 3},//17
        {1, 0, 0, 0, 3, 3, 0, 0, 0, 1},
        {3, 3, 0, 0, 2, 0, 0, 0, 3, 3},
        {8, 8, 8, 0, 3, 3, 3, 0, 0, 2},
        {8, 8, 8, 3, 3, 3, 2, 3, 3, 3},
        {3, 3, 3, 3, 3, 3, 3, 3, 3, 3},
        {2, 3, 3, 3, 3, 3, 8, 8, 8, 2},
        {3, 3, 3, 2, 2, 2, 8, 8, 8, 3},//24
        #endregion
        #region Phase4 Mage's
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 4},//25
        {4, 0, 0, 0, 0, 0, 0, 0, 0, 4},
        {4, 4, 0, 0, 3, 3, 0, 0, 4, 4},
        {4, 4, 0, 0, 7, 7, 0, 0, 4, 4},
        {4, 4, 0, 0, 8, 8, 0, 0, 4, 4},
        {4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
        {4, 4, 4, 8, 8, 8, 8, 4, 4, 4},
        {4, 8, 7, 3, 2, 1, 3, 7, 8, 4},//32
        #endregion
        #region Phase5 Ice
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 5},//33
        {3, 3, 0, 0, 3, 3, 0, 0, 8, 6},
        {8, 8, 8, 6, 3, 3, 8, 8, 8, 6},
        {4, 4, 0, 0, 6, 6, 0, 0, 5, 5},
        {6, 6, 6, 3, 3, 3, 3, 6, 6, 6},
        {6, 6, 6, 6, 6, 6, 6, 6, 6, 6},
        {8, 3, 6, 5, 5, 5, 3, 6, 3, 8},
        {4, 4, 0, 0, 6, 6, 5, 5, 5, 5}};//40
        #endregion
        #endregion
        //here 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16

        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;

        List<BasicEnemy> BEList = new List<BasicEnemy>();
        List<BlindGiant> BGList = new List<BlindGiant>();
        List<RocketLauncher> LauncherList = new List<RocketLauncher>();
        List<Mage> MageList = new List<Mage>(); 
        List<IceMage> IceMageList = new List<IceMage>();
        List<IceGiant> IGList = new List<IceGiant>();
        List<Gunner> StdGun1List = new List<Gunner>();
        List<Gunner> AKGun1List = new List<Gunner>();
        List<HandGun> HGList = new List<HandGun>();
        List<AK47> AKList = new List<AK47>();
        List<Launcher> RLList = new List<Launcher>();
        List<MageHat> MHatList = new List<MageHat>();
        List<IceMageHat> IMHatList = new List<IceMageHat>();
        Balloon balloon;
        Crate crate;
        EmptyBalloon emptyBalloon;
        public struct Powers
        {
            public Rectangle ARect;
            public string type;
            public int index;
        }
        #endregion

        public Enemies(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            Test = Game.Content.Load<Texture2D>(@"MainCharacter\untitled");

            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            spriteFont = Game.Content.Load<SpriteFont>("SpriteFont");
            bool succeed = false;
            while (!succeed)
            {
                if (LoadAudioEngine())
                    succeed = true;
                else succeed = false;
            }
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");

            //balloon = new Balloon(Game.Content.Load<Texture2D>(@"Powerups\Balloon"),
            //    Game.Content.Load<Texture2D>(@"Powerups\Crate"), new Vector2(0, 200),
            //    new Point(60, 80), new Point(50, 50), new Point(1, 0));
            base.LoadContent();
        }

        private bool LoadAudioEngine()
        {
            try
            {
                audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
                return true;
            }
            catch
            {
                LoadAudioEngine();
                return false;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            DrawHelpLine = "";
            ColDet();
            EnemyDisposal();
            GameLogic(gameTime);
            UpdateEnemies(gameTime);
            base.Update(gameTime);
        }
        //here 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16
        private void UpdateEnemies(GameTime gameTime)
        {
            Rectangle ClientBounds = Game.Window.ClientBounds;
            foreach (BasicEnemy b in BEList)
            {
                b.Update(gameTime, ClientBounds);
            }
            foreach (BlindGiant b in BGList)
            {
                b.Update(gameTime, ClientBounds);
            }
            foreach (RocketLauncher r in LauncherList)
            {
                r.Update(gameTime, ClientBounds);
            }
            foreach (Mage m in MageList)
            {
                m.Update(gameTime, ClientBounds);
            }
            foreach (IceMage i in IceMageList)
            {
                i.Update(gameTime, ClientBounds);
            }
            foreach (IceGiant i in IGList)
            {
                i.Update(gameTime, ClientBounds);
            }
            foreach (Gunner g in StdGun1List)
            {
                g.Update(gameTime, ClientBounds);
            }
            foreach (Gunner g in AKGun1List)
            {
                g.Update(gameTime, ClientBounds);
            }
            foreach (HandGun g in HGList)
            {
                g.Update(gameTime);
            }
            foreach (AK47 a in AKList)
            {
                a.Update(gameTime);
            }
            foreach (Launcher l in RLList)
            {
                l.Update(gameTime);
            }
            foreach (MageHat m in MHatList)
            {
                m.Update(gameTime);
            }
            foreach (IceMageHat i in IMHatList)
            {
                i.Update(gameTime);
            }
            if (balloon != null)
                balloon.Update(gameTime, ClientBounds);
            if (emptyBalloon != null)
                emptyBalloon.Update(gameTime, ClientBounds);
            if (crate != null)
                crate.Update(gameTime, ClientBounds);
        }
        //here 1 2 3 4 5 6 7 8 9 10 11 12 13 15
        private void GameLogic(GameTime gameTime)
        {
            if (Level == 40)
                gameComplete = true;
            else if (ScreenIsClear())
            {
                LevelGapTimer += gameTime.ElapsedGameTime.Milliseconds;
                if (!LevelIsChanging && LevelGapTimer >= 1000)
                {
                    LevelGapTimer = 0;
                    for (int Column = 0; Column < LevelInfo.GetLength(1); Column++)
                    {
                        CanLevel = true;
                        switch (LevelInfo[Level, Column])
                        {
                            #region 0
                            case 0:
                                {
                                    break;
                                }
                            #endregion
                            #region BasicEnemy
                            case 1:
                                {
                                    BEList.Add(new BasicEnemy(Game.Content.Load<Texture2D>(@"Enemies\BasicEnemy"),
                                        new Vector2(Rnd.Next(0, Game.Window.ClientBounds.Width), 100),
                                        new Point(50, 50),
                                            new Vector2(25, 25), 0, Color.White));
                                    break;
                                }
                            #endregion
                            #region BlindGiant
                            case 2:
                                {
                                    BGList.Add(new BlindGiant(Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\Running"),
                                        Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\HitWall"),
                                        Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\TakeHit"),
                                        Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\Dying"),
                                        new Vector2(-Rnd.Next(250, 1000), Game.Window.ClientBounds.Height - 260),
                                        new Point(350, 250), new Point(0, 0), 'R'));
                                    break;
                                }
                            #endregion
                            #region RocketLauncher
                            case 3:
                                {
                                    Random Rnd = new Random();
                                    char Direction;
                                    Vector2 Pos;
                                    if (Column % 2 > 0)
                                    {
                                        Direction = 'L';
                                        Pos = new Vector2(Game.Window.ClientBounds.Width + (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }
                                    else
                                    {
                                        Direction = 'R';
                                        Pos = new Vector2(-70 - (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }

                                    LauncherList.Add(new RocketLauncher(
                                        Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\StandMove"),
                                        Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\Launch"),
                                        Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\Rocket"),
                                        Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\DieBack"),
                                        Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\DieForward"),
                                        Game.Content.Load<Texture2D>(@"Enemies\RocketLauncher\Explosion"),
                                        Pos, new Point(70, 50), new Point(0, 0), Direction, Column + 1));
                                    break;
                                }
                            #endregion
                            #region Mage
                            case 4:
                                {
                                    Random Rnd = new Random();
                                    char Direction;
                                    Vector2 Pos;
                                    if (Column % 2 > 0)
                                    {
                                        Direction = 'L';
                                        Pos = new Vector2(Game.Window.ClientBounds.Width + (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }
                                    else
                                    {
                                        Direction = 'R';
                                        Pos = new Vector2(-70 - (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }

                                    MageList.Add(new Mage(
                                        Game.Content.Load<Texture2D>(@"Enemies\Mage\StandMove"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Mage\CastBall"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Mage\Ball"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Mage\Blast"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Mage\ShieldM"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Mage\Shield"),
                                        Pos, new Point(70, 50), new Point(0, 0), Direction, Column + 1));
                                    break;
                                }
                            #endregion
                            #region IceMage
                            case 5:
                                {
                                    Random Rnd = new Random();
                                    char Direction;
                                    Vector2 Pos;
                                    if (Column % 2 > 0)
                                    {
                                        Direction = 'L';
                                        Pos = new Vector2(Game.Window.ClientBounds.Width + (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }
                                    else
                                    {
                                        Direction = 'R';
                                        Pos = new Vector2(-70 - (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }

                                    IceMageList.Add(new IceMage(
                                        Game.Content.Load<Texture2D>(@"Enemies\IceMage\StandMove"),
                                        Game.Content.Load<Texture2D>(@"Enemies\IceMage\CastBall"),
                                        Game.Content.Load<Texture2D>(@"Enemies\IceMage\Ball"),
                                        Game.Content.Load<Texture2D>(@"Enemies\IceMage\Blast"),
                                        Game.Content.Load<Texture2D>(@"Enemies\IceMage\ShieldM"),
                                        Game.Content.Load<Texture2D>(@"Enemies\IceMage\Shield"),
                                        Pos, new Point(70, 50), new Point(0, 0), Direction, Column + 1));
                                    break;
                                }
                            #endregion
                            #region IceGiant
                            case 6:
                                {
                                    IGList.Add(new IceGiant(Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\Running"),
                                        Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\HitWall"),
                                        Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\TakeHit"),
                                        Game.Content.Load<Texture2D>(@"Enemies\BlindGiant\Dying"),
                                        new Vector2(-Rnd.Next(250, 1000), Game.Window.ClientBounds.Height - 260),
                                        new Point(350, 250), new Point(0, 0), 'R'));
                                    break;
                                }
                            #endregion
                            #region StdGun1
                            case 7:
                                {
                                    Random Rnd = new Random();
                                    char Direction;
                                    Vector2 Pos;
                                    if (Column % 2 > 0)
                                    {
                                        Direction = 'L';
                                        Pos = new Vector2(Game.Window.ClientBounds.Width + (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }
                                    else
                                    {
                                        Direction = 'R';
                                        Pos = new Vector2(-70 - (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }

                                    StdGun1List.Add(new Gunner(Game.Content.Load<Texture2D>(@"Enemies\Gunner1\StandMoveLR"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Gunner1\Arms"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Gunner1\TakeHit"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Gunner1\Bullet"),
                                        Pos, new Point(70, 50), new Point(0, 0), Direction, Column, 500, 2, 50));
                                    break;
                                }
                            #endregion
                            #region AKGun1
                            case 8:
                                {
                                    Random Rnd = new Random();
                                    char Direction;
                                    Vector2 Pos;
                                    if (Column % 2 > 0)
                                    {
                                        Direction = 'L';
                                        Pos = new Vector2(Game.Window.ClientBounds.Width + (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }
                                    else
                                    {
                                        Direction = 'R';
                                        Pos = new Vector2(-70 - (10 * Column),
                                            Game.Window.ClientBounds.Height - 60);
                                    }

                                    AKGun1List.Add(new Gunner(Game.Content.Load<Texture2D>(@"Enemies\Gunner2\StandMoveLR"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Gunner2\Arms"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Gunner2\TakeHit"),
                                        Game.Content.Load<Texture2D>(@"Enemies\Gunner1\Bullet"),
                                        Pos, new Point(70, 50), new Point(0, 0), Direction, Column, 83, 5, 50));
                                    break;
                                }
                            #endregion
                        }
                    }
                }
                else if (Level < 40 && CanLevel)
                {
                    CanLevel = false;
                    Level += 1;
                }
            }
        }
        //here 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16
        private bool ScreenIsClear()
        {
            if (BEList.Count() == 0 && BGList.Count() == 0 && LauncherList.Count() == 0
                && MageList.Count() == 0 && IceMageList.Count() == 0 && IGList.Count() == 0
                && StdGun1List.Count() == 0 && AKGun1List.Count() == 0 && balloon == null)
            {
                return true;
            }
            else
                return false;
        }
        //here 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16
        private void EnemyDisposal()
        {
            #region BasicEnemy
            for (int i = 0; i < BEList.Count(); i++)
            {
                bool Dispose = BEList[i].Dispose();
                if (Dispose)
                {
                    BEList.Remove(BEList[i]);
                }
            }
            #endregion
            #region BlindGiant
            for (int i = 0; i < BGList.Count(); i++)
            {
                bool Dispose = BGList[i].Dispose();
                if (Dispose)
                {
                    BGList.Remove(BGList[i]);
                }
            }
            #endregion
            #region RocketLauncher
            for (int i = 0; i < LauncherList.Count(); i++)
            {
                bool Dispose = LauncherList[i].Dispose();
                if (Dispose)
                {
                    RLList.Add(new Launcher(Game.Content.Load<Texture2D>(@"Enemies\Objects\Launcher"),
                        new Vector2(LauncherList[i].GetPos().X + 10, LauncherList[i].GetPos().Y + 35),
                        new Point(40, 12), new Point(0, 0), 'L', 1f));
                    LauncherList.Remove(LauncherList[i]);
                }
            }
            #endregion
            #region Mage
            for (int i = 0; i < MageList.Count(); i++)
            {
                bool Dispose = MageList[i].Dispose();
                if (Dispose)
                {
                    MHatList.Add(new MageHat(Game.Content.Load<Texture2D>(@"Enemies\Objects\MageHat"),
                        MageList[i].GetPos(), new Point(81, 85), new Point(0, 0), 'L', 0.5f));
                    MageList.Remove(MageList[i]);
                }
            }
            #endregion
            #region IceMage
            for (int i = 0; i < IceMageList.Count(); i++)
            {
                bool Dispose = IceMageList[i].Dispose();
                if (Dispose)
                {
                    IMHatList.Add(new IceMageHat(Game.Content.Load<Texture2D>(@"Enemies\Objects\IceMageHat"),
                        IceMageList[i].GetPos(), new Point(81, 85), new Point(0, 0), 'L', 0.5f));
                    IceMageList.Remove(IceMageList[i]);
                }
            }
            #endregion
            #region IceGiant
            for (int i = 0; i < IGList.Count(); i++)
            {
                bool Dispose = IGList[i].Dispose();
                if (Dispose)
                {
                    IGList.Remove(IGList[i]);
                }
            }
            #endregion
            #region StdGunner
            for (int i = 0; i < StdGun1List.Count(); i++)
            {
                bool Dispose = StdGun1List[i].Dispose();
                if (Dispose)
                {
                    HGList.Add(new HandGun(Game.Content.Load<Texture2D>(@"Enemies\Objects\HandGun"), 
                        new Vector2(StdGun1List[i].GetPos().X + 10, StdGun1List[i].GetPos().Y + 35), new Point(50, 35),
                        new Point (0, 0), StdGun1List[i].GetDirection(), 0.5f));
                    StdGun1List.Remove(StdGun1List[i]);
                }
            }
            #endregion
            #region AKGunner
            for (int i = 0; i < AKGun1List.Count(); i++)
            {
                bool Dispose = AKGun1List[i].Dispose();
                if (Dispose)
                {
                    AKList.Add(new AK47(Game.Content.Load<Texture2D>(@"Enemies\Objects\AK47"),
                        new Vector2(AKGun1List[i].GetPos().X + 10, AKGun1List[i].GetPos().Y + 35), new Point(80, 36),
                        new Point(0, 0), AKGun1List[i].GetDirection(), 0.5f));
                    AKGun1List.Remove(AKGun1List[i]);
                }
            }
            #endregion
            #region HandGun
            for (int i = 0; i < HGList.Count(); i++)
            {
                bool Dispose = HGList[i].Dispose();
                if (Dispose)
                {
                    HGList.Remove(HGList[i]);
                }
            }
            #endregion
            #region AK47
            for (int i = 0; i < AKList.Count(); i++)
            {
                bool Dispose = AKList[i].Dispose();
                if (Dispose)
                {
                    AKList.Remove(AKList[i]);
                }
            }
            #endregion
            #region Launcher
            for (int i = 0; i < RLList.Count(); i++)
            {
                bool Dispose = RLList[i].Dispose();
                if (Dispose)
                {
                    RLList.Remove(RLList[i]);
                }
            }
            #endregion
            #region MageHat
            for (int i = 0; i < MHatList.Count(); i++)
            {
                bool Dispose = MHatList[i].Dispose();
                if (Dispose)
                {
                    MHatList.Remove(MHatList[i]);
                }
            }
            #endregion
            #region IMageHat
            for (int i = 0; i < IMHatList.Count(); i++)
            {
                bool Dispose = IMHatList[i].Dispose();
                if (Dispose)
                {
                    IMHatList.Remove(IMHatList[i]);
                }
            }
            #endregion
            #region Balloon
            if (balloon != null)
            {
                bool Dispose = balloon.Dispose();
                if (Dispose)
                {
                    balloon = null;
                }
            }
            #endregion
            #region emptyBalloon
            if (emptyBalloon != null)
            {
                bool Dispose = emptyBalloon.Dispose();
                if (Dispose)
                {
                    emptyBalloon = null;
                }
            }
            #endregion
            #region Crate
            if (crate != null)
            {
                bool Dispose = crate.Dispose();
                if (Dispose)
                {
                    crate = null;
                }
            }
            #endregion
        }
        //here 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16
        private void ColDet()
        {
            List<Powers> ColRects = new List<Powers>();
            foreach (MainCharacter.Powers p in Game1.mainCharacter.GetColRects())
            {
                Powers po = new Powers();
                po.ARect = p.ARect;
                po.index = p.index;
                po.type = p.type;
                ColRects.Add(po);
            }
            #region BasicEnemy
            foreach (BasicEnemy b in BEList)
            {
                Vector2 Pos = b.GetPos();
                Rectangle BRec = b.GetRect();
                Rectangle BRect = Game1.mainCharacter.GetBRect();
                #region CharacterMoves
                Rectangle ARect = Game1.mainCharacter.GetARect();
                if (ARect.Intersects(BRec))
                {
                    Game1.mainCharacter.StickHit();
                    Char Direction = Game1.mainCharacter.GetDirection();
                    if (Direction == 'L')
                    {
                        b.CombatHit(Game1.mainCharacter.GetDamage(), -5);
                    }
                    else
                    {
                        b.CombatHit(Game1.mainCharacter.GetDamage(), 5);
                    }
                }
                if (BRect.Intersects(BRec))
                {
                    Game1.mainCharacter.StickHit();
                    Char Direction = Game1.mainCharacter.GetDirection();
                    if (Direction == 'L')
                    {
                        b.CombatHit(Game1.mainCharacter.GetDamage(), -5);
                    }
                    else
                    {
                        b.CombatHit(Game1.mainCharacter.GetDamage(), 5);
                    }
                }
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                if (DRect.Intersects(BRec))
                {
                    Game1.mainCharacter.Hit(Pos, 5, "Chainsaw");
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(BRec))
                    {
                        b.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
                #endregion
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    b.CombatHit(10, 0);
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    if (BRec.Intersects(SRect))
                    {
                        soundBank.PlayCue("Chainsaw");
                        b.HitShield();
                    }
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(BRec))
                    {
                        soundBank.PlayCue("Chainsaw");
                        b.HitWall();
                    }
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(BRec))
                    {
                        f.Kill();
                        b.HitBall();
                    }
                }
                #endregion

            }
            #endregion
            #region BlindGiant
            foreach (BlindGiant b in BGList)
            {
                Vector2 Pos = b.GetPos();
                Rectangle DRec = b.GetDRect();
                Rectangle ARec = b.GetARect();
                #region CharacterMoves
                Rectangle ARect = Game1.mainCharacter.GetARect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                if (ARect.Intersects(DRec))
                {
                    Game1.mainCharacter.StickHit();
                    b.CombatHit(Game1.mainCharacter.GetDamage());
                }
                if (DRect.Intersects(ARec))
                {
                    Game1.mainCharacter.Hit(Pos, 40, "null");
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(DRec))
                    {
                        b.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
                #endregion
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    b.CombatHit(10);
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    if (ARec.Intersects(SRect))
                    {
                        b.HitShield();
                    }
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(ARec))
                    {
                        b.HitFireWall();
                    }
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(DRec))
                    {
                        f.Kill();
                        b.CombatHit(10);
                    }
                    if (fRect.Intersects(ARec))
                    {
                        f.Kill();
                    }
                }
                #endregion
            }
            #endregion
            #region RocketLauncher
            foreach (RocketLauncher l in LauncherList)
            {
                Vector2 CharPos = Game1.mainCharacter.GetPos();
                Vector2 Pos = l.GetPos();
                Vector2 Zero = new Vector2(0, 0);
                Rectangle DRec = l.GetDRect();
                Rectangle ARec = l.GetARect();
                #region CharacterMoves
                Rectangle ARect = Game1.mainCharacter.GetARect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                if (ARect.Intersects(DRec))
                {
                    Game1.mainCharacter.StickHit();
                    l.CombatHit();
                }
                if (DRect.Intersects(ARec))
                {
                    Game1.mainCharacter.Hit(Pos, 20, "null");
                    l.RocketHit(CharPos);
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(DRec))
                    {
                        l.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
                #endregion
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    l.CombatHit();
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    if (ARec.Intersects(SRect) && l.ColDet())
                        l.RocketHit(Zero);
                    if (DRec.Intersects(SRect))
                        l.HitShield();
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(DRec))
                        l.HitFireWall();
                    if (WRect.Intersects(ARec) && l.ColDet())
                        l.RocketHit(Zero);
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(DRec))
                    {
                        f.Kill();
                        l.CombatHit();
                    }
                    if (fRect.Intersects(ARec) && l.ColDet())
                    {
                        f.Kill();
                        l.RocketHit(Zero);
                    }
                }
                #endregion
            }
            #endregion
            #region Mage
            foreach (Mage m in MageList)
            {
                Vector2 CharPos = Game1.mainCharacter.GetPos();
                Vector2 Pos = m.GetPos();
                Vector2 Zero = new Vector2(0, 0);
                Rectangle DRec = m.GetDRect();
                Rectangle ARec = m.GetARect();
                Rectangle BRec = m.GetBRect();
                Rectangle BRect = Game1.mainCharacter.GetBRect();
                Rectangle ARect = Game1.mainCharacter.GetARect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    m.CombatHit(20, false);
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    if (ARec.Intersects(SRect) && m.ColDet())
                        m.BallHit(Zero);
                    if (DRec.Intersects(SRect))
                        m.HitShield();
                    if (BRec.Intersects(SRect))
                        m.BlastHit();
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(DRec))
                        m.HitFireWall();
                    if (WRect.Intersects(ARec) && m.ColDet())
                        m.BallHit(Zero);
                    if (BRec.Intersects(WRect))
                        m.BlastHit();
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(DRec))
                    {
                        f.Kill();
                        m.CombatHit(8, false);
                    }
                    if (fRect.Intersects(ARec) && m.ColDet())
                    {
                        f.Kill();
                        m.BallHit(Zero);
                    }
                    if (BRec.Intersects(fRect))
                        f.Kill();
                }
                #endregion
                #region CharacterMoves
                if (ARect.Intersects(DRec))
                {
                    Game1.mainCharacter.StickHit();
                    m.CombatHit(Game1.mainCharacter.GetDamage(), false);
                }
                if (ARec.Intersects(BRect))
                {
                    Game1.mainCharacter.Deflect("Ball");
                    m.BallHit(CharPos);
                }
                if (BRec.Intersects(BRect))
                {
                    m.BlastHit();
                }
                if (DRect.Intersects(ARec))
                {
                    Game1.mainCharacter.Hit(Pos, 10, "null");
                    Game1.mainCharacter.RandomisePos();
                    m.BallHit(CharPos);
                }
                if (DRect.Intersects(BRec) && m.BlastIsInclined())
                {
                    Game1.mainCharacter.Hit(Pos, 30, "null");
                    Game1.mainCharacter.RandomisePos();
                    m.BlastHit();
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(DRec))
                    {
                        m.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
                #endregion
            }
            #endregion
            #region IceMage
            foreach (IceMage m in IceMageList)
            {
                Vector2 CharPos = Game1.mainCharacter.GetPos();
                Vector2 Pos = m.GetPos();
                Vector2 Zero = new Vector2(0, 0);
                Rectangle DRec = m.GetDRect();
                List<Rectangle> ARec = m.GetARect();
                Rectangle BRec = m.GetBRect();
                Rectangle BRect = Game1.mainCharacter.GetBRect();
                Rectangle ARect = Game1.mainCharacter.GetARect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    m.CombatHit(10, "Demolition");
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(SRect) && m.ColDet(i))
                            m.BallHit(Zero, i);
                    }
                    if (DRec.Intersects(SRect))
                        m.HitShield();
                    if (BRec.Intersects(SRect))
                    {
                        s.Kill();
                        m.BlastHit();
                    }
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(DRec))
                        m.HitFireWall();
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(WRect) && m.ColDet(i))
                            m.BallHit(Zero, i);
                    }
                    if (BRec.Intersects(WRect))
                    {
                        f.Kill();
                        m.BlastHit();
                    }
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(DRec))
                    {
                        if (m.HasSheild())
                            f.KillNow();
                        else
                            f.Kill();
                        m.CombatHit(50, "FireBall");
                    }
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(fRect) && m.ColDet(i))
                        {
                            f.Kill();
                            m.BallHit(Zero, i);
                        }
                    }
                    if (BRec.Intersects(fRect))
                        f.Kill();
                }
                #endregion
                #region CharacterMoves
                if (ARect.Intersects(DRec))
                {
                    Game1.mainCharacter.StickHit();
                    m.CombatHit(Game1.mainCharacter.GetDamage(), "");
                }
                for (int i = 0; i < ARec.Count(); i++)
                {
                    if (ARec[i].Intersects(BRect))
                    {
                        Game1.mainCharacter.Deflect("Ball");
                        m.BallHit(Zero, i);
                    }
                    if (ARec[i].Intersects(DRect) && m.ColDet(i))
                    {
                        Game1.mainCharacter.Hit(Pos, 15, "null");
                        Game1.mainCharacter.IceBallHit(5000);
                        m.BallHit(Zero, i);
                    }
                }
                if (BRect.Intersects(BRec) && m.BlastIsInclined())
                {
                    m.BlastHit();
                }
                if (DRect.Intersects(BRec) && m.BlastIsInclined())
                {
                    Game1.mainCharacter.Hit(Pos, 35, "null");
                    Game1.mainCharacter.IceBallHit(10000);
                    m.BlastHit();
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(DRec))
                    {
                        m.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
                #endregion
            }
            #endregion
            #region IceGiant
            foreach (IceGiant i in IGList)
            {
                Vector2 Pos = i.GetPos();
                Rectangle DRec = i.GetDRect();
                Rectangle ARec = i.GetARect();
                #region CharacterMoves
                Rectangle ARect = Game1.mainCharacter.GetARect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                if (ARect.Intersects(DRec))
                {
                    Game1.mainCharacter.StickHit();
                    i.CombatHit(Game1.mainCharacter.GetDamage());
                }
                if (DRect.Intersects(ARec))
                {
                    Game1.mainCharacter.Hit(Pos, 50, "null");
                }
                for (int ii = 0; ii < ColRects.Count(); ii++)
                {
                    if (ColRects[ii].ARect.Intersects(DRec))
                    {
                        i.PowerHit(ColRects[ii].type);
                        Game1.mainCharacter.PowerHit(ColRects[ii].type, ColRects[ii].index);
                        ColRects.Remove(ColRects[ii]);
                    }
                }
                #endregion
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    i.CombatHit(10);
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    if (ARec.Intersects(SRect))
                    {
                        i.HitShield();
                        s.Kill();
                    }
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(ARec))
                    {
                        i.HitFireWall();
                        f.Kill();
                    }
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(DRec))
                    {
                        f.Kill();
                        i.CombatHit(10);
                    }
                    if (fRect.Intersects(ARec))
                    {
                        f.Kill();
                    }
                }
                #endregion
            }
            #endregion
            #region StdGun1
            foreach (Gunner g in StdGun1List)
            {
                Vector2 CharPos = Game1.mainCharacter.GetPos();
                Vector2 Pos = g.GetPos();
                Vector2 Zero = new Vector2(0, 0);
                Rectangle DRec = g.GetDRect();
                List<Rectangle> ARec = g.GetARect();
                Rectangle ARect = Game1.mainCharacter.GetARect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                Rectangle BRect = Game1.mainCharacter.GetBRect();
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    g.CombatHit(10);
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(SRect))
                            g.BulletHit(i);
                    }
                    if (DRec.Intersects(SRect))
                        g.HitShield();
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(DRec))
                        g.HitFireWall();
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(WRect))
                            g.BulletHit(i);
                    }
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(DRec))
                    {
                        f.Kill();
                        g.CombatHit(10);
                    }
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(fRect))
                        {
                            g.BulletHit(i);
                        }
                    }
                }
                #endregion
                #region CharacterMoves
                if (ARect.Intersects(DRec))
                {
                    Game1.mainCharacter.StickHit();
                    g.CombatHit(Game1.mainCharacter.GetDamage());
                }
                for (int i = 0; i < ARec.Count(); i++)
                {
                    if (ARec[i].Intersects(BRect))
                    {
                        Game1.mainCharacter.Deflect("Bullet");
                        g.BulletHit(i);
                    }
                    if (ARec[i].Intersects(DRect))
                    {
                        Game1.mainCharacter.Hit(Pos, 2, "null");
                        g.BulletHit(i);
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(DRec))
                    {
                        g.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
                #endregion
            }
            #endregion
            #region AKGun1
            foreach (Gunner g in AKGun1List)
            {
                Vector2 CharPos = Game1.mainCharacter.GetPos();
                Vector2 Pos = g.GetPos();
                Vector2 Zero = new Vector2(0, 0);
                Rectangle DRec = g.GetDRect();
                List<Rectangle> ARec = g.GetARect();
                Rectangle ARect = Game1.mainCharacter.GetARect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();
                Rectangle BRect = Game1.mainCharacter.GetBRect();
                #region powers
                if (Game1.mainCharacter.Demolition())
                {
                    g.CombatHit(10);
                }
                foreach (Shield s in Game1.mainCharacter.ShieldList)
                {
                    Rectangle SRect = s.GetRect();
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(SRect))
                            g.BulletHit(i);
                    }
                    if (DRec.Intersects(SRect))
                        g.HitShield();
                }
                foreach (FireWall f in Game1.mainCharacter.WallList)
                {
                    Rectangle WRect = f.GetRect();
                    if (WRect.Intersects(DRec))
                        g.HitFireWall();
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(WRect))
                            g.BulletHit(i);
                    }
                }
                foreach (FireBall f in Game1.mainCharacter.BallList)
                {
                    Rectangle fRect = f.GetRect();
                    if (fRect.Intersects(DRec))
                    {
                        f.Kill();
                        g.CombatHit(10);
                    }
                    for (int i = 0; i < ARec.Count(); i++)
                    {
                        if (ARec[i].Intersects(fRect))
                        {
                            g.BulletHit(i);
                        }
                    }
                }
                #endregion
                #region CharacterMoves
                if (ARect.Intersects(DRec))
                {
                    Game1.mainCharacter.StickHit();
                    g.CombatHit(Game1.mainCharacter.GetDamage());
                }
                for (int i = 0; i < ARec.Count(); i++)
                {
                    if (ARec[i].Intersects(BRect))
                    {
                        Game1.mainCharacter.Deflect("Bullet");
                        g.BulletHit(i);
                    }
                    if (ARec[i].Intersects(DRect))
                    {
                        Game1.mainCharacter.Hit(Pos, 2, "null");
                        g.BulletHit(i);
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(DRec))
                    {
                        g.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
                #endregion
            }
            #endregion

            #region HandGun
            foreach (HandGun g in HGList)
            {
                Rectangle HGRect = g.GetRect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();

                if (HGRect.Intersects(DRect))
                {
                    DrawHelpLine = "Press 'F' to pick up";
                    if (Keyboard.GetState().IsKeyDown(Keys.F))
                    {
                        Game1.mainCharacter.SetMode("HandGunner", 12);
                        g.Kill();
                        soundBank.PlayCue("Reload");
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(HGRect) && ColRects[i].type == "Bullet")
                    {
                        g.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
            }
            #endregion
            #region AK47
            foreach (AK47 a in AKList)
            {
                Rectangle HGRect = a.GetRect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();

                if (HGRect.Intersects(DRect))
                {
                    DrawHelpLine = "Press 'F' to pick up";
                    if (Keyboard.GetState().IsKeyDown(Keys.F))
                    {
                        Game1.mainCharacter.SetMode("AKGunner", 30);
                        a.Kill();
                        soundBank.PlayCue("Reload");
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(HGRect) && ColRects[i].type == "Bullet")
                    {
                        a.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
            }
            #endregion
            #region Launcher
            foreach (Launcher a in RLList)
            {
                Rectangle RLRect = a.GetRect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();

                if (RLRect.Intersects(DRect))
                {
                    DrawHelpLine = "Press 'F' to pick up";
                    if (Keyboard.GetState().IsKeyDown(Keys.F))
                    {
                        Game1.mainCharacter.SetMode("Launcher", 2);
                        a.Kill();
                        soundBank.PlayCue("Reload");
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(RLRect) && ColRects[i].type == "Bullet")
                    {
                        a.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
            }
            #endregion
            #region MageHat
            foreach (MageHat m in MHatList)
            {
                Rectangle MRect = m.GetRect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();

                if (MRect.Intersects(DRect))
                {
                    DrawHelpLine = "Press 'F' to pick up";
                    if (Keyboard.GetState().IsKeyDown(Keys.F))
                    {
                        Game1.mainCharacter.SetMode("Mage", 0);
                        m.Kill();
                        soundBank.PlayCue("MageBall");
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(MRect) && ColRects[i].type == "Bullet")
                    {
                        m.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
            }
            #endregion
            #region IceMageHat
            foreach (IceMageHat m in IMHatList)
            {
                Rectangle MRect = m.GetRect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();

                if (MRect.Intersects(DRect))
                {
                    DrawHelpLine = "Press 'F' to pick up";
                    if (Keyboard.GetState().IsKeyDown(Keys.F))
                    {
                        Game1.mainCharacter.SetMode("IceMage", 0);
                        m.Kill();
                        soundBank.PlayCue("IMageBall");
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(MRect) && ColRects[i].type == "Bullet")
                    {
                        m.PowerHit(ColRects[i].type);
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
            }
            #endregion
            #region Balloon
            if (balloon != null)
            {
                Rectangle MRect = balloon.GetRect();

                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(MRect) && ColRects[i].type == "Bullet")
                    {
                        crate = new Crate(Game.Content.Load<Texture2D>(@"Powerups\Crate"),
                            new Vector2(balloon.GetPos().X + 5, balloon.GetPos().Y + 66),
                            new Point(50, 50), new Point(balloon.GetFrame(), 0));
                        balloon.Hit();
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                    }
                }
            }
            #endregion
            #region crate
            if (crate != null)
            {
                Rectangle MRect = crate.GetRect();
                Rectangle DRect = Game1.mainCharacter.GetDRect();

                if (MRect.Intersects(DRect))
                {
                    DrawHelpLine = "Press 'F' to open";
                    if (Keyboard.GetState().IsKeyDown(Keys.F))
                    {
                        if (crate.GetCType() == 'L')
                            Lightning();

                        Game1.mainCharacter.PowerUp(crate.GetCType());
                        crate.Kill();
                    }
                }
                for (int i = 0; i < ColRects.Count(); i++)
                {
                    if (ColRects[i].ARect.Intersects(MRect) && ColRects[i].type == "Bullet")
                    {
                        Game1.mainCharacter.CreateDust(15, crate.GetPos());
                        Game1.mainCharacter.PowerHit(ColRects[i].type, ColRects[i].index);
                        ColRects.Remove(ColRects[i]);
                        crate.Kill();
                    }
                }
            }
            #endregion
        }

        private void Lightning()
        {
            if (!LevelIsChanging)
            {
                bool HasKilledEnemy = false;
                while (!HasKilledEnemy)
                {
                    int RndColumn = Rnd.Next(0, 9);
                    switch (LevelInfo[Level, RndColumn])
                    {
                        #region BasicEnemy
                        case 1:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (BEList[i] != null)
                                    {
                                        BEList[i].CombatHit(100000, 0);
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                        #region BlindGiant
                        case 2:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (BGList[i] != null)
                                    {
                                        BGList[i].CombatHit(100000);
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                        #region RocketLauncher
                        case 3:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (LauncherList[i] != null)
                                    {
                                        LauncherList[i].CombatHit();
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                        #region Mage
                        case 4:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (MageList[i] != null)
                                    {
                                        MageList[i].CombatHit(100000, true);
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                        #region IceMage
                        case 5:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (IceMageList[i] != null)
                                    {
                                        IceMageList[i].CombatHit(100000, "Demolition");
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                        #region IceGiant
                        case 6:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (IGList[i] != null)
                                    {
                                        IGList[i].CombatHit(100000);
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                        #region StdGun1
                        case 7:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (StdGun1List[i] != null)
                                    {
                                        StdGun1List[i].CombatHit(100000);
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                        #region AKGun1
                        case 8:
                            {
                                bool HasKilledEnemy2 = false;
                                int i = 0;
                                while (!HasKilledEnemy2)
                                {
                                    if (AKGun1List[i] != null)
                                    {
                                        AKGun1List[i].CombatHit(100000);
                                        HasKilledEnemy = true;
                                        HasKilledEnemy2 = true;
                                    }
                                    else if (i > 9)
                                        break;
                                    else
                                        i++;
                                }
                                break;
                            }
                        #endregion
                    }
                }
            }
        }

        //Don't forget to check ColRect's here 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
            foreach (BasicEnemy b in BEList)
            {
                //spriteBatch.Draw(Test, b.GetRect(), Color.White);
                b.Draw(spriteBatch);
            }
            foreach (BlindGiant b in BGList)
            {
                b.Draw(spriteBatch);
                //spriteBatch.Draw(Test, b.GetARect(), Color.White);
                //spriteBatch.Draw(Test, b.GetDRect(), Color.Black);
                
            }
            foreach (RocketLauncher r in LauncherList)
            {
                r.Draw(spriteBatch);
                r.DrawRockets(spriteBatch);
                //spriteBatch.Draw(Test, r.GetARect(), Color.White);
                //spriteBatch.Draw(Test, r.GetDRect(), Color.White);
            }
            foreach (Mage m in MageList)
            {
                m.DrawShield(spriteBatch);
                m.Draw(spriteBatch);
                m.DrawMageBalls(spriteBatch);
                m.DrawMageBlast(spriteBatch);
                //spriteBatch.Draw(Test, m.GetBRect(), Color.White);
                //spriteBatch.Draw(Test, m.GetDRect(), Color.White);
            }
            foreach (IceMage m in IceMageList)
            {
                m.DrawShield(spriteBatch);
                m.Draw(spriteBatch);
                m.DrawMageBalls(spriteBatch);
                m.DrawMageBlast(spriteBatch);
                //spriteBatch.Draw(Test, m.GetBRect(), Color.White);
                //spriteBatch.Draw(Test, m.GetDRect(), Color.White);
            }
            foreach (IceGiant i in IGList)
            {
                i.Draw(spriteBatch);
                //spriteBatch.Draw(Test, i.GetARect(), Color.White);
                //spriteBatch.Draw(Test, i.GetDRect(), Color.Black);
            }
            foreach (Gunner g in StdGun1List)
            {
                g.Draw(spriteBatch);
                g.DrawBullets(spriteBatch);
                g.DrawArms(spriteBatch);
                /*foreach (Rectangle r in g.GetARect())
                {
                    spriteBatch.Draw(Test, r, Color.White);
                }
                spriteBatch.Draw(Test, g.GetDRect(), Color.Black);*/
            }
            foreach (Gunner g in AKGun1List)
            {
                g.Draw(spriteBatch);
                g.DrawBullets(spriteBatch);
                g.DrawArms(spriteBatch);
                //spriteBatch.Draw(Test, g.GetARect(), Color.White);
                //spriteBatch.Draw(Test, g.GetDRect(), Color.Black);
            }
            foreach (HandGun g in HGList)
            {
                g.Draw(spriteBatch);
            }
            spriteBatch.DrawString(spriteFont, DrawHelpLine, new Vector2(40, 400),
                Color.Black);
            foreach (AK47 a in AKList)
            {
                a.Draw(spriteBatch);
            }
            foreach (Launcher l in RLList)
            {
                l.Draw(spriteBatch);
            }
            foreach (MageHat m in MHatList)
            {
                m.Draw(spriteBatch);
            }
            foreach (IceMageHat i in IMHatList)
            {
                i.Draw(spriteBatch);
            }
            if (balloon != null)
            {
                balloon.Draw(spriteBatch);
            }
            if (emptyBalloon != null)
            {
                emptyBalloon.Draw(spriteBatch);
            }
            if (crate != null)
            {
                crate.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #region Returns
        public int GetLevel()
        {
            if (Level > 69)
                return 100;
            else
                return Level;
        }
        public void IsLevelChanging(bool LevelIsChanging)
        {
            this.LevelIsChanging = LevelIsChanging;
        }
        public bool IsGameComplete()
        {
            return gameComplete;
        }
        public void SetLevel(int setLevel)
        {
            Level = setLevel;
        }
        public void PlaySound(string Sound)
        {
            soundBank.PlayCue(Sound);
        }
        public void CreateCrate(Vector2 Pos, int CurrentFrameX)
        {
            emptyBalloon = new EmptyBalloon(Game.Content.Load<Texture2D>(@"Powerups\Balloon"),
                Pos, new Point(60, 80));
            balloon = null;
        }
        public void CreateRandomBalloon()
        {
            balloon = new Balloon(Game.Content.Load<Texture2D>(@"Powerups\Balloon"),
                Game.Content.Load<Texture2D>(@"Powerups\Crate"), new Vector2(0, 200),
                new Point(60, 80), new Point(50, 50), new Point(Rnd.Next(1, 6), 0));
        }
        #endregion
    }
}