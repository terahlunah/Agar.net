using System;
using SFML;
using SFML.Graphics;
using SFML.Window;
using WebSocketSharp;

namespace Agar.net
{
    
    class Session 
    {


        private WebSocket _ws;
        private bool _open;
        private World _world;


        // handlers
        private void process(byte[] data)
        {
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


        public Session(World world)
        {
            _open = false;
            _ws = null;
            _world = world;
        }


        public void FindSession(string mode , string region)
        {
            /*
            Http http = new Http("http://m.agar.io");

            Http.Request request;
            request.setMethod(Http.Request.Post);
            request.setUri("/");
            request.setBody(region + mode);

            Http.Response response = http.sendRequest(request);

            string body = response.getBody();
            stringstream ss(body);
            string url, key;
            ss >> url >> key;

            connectToServer(url, key);
            */

        }

        public void ConnectToServer(string url, string key)
        {
            /*
            _ws = WebSocket.from_url("ws://" + url, "http://agar.io");

            _ws.poll();

            ByteBuffer buffer;
            buffer.put(254);
            buffer.putInt(4);
            _ws.sendBinary(buffer.asVector());

            buffer.clear();
            buffer.put(255);
            buffer.putInt(673720361);
            _ws.sendBinary(buffer.asVector());

            buffer.clear();
            buffer.put(80);
            buffer.putString(key);
            _ws.sendBinary(buffer.asVector());

            _open = true;

            cout << "Connection opened to " << url << " - " << key << endl;
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

   
    }
}
