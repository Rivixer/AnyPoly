using AnyPoly.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AnyPoly;

/// <summary>
/// Represents the main game class.
/// </summary>
internal class AnyPolyGame : Game
{
    private readonly GraphicsDeviceManager graphics;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnyPolyGame"/> class.
    /// </summary>
    public AnyPolyGame()
    {
        Instance = this;

        this.graphics = new GraphicsDeviceManager(this);

        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="AnyPolyGame"/> class.
    /// </summary>
    public static AnyPolyGame Instance { get; private set; } = default!;

    /// <summary>
    /// Initializes the game.
    /// </summary>
    protected override void Initialize()
    {
        ContentController.Initialize(this.Content);

        var spriteBatch = new SpriteBatch(this.GraphicsDevice);
        SpriteBatchController.Initialize(spriteBatch);

        ScreenController.Initialize(this.graphics);
        ScreenController.Change(1366, 768, ScreenType.Windowed);
        ScreenController.ApplyChanges();

        DebugConsole.Initialize();

#if DEBUG
        DebugConsole.SendWarning("You are running on a debug build.");
#endif

        base.Initialize();
    }

    /// <summary>
    /// Loads the game content.
    /// </summary>
    protected override void LoadContent()
    {
        Scene.InitializeScenes();

        base.LoadContent();

        DebugConsole.SendMessage("Type 'help' to get list of available commands.");
    }

    /// <summary>
    /// Updates the game state.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    /// <remarks>It is called once per frame.</remarks>
    protected override void Update(GameTime gameTime)
    {
        KeyboardController.Update();
        MouseController.Update();
        ScreenController.Update();

        Scene.Current.Update(gameTime);

        DebugConsole.Update(gameTime);

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the game.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    /// <remarks>It is called once per frame.</remarks>
    protected override void Draw(GameTime gameTime)
    {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatchController.SpriteBatch.Begin();

        Scene.Current.Draw(gameTime);

        DebugConsole.Draw(gameTime);

        SpriteBatchController.SpriteBatch.End();

        base.Draw(gameTime);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        this.graphics.Dispose();
        base.Dispose(disposing);
    }
}
