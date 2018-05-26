using Microsoft.Xna.Framework;

namespace Age.Core
{
    internal interface IScreenInformation
    {
        Vector2 CenterOfScreenInStandardPixels { get; }
        float ZoomLevel { get; }
    }
}