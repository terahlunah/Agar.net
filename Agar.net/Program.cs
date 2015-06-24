using System;


namespace Agar
{

    class Program
    {
        static void Main(string[] args)
        {
            World world = new World();
            world.Init();
            world.Run();
        }
    }

}
