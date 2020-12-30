using System;
using System.Drawing;
using System.Numerics;
using Yak2D;
using FontStashSharp.Interfaces;

namespace Draw_FontExample
{
    public class FontStashRenderer : IFontStashRenderer
    {
        private IDrawing _drawService;
        private IDrawStage _drawStage;

        private CoordinateSpace _coordinateSpace;
        private float _screenHeight;

        public FontStashRenderer(int screenHeight)
        {
            _screenHeight = (float)screenHeight;
            _coordinateSpace = CoordinateSpace.Screen; //Must be coordinate space as FontStashSharp requires positive Y to be negative
        }

        public void SetYakDrawingObjects(IDrawStage drawStage, IDrawing draw)
        {
            _drawStage = drawStage;
            _drawService = draw;
        }

        public void SetCameraYResolutionForTransform(float screenHeight)
        {
            _screenHeight = screenHeight;
        }

        public void Draw(ITexture2D texture,
                         PointF position,
                         Rectangle? sourceRectangle,
                         System.Drawing.Color color,
                         float rotation,
                         PointF origin,
                         PointF scale,
                         float depth)
        {
            //I've assumed that these paramaters match the function of those that would be used in a "spritebatch" implementation (such as XNA)

            if (_drawStage == null)
            {
                throw new NullReferenceException("No Yak2D DrawStage has been set in FontStashRenderer");
            }

            if (_drawService == null)
            {
                throw new NullReferenceException("No Yak2D Drawing Service has been set in FontStashRenderer");
            }

            //Generate Colour in Yak2D format (going from 4 bytes (ints 0 to 255) to 4 floats 0 to 1
            var colour = new Colour(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);

            //Generate Texture Coordinates from source Rectangle. Source rectangle is in Texels, required Yak2D coordinates are fractional

            //We need to know entire size of texture we are dealing with
            //We assume here a cast from Interface to implementation. Not good practice, but suitable for example.
            var textureObject = texture as Texture2D;
            var textureSize = _drawService.GetSurfaceDimensions(textureObject.YakTexture);

            //Set as if whole texture first (Yak2D texture coordinates are origin top left)
            var minx = 0.0f;
            var maxx = 1.0f;
            var miny = 0.0f;
            var maxy = 1.0f;

            var unscaledWidth = textureSize.Width;
            var unscaledHeight = textureSize.Height;

            if (sourceRectangle.HasValue)
            {
                var rect = sourceRectangle.Value; //System.Drawing.Rectangle is also origin top left (x, y)

                minx = (float)rect.X / (float)textureSize.Width;
                maxx = (float)(rect.X + rect.Width) / (float)textureSize.Width;

                miny = (float)rect.Y / (float)textureSize.Height;
                maxy = (float)(rect.Y + rect.Height) / (float)textureSize.Height;

                unscaledWidth = rect.Width;
                unscaledHeight = rect.Height;
            }

            //Position in Yak2D helper function is the centre of the quad. Position in SpriteBatch is related to the origin set for the sprite, rotation and the position given
            //We must transform the position

            var centreUnscaled = new Vector2(unscaledWidth, unscaledHeight) * 0.5f;
            var originVector = new Vector2(origin.X, origin.Y);

            var originToQuadCentre = centreUnscaled - originVector;

            var originPosition = new Vector2(position.X, position.Y);

            if (originToQuadCentre != Vector2.Zero)
            {
                //Spritebatch rotates clockwise, this is a negative rotation using Vector2 transform matrices
                originToQuadCentre = Vector2.Transform(originToQuadCentre, Matrix3x2.CreateRotation(-rotation));
            }

            var quadPosition = originPosition + originToQuadCentre;
            
            //HERE is where we account for the coordinate system differences
            //Convert Y position from "origin top left" to "origin middle of screen and positive Y towards top of screen"
            var midY = _screenHeight * 0.5f;
            var distanceFromMidY = midY - quadPosition.Y;
            quadPosition.Y = distanceFromMidY;

            var width = unscaledWidth * scale.X;
            var height = unscaledHeight * scale.Y;

            _drawService.Helpers.DrawTexturedQuad(_drawStage,
                                                  _coordinateSpace,
                                                  textureObject.YakTexture,
                                                  colour,
                                                  quadPosition,
                                                  width,
                                                  height,
                                                  depth,
                                                  0,
                                                  rotation,
                                                  minx,
                                                  miny,
                                                  maxx,
                                                  maxy,
                                                  TextureCoordinateMode.Wrap);
        }
    }
}