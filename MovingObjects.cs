using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MineGame
{
    class MovingObject
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public MovingObject(Vector2 position, Texture2D texture)
        {
            Position = position;
            Texture = texture;
        }

        public void Update(Vector2 velocity)
        {
            Position += velocity;
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(Texture, Position, color);
        }

        public float GetPosX()
        {
            return Position.X;
        }
        public float GetPosY()
        {
            return Position.Y;
        }
    }

    class Player : MovingObject
    {
        public Player(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }

        public double PlayerUpdate(KeyboardState keyboard)
        {
            double rotation = 0;
            Vector2 velocity = Vector2.Zero;
            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) velocity += new Vector2(0, -1);
            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left)) velocity += new Vector2(-1, 0);
            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down)) velocity += new Vector2(0, 1);
            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) velocity += new Vector2(1, 0);
            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
                rotation = Math.Atan2(velocity.Y, velocity.X) + Math.PI / 2; //beräknar spelarens rotation baserat på riktiningen spelaren rör sig
            }
            velocity *= 2;
            Update(velocity);
            Position = new Vector2(Math.Clamp(Position.X, 0, 800 - Texture.Width), Math.Clamp(Position.Y, 0, 480 - Texture.Height)); //Begränsar spelarens position till spelplanen
            return rotation;
        }
    }

    class AdvancedMine : MovingObject
    {
        public float AdvRotate { get; set; }
        public AdvancedMine(Texture2D texture, Vector2 position, float rotation) : base(position, texture)
        {
            AdvRotate = rotation;
        }
        public void AdvancedUpdate(float playerX, float playerY, float difficulty)
        {
            if (AdvRotate == 180 || AdvRotate == 0)//kollar om minan åker horizontellt
            {
                float rotation = 1;
                if (AdvRotate == 180) rotation = -1;
                if (GetPosX() > playerX) Update(new Vector2(-0.75f, rotation * difficulty / 2 + 0.5f * rotation));
                else if (GetPosX() < playerX) Update(new Vector2(0.75f, rotation * difficulty / 2 + 0.5f * rotation));
                else Update(new Vector2(0, rotation * difficulty / 2 + 0.5f * rotation));
                //Rör minan mot spelaren baserat på svårighetsgrad och minans roatation
            }
            else
            {
                float rotation = 1;
                if (AdvRotate == 90) rotation = -1;
                if (GetPosY() > playerY) Update(new Vector2(rotation * difficulty / 2 + 0.5f * rotation, -0.75f));
                else if (GetPosY() < playerY) Update(new Vector2(rotation * difficulty / 2 + 0.5f * rotation, 0.75f));
                else Update(new Vector2(rotation * difficulty / 2 + 0.5f * rotation, 0));
                //Rör minan mot spelaren baserat på svårighetsgrad och minans roatation
            }
        }
    }
    class RegularMine : MovingObject
    {
        public RegularMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class StatMine : MovingObject
    {
        public int Timer { get; set; }
        public StatMine(Texture2D texture, Vector2 position,int timer) : base(position, texture)
        {
            Timer = timer;
        }
    }
    class SideMine : MovingObject
    {
        public SideMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class SideMine2 : MovingObject
    {
        public SideMine2(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class UpMine : MovingObject
    {
        public UpMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    class BounceMine : MovingObject
    {
         public Vector2 BounceVelocity { get; set; }
        public BounceMine(Texture2D texture, Vector2 position, Vector2 velocity) : base(position, texture)
        {
            BounceVelocity = velocity;
        }
        public void BounceUpdate(int screenWidth, int screenHeight)
        {
            Position = new Vector2(Math.Clamp(Position.X, 0, 800 - Texture.Width), Math.Clamp(Position.Y, 0, 480 - Texture.Height));
            Position += BounceVelocity;
            if (Position.X < 0 || Position.X > screenWidth - Texture.Width) BounceVelocity *= new Vector2(-1, 1);
            if (Position.Y < 0 || Position.Y > screenHeight - Texture.Height) BounceVelocity *= new Vector2(1, -1);
        }
    }
    class BigBlock : MovingObject
    {
        public Vector2 Velocity { get; set; }
        public BigBlock(Texture2D texture, Vector2 position, Vector2 velocity) : base(position, texture)
        {
            Velocity = velocity;
        }
    }
}