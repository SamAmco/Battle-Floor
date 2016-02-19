using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class EmptyBalloon : PowerUpBase
    {
        Vector2 InitialPos = new Vector2(0, 0);
        Vector2 Speed = new Vector2(1, -5);
        int YLine = 100;
        bool Destroy = false;

        public EmptyBalloon(Texture2D BalloonTex, Vector2 Pos, Point BalFrameSize)
            : base(BalloonTex, Pos, BalFrameSize)
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
            if (Pos.Y <= -80)
                Destroy = true;
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

        public bool Dispose()
        {
            return Destroy;
        }
    }
}