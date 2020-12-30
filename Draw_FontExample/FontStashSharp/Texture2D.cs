using FontStashSharp.Interfaces;
using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Yak2D;

namespace Draw_FontExample
{
    public class Texture2D : ITexture2D
    {
        private ISurfaces _surfaces;
        private uint _width;
        private uint _height;

        private ITexture _texture;
        private Vector4[] _pixelData;

        public Texture2D(ISurfaces surfaces, int width, int height)
        {
            if (surfaces == null)
            {
                throw new NullReferenceException("The Yak2D Surfaces service reference is NULL. Do not pass a NULL service");
            }

            if (width < 1 || height < 1)
            {
                throw new ArgumentException("Trying to create a texture with dimension 0 or negative, doesn't make sense");
            }

            _surfaces = surfaces;
            _width = (uint)width;
            _height = (uint)height;

            //Fill pixel data array with clear pixels
            var numberOfPixels = _width * _height;
            _pixelData = Enumerable.Repeat<Vector4>(Vector4.Zero, (int)numberOfPixels).ToArray();
        }

        public ITexture YakTexture => ReturnTexture();

        private ITexture ReturnTexture()
        {
            if (_texture == null)
            {
                CreateTexture();
            }

            return _texture;
        }

        private void CreateTexture()
        {
            if (_texture != null)
            {
                _surfaces.DestroySurface(_texture);
            }

            _texture = _surfaces.CreateRgbaFromData((uint)_width, (uint)_height, _pixelData);
        }

        public void Dispose()
        {
            _surfaces.DestroySurface(_texture);
        }

        public void SetData(Rectangle bounds, byte[] data)
        {
            //This implementation just creates a new texture on each setdata call
            //I may expose updating part of a texture in Yak2D to save on data transfer for these types of operations

            //Update the data array
            //The byte array has 4 bytes R, G, B, A for every pixel

            var updateDataCount = 0;
            for (var y = bounds.Y; y < bounds.Y + bounds.Height; y++)
            {
                for (var x = bounds.X; x < bounds.X + bounds.Width; x++)
                {
                    var textureDataLinearIndex = (y * _width) + x;

                    var r = updateDataCount;
                    var g = r + 1;
                    var b = g + 1;
                    var a = b + 1;

                    _pixelData[textureDataLinearIndex] = new Vector4(((int)data[r]) / 255.0f, ((int)data[g]) / 255.0f, ((int)data[b]) / 255.0f, ((int)data[a]) / 255.0f);

                    updateDataCount += 4;
                }
            }

            //Recreate Texture
            CreateTexture();
        }
    }
}