
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class BindingCoreElementTraits<TViewModelsEnum> : VisualElement.UxmlTraits where TViewModelsEnum : struct, IConvertible
    {
        private UxmlEnumAttributeDescription<TViewModelsEnum> m_knownViewModel =
            new UxmlEnumAttributeDescription<TViewModelsEnum> { name = "known-view-model", defaultValue = default(TViewModelsEnum) };

        public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
        {
            get { yield return m_knownViewModel; }
        }

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            BindingCoreElement<TViewModelsEnum> baseBinding = ve as BindingCoreElement<TViewModelsEnum>;
            baseBinding.knownViewModel = m_knownViewModel.GetValueFromBag(bag, cc);
        }
    }

}
