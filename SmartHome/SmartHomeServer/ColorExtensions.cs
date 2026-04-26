using SmartHome;

namespace SmartHomeServer
{
    public static class ColorExtensions
    {
        public static void Deconstruct(this Color color, out short r, out short g, out short b)
        {
            r = color.r;
            g = color.g;
            b = color.b;
        }
    }
}
