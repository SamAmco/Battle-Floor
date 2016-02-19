using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    public class Shield : Powers
    {
        bool Dispose = false;
        int Life = 0;
        Rectangle SRect = new Rectangle(32, 34, 144, 170);

        public Shield(Texture2D ShieldTex, Vector2 Pos, Point CurrentFrame, Point FrameSize, float Scale)
            : base(ShieldTex, Pos, CurrentFrame, FrameSize, Scale)
        {
        }

        public void Update(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Life += gameTime.ElapsedGameTime.Milliseconds;
            if (Life >= 30000)
                Dispose = true;
            UpdateFrames();
            Draw(spriteBatch);
        }

        private void UpdateFrames()
        {
            if (CurrentFrame.X < 3)
                CurrentFrame.X += 1;
            else if (CurrentFrame.X == 3 && CurrentFrame.Y < 2)
            {
                CurrentFrame.X = 0;
                CurrentFrame.Y += 1;
            }
            else if (CurrentFrame.X == 3 && CurrentFrame.Y == 2)
            {
                CurrentFrame.X = 0;
                CurrentFrame.Y = 1;
            }
        }

        public bool Destroy()
        {
            return Dispose;
        }

        public Rectangle GetRect()
        {
            Rectangle ColRect = new Rectangle((int)Position.X + 15, (int)Position.Y + 10, 150, 170);
            return ColRect;
        }
        public void Kill()
        {
            Dispose = true;
        }
    }
}
