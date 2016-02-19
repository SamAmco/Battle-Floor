using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
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
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static MainCharacter mainCharacter;
        public static Enemies enemies;
        enum GameState {MainMenu, InGame, GameOver, InGameMenu, Credits};
        GameState CurrentGameState = GameState.MainMenu;

        MouseState PrevMouseState;
        KeyboardState PrevKeystate;

        Texture2D MenuWash;
        Texture2D MainMenuScreen;
        Texture2D ResumeButton;
        Texture2D ReplayButton;
        Texture2D LoadButton;
        Texture2D SaveButton;
        Texture2D ExitButton;
        Texture2D MainMenuButton;
        Texture2D HelpScreen;
        Texture2D Lives;

        SpriteFont spriteFont;

        List<GoldDust> DustList = new List<GoldDust>();

        bool DrawHelp = false;
        bool Welcome = false;
        bool PowBall = false;
        bool FiBall = false;
        bool Wall = false;
        bool Shieldmes = false;
        bool Expmes = false;
        bool GunnerModemes = false;

        int Livesi = 10;
        int livesILoad = 0;
        int LastLevel = 0;
        bool ChangeLevel = false;
        Texture2D CurrentLevel;
        Texture2D NextLevel;
        Vector2 CurrentLevelPos = new Vector2(0, 0);
        Vector2 NextLevelPos = new Vector2(800, 0);

        public AudioEngine audioEngine;
        public SoundBank soundBank;
        public WaveBank waveBank;
        Cue MenuMusic;

        [Serializable]
        public struct SaveGameData
        {
            public string CurrentLevelTex;
            public string NextLevelTex;
            public int CurrentXP;
            public int CurrentLevelint;
            public int Lives;
            public int initialLives;

            public int HGBullets;
            public int AKBullets;
            public int Rockets;
            public bool HasMageHat;
            public bool HasIceMageHat;
        }
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600; 
            graphics.ApplyChanges();
            PrevMouseState = Mouse.GetState();
            PrevKeystate = Keyboard.GetState();
            mainCharacter = new MainCharacter(this);
            Components.Add(mainCharacter);
            enemies = new Enemies(this);
            Components.Add(enemies);
            enemies.DrawOrder = 1;
            mainCharacter.DrawOrder = 2;
            enemies.Enabled = false;
            enemies.Visible = false;
            mainCharacter.Enabled = false;
            mainCharacter.Visible = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MainMenuButton = Content.Load<Texture2D>(@"Menu\MainMenuButton");
            ExitButton = Content.Load<Texture2D>(@"Menu\ExitButton");
            SaveButton = Content.Load<Texture2D>(@"Menu\SaveButton");
            MenuWash = Content.Load<Texture2D>(@"Menu\MenuWash");
            MainMenuScreen = Content.Load<Texture2D>(@"Menu\MainMenuScreen");
            ResumeButton = Content.Load<Texture2D>(@"Menu\ResumeButton");
            ReplayButton = Content.Load<Texture2D>(@"Menu\ReplayButton");
            LoadButton = Content.Load<Texture2D>(@"Menu\LoadButton");
            Lives = Content.Load<Texture2D>(@"Menu\Lives");
            spriteFont = Content.Load<SpriteFont>("SpriteFont");
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            MenuMusic = soundBank.GetCue("Menu");
            if (!SaveExists())
            {
                CurrentLevel = Content.Load<Texture2D>(@"Menu\Level's\Level1");
                NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level2");
            }
            else
                LoadGame();
            MenuMusic.Play();
        }

        protected override void Update(GameTime gameTime)
        {
            for (int i = 0; i < DustList.Count(); i++)
            {
                if (DustList[i].GetPos().X > Window.ClientBounds.Width || DustList[i].GetPos().X < 0
                    || DustList[i].GetPos().Y > Window.ClientBounds.Height || DustList[i].GetPos().Y < 0)
                {
                    DustList.Remove(DustList[i]);
                }
            }
            for (int i = 0; i < DustList.Count(); i++)
            {
                int T = (int)DustList[i].GetT();
                if (T >= 100)
                {
                    DustList.Remove(DustList[i]);
                }
            }
            UpdateLevel();
            CheckGameState();

            base.Update(gameTime);
        }
        
        private void UpdateLevel()
        {
            if (enemies.GetLevel() == 100)
            {
                CurrentGameState = GameState.Credits;
            }
            else if (enemies.GetLevel() > LastLevel)
            {
                ChangeLevel = true;
                if (enemies.GetLevel() % 5 == 0)
                    enemies.CreateRandomBalloon();
            }
            else if (ChangeLevel)
            {
                if (NextLevelPos.X > 0)
                {
                    CurrentLevelPos.X -= 1;
                    NextLevelPos.X -= 1;
                }
                else
                {
                    CurrentLevelPos.X = 0;
                    NextLevelPos.X = 800;
                    CurrentLevel = NextLevel;
                    NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level" + (enemies.GetLevel() + 2));
                    ChangeLevel = false;                  
                }
            }
            LastLevel = enemies.GetLevel();
            enemies.IsLevelChanging(ChangeLevel);
        }

        private void SaveGame()
        {
            try
            {
                SaveGameData saveGameData = new SaveGameData();

                saveGameData.CurrentLevelint = enemies.GetLevel();
                if (enemies.GetLevel() == 0)
                {
                    saveGameData.CurrentLevelTex = (string)("Menu\\Level's\\Level" + 1);
                    saveGameData.NextLevelTex = (string)("Menu\\Level's\\Level" + 2);
                }
                else
                {
                    saveGameData.CurrentLevelTex = (string)("Menu\\Level's\\Level" + enemies.GetLevel());
                    saveGameData.NextLevelTex = (string)("Menu\\Level's\\Level" + (enemies.GetLevel() + 1));
                }
                saveGameData.CurrentXP = mainCharacter.GetXPlevel();
                saveGameData.Lives = mainCharacter.GetLives();
                saveGameData.HGBullets = mainCharacter.HGAmmo;
                saveGameData.Rockets = mainCharacter.LAmmo;
                saveGameData.AKBullets = mainCharacter.AKAmmo;
                saveGameData.HasMageHat = mainCharacter.HasMageHat;
                saveGameData.HasIceMageHat = mainCharacter.HasIMageHat;
                saveGameData.HGBullets = mainCharacter.HGAmmo;
                saveGameData.initialLives = Livesi;

                StorageDevice device;
                IAsyncResult result;
                result = StorageDevice.BeginShowSelector(null, null);
                device = StorageDevice.EndShowSelector(result);
                result = device.BeginOpenContainer("BattleFloorSave", null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = device.EndOpenContainer(result);
                result.AsyncWaitHandle.Close();

                string filename = "save.sav";
                if (container.FileExists(filename))
                    container.DeleteFile(filename);

                Stream stream = container.CreateFile(filename);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                serializer.Serialize(stream, saveGameData);
                stream.Close();
                container.Dispose();
            }
            catch { }
        }

        private void LoadGame()
        {
            try
            {
                //System.IO.Stream stream = TitleContainer.OpenStream(@"save.sav");
                //XmlSerializer Serializer = new XmlSerializer(typeof(SaveGameData));
                //SaveGameData saveGameData = new SaveGameData();
                //saveGameData = (SaveGameData)Serializer.Deserialize(stream);
                //stream.Close();

                StorageDevice device;
                IAsyncResult result;
                result = StorageDevice.BeginShowSelector(null, null);
                device = StorageDevice.EndShowSelector(result);
                result = device.BeginOpenContainer("BattleFloorSave", null, null);
                result.AsyncWaitHandle.Close();
                StorageContainer container = device.EndOpenContainer(result);
                result.AsyncWaitHandle.Close();
                string filename = "save.sav";
                if (!container.FileExists(filename))
                {
                    container.Dispose();
                    return;
                }
                Stream stream = container.OpenFile(filename, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                SaveGameData saveGameData = (SaveGameData)serializer.Deserialize(stream);
                stream.Close();
                container.Dispose();

                //Components.Clear();
                //mainCharacter = new MainCharacter(this);
                //enemies = new Enemies(this);
                //Components.Add(mainCharacter);
                //Components.Add(enemies);
                mainCharacter.SetXPLevel(saveGameData.CurrentXP);
                mainCharacter.SetLives(saveGameData.Lives);
                CurrentLevel = Content.Load<Texture2D>(saveGameData.CurrentLevelTex);
                NextLevel = Content.Load<Texture2D>(saveGameData.NextLevelTex);
                enemies.SetLevel(saveGameData.CurrentLevelint);
                mainCharacter.AKAmmo = saveGameData.AKBullets;
                mainCharacter.HGAmmo = saveGameData.HGBullets;
                mainCharacter.LAmmo = saveGameData.Rockets;
                mainCharacter.HasMageHat = saveGameData.HasMageHat;
                mainCharacter.HasIMageHat = saveGameData.HasIceMageHat;
                livesILoad = saveGameData.initialLives;
            }
            catch { }
        }

        private void DeleteSavedGame()
        {
            StorageDevice device;
            IAsyncResult result;
            result = StorageDevice.BeginShowSelector(null, null);
            device = StorageDevice.EndShowSelector(result);
            result = device.BeginOpenContainer("BattleFloorSave", null, null);
            result.AsyncWaitHandle.Close();
            StorageContainer container = device.EndOpenContainer(result);
            result.AsyncWaitHandle.Close();
            string filename = "save.sav";
            if (!container.FileExists(filename))
            {
                container.Dispose();
                return;
            }
            container.DeleteFile(filename);
            container.Dispose();
        }

        private bool SaveExists()
        {
            StorageDevice device;
            IAsyncResult result;
            result = StorageDevice.BeginShowSelector(null, null);
            device = StorageDevice.EndShowSelector(result);
            result = device.BeginOpenContainer("BattleFloorSave", null, null);
            result.AsyncWaitHandle.Close();
            StorageContainer container = device.EndOpenContainer(result);
            result.AsyncWaitHandle.Close();
            string filename = "save.sav";
            if (!container.FileExists(filename))
            {
                container.Dispose();
                return false;
            }
            else
            {
                container.Dispose();
                return true;
            }
        }

        public void CheckGameState()
        {
            if (mainCharacter.CheckGameOver() && CurrentGameState != GameState.GameOver)
            {
                mainCharacter.StopAllSound();
                CurrentGameState = GameState.GameOver;
            }
            switch (CurrentGameState)
            {
                #region MainMenu
                case GameState.MainMenu:
                {
                    this.IsMouseVisible = true;
                    enemies.Enabled = false;
                    enemies.Visible = false;
                    mainCharacter.Enabled = false;
                    mainCharacter.Visible = false;
                    Rectangle MouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                    Rectangle PlayRect = new Rectangle(1, 209, 104, 367);
                    Rectangle ExitRect = new Rectangle(699, 214, 81, 369);
                    Rectangle AddLives = new Rectangle(284, 328, 26, 27);
                    Rectangle TakeLives = new Rectangle(285, 364, 27, 17);
                    if (MouseRect.Intersects(AddLives))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Purple));

                        if (Mouse.GetState().LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            if (Livesi < 999)
                                Livesi += 1;
                        }
                    }
                    if (MouseRect.Intersects(TakeLives))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Purple));

                        if (Mouse.GetState().LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            if (Livesi > 0)
                                Livesi -= 1;
                        }
                    }
                    if (MouseRect.Intersects(PlayRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Red));
                        if (Mouse.GetState().LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            if (!SaveExists())
                            {
                                mainCharacter.SetLives(Livesi);
                                livesILoad = Livesi;
                            }
                            CurrentGameState = GameState.InGame;
                            DustList.Clear();
                            MenuMusic.Stop(AudioStopOptions.Immediate);
                            MenuMusic.Dispose();
                        }
                    }
                    else if (MouseRect.Intersects(ExitRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.White));
                        if (Mouse.GetState().LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Exit();
                        }
                    }
                    break;
                }
                #endregion
                #region InGame
                case GameState.InGame:
                {
                    if (enemies.IsGameComplete())
                    {
                        mainCharacter.StopAllSound();
                        enemies.Enabled = true;
                        enemies.Visible = true;
                        mainCharacter.Enabled = true;
                        mainCharacter.Visible = true;
                        this.IsMouseVisible = true;
                        CurrentGameState = GameState.Credits;
                        MenuMusic = soundBank.GetCue("Menu");
                        MenuMusic.Play();
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        CurrentGameState = GameState.InGameMenu;
                        mainCharacter.StopAllSound();
                    }
                    if (!DrawHelp)
                    {
                        enemies.Enabled = true;
                        enemies.Visible = true;
                        mainCharacter.Enabled = true;
                        mainCharacter.Visible = true;
                        this.IsMouseVisible = false;
                    }
                    else if (DrawHelp)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            enemies.Enabled = true;
                            enemies.Visible = true;
                            mainCharacter.Enabled = true;
                            mainCharacter.Visible = true;
                            DrawHelp = false;
                        }
                    }
                    
                    int XP = mainCharacter.GetXPlevel();

                    #region HelpScreens
                    if (XP >= 0 && !Welcome)
                    {
                        enemies.Enabled = false;
                        enemies.Visible = true;
                        mainCharacter.Enabled = false;
                        mainCharacter.Visible = true;
                        HelpScreen = Content.Load<Texture2D>(@"Menu\Helpscreens\Welcome");
                        DrawHelp = true;
                        Welcome = true;
                    }
                    else if (XP >= 50 && !PowBall)
                    {
                        enemies.Enabled = false;
                        enemies.Visible = true;
                        mainCharacter.Enabled = false;
                        mainCharacter.Visible = true;
                        HelpScreen = Content.Load<Texture2D>(@"Menu\Helpscreens\PowBallmes");
                        DrawHelp = true;
                        PowBall = true;
                    }
                    else if (XP >= 100 && !FiBall)
                    {
                        enemies.Enabled = false;
                        enemies.Visible = true;
                        mainCharacter.Enabled = false;
                        mainCharacter.Visible = true;
                        HelpScreen = Content.Load<Texture2D>(@"Menu\Helpscreens\FireBallmes");
                        DrawHelp = true;
                        FiBall = true;
                    }
                    else if (XP >= 200 && !Wall)
                    {
                        enemies.Enabled = false;
                        enemies.Visible = true;
                        mainCharacter.Enabled = false;
                        mainCharacter.Visible = true;
                        HelpScreen = Content.Load<Texture2D>(@"Menu\Helpscreens\FireWallmes");
                        DrawHelp = true;
                        Wall = true;
                    }
                    else if (XP >= 300 && !Shieldmes)
                    {
                        enemies.Enabled = false;
                        enemies.Visible = true;
                        mainCharacter.Enabled = false;
                        mainCharacter.Visible = true;
                        HelpScreen = Content.Load<Texture2D>(@"Menu\Helpscreens\Shieldmes");
                        DrawHelp = true;
                        Shieldmes = true;
                    }
                    else if (XP >= 1000 && !Expmes)
                    {
                        enemies.Enabled = false;
                        enemies.Visible = true;
                        mainCharacter.Enabled = false;
                        mainCharacter.Visible = true;
                        HelpScreen = Content.Load<Texture2D>(@"Menu\Helpscreens\Expmes");
                        DrawHelp = true;
                        Expmes = true;
                    }
                    else if (mainCharacter.HGAmmo > 0 && !GunnerModemes)
                    {
                        enemies.Enabled = false;
                        enemies.Visible = true;
                        mainCharacter.Enabled = false;
                        mainCharacter.Visible = true;
                        HelpScreen = Content.Load<Texture2D>(@"Menu\Helpscreens\GunnerModemes");
                        DrawHelp = true;
                        GunnerModemes = true;
                    }
                    #endregion
                    break;
                }
                #endregion
                #region InGameMenu
                case GameState.InGameMenu:
                {
                    enemies.Enabled = false;
                    enemies.Visible = true;
                    mainCharacter.Enabled = false;
                    mainCharacter.Visible = true;
                    this.IsMouseVisible = true;
                    Rectangle MouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                    Rectangle ResumeRect = new Rectangle(100, 100, 100, 30);
                    Rectangle ReplayRect = new Rectangle(130, 130, 100, 30);
                    Rectangle MenuRect = new Rectangle(160, 160, 200, 30);
                    Rectangle SaveRect = new Rectangle(190, 190, 100, 30);
                    Rectangle LoadRect = new Rectangle(220, 220, 100, 30);
                    Rectangle ExitRect = new Rectangle(250, 250, 100, 30);

                    if (MouseRect.Intersects(ResumeRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Red));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            CurrentGameState = GameState.InGame;
                            DustList.Clear();
                        }
                    }
                    if (MouseRect.Intersects(ReplayRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Green));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Components.Clear();
                            mainCharacter = new MainCharacter(this);
                            enemies = new Enemies(this);
                            Components.Add(mainCharacter);
                            Components.Add(enemies);
                            CurrentLevel = Content.Load<Texture2D>(@"Menu\Level's\Level1");
                            NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level2");
                            DrawHelp = false;
                            Welcome = false;
                            PowBall = false;
                            FiBall = false;
                            Wall = false;
                            Shieldmes = false;
                            Expmes = false;
                            GunnerModemes = false;
                            mainCharacter.SetLives(Livesi);
                            CurrentGameState = GameState.InGame;
                            DustList.Clear();
                            DeleteSavedGame();
                        }
                    }
                    if (MouseRect.Intersects(MenuRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Black));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Components.Clear();
                            mainCharacter = new MainCharacter(this);
                            enemies = new Enemies(this);
                            Components.Add(mainCharacter);
                            Components.Add(enemies);
                            CurrentLevel = Content.Load<Texture2D>(@"Menu\Level's\Level1");
                            NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level2");
                            DrawHelp = false;
                            Welcome = false;
                            PowBall = false;
                            FiBall = false;
                            Wall = false;
                            Shieldmes = false;
                            Expmes = false;
                            GunnerModemes = false;
                            mainCharacter.SetLives(Livesi);
                            DustList.Clear();
                            DeleteSavedGame();
                            CurrentGameState = GameState.MainMenu;
                            DustList.Clear();
                            MenuMusic = soundBank.GetCue("Menu");
                            MenuMusic.Play();
                        }
                    }
                    if (MouseRect.Intersects(SaveRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Yellow));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            SaveGame();
                        }
                    }
                    if (MouseRect.Intersects(LoadRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Blue));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            CurrentGameState = GameState.InGame;
                            DustList.Clear();
                            if (SaveExists())
                                LoadGame();
                        }
                    }
                    if (MouseRect.Intersects(ExitRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.White));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Exit();
                        }
                    }
                    break;
                }
                #endregion
                #region GameOver
                case GameState.GameOver:
                {
                    enemies.Enabled = false;
                    enemies.Visible = true;
                    mainCharacter.Enabled = false;
                    mainCharacter.Visible = true;
                    this.IsMouseVisible = true;
                    Rectangle MouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                    Rectangle ReplayRect = new Rectangle(100, 90, 100, 30);
                    Rectangle ExitRect = new Rectangle(160, 160, 100, 30);
                    Rectangle MenuRect = new Rectangle(130, 130, 200, 30);
                    Rectangle LoadRect = new Rectangle(190, 190, 100, 30);

                    if (MouseRect.Intersects(ReplayRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Green));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Components.Clear();
                            mainCharacter = new MainCharacter(this);
                            enemies = new Enemies(this);
                            Components.Add(mainCharacter);
                            Components.Add(enemies);
                            CurrentLevel = Content.Load<Texture2D>(@"Menu\Level's\Level1");
                            NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level2");
                            DrawHelp = false;
                            Welcome = false;
                            PowBall = false;
                            FiBall = false;
                            Wall = false;
                            Shieldmes = false;
                            Expmes = false;
                            GunnerModemes = false;
                            mainCharacter.SetLives(Livesi);
                            CurrentGameState = GameState.InGame;
                            DustList.Clear();
                            if (SaveExists())
                                DeleteSavedGame();
                        }
                    }
                    if (MouseRect.Intersects(ExitRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.White));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Exit();
                        }
                    }
                    if (MouseRect.Intersects(MenuRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Black));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Components.Clear();
                            mainCharacter = new MainCharacter(this);
                            enemies = new Enemies(this);
                            Components.Add(mainCharacter);
                            Components.Add(enemies);
                            CurrentLevel = Content.Load<Texture2D>(@"Menu\Level's\Level1");
                            NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level2");
                            DrawHelp = false;
                            Welcome = false;
                            PowBall = false;
                            FiBall = false;
                            Wall = false;
                            Shieldmes = false;
                            Expmes = false;
                            GunnerModemes = false;
                            mainCharacter.SetLives(Livesi);
                            DustList.Clear();
                            if (SaveExists())
                                DeleteSavedGame();
                            CurrentGameState = GameState.MainMenu;
                            DustList.Clear();
                            MenuMusic = soundBank.GetCue("Menu");
                            MenuMusic.Play();
                        }
                    }
                    if (MouseRect.Intersects(LoadRect))
                    {
                        Random Rnd = new Random();
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Blue));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            CurrentGameState = GameState.InGame;
                            DustList.Clear();
                            if (SaveExists())
                            {
                                Components.Clear();
                                mainCharacter = new MainCharacter(this);
                                enemies = new Enemies(this);
                                Components.Add(mainCharacter);
                                Components.Add(enemies);
                                LoadGame();
                                enemies.CreateRandomBalloon();
                            }
                        }
                    }
                    break;
                }
                #endregion
                #region Credits
                case GameState.Credits:
                {
                    enemies.Enabled = false;
                    enemies.Visible = true;
                    mainCharacter.Enabled = false;
                    mainCharacter.Visible = true;
                    this.IsMouseVisible = true;
                    Rectangle MouseRect = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
                    Rectangle ReplayRect = new Rectangle(170, 400, 100, 30);
                    Rectangle MenuRect = new Rectangle(270, 400, 200, 30);
                    Rectangle ExitRect = new Rectangle(470, 400, 100, 30);
                    Random Rnd = new Random();

                    if (MouseRect.Intersects(MenuRect))
                    {
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Black));

                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Components.Clear();
                            mainCharacter = new MainCharacter(this);
                            enemies = new Enemies(this);
                            Components.Add(mainCharacter);
                            Components.Add(enemies);
                            CurrentLevel = Content.Load<Texture2D>(@"Menu\Level's\Level1");
                            NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level2");
                            DrawHelp = false;
                            Welcome = false;
                            PowBall = false;
                            FiBall = false;
                            Wall = false;
                            Shieldmes = false;
                            Expmes = false;
                            GunnerModemes = false;
                            mainCharacter.SetLives(Livesi);
                            DustList.Clear();
                            DeleteSavedGame();
                            CurrentGameState = GameState.MainMenu;
                            DustList.Clear();
                            MenuMusic = soundBank.GetCue("Menu");
                            MenuMusic.Play();
                        }
                    }

                    if (MouseRect.Intersects(ExitRect))
                    {
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.White));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Exit();
                        }
                    }

                    if (MouseRect.Intersects(ReplayRect))
                    {
                        DustList.Add(new GoldDust(Content.Load<Texture2D>(@"Powers\GoldDust"),
                            new Vector2(MouseRect.X, MouseRect.Y), new Point(0, 0), new Point(17, 16),
                            new Vector2(Rnd.Next(0, 101) - 50, Rnd.Next(0, 101) - 50), Color.Green));
                        if (Mouse.GetState().LeftButton == ButtonState.Released &&
                            PrevMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Components.Clear();
                            mainCharacter = new MainCharacter(this);
                            enemies = new Enemies(this);
                            Components.Add(mainCharacter);
                            Components.Add(enemies);
                            CurrentLevel = Content.Load<Texture2D>(@"Menu\Level's\Level1");
                            NextLevel = Content.Load<Texture2D>(@"Menu\Level's\Level2");
                            DrawHelp = false;
                            Welcome = false;
                            PowBall = false;
                            FiBall = false;
                            Wall = false;
                            Shieldmes = false;
                            Expmes = false;
                            GunnerModemes = false;
                            mainCharacter.SetLives(Livesi);
                            CurrentGameState = GameState.InGame;
                            DustList.Clear();
                            DeleteSavedGame();
                            MenuMusic.Stop(AudioStopOptions.Immediate);
                            MenuMusic.Dispose();
                        }
                    }

                    break;
                }
                #endregion
            }
            PrevMouseState = Mouse.GetState();
        }//here

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            
            spriteBatch.Begin();
            switch (CurrentGameState)
            {
                case GameState.InGameMenu:
                    {
                        spriteBatch.Draw(CurrentLevel, CurrentLevelPos, Color.White);
                        spriteBatch.Draw(NextLevel, NextLevelPos, Color.White);
                        spriteBatch.Draw(MenuWash, Vector2.Zero, Color.White);
                        spriteBatch.Draw(ResumeButton, new Vector2(100, 100), Color.White);
                        spriteBatch.Draw(ReplayButton, new Vector2(130, 125), Color.White);
                        spriteBatch.Draw(MainMenuButton, new Vector2(160, 161), Color.White);
                        spriteBatch.Draw(SaveButton, new Vector2(193, 184), Color.White);
                        spriteBatch.Draw(LoadButton, new Vector2(220, 220), Color.White);
                        spriteBatch.Draw(ExitButton, new Vector2(250, 250), Color.White);
                        foreach (GoldDust g in DustList)
                        {
                            g.Update(spriteBatch, Window.ClientBounds);
                        }
                        break;
                    }
                case GameState.InGame:
                    {
                        spriteBatch.Draw(CurrentLevel, CurrentLevelPos, Color.White);
                        spriteBatch.Draw(NextLevel, NextLevelPos, Color.White);
                        if (DrawHelp)
                        {
                            spriteBatch.Draw(HelpScreen, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
                        }
                        break;
                    }
                case GameState.MainMenu:
                    {
                        spriteBatch.Draw(MainMenuScreen, Vector2.Zero, Color.White);
                        spriteBatch.Draw(Lives, new Vector2(90, 325), Color.White);
                        spriteBatch.DrawString(spriteFont, " " + Livesi, new Vector2(210, 325),
                            Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                        foreach (GoldDust g in DustList)
                        {
                            g.Update(spriteBatch, Window.ClientBounds);
                        }
                        break;
                    }
                case GameState.GameOver:
                    {
                        spriteBatch.Draw(CurrentLevel, CurrentLevelPos, Color.White);
                        spriteBatch.Draw(NextLevel, NextLevelPos, Color.White);
                        spriteBatch.Draw(MenuWash, Vector2.Zero, Color.White);
                        spriteBatch.Draw(ReplayButton, new Vector2(100, 90), Color.White);
                        spriteBatch.Draw(MainMenuButton, new Vector2(130, 130), Color.White);
                        spriteBatch.Draw(ExitButton, new Vector2(160, 160), Color.White);
                        spriteBatch.Draw(LoadButton, new Vector2(190, 190), Color.White);
                        foreach (GoldDust g in DustList)
                        {
                            g.Update(spriteBatch, Window.ClientBounds);
                        }
                        break;
                    }
                case GameState.Credits:
                    {
                        spriteBatch.Draw(CurrentLevel, CurrentLevelPos, Color.White);
                        spriteBatch.Draw(NextLevel, NextLevelPos, Color.White);
                        spriteBatch.Draw(MenuWash, Vector2.Zero, Color.White);
                        spriteBatch.Draw(ReplayButton, new Vector2(160, 400), Color.White);
                        spriteBatch.Draw(MainMenuButton, new Vector2(270, 405), Color.White);
                        spriteBatch.Draw(ExitButton, new Vector2(470, 405), Color.White);

                        string creditString = "You have defeated the battle floors!"
                            + "\n   Score: "
                            + "\n       (Lives + 1) * (XP/Initial Lives): " 
                            + "\n           " + ((mainCharacter.GetLives() + 1) * (mainCharacter.GetXPlevel() / livesILoad)).ToString();
                        spriteBatch.DrawString(spriteFont, creditString, new Vector2(100, 90), Color.BlanchedAlmond);

                        foreach (GoldDust g in DustList)
                        {
                            g.Update(spriteBatch, Window.ClientBounds);
                        }
                        break;
                    }
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
