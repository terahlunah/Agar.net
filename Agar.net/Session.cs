using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using System.Net;
using System.IO;
using System.Text;
using WebSocketSharp;

namespace Agar.net
{
    
    class Session 
    {


        private WebSocket _ws;
        private bool _open;
        private World _world;



        public Session(World world)
        {
            _open = false;
            _ws = null;
            _world = world;
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
                this.Process(e.RawData);
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
                writer.Write(2207389747);
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
            /*
            if (_ws->getReadyState() != easywsclient::WebSocket::CLOSED) // Websocket connection loop
            {
                _ws->poll();
                bool hasData = false;

                do
                {
                    hasData = false;

                    _ws->dispatchBinary([this, &hasData](const std::vector<uint8_t>&inData)
            {
                        process(ByteBuffer(inData));
                        hasData = true;
                    });

                } while (hasData);

            }
            */
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
            Console.WriteLine("Received data !");
            

            /*
            uint32 opcode = static_cast<int>(data.get());
            switch (opcode)
            {
                case 16: // Node update
                    handleUpdateCells(data);
                    break;

                case 17: // Update client in spectator mode
                    std::cout << "Unhandled spectator client update" << std::endl;
                    break;

                case 32: // Add Client cell
                    handleAddCell(data);
                    break;


                case 49: // (FFA) Leaderboard Update
                    handleFFALeaderboardUpdate(data);
                    break;

                case 50: // (Team) Leaderboard Update
                    handleTeamLeaderboardUpdate(data);
                    break;

                case 64: // World size message
                    handleWorldInfo(data);
                    break;

                default:
                    std::cout << "Unknown opcode : " << opcode << std::endl;

                    break;
            }
            */
        }

        private void handleWorldInfo(byte[] data)
        {
            /*
            std::cout << "World Info";

            double worldX = data.getDouble();
            double worldY = data.getDouble();
            double worldW = data.getDouble();
            double worldH = data.getDouble();

            _world->setPosition(sf::Vector2f(worldX, worldY));
            _world->setSize(sf::Vector2f(worldW, worldH));

            std::cout << " # " << worldX << " # " << worldY << " # " << worldW << " # " << worldH << std::endl;
            */
        }

        private void handleFFALeaderboardUpdate(byte[] data)
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

        private void handleTeamLeaderboardUpdate(byte[] data)
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

        private void handleAddCell(byte[] data)
        {
            /*
            std::cout << "Add new cell" << std::endl;
            */
        }

        private void handleUpdateCells(byte[] data)
        {
            /*
            // Mergers
            uint32 mergeCount = data.getShort();
            for (uint32 i = 0; i < mergeCount; ++i)
            {
                uint32 hunter = data.getInt();
                uint32 prey = data.getInt();
                _world->removeCell(prey);
                //_world->removeCell(hunter);
            }

            // Updates
            uint32 id;
            while (id = data.getInt())
            {
                Cell* c = _world->getCell(id);
                if (!c)
                    c = _world->addCell(id);

                uint16 x, y, size;
                x = data.getShort();
                y = data.getShort();
                size = data.getShort();

                uint8 r, g, b, flags;
                r = data.get();
                g = data.get();
                b = data.get();
                flags = data.get();

                if (flags & 2)
                    data.setReadPos(data.getReadPos() + 4);
                if (flags & 4)
                    data.setReadPos(data.getReadPos() + 8);
                if (flags & 8)
                    data.setReadPos(data.getReadPos() + 16);

                std::string name = data.getUTF16String();

                c->setPosition(sf::Vector2i(x, y));
                c->setSize(size);
                c->setColor(sf::Color(r, g, b));
                c->setName(name);


                //debug

                _world->updateZone(x, y, size);
            }


            //Death
            uint32 deathCount = data.getShort();
            for (uint32 i = 0; i < deathCount; ++i)
            {
                uint32 id = data.getInt();
                _world->removeCell(id);
            }

            */
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
            /*
            if (!_open)
                return;

            ByteBuffer buf;
            buf.put((uint8)1);
            _ws->sendBinary(buf.asVector());

            std::cout << "spectating" << std::endl;
            */
        }




   
    }
}
