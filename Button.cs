using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MineGame
{
    class Button
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public float Difficulty { get; set; }
        public Button(Vector2 position, Texture2D texture, float difficulty)
        {
            Position = position;
            Texture = texture;
            Difficulty = difficulty;
        }
        public bool MouseOnButton(MouseState MouseInput)
        {
            if (MouseInput.X < Position.X + Texture.Width &&
                MouseInput.X > Position.X &&
                MouseInput.Y < Position.Y + Texture.Height &&
                MouseInput.Y > Position.Y)
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

    }
}
