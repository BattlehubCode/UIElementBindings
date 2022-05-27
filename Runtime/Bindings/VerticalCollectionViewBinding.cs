using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battlehub.UIElements.Bindings
{
    public class VerticalCollectionViewBindingUxmlTraits<TViewModelsEnum> : TwoWayBindingUxmlTraits<TViewModelsEnum> where TViewModelsEnum : struct, IConvertible
    {
    }
    public class VerticalCollectionViewBinding<TViewModelsEnum> : TwoWayBinding<TViewModelsEnum>
    {
        protected override void Bind()
        {
            viewPropertyName = nameof(BaseVerticalCollectionView.itemsSource);

            BaseVerticalCollectionView collectionView = view as BaseVerticalCollectionView;

            collectionView.onItemsChosen += OnItemsChosen;
            collectionView.itemIndexChanged += OnItemIndexChanged;
            collectionView.itemsSourceChanged += OnItemsSourceChanged;
            collectionView.onSelectionChange += OnSelectionChange;
            collectionView.onSelectedIndicesChange += OnSelectedIndicesChange;

            collectionView.makeItem = () => new Label();
            collectionView.bindItem = (e, i) =>
            {
                IList list = (IList)viewModelPropertyValue;
                ((Label)e).text = list[i].ToString();
            };

            collectionView.reorderable = true;
            collectionView.selectionType = SelectionType.Multiple;

            base.Bind();
        }
        

        protected override void Unbind()
        {
            base.Unbind();

            BaseVerticalCollectionView collectionView = view as BaseVerticalCollectionView;
            if(collectionView != null)
            {
                collectionView.onItemsChosen -= OnItemsChosen;
                collectionView.itemIndexChanged -= OnItemIndexChanged;
                collectionView.itemsSourceChanged -= OnItemsSourceChanged;
                collectionView.onSelectionChange -= OnSelectionChange;
                collectionView.onSelectedIndicesChange -= OnSelectedIndicesChange;
            }
        }

        private void OnSelectedIndicesChange(IEnumerable<int> obj)
        {
            Debug.Log("OnSelectedIndicesChange");
            foreach(int index in obj)
            {
                Debug.Log(index);
            }
        }

        private void OnSelectionChange(IEnumerable<object> obj)
        {
            Debug.Log("OnSelectionChange");
            foreach (object o in obj)
            {
                Debug.Log(o);
            }
        }

        private void OnItemsSourceChanged()
        {
            Debug.Log("OnItemsSourceChanged");
        }

        private void OnItemsChosen(IEnumerable<object> obj)
        {
            Debug.Log("OnItemsChosen");
            foreach (object o in obj)
            {
                Debug.Log(o);
            }
        }

        private void OnItemIndexChanged(int arg1, int arg2)
        {
            Debug.Log($"OnItemIndexChanged {arg1} -> {arg2}");
        }
    }
}
