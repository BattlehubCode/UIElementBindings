using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{

    public abstract class BindingCoreElement<TViewModelsEnum> : VisualElement
    {
        public TViewModelsEnum knownViewModel
        {
            get;
            set;
        }
    
        protected void LogError(string error)
        {
            Debug.LogError($"{error}. Element {DebugHierarchyPath()}");
        }

        protected string DebugHierarchyPath()
        {
            Stack<string> path = new Stack<string>();
            VisualElement element = this;
            while (element != null)
            {
                path.Push(element.GetType().Name + (!string.IsNullOrEmpty(element.name) ? $":{element.name}" : ""));
                element = element.parent;
            }

            StringBuilder sb = new StringBuilder();
            foreach(string elementName in path)
            {
                sb.Append("/");
                sb.Append(elementName);
            }

            return sb.ToString();
        }
    }
}
