using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuckHunt
{
    class MenuButton
    {
        private Game1 game;
        private Vector2 position;
        private Texture2D texDefault;
        public Rectangle boundingBox;

        public bool isActive { get; set; }
        public bool lastIsDown = false;
        private bool _isDown = false;
        private bool _isHovered = false;

        public void SetDown(bool isDown)
        {
            if (!_isDown && isDown)
            {
                game.sndBtnDown.Play();
            }
            _isDown = isDown;

        }
        public void SetHovered(bool isHovered)
        {
            if (!_isHovered && !_isDown && isHovered)
            {
                game.sndBtnOver.Play();
            }
            _isHovered = isHovered;
        }

        public MenuButton(Game1 game, Vector2 position, Texture2D texDefault)
        {
            this.game = game;
            this.position = position;
            this.texDefault = texDefault;
            boundingBox = new Rectangle((int)position.X, (int)position.Y, this.texDefault.Width, this.texDefault.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                spriteBatch.Draw(texDefault, position, Color.White);
            }
        }
    }
}
