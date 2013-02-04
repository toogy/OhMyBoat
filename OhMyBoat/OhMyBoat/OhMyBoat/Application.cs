using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using OhMyBoat.Maps;
using OhMyBoat.Menu;
using OhMyBoat.Menu.MenuItems;
using OhMyBoat.Network;
using System.Net.Sockets;
using OhMyBoat.Network.Events;

namespace OhMyBoat
{
    public class Application : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Client _client;
        private bool _isServer;
        private string _serverIp;


        private Stack<GameState> _gameStates;

        public Application( /*bool isServer, string serverIp = ""*/)
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Title = "Oh My Boat! What a ballzy boat!";

            /*_isServer = isServer;
            _serverIp = serverIp;*/

        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            IsMouseVisible = true;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameStates = new Stack<GameState>();

            //////////////////////////////////////////////////////////

            GameDatas.Theme = new Theme("PinkTheme", 10, 40, 17, 3, 28, 33, 5, 50, 48);

            GameDatas.Theme.Load(Content);

            //////////////////////////////////////////////////////////

            _graphics.PreferredBackBufferWidth = GameDatas.Theme.GridTexture.Width*2 + 50;
            _graphics.PreferredBackBufferHeight = GameDatas.Theme.GridTexture.Height +
                                                  GameDatas.Theme.LogoTexture.Height + 50;
            _graphics.ApplyChanges();

            GameDatas.WindowWidth = Window.ClientBounds.Width;
            GameDatas.WindowHeight = Window.ClientBounds.Height;

            //////////////////////////////////////////////////////////

            /*if (_isServer)
                _client = new Server().AcceptClient();
            else
            {
                var client = new TcpClient();
                client.Connect(_serverIp, 4242);
                _client = new Client(client);
            } */

            var logo = new MenuPassive(GameDatas.Theme.LogoTexture);

            // CREATION MENU CREATE GAME

            var createNameTextBox = new MenuTextBox("What's your name?");
            var submitCreateGame = new MenuButton("Go !");
            submitCreateGame.Click = _launchGame;

            var createGameMenuItems = new List<MenuItem> { logo, createNameTextBox, submitCreateGame };
            var createGameMenuState = new MenuState(createGameMenuItems, true);
            createGameMenuState.SetPositions();

            // CREATION MENU JOIN GAME

            var joinNameTextBox = new MenuTextBox("What's your name?");
            var serverIpTextBox = new MenuTextBox("IP Server :D");
            var submitJoinGame = new MenuButton("Go !");
            var comeBackButton = new MenuButton("Come Back :)");

            var joinGameMenuItems = new List<MenuItem> {logo, joinNameTextBox, serverIpTextBox, submitJoinGame, comeBackButton};
            var joinGameMenuState = new MenuState(joinGameMenuItems, true);
            joinGameMenuState.SetPositions();

            comeBackButton.Click = _comeBack;

            // CREATION MENU ACCUEIL

            var createGameButton = new MenuButton("Create a Game");
            var joinGameButton = new MenuButton("Join a Game");

            createGameButton.subMenu = createGameMenuState;
            createGameButton.Click = _launchMenu;
            joinGameButton.subMenu = joinGameMenuState;
            joinGameButton.Click = _launchMenu;

            var homeMenuItems = new List<MenuItem> {logo, createGameButton, joinGameButton};
            var homeMenuState = new MenuState(homeMenuItems, true);
            homeMenuState.SetPositions();

            // FIN CREATION MENUS

            _gameStates.Push(homeMenuState);
            _gameStates.Peek().Initialize();
            _gameStates.Peek().LoadContent(Content);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            GameDatas.KeyboardState = Keyboard.GetState();
            GameDatas.MouseState = Mouse.GetState();

            //////////////////////////////////////////////////////////

            _gameStates.Peek().Update(gameTime);

            //////////////////////////////////////////////////////////

            GameDatas.PreviousKeyboardState = GameDatas.KeyboardState;
            GameDatas.PreviousMouseState = GameDatas.MouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            //////////////////////////////////////////////////////////

            _gameStates.Peek().Draw(_spriteBatch);

            //////////////////////////////////////////////////////////

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void _launchMenu(MenuState m)
        {
            _gameStates.Push(m);
        }

        private void _comeBack(MenuState m)
        {
            if (_gameStates.Count > 1)
                _gameStates.Pop();
        }

        private void _launchGame(MenuState m)
        {
            PlayState p = new PlayState();
            _gameStates.Push(p);
        }
    }
}