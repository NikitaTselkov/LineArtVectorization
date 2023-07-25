using System.Windows;

namespace LineArtVectorization.Core
{
    public record Line
    {
        public Point Start { get; set; }
        public Point End { get; set; }
    }
}
