using FontStashSharp.Interfaces;
using Yak2D;

namespace Draw_FontExample
{
    public class Texture2DCreator : ITexture2DCreator
    {
        private ISurfaces _surfaces;

        public Texture2DCreator(ISurfaces surfaces)
        {
            _surfaces = surfaces;
        }

        public ITexture2D Create(int width, int height)
        {
            return new Texture2D(_surfaces, width, height);
        }
    }
}
