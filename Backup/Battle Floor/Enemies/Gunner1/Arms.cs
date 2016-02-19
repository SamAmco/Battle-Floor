using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Battle_Floor
{
    class Arms : EnemyBase
    {
        Gunner Parent;
        Vector2 CharPos;
        Vector2 RCPos;
        Vector2 OppAdj;

        public Arms(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Vector2 RC, float Rotation, char Direction, Gunner Parent)
            : base (Tex, Pos, FrameSize, RC, Rotation, Color.White, Direction)
        {
            this.Parent = Parent;
        }

        public void Update(GameTime gameTime, char Direction)
        {
            UpdateRotation();
            UpdatePos(Direction);
        }

        private void UpdatePos(char Direction)
        {
            Pos = new Vector2(Parent.Pos.X + 35, Parent.Pos.Y + 19);
            base.Direction = Direction;
        }

        private void UpdateRotation()
        {
            CharPos = new Vector2(Game1.mainCharacter.GetPos().X + 35,
                Game1.mainCharacter.GetPos().Y + 19);
            RCPos = new Vector2(Pos.X + RC.X, Pos.Y + RC.Y);
            OppAdj = new Vector2(RCPos.X - CharPos.X, RCPos.Y - CharPos.Y);
            if (Direction == 'L')
            {
                RCDeg = -((float)Math.Atan(OppAdj.Y / OppAdj.X) - MathHelper.ToRadians(3));
            }
            else if (Direction == 'R')
            {
                RCDeg = ((float)Math.Atan(OppAdj.Y / OppAdj.X) + MathHelper.ToRadians(3));
            }
        }

        public Vector2 GetVelocity()
        {
            Random Rnd = new Random();
            Vector2 Velocity = new Vector2((float)Math.Cos(RCDeg) * 30, (float)Math.Sin(RCDeg) * 30);
            return Velocity;
        }
    }
}
