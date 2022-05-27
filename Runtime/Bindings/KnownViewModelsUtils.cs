using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public static class KnownViewModelsUtils
    {
        public static string TypeToEnumName(string typeName)
        {
            return typeName.Replace(".", "___").Replace("+", "_plus_").Replace("-", "_minus_").Replace(",", "_asm_");
        }

        public static string EnumToTypeName(string enumValue)
        {
            return enumValue.Replace("___", ".").Replace("_plus_", "+").Replace("_minus_", "-").Replace("_asm_", ",");
        }

        internal static object Instantiate<TViewModelsEnum>(this TViewModelsEnum value)
        {
            string typeName = EnumToTypeName(value.ToString());
            Type type = Type.GetType(typeName);
            return type != null ? Activator.CreateInstance(type) : null;
        }

        internal static object FindViewModelFor<TViewModelsEnum>(this TViewModelsEnum viewModel, VisualElement element)
        {
            VisualElement parent = element;
            var parents = new Stack<VisualElement>();
            while (true)
            {
                ViewModelInstance<TViewModelsEnum> viewModelInstance = parent as ViewModelInstance<TViewModelsEnum>;
                if(viewModelInstance != null && viewModelInstance.knownViewModel.Equals(viewModel))
                {
                    return viewModelInstance.instance;
                }

                if(parent.hierarchy.parent == null)
                {
                    break;
                }

                parent = parent.hierarchy.parent;
                parents.Push(parent);
            }

            while(parents.Count > 0)
            {
                foreach (VisualElement child in parents.Pop().Children())
                {
                    ViewModelInstance<TViewModelsEnum> viewModelInstance = child as ViewModelInstance<TViewModelsEnum>;
                    if (viewModelInstance != null && viewModelInstance.knownViewModel.Equals(viewModel))
                    {
                        return viewModelInstance.instance;
                    }
                }

            }

            return null;
        }
    }
}
