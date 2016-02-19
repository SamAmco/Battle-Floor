using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Battle_Floor
{
    class MCArms : EnemyBase
    {
        Vector2 MousePos;
        Vector2 Offset = new Vector2(-2, 1);
        Vector2 RCPos;
        Vector2 OppAdj;

        public MCArms(Texture2D Tex, Vector2 Pos, Point FrameSize,
            Vector2 RC, float Rotation, char Direction)
            : base(Tex, Pos, FrameSize, RC, Rotation, Color.White, Direction)
        {
        }

        public void Update(GameTime gameTime, char Direction)
        {
            UpdateRotation();
            UpdatePos(Direction);
        }

        private void UpdatePos(char Direction)
        {
            Pos = new Vector2(Game1.mainCharacter.GetPos().X + Offset.X,
                Game1.mainCharacter.GetPos().Y + Offset.Y);
            base.Direction = Direction;
        }

        private void UpdateRotation()
        {
            MousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            RCPos = new Vector2(Pos.X + RC.X, Pos.Y + RC.Y);
            OppAdj = new Vector2(RCPos.X - MousePos.X, RCPos.Y - MousePos.Y);
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
            Vector2 Velocity = new Vector2((float)Math.Cos(RCDeg) * 30, (float)Math.Sin(RCDeg) * 30);
            return Velocity;
        }
        public Vector2 GetArmsEndPos()
        {
            Vector2 Velocity = new Vector2((float)Math.Cos(RCDeg) * 30, (float)Math.Sin(RCDeg) * 30);
            Velocity.Normalize();
            if (Direction == 'L')
                Velocity = new Vector2(-Velocity.X * 34, Velocity.Y * 34);
            if (Direction == 'R')
                Velocity = new Vector2(Velocity.X * 34, Velocity.Y * 34);
            return Velocity;
        }
        public void SetOffset(Vector2 Offset)
        {
            this.Offset = Offset;
        }
    }
}
