using System;
using SFML;
using SFML.Graphics;
using SFML.Window;

namespace Agar.net
{
    class World
    {
        RenderWindow _window;



        public void StartSFMLProgram()
        {
            _window = new RenderWindow(new VideoMode(800, 600), "SFML window");
            _window.SetVisible(true);
            _window.Closed += new EventHandler(OnClosed);
            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                _window.Clear(Color.Red);
                _window.Display();
            }

        }
        void OnClosed(object sender, EventArgs e)
        {
            _window.Close();
        }
    }
}
