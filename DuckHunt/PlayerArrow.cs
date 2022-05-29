using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuckHunt
{
    class PlayerArrow : Entity
    {
        private Texture2D texture;
        public PlayerArrow(Texture2D texture, Vector2 position, Vector2 velocity) : base()
        {
            this.texture = texture;
            this.position = position;
            body.velocity = velocity;

            setupBoundingBox(this.texture.Width, this.texture.Height);
        }
        public new void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
