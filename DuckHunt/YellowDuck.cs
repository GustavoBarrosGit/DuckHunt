using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuckHunt
{
    class YellowDuck : Enemy
    {
        private int shootDelay = 60 * 2;
        private int shootTick = 0;
        public bool canShoot = false;


        public YellowDuck(Texture2D texture, Vector2 position, Vector2 velocity) : base(texture, position, velocity)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!canShoot)
            {
                if (shootTick < shootDelay)
                {
                    shootTick++;
                }
                else
                {
                    canShoot = true;
                }
            }

            sprite.Update(gameTime);

            base.Update(gameTime);
        }

        public void resetCanShoot()
        {
            canShoot = false;
            shootTick = 0;
        }
    }
}
