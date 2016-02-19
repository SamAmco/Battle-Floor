using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class PowerBall : Powers
    {
        int Life = 0;
        bool Dispose = false;
        Vector2 Force = Vector2.Zero;
        Vector2 Speed = Vector2.Zero;
        Vector2 Posi = Vector2.Zero;
        Random Rnd = new Random();

        public PowerBall(Texture2D PowTex, Vector2 Pos, Point CurrentFrame, Point FrameSize, float Scale)
            : base(PowTex, Pos, CurrentFrame, FrameSize, Scale)
        {
            Posi = Pos;
        }

        public void Update(SpriteBatch spriteBatch, GameTime gameTime, Vector2 CharPos)
        {
            Life += gameTime.ElapsedGameTime.Milliseconds;
            if (Life >= 20000)
                Dispose = true;

            UpdatePos(gameTime, CharPos);
            Draw(spriteBatch);
        }

        private void UpdatePos(GameTime gameTime, Vector2 CharPos)
        {
            Position.X = CharPos.X + 18;
            Position.Y = CharPos.Y - 18;
        }

        public bool Destroy()
        {
            return Dispose;
        }
    }
}
