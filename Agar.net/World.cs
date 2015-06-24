using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Collections.Generic;
using System.Threading;

namespace Agar
{
    class World
    {
        private RenderWindow _window;
        private Session _sess;
        private Vector2f _position;
        private Vector2f _size;

        private Dictionary<uint, Cell> cells;
        private List<uint> ownedCells;

        private float _viewX, _viewY, _viewRatio;

        private bool playing, spectating;

        public World()
        {
            
        }


        public void Init()
        {
            ContextSettings settings = new ContextSettings();
            settings.AntialiasingLevel = 8;
            _window = new RenderWindow(new VideoMode(1600, 800), "Agar.net", Styles.Default, settings);

            _window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            _window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);

            _sess = new Session(this);
            cells = new Dictionary<uint, Cell>();
            ownedCells = new List<uint>();
            _size = new Vector2f(10000, 10000);
            _viewX = _viewY = 5500;
            _viewRatio = 1;
            UpdateView();

            playing = spectating = false;
        }

        public void FindSession(string mode, string region)
        {
            _sess.FindSession(mode, region);

            playing = false;
            spectating = false;
            cells.Clear();
            ownedCells.Clear();

            _viewX = _viewY = 5500;
            _viewRatio = 1;
            UpdateView();
        }


        public void Run()
        {
            FindSession(Mode.FFA, Region.EU);
           
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

            UpdateMouse();
            UpdateView();

            _sess.Update();
        }

        private void UpdateMouse()
        {
            if(playing)
            {
                Vector2f pos = (Vector2f)Mouse.GetPosition(_window);

                float x = pos.X - (_window.Size.X / 2) + _viewX;
                float y = pos.Y - (_window.Size.Y / 2) + _viewY;

                x = Math.Max(Math.Min(x, _size.X), 0);
                y = Math.Max(Math.Min(y, _size.Y), 0);

                _sess.SendAim(x, y);
            }
        }

        public void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.S)
            {
                _sess.Spectate();
                playing = false;
                spectating = true;
            }
            if (e.Code == Keyboard.Key.P)
            {
                _sess.Spawn();
                playing = true;
                spectating = false;
            }

            if (e.Code == Keyboard.Key.Num1)
            {
                FindSession(Mode.FFA, Region.EU);
            }
            if (e.Code == Keyboard.Key.Num2)
            {
                FindSession(Mode.Team, Region.EU);
            }
            if (e.Code == Keyboard.Key.Num3)
            {
                FindSession(Mode.Experimental, Region.EU);
            }
        }

        public void OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if(e.Button == Mouse.Button.Left)
            {
                _sess.SendEjectMass();
            }
            if (e.Button == Mouse.Button.Right)
            {
                _sess.SendSplit();
            }
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
            ownedCells.Remove(id);
        }

        public void AddOwnedCell(uint id)
        {
            ownedCells.Add(id);
        }


        private void UpdateView()
        {
            if (ownedCells.Count != 0)
            {
                Vector2i pos = GetCell(ownedCells[0]).Position;
                _viewX = pos.X;
                _viewY = pos.Y;
            }

            View view = _window.GetView();
            view.Size = new Vector2f(2000, 1000);
            view.Center = new Vector2f(_viewX, _viewY);
            view.Zoom(1 / _viewRatio);
            _window.SetView(view);
        }


    }
}
