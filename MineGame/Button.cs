/// <summary>
/// Namn: Leo Magnusson
/// Klass: SU21
/// Info:
/// Innehåller klass för knappar som kan tryckas på för programmet X
/// </summary>
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace MineGame
{
    /// <summary>
    /// Klass för knappar
    /// </summary>
    class Button
    {
        /// <value>
        /// Knappens position
        /// </value>
        public Vector2 Position { get; set; }
        /// <value>
        /// Knappens textur
        /// </value>
        public Texture2D Texture { get; set; }
        /// <value>
        /// Svårighetsgrad som spelet blir när man trycker på knappen
        /// </value>
        public float Difficulty { get; set; }
        /// <summary>
        /// Konstruktor för knappar
        /// </summary>
        /// <param name="position">Vektor för position</param>
        /// <param name="texture">Textur för objektet</param>
        /// <param name="difficulty">Nya svårighetsgraden när knappen trycks på</param>
        public Button(Vector2 position, Texture2D texture, float difficulty)
        {
            Position = position;
            Texture = texture;
            Difficulty = difficulty;
        }/// <summary>
        /// Kollar om musen är ovanpå knappen
        /// </summary>
        /// <param name="MouseInput">Information om musen</param>
        /// <returns>Returnerar en bool baserad på om musens position är på knapen</returns>
        public bool MouseOnButton(MouseState MouseInput)
        {
            return (MouseInput.X < Position.X + Texture.Width &&
                MouseInput.X > Position.X &&
                MouseInput.Y < Position.Y + Texture.Height &&
                MouseInput.Y > Position.Y); 
            //Kollar om musens position är på knappen
        }
        /// <summary>
        /// Ritar knappen
        /// </summary>
        /// <param name="spriteBatch">Spritebatchen</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

    }
}
