using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class GoldDust : Powers
    {
        Vector2 Velocity;
        Vector2 CharPos;
        int Timer = 0;

        public GoldDust(Texture2D Tex, Vector2 Pos, Point CurrentFrame, Point FrameSize, Vector2 Velocity, Color color)
            : base(Tex, Pos, CurrentFrame, FrameSize, 1, color)
        {
            this.Velocity = Velocity;
        }

        public void Update(SpriteBatch spriteBatch, Rectangle ClientBounds)
        {
            UpdatePos(ClientBounds);
            Draw(spriteBatch);
        }

        private void UpdatePos(Rectangle ClientBounds)
        {
            CharPos = Game1.mainCharacter.GetPos();

            if (CharPos.X > Position.X)
            {
                Velocity.X += 1;
            }
            else if (CharPos.X < Position.X)
            {
                Velocity.X -= 1;
            }

            if (CharPos.Y > Position.Y)
            {
                Velocity.Y += 1;
            }
            else if (CharPos.Y < Position.Y)
            {
                Velocity.Y -= 1;
            }

            Position += Velocity;

            if (Position.X > ClientBounds.Width + 11)
            {
                Position.X = ClientBounds.Width + 11;
            }
            else if (Position.X < -11)
            {
                Position.X = -11;
            }
            if (Position.Y > ClientBounds.Height + 11)
            {
                Position.Y = ClientBounds.Height + 11;
            }
            else if (Position.Y < -11)
            {
                Position.Y = -11;
            }
        }

        public Vector2 GetPos()
        {
            return Position;
        }
        public int GetT()
        {
            Timer += 1;
            return Timer;
        }
    }
}
