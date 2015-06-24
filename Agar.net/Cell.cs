using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace Agar
{
    class Cell
    {
        public Cell(uint id)
        {
            this.id = id;
            mass = 1;
        }

        private uint mass;
        public uint Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        private Vector2i position;
        public Vector2i Position
        {
            get { return position; }
            set { position = value; }
        }

        private uint id;
        public uint Id
        {
            get { return id; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public void Draw(RenderWindow window)
        {
            // Draw outer dark circle
            CircleShape shape = new CircleShape(mass, 40);
            shape.FillColor = new Color((byte)Math.Max(color.R - 50, 0), (byte)Math.Max(color.G - 50, 0), (byte)Math.Max(color.B - 50, 0));
            shape.Position = new Vector2f(position.X - (ushort)mass, position.Y - (ushort)mass);
            window.Draw(shape);


            float diff = 4 + mass / 50;
            // Draw inner circle
            shape.Radius -= diff;
            shape.FillColor = color;
            shape.Position = new Vector2f(position.X - (ushort)mass + diff, position.Y - (ushort)mass + diff);
            window.Draw(shape);
        }

        

    }
}
