/// <summary>
/// Namn: Leo Magnusson
/// Klass: SU21
/// Info:
/// Innehåller klasser för logik för minorna för programmet X
/// </summary>

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MineGame
{
    /// <summary>
    /// Klass för objekt som rör sig
    /// </summary>
    class MovingObject
    {
        /// <value>
        /// Vector för positionen för minan
        /// </value>
        public Vector2 Position { get; set; }

        /// <value>
        /// Innehåller minans textur
        /// </value>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// Konstruktor för klassen MovingObject
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="texture">Textur</param>
        public MovingObject(Vector2 position, Texture2D texture)
        {
            Position = position;
            Texture = texture;
        }

        /// <summary>
        /// Uppdaterar minans position
        /// </summary>
        /// <param name="velocity"></param>
        public void Update(Vector2 velocity)
        {
            Position += velocity;
        }
        /// <summary>
        /// Ritar ut minan
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="color"></param>
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(Texture, Position, color);
        }
        /// <summary>
        /// Get värdet på minans X-position
        /// </summary>
        /// <returns>Returnerar minans X-position</returns>
        public float GetPosX()
        {
            return Position.X;
        }
        /// <summary>
        /// Get värdet på minans Y-position
        /// </summary>
        /// <returns>Returnerar minans Y-position</returns>
        public float GetPosY()
        {
            return Position.Y;
        }
    }
    /// <summary>
    /// Klass för spelaren
    /// </summary>
    class Player : MovingObject
    {
        /// <summary>
        /// Konstruktor för spelaren som ärver moving object
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public Player(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }

        /// <summary>
        /// Med hjälp av Keyboardstate skapas en vektor för spelarens rörelse
        /// </summary>
        /// <param name="keyboard"></param>
        /// <returns>Returnerar en uppdaterad position</returns>
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
    /// <summary>
    /// En klass för minan som försöker vara i samma X eller Y led som spelaren
    /// </summary>
    class AdvancedMine : MovingObject
    {
        /// <value>
        /// En float som innehåller rotationen på minan
        /// </value>
        public float AdvRotation { get; set; }
        /// <summary>
        /// Konstruktor för Avancerade Minan
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public AdvancedMine(Texture2D texture, Vector2 position, float rotation) : base(position, texture)
        {
            AdvRotation = rotation;
        }
        /// <summary>
        /// Uppdaterar avancerade minans position
        /// </summary>
        /// <param name="playerX"></param>
        /// <param name="playerY"></param>
        /// <param name="difficulty"></param>
        public void AdvancedUpdate(float playerX, float playerY, float difficulty)
        {
            if (AdvRotation == 180 || AdvRotation == 0)//kollar om minan åker horizontellt
            {
                float rotation = 1;
                if (AdvRotation == 180) rotation = -1;
                if (GetPosX() > playerX) Update(new Vector2(-0.75f, rotation * difficulty / 2 + 0.5f * rotation));
                else if (GetPosX() < playerX) Update(new Vector2(0.75f, rotation * difficulty / 2 + 0.5f * rotation));
                else Update(new Vector2(0, rotation * difficulty / 2 + 0.5f * rotation));
                //Rör minan mot spelaren baserat på svårighetsgrad och minans roatation
            }
            else
            {
                float rotation = 1;
                if (AdvRotation == 90) rotation = -1;
                if (GetPosY() > playerY) Update(new Vector2(rotation * difficulty / 2 + 0.5f * rotation, -0.75f));
                else if (GetPosY() < playerY) Update(new Vector2(rotation * difficulty / 2 + 0.5f * rotation, 0.75f));
                else Update(new Vector2(rotation * difficulty / 2 + 0.5f * rotation, 0));
                //Rör minan mot spelaren baserat på svårighetsgrad och minans roatation
            }
        }
    }
    /// <summary>
    /// Klass för vanlig mina
    /// </summary>
    class RegularMine : MovingObject
    {
        /// <summary>
        /// Konstruktor för den vanliga minan
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public RegularMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    /// <summary>
    /// Klass för statisk mina
    /// </summary>
    class StatMine : MovingObject
    {
        /// <value>
        /// Variabel som håller koll på hur länge minan har existerat
        /// </value>
        public int Timer { get; set; }
        /// <summary>
        /// Konstruktor för statisk mina
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="timer"></param>
        public StatMine(Texture2D texture, Vector2 position,int timer) : base(position, texture)
        {
            Timer = timer;
        }
    }
    /// <summary>
    /// Mina som rör sig fån ena sidan
    /// </summary>
    class SideMine : MovingObject
    {
        /// <summary>
        /// Konstruktor för mina som rör sig i sidled
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public SideMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    /// <summary>
    /// Mina som rör sig fån andra sidan
    /// </summary>
    class SideMine2 : MovingObject
    {
        /// <summary>
        /// onstruktor för mina som rör sig i sidled åt andra hållet
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public SideMine2(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    /// <summary>
    /// Klass för mina som rör sig uppåt
    /// </summary>
    class UpMine : MovingObject
    {
        /// <summary>
        /// Konstruktor för minan som rör sig uppåt
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public UpMine(Texture2D texture, Vector2 position) : base(position, texture)
        {

        }
    }
    /// <summary>
    /// Klass för mina som studsar på kanterna
    /// </summary>
    class BounceMine : MovingObject
    {   
        /// <value>
        /// Vektor för minans rörelse
        /// </value>
         public Vector2 BounceVelocity { get; set; }
        /// <summary>
        /// Konstruktor för minan som studsar
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public BounceMine(Texture2D texture, Vector2 position, Vector2 velocity) : base(position, texture)
        {
            BounceVelocity = velocity;
        }
        /// <summary>
        /// Uppdaterar minans position och roterar vektorn om den är utanför spelplanen
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        public void BounceUpdate(int screenWidth, int screenHeight)
        {
            Position = new Vector2(Math.Clamp(Position.X, 0, 800 - Texture.Width), Math.Clamp(Position.Y, 0, 480 - Texture.Height));
            Position += BounceVelocity;
            if (Position.X < 0 || Position.X > screenWidth - Texture.Width) BounceVelocity *= new Vector2(-1, 1);
            if (Position.Y < 0 || Position.Y > screenHeight - Texture.Height) BounceVelocity *= new Vector2(1, -1);
        }
    }
    /// <summary>
    /// Klass för block som puttar spelaren
    /// </summary>
    class BigBlock : MovingObject
    {
        /// <summary>
        /// Blockets rörelse
        /// </summary>
        public Vector2 Velocity { get; set; }
        /// <summary>
        /// Konstruktor för blocket
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        public BigBlock(Texture2D texture, Vector2 position, Vector2 velocity) : base(position, texture)
        {
            Velocity = velocity;
        }
    }
}