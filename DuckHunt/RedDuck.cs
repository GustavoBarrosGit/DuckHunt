using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckHunt
{
    class RedDuck : Enemy
    {
        public enum States
        {
            MOVE_DOWN,
            CHASE
        }

        private States state = States.MOVE_DOWN;

        public void SetState(States state)
        {
            this.state = state;
            isRotatable = false;
        }

        public States GetState()
        {
            return state;
        }

        public RedDuck(Texture2D texture, Vector2 position, Vector2 velocity) : base(texture, position, velocity)
        {
            angle = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
