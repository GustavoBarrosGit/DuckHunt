using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;




namespace DuckHunt
{
    class Entity        //Player and any other enemies will inherit this class
    {
        protected bool isRotatable = false;
        public Vector2 scale = new Vector2(2f, 2f);
        public Vector2 position = new Vector2(0, 0);
        protected Vector2 sourceOrigin = new Vector2(0, 0);
        public Vector2 destOrigin = new Vector2(0, 0);
        public PhysicsBody body { get; set; }

        //Constructor -- When a class inherits this class, a physics body will automatically be created for that class 
        public Entity()
        {
            body = new PhysicsBody()
            {

            };
        }

        //Setup BoundingBox to entities
        public void setupBoundingBox(int width, int height)
        {
            body.boundingBox = new Rectangle((int)(position.X - destOrigin.X), (int)(position.Y - destOrigin.Y), (int)(width * scale.X), (int)(height * scale.Y));

        }

        //Update
        public void Update(GameTime gameTime)
        {
            if (body != null)
            {
                position.X += body.velocity.X;
                position.Y += body.velocity.Y;

                body.boundingBox = new Rectangle((int)position.X - (int)destOrigin.X, (int)position.Y - (int)destOrigin.Y, body.boundingBox.Width, body.boundingBox.Height);

            }
            else
            {
                Console.WriteLine("[BaseEntity] body not found, skipping position updates.");
            }
        }
    }
}
