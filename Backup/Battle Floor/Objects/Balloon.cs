using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Balloon : PowerUpBase
    {
        Vector2 InitialPos = new Vector2(0, 0);
        Vector2 Velocity = new Vector2(0, 0);
        Vector2 Speed = new Vector2(1, 0);
        int YLine = 100;
        bool Destroy = false;

        public Balloon(Texture2D BalloonTex, Texture2D CrateTex, Vector2 Pos,
            Point BalFrameSize, Point CraFrameSize, Point CurrentFrame) 
            : base(BalloonTex, CrateTex, Pos, new Vector2(5, 66), BalFrameSize, CraFrameSize, CurrentFrame)
        {
            InitialPos = Pos;
        }
        
        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            UpdatePos(gameTime, ClientBounds);
        }

        private void UpdatePos(GameTime gameTime, Rectangle ClientBounds)
        {
            #region Horizontal
            if (InitialPos.X < 400)
            {
                Pos.X += Speed.X;
            }
            else
                Pos.X -= Speed.X;
            if (Pos.X < -60 || Pos.X > 800)
            {
                Destroy = true;
            }
            #endregion
            #region Vertical
            if (Pos.Y < YLine)
            {
                Speed.Y += 0.001f;
            }
            else
                Speed.Y -= 0.001f;

            Pos.Y += Speed.Y;
            #endregion
        }

        #region returns
        public Vector2 GetPos()
        {
            return Pos;
        }
        public Rectangle GetRect()
        {
            return new Rectangle((int)Pos.X + 2, (int)Pos.Y, 55, 118);
        }
        public bool Dispose()
        {
            return Destroy;
        }
        public void Hit()
        {
            Game1.enemies.CreateCrate(Pos, CurrentFrame2.X);
            Destroy = true;
        }
        public int GetFrame()
        {
            return CurrentFrame2.X;
        }
        #endregion
    }
}
