using SampleBase;
using System.Numerics;
using Yak2D;

namespace Draw_FontExample
{
    /// <summary>
    /// Bitmap Font text rendering example
    /// </summary>
    public class DrawFontExample : ApplicationBase
    {
        private IDrawStage _drawStage;
        private ICamera2D _camera;

        public override string ReturnWindowTitle()
        {
           
            return "Drawing Bitmap Fonts";
        }

        public override void OnStartup() {
         
        
        }

        public override bool CreateResources(IServices yak)
        {
            _drawStage = yak.Stages.CreateDrawStage();

            _camera = yak.Cameras.CreateCamera2D(960, 540, 1.0f);

           
            return true;
        }

        public override bool Update_(IServices yak, float timeSinceLastUpdateSeconds) => true;
        public override void PreDrawing(IServices yak, float timeSinceLastDrawSeconds, float timeSinceLastUpdateSeconds) { }

        public override void Drawing(IDrawing draw,
                                     IFps fps,
                                     IInput input,
                                     ICoordinateTransforms transforms,
                                     float timeSinceLastDrawSeconds,
                                     float timeSinceLastUpdateSeconds)
        {
           
            draw.DrawString(_drawStage,
                             CoordinateSpace.Screen,
                             "Test Emoji 🎄😃😜🙉😉😍😝🙅🙊✂✈✊🚄🚑🆗♻⚽🍚🎄",
                             Colour.IndianRed,
                             18.0f,
                             new Vector2(-430.0f, 220.0f),
                             TextJustify.Left,
                             0.5f,
                             0);

           
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

