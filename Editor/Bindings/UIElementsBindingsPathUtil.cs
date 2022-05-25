using Battlehub.PackageUtils;

namespace Battlehub.UIElements.Bindings
{
    public class UIElementsBindingsPathUtil : PathUtil
    {
        public static string PackageRootPath
        {
            get { return "Packages/net.battlehub.uielementbindings"; }
        }

        public UIElementsBindingsPathUtil() : 
            base("Assets/Battlehub/Generated/UIElements/Runtime/Bindings", $"{typeof(UIElementsBindingsPathUtil).Namespace}.GeneratedAssetsPath")
        {
        }

        private static PathUtil s_instance;
        public static PathUtil Instance
        {
            get
            {
                if(s_instance == null)
                {
                    s_instance = new UIElementsBindingsPathUtil();
                }
                return s_instance;
            }
        }
    }

}
