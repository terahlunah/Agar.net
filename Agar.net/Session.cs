using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using System.Net;
using System.IO;
using System.Text;
using WebSocketSharp;
using System.Collections.Generic;
using SFML.System;
using System.Threading;

namespace Agar
{
    
    class Session 
    {


        private WebSocket _ws;
        private bool _open;
        private World _world;

        private Queue<byte[]> _dataQueue;
        private Mutex _dataMutex;


        public Session(World world)
        {
            _open = false;
            _ws = null;
            _world = world;
            _dataQueue = new Queue<byte[]>();
            _dataMutex = new Mutex();
        }


        public void FindSession(string mode, string region)
        {

            WebRequest request = WebRequest.Create("http://m.agar.io");
            request.Method = "POST";

            byte[] byteArray = Encoding.UTF8.GetBytes(region + mode);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string[] responseFromServer = reader.ReadToEnd().Split('\n');
            reader.Close();
            dataStream.Close();
            response.Close();

            string url = responseFromServer[0];
            string key = responseFromServer[1];

            ConnectToServer(url, key);

        }

        public void ConnectToServer(string url, string key)
        {
            Console.WriteLine("opening connection to " + url);

            _ws = new WebSocket("ws://"+url);
            _ws.Origin = "http://agar.io";

            _ws.OnMessage += (sender, e) => {
                _dataMutex.WaitOne();
                _dataQueue.Enqueue(e.RawData);
                _dataMutex.ReleaseMutex();
            };

            _ws.OnOpen += (sender, e) =>
            {
                Console.WriteLine("opened");
                _open = true;

                this.SendHandShake(key);

                Console.WriteLine("Connection opened to " + url + " - " + key);

            };

            _ws.Connect();
        }

        private void SendHandShake(string key)
        {
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);

                writer.Write((byte)254);
                writer.Write(4);
                Send(ms.ToArray());
            }

            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);

                writer.Write((byte)255);
                writer.Write(154669603);
                Send(ms.ToArray());
            }

            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);

                writer.Write((byte)80);
                writer.Write(ASCIIEncoding.ASCII.GetBytes(key));
                Send(ms.ToArray());
            }
        }

        public void Update()
        {
            _dataMutex.WaitOne();
            while(_dataQueue.Count != 0)
            {
                Process(_dataQueue.Dequeue());
            }
            _dataMutex.ReleaseMutex();
        }

        public bool IsOpen()
        {
            return _open;
        }



        private void Send(byte[] data)
        {
            if (!_open)
                return;

            _ws.Send(data);
            
        }



        // handlers
        private void Process(byte[] data)
        {
            //Console.WriteLine("Received data !");

            MemoryStream ms = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(ms, new UnicodeEncoding());


            uint opcode = reader.ReadByte();
            switch (opcode)
            {
                case 16: // Node update
                    handleUpdateCells(reader);
                    break;

                case 17: // Update camera in spectator mode
                    handleSpectateCameraMove(reader);
                    break;

                case 32: // Add Client cell
                    handleAddCell(reader);
                    break;

                case 49: // (FFA) Leaderboard Update
                    handleFFALeaderboardUpdate(reader);
                    break;

                case 50: // (Team) Leaderboard Update
                    handleTeamLeaderboardUpdate(reader);
                    break;

                case 64: // World size message
                    handleWorldInfo(reader);
                    break;

                default:
                    Console.WriteLine("Unknown opcode : " + opcode);

                    break;
            }
            
        }

        private void handleWorldInfo(BinaryReader data)
        {
            float worldX = (float)data.ReadDouble();
            float worldY = (float)data.ReadDouble();
            float worldW = (float)data.ReadDouble();
            float worldH = (float)data.ReadDouble();

            _world.SetPosition(new Vector2f(worldX, worldY));
            _world.SetSize(new Vector2f(worldW, worldH));

            Console.WriteLine("World info : # " + worldX + " # " + worldY + " # " + worldW + " # " + worldH);
        }

        private void handleSpectateCameraMove(BinaryReader data)
        {
            float x = data.ReadSingle();
            float y = data.ReadSingle();
            float ratio = data.ReadSingle();

            _world.SetView(x, y, ratio);
            //Console.WriteLine("Camera update : " + x + " - " + y + " : " + ratio);
        }

        private void handleFFALeaderboardUpdate(BinaryReader data)
        {
            /*
            uint32 count = data.getInt();

            for (uint32 i = 0; i < count; ++i)
            {
                uint32 score = data.getInt();
                std::string name = data.getUTF16String();
            }
            */
        }

        private void handleTeamLeaderboardUpdate(BinaryReader data)
        {
            /*
            uint32 teamCount = data.getInt();
            std::vector<float> score;

            for (uint32 i = 0; i < teamCount; ++i)
                score.push_back(data.getFloat());

            t0 = score[0];
            t1 = score[1];
            t2 = score[2];
            */
        }

        private void handleAddCell(BinaryReader data)
        {
            /*
            std::cout << "Add new cell" << std::endl;
            */
        }

        private void handleUpdateCells(BinaryReader data)
        {
            
            // Mergers
            ushort mergeCount = data.ReadUInt16();
            for (uint i = 0; i < mergeCount; ++i)
            {
                uint hunter = data.ReadUInt32();
                uint prey = data.ReadUInt32();
                _world.RemoveCell(prey);
                //_world->removeCell(hunter);
            }

            // Updates
            uint id;
            while ((id = data.ReadUInt32()) != 0)
            {
                Cell c = _world.GetCell(id);
                if (c == null)
                    c = _world.AddCell(id);

                ushort x, y, size;
                x = data.ReadUInt16();
                y = data.ReadUInt16();
                size = data.ReadUInt16();

                byte r, g, b, flags;
                r = data.ReadByte();
                g = data.ReadByte();
                b = data.ReadByte();
                flags = data.ReadByte();

                if ((flags & 2) != 0)
                    data.ReadBytes(4);
                if ((flags & 4) != 0)
                    data.ReadBytes(8);
                if ((flags & 8) != 0)
                    data.ReadBytes(16);

                ushort t;
                string name = "";
                while ((t = data.ReadUInt16()) != 0)
                {
                    name += new string((char)t, 1);
                }


                c.SetPosition(new Vector2i(x, y));
                c.SetMass(size);
                c.SetColor(new Color(r, g, b, 255));
                c.SetName(name);

            }


            //Death
            uint deathCount = data.ReadUInt32();
            for (uint i = 0; i < deathCount; ++i)
            {
                uint did = data.ReadUInt32();
                _world.RemoveCell(did);
            }

            
        }




        public void Spawn(string name = "")
        {

            /*
            if (!_open)
                return;

            ByteBuffer buf;
            buf.put((uint8)0);
            auto enc = sf::String(name).toUtf16();
            for (uint32 i = 0; i < enc.length(); ++i)
                buf.putShort(enc[i]);

            _ws->sendBinary(buf.asVector());

            std::cout << "joined" << std::endl;
            */
        }

        public void Spectate()
        {
            
            if (!_open)
                return;

            MemoryStream ms = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(ms);

            writer.Write((byte)1);
            _ws.Send(ms.ToArray());
        }




   
    }
}
