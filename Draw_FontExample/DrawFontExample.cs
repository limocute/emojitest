using FontStashSharp;
using SampleBase;
using System.IO;
using Yak2D;

namespace Draw_FontExample
{
    /// <summary>
    /// Bitmap Font text rendering example (using FontStashSharp rather than Yak2D fonts)
    /// </summary>
    public class DrawFontExample : ApplicationBase
    {
        private IDrawStage _drawStage;
        private ICamera2D _camera;

        private DynamicSpriteFont _emojiFont;
        private FontStashRenderer _fontRenderer;

        public override string ReturnWindowTitle()
        {
            return "Drawing Bitmap Fonts with Font Stash Sharp";
        }

        public override void OnStartup() { }

        public override bool CreateResources(IServices yak)
        {
            _drawStage = yak.Stages.CreateDrawStage();

            _camera = yak.Cameras.CreateCamera2D(960, 540, 1.0f);

            _emojiFont = LoadFont(0, 0, yak.Surfaces).GetFont(24);

            _fontRenderer = new FontStashRenderer(540);

            return true;
        }

        private FontSystem LoadFont(int blur, int stroke, ISurfaces surfaces)
        {
            var fontLoader = StbTrueTypeSharpFontLoader.Instance;
            var textureCreator = new Texture2DCreator(surfaces);

            var font = new FontSystem(fontLoader, textureCreator, 1024, 1024, blur, stroke);
            font.AddFont(File.ReadAllBytes(@"TtfFonts/Segoe UI Emoji.ttf"));

            return font;
        }

        public override bool Update_(IServices yak, float timeSinceLastUpdateSeconds) => true;
        public override void PreDrawing(IServices yak, float timeSinceLastDrawSeconds, float timeSinceLastUpdateSeconds)
        {
            //As we need to know the current screen virtual Y resolution when transforming positions from FontStashSharp to Yak, we check each frame 
            //to make sure we are using the correct Y resolution of the camera being used
            _fontRenderer.SetCameraYResolutionForTransform(yak.Cameras.GetCamera2DVirtualResolution(_camera).Height);
        }

        public override void Drawing(IDrawing draw,
                                     IFps fps,
                                     IInput input,
                                     ICoordinateTransforms transforms,
                                     float timeSinceLastDrawSeconds,
                                     float timeSinceLastUpdateSeconds)
        {
            //Ensure font rendered is using the desired draw stage and has access to the right yak2d drawing services
            _fontRenderer.SetYakDrawingObjects(_drawStage, draw);

            // FontStashSharp appears to require the drawing coordinate system to be X positive right, Y positive downwards
            // (i.e. the origin of the window in the top left)
            // Yak2D always has positive Y in the upwards direction, with the screen space origin in the centre
            // The only reason this cannot be ignored is due to the Y shift that Font Stash Sharp does to each letter
            // Using untransformed Yak2D coordinates results in letters being shifted in the wrong y direction
            _emojiFont.DrawText(_fontRenderer,
                                -430.0f,
                                40.0f, //Need to give FontStashSharp a Y position relative from top of screen where Y == 0
                                "Test Emoji 🎄😃😜🙉😉😍😝🙅🙊✂✈✊🚄🚑🆗♻⚽🍚🎄",
                                System.Drawing.Color.IndianRed,
                                0.5f);
        }

        public override void Rendering(IRenderQueue q, IRenderTarget windowRenderTarget)
        {
            q.ClearColour(windowRenderTarget, Colour.Clear);
            q.ClearDepth(windowRenderTarget);
            q.Draw(_drawStage, _camera, windowRenderTarget);
        }

        public override void Shutdown() { }
    }
}