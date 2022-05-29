using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DuckHunt
{
    class AnimatedSprite
    {
        //Properties every animation sprite have in commmon:
        //Width, Height of each frame;
        //Duration of each frame, the amount of frames, the current frame;
        //If animation can repeat, if that animation can’t repeat, we need to know if the animation has finished playing.
        private Texture2D texture;
        public int frameWidth { get; set; }
        public int frameHeight { get; set; }
        public int duration { get; set; }
        public Rectangle sourceRect { get; set; }
        public int amountFrames { get; set; }
        public int currentFrame { get; set; }
        private int updateTick = 0;
        private bool _repeats = true;
        public void setCanRepeat(bool canRepeat)
        {
            _repeats = canRepeat;
        }
        private bool _finished = false;
        public bool isFinished()
        {
            return _finished;
        }

        //Constructor
        public AnimatedSprite(Texture2D texture, int frameWidth, int frameHeight, int duration)
        {
            this.texture = texture;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.duration = duration;
            amountFrames = this.texture.Width / this.frameWidth;
            sourceRect = new Rectangle(currentFrame * this.frameWidth, 0, this.frameWidth, this.frameHeight);
        }

        //Update
        public void Update(GameTime gameTime)
        {
            if (updateTick < duration)
            {
                updateTick++;
            }
            else
            {
                if (currentFrame < amountFrames - 1)
                {
                    currentFrame++;
                }
                else
                {
                    if (_repeats)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        _finished = true;
                    }
                }

                sourceRect = new Rectangle(currentFrame * this.frameWidth, 0, this.frameWidth, this.frameHeight);
                updateTick = 0;
            }
        }
    }
}