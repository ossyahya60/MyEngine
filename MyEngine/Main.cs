﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MyEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ResolutionIndependentRenderer RIR;
        Camera2D Camera;

        ////////<Variables>/////
        Scene TempScene;
        Vector2 Resolution;
        GameObject Image, Image2, Arrow, Clone, mouse;
        Animation idle, run;
        Animator AM;
        Vector2 MousePos;
        GameObject canvas, panel;
        SpriteFont spriteFont;

        Sprite IDLE, RUN;

        GameObject Sand;
        ////////////////////////

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            RIR = new ResolutionIndependentRenderer(this);

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        private void ImportantIntialization()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Setup.Initialize(graphics, Content, spriteBatch, RIR, Window, Camera, this);
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            RIR.VirtualWidth = 1366;
            RIR.VirtualHeight = 768;
            graphics.ApplyChanges();
            /////////Camera And Resolution Independent Renderer/////// -> Mandatory
            Camera = new Camera2D(RIR);
            Camera.Zoom = 1f;
            Camera.Position = new Vector2(RIR.VirtualWidth / 2, RIR.VirtualHeight / 2);

            RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            ImportantIntialization();

            SceneManager.Start();

            SceneManager.AddInitializer(MainScene, 0);
            //////////////////////////////////////////////////////////
            SceneManager.LoadScene(new Scene("MainScene", 0)); //Main Menu
        }

        private void MainScene()
        {
            // TODO: use this.Content to load your game content here
            spriteFont = Content.Load<SpriteFont>("Font");

            GameObject Arrow1 = new GameObject();
            Arrow1.Name = "Arrow1";
            Arrow1.AddComponent<Transform>(new Transform());
            Arrow1.AddComponent<SpriteRenderer>(new SpriteRenderer());

            SceneManager.ActiveScene.AddGameObject(Arrow1);

            SceneManager.ActiveScene.Start();

            Arrow1.GetComponent<SpriteRenderer>().Sprite.LoadTexture("Arrow");

            GameObject Arrow2 = GameObject.Instantiate(Arrow1);
            //SceneManager.ActiveScene.RemoveGameObject(Arrow1);
            Arrow2.Transform.Position = Vector2.One * 100;
            Arrow2.Name = "Arrow2";
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.GetState(); //This has to be called at the start of update method!!

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            /////////Resolution related//////////// -> Mandatory
            if (Resolution != new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight))
                RIR.InitializeResolutionIndependence(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, Camera);

            Resolution = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ///////////////////////////////////////
            // TODO: Add your update logic here
            if (Input.GetKey(Keys.D))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").Transform.Move((float)gameTime.ElapsedGameTime.TotalSeconds * 10, 0);
            else if (Input.GetKey(Keys.A))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow1").Transform.Move(-(float)gameTime.ElapsedGameTime.TotalSeconds * 10, 0);

            if (Input.GetKey(Keys.Right))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow2").Transform.Move((float)gameTime.ElapsedGameTime.TotalSeconds * 10, 0);
            else if (Input.GetKey(Keys.Left))
                SceneManager.ActiveScene.FindGameObjectWithName("Arrow2").Transform.Move(-(float)gameTime.ElapsedGameTime.TotalSeconds * 10, 0);


            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                Camera.Zoom += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (Keyboard.GetState().IsKeyDown(Keys.X))
                Camera.Zoom -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //passing a property as a refrence using delegates
            //Arrow.GetComponent<PropertiesAnimator>().GetKeyFrame("Rotate360").GetFeedback(value => Arrow.Transform.Rotation = value);

            SceneManager.ActiveScene.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            RIR.BeginDraw(); //Resolution related -> Mandatory
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.GetViewTransformationMatrix()); // -> Mandatory

            SceneManager.ActiveScene.Draw(spriteBatch);
            spriteBatch.DrawString(spriteFont, SceneManager.ActiveScene.FindGameObjectWithName("Arrow2").GetComponent<SpriteRenderer>().Sprite.Transform.Position.ToString(), Vector2.Zero, Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}