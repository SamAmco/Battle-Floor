using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Crate : PowerUpBase
    {
        Vector2 Velocity = new Vector2(0, 0);        
        bool Destroy = false;
        int Timer = 0;
        char type;

        public Crate(Texture2D Tex, Vector2 Pos, Point FrameSize, Point CurrentFrame) 
            : base(Tex, Pos, FrameSize, CurrentFrame)
        {
            switch (CurrentFrame.X)
            {
                case 0:
                    type = 'A';
                    break;
                case 1 :
                    type = 'R';
                    break;
                case 2:
                    type = '+';
                    break;
                case 3:
                    type = 'L';
                    break;
                case 4:
                    type = 'I';
                    break;
                case 5:
                    type = 'D';
                    break;
            }
        }
        
        public void Update(GameTime gameTime, Rectangle ClientBounds)
        {
            Timer += gameTime.ElapsedGameTime.Milliseconds;
            if (Timer >= 10000)
            {
                Destroy = true;
            }
            UpdatePos(gameTime, ClientBounds);
        }

        private void UpdatePos(GameTime gameTime, Rectangle ClientBounds)
        {
            #region Horizontal
            if (Pos.X > ClientBounds.Width)
                Pos.X = ClientBounds.Width;
            else if (Pos.X < 0)
                Pos.X = 0;
            #endregion
            #region Vertical
            if (Velocity.Y < 20)
                Velocity.Y += 1;
            if (Pos.Y < ClientBounds.Height - 60)
                Pos.Y += Velocity.Y;
            if (Pos.Y > ClientBounds.Height - 60)
            {
                Velocity.Y = 0;
                Pos.Y = ClientBounds.Height - 50;
            }
            Pos += Velocity;
            #endregion
        }

        #region returns
        public Vector2 GetPos()
        {
            return Pos;
        }
        public Rectangle GetRect()
        {
            return new Rectangle((int)Pos.X, (int)Pos.Y, 50, 50);
        }
        public bool Dispose()
        {
            return Destroy;
        }
        public void Kill()
        {
            Destroy = true;
        }
        public Char GetCType()
        {
            return type;
        }
        #endregion
    }
}
