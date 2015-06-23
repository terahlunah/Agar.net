using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;

namespace Agar.net
{
    class World
    {
        private RenderWindow _window;
        private Session _sess;
        private Vector2f _position;
        private Vector2f _size;
        private Dictionary<uint, Cell> cells;

        public World()
        {
            
        }


        public void Init()
        {
            _window = new RenderWindow(new VideoMode(1600, 800), "Agar.io");
            //_sess = new Session(this);
            cells = new Dictionary<uint, Cell>();
            _size = new Vector2f(10000, 10000);
            
        }

        public void Run()
        {
           // _sess.findSession(MODE_FFA, REGION_EU);
            //_sess->spectate();


           
            _window.SetVisible(true);
            _window.Closed += new EventHandler(OnClosed);
            while (_window.IsOpen)
            {
                Update();
                Display();
            }


           
        }
        public void OnClosed(object sender, EventArgs e)
        {
            _window.Close();
        }

        private void Update()
        {
            _window.DispatchEvents();
            //_sess.update();
        }

        private void Display()
        {
            _window.Clear(new Color(20,20,20,255));

            DrawGrid();

            foreach (var c in cells)
                c.Value.Draw(_window);

            _window.Display();
        }

        private void DrawGrid()
        {
            RectangleShape line = new RectangleShape();
            line.FillColor = new Color(50, 50, 50, 200);

            line.Size = new Vector2f(_size.X, 1);
            for(uint i = 0; i<_size.Y; i += 50)
            {
                line.Position = new Vector2f(0,i);
                _window.Draw(line);
            }

            line.Size = new Vector2f(1, _size.Y);
            for (uint i = 0; i < _size.X; i += 50)
            {
                line.Position = new Vector2f(i, 0);
                _window.Draw(line);
            }
        }


        public void SetPosition(Vector2f position)
         {
        _position = position; UpdateView();
         }

    public void SetSize(Vector2f size)
        {
            _size = size; UpdateView();
        }


        public Cell AddCell(uint id)
        {
            cells[id] = new Cell(id);
            return cells[id];
        }

        public Cell GetCell(uint id)
        {

            if (cells.ContainsKey(id))
                return cells[id];
            else
                return null;
        }

        public void RemoveCell(uint id)
        {
            cells.Remove(id);
        }


        private void UpdateView()
        {
            View view = _window.GetView();
            view.Size =new Vector2f(4000, 2000);
            view.Center = new Vector2f (_size.X / 2.0f, _size.Y / 2.0f);
            view.Zoom(1);
            _window.SetView(view);
        }


    }
}
