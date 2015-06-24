using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
using System.Threading;

namespace Agar.net
{
    class World
    {
        private RenderWindow _window;
        private Session _sess;
        private Vector2f _position;
        private Vector2f _size;
        volatile private Dictionary<uint, Cell> _cells;

        private float _viewX, _viewY, _viewRatio;

        public World()
        {
            
        }


        public void Init()
        {
            ContextSettings settings = new ContextSettings();
            settings.AntialiasingLevel = 8;
            _window = new RenderWindow(new VideoMode(1600, 800), "Agar.net", Styles.Default, settings);
            _window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);

            _sess = new Session(this);
            _cells = new Dictionary<uint, Cell>();
            _size = new Vector2f(10000, 10000);
            _viewX = _viewY = 5500;
            _viewRatio = 1;
            UpdateView();
        }

        public void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if(e.Code == Keyboard.Key.S)
            {
                _sess.Spectate();
            }
        }

        public void Run()
        {
            _sess.FindSession(Mode.Experimental, Region.EU);
           
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
            _sess.Update();
        }

        private void Display()
        {
            _window.Clear(new Color(20,20,20,255));

            DrawGrid();

            foreach (var c in _cells)
                c.Value.Draw(_window);

            _window.Display();
        }

        private void DrawGrid()
        {
            RectangleShape line = new RectangleShape();
            line.FillColor = new Color(50, 50, 50, 150);

            line.Size = new Vector2f(20000, 2);
            for(int i = -5000; i<15000; i += 80)
            {
                line.Position = new Vector2f(-5000,i);
                _window.Draw(line);
            }

            line.Size = new Vector2f(2, 20000);
            for (int i = -5000; i < 15000; i += 80)
            {
                line.Position = new Vector2f(i, -5000);
                _window.Draw(line);
            }

        }


        public void SetPosition(Vector2f position)
         {
            _position = position;
            UpdateView();
         }

        public void SetSize(Vector2f size)
        {
            _size = size;
            UpdateView();
        }

        public void SetView(float x, float y, float ratio)
        {
            _viewX = x;
            _viewY = y;
            _viewRatio = ratio;
            UpdateView();
        }


        public Cell AddCell(uint id)
        {
            _cells[id] = new Cell(id);
            return _cells[id];
        }

        public Cell GetCell(uint id)
        {

            if (_cells.ContainsKey(id))
                return _cells[id];
            else
                return null;
        }

        public void RemoveCell(uint id)
        {
            _cells.Remove(id);
        }


        private void UpdateView()
        {
            View view = _window.GetView();
            view.Size =new Vector2f(2000, 1000);
            view.Center = new Vector2f (_viewX, _viewY);
            view.Zoom(1/_viewRatio);
            _window.SetView(view);
        }


    }
}
