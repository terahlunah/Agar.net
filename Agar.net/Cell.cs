using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace Agar.net
{
    class Cell
    {
        private uint _id;
        Vector2i _position;
        uint _size;
        Color _color;
        string _name;

        public Cell(uint id)
        {
            _id = id;
            _size = 1;

        }

        public void Draw(RenderWindow window)
        {
            CircleShape shape = new CircleShape(50);
            shape.FillColor = _color;
            shape.Position = new Vector2f(_position.X - _size, _position.Y - _size);
            shape.Radius = _size;

            window.Draw(shape);
        }

        public void SetSize(uint size)
        {
            _size = size;
        }

        public void SetPosition(Vector2i position)
        {
            _position = position;
        }

        public void SetColor(Color color)
        {
            _color = color;
        }

        public void SetName(string name)
        {
            _name = name;
        }

    }
}
