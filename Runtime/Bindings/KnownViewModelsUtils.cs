
using System;
using System.Linq;
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
            while(true)
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
            }

            return viewModel.FindViewModelInChildren(parent);

        }

        private static object FindViewModelInChildren<TViewModelsEnum>(this TViewModelsEnum viewModel, VisualElement element)
        {
            foreach(VisualElement child in element.Children())
            {
                ViewModelInstance<TViewModelsEnum> viewModelInstance = child as ViewModelInstance<TViewModelsEnum>;
                if (viewModelInstance != null && viewModelInstance.knownViewModel.Equals(viewModel))
                {
                    return viewModelInstance.instance;
                }
            }

            foreach (VisualElement child in element.Children())
            {
                object instance = viewModel.FindViewModelInChildren(child);
                if(instance != null)
                {
                    return instance;
                }
            }
            return null;
        }
    }
}
