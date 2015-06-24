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
        public Vector2i _position;
        private uint _mass;
        private Color _color;
        public string _name;
        private float _radius;

        public Cell(uint id)
        {
            _id = id;
            _mass = 1;
        }

        public void Draw(RenderWindow window)
        {
            CircleShape shape = new CircleShape(_radius, (uint)_radius);
            shape.FillColor = _color;
            shape.Position = new Vector2f(_position.X - (ushort)_radius, _position.Y - (ushort)_radius);

            window.Draw(shape);
        }

        public void SetMass(uint mass)
        {
            _mass = mass;
            _radius = _mass;// / ((float)Math.PI * (float)Math.PI);
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
