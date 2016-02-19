using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class MageShield : EnemyBase
    {
        Mage ParentMage;
        int Health = 800;

        public MageShield(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Point CurrentFrame, Mage ParentMage)
            : base (Tex, Pos, FrameSize, CurrentFrame, Color.White, 'L')
        {
            this.ParentMage = ParentMage;
        }


        public bool Update(Rectangle ClientBounds, GameTime gameTime)
        {
            Pos.X = ParentMage.Pos.X + 15;
            Pos.Y = ParentMage.Pos.Y - 10;
            if (Health <= 0)
                return true;
            else
                return false;
        }

        public Rectangle GetDRect()
        {
            return new Rectangle((int)Pos.X, (int)Pos.Y, 45, 60);
        }
        public void Hit(int Damage)
        {
            Health -= Damage;
            if (Health > 400)
                CurrentFrame.Y = 0;
            else
                CurrentFrame.Y = 1;

            if (Health > 700)
                CurrentFrame.X = 0;
            else if (Health > 600)
                CurrentFrame.X = 1;
            else if (Health > 500)
                CurrentFrame.X = 2;
            else if (Health > 400)
                CurrentFrame.X = 3;
            else if (Health > 300)
                CurrentFrame.X = 0;
            else if (Health > 200)
                CurrentFrame.X = 1;
            else if (Health > 100)
                CurrentFrame.X = 2;
            else if (Health > 0)
                CurrentFrame.X = 3;
                        
        }
    }
}
