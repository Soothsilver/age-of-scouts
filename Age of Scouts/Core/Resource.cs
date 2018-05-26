using Auxiliary;

namespace Age.Core
{
    enum Resource
    {
        Wood = 1,
        Food = 2,
        Clay = 3
    }
    static class ResourceExtensions
    {
        public static TextureName ToTextureName(this Resource resource)
        {
            switch(resource)
            {
                case Resource.Clay: return TextureName.MudIcon;
                case Resource.Wood: return TextureName.WoodIcon;
                case Resource.Food: return TextureName.MeatIcon;
                default: throw new System.Exception("This resource does not have an icon.");
            }
        }
    }
}