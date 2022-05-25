using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace Battlehub.UIElements.Bindings
{
    public static class CodeGen
    {
        private static TAttribute GetAttribute<TAttribute>(object value)
             where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .FirstOrDefault();
        }

        [MenuItem("Tools/Battlehub/UI Element Bindings/Init")]
        public static void Init()
        {
            if (ShouldUpdateKnownViewModelsEnum(out string[] existingViewModelTypeNames, out string[] obsoletViewModelTypeNames))
            {
                GenerateKnownViewModelsEnaum(existingViewModelTypeNames, obsoletViewModelTypeNames);
            }
            CopyTemplates();
        }


        [DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            if (ShouldUpdateKnownViewModelsEnum(out string[] existingViewModelTypeNames, out string[] obsoletViewModelTypeNames))
            {
                GenerateKnownViewModelsEnaum(existingViewModelTypeNames, obsoletViewModelTypeNames);
                CopyTemplates();
            }
        }

        private static void CopyTemplates()
        {
            string[] generatedAssetsPath = { $"{UIElementsBindingsPathUtil.PackageRootPath}/Runtime/Templates" };
            foreach (string asset in AssetDatabase.FindAssets("", generatedAssetsPath))
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                if(!path.EndsWith(".cs"))
                {
                    continue;
                }

                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                string content = textAsset.text;
                content = content.Replace("namespace Battlehub.UIElements.Bindings.Templates", "namespace Battlehub.UIElements.Bindings");
                content = content.Replace("internal class", "public class");

                UIElementsBindingsPathUtil.Instance.EnsureGeneratedAssetFolderExists();

                string fullPath = $"{UIElementsBindingsPathUtil.Instance.GeneratedAssetsPath}/{textAsset.name}.cs";
                File.WriteAllText(fullPath, content);
                AssetDatabase.ImportAsset(fullPath);
                AssetDatabase.SaveAssets();
            }
        }

        private static void GenerateKnownViewModelsEnaum(string[] existingViewModelTypeNames, string[] obsoletViewModelTypeNames)
        {
            string t = "\t";
            string nl = Environment.NewLine;
            string nlt = nl + t;
            string nl2t = nlt + t;

            for (int i = 0; i < existingViewModelTypeNames.Length; ++i)
            {
                existingViewModelTypeNames[i] = $"[InspectorName(\"{KnownViewModelsUtils.EnumToTypeName(existingViewModelTypeNames[i])}\")]{nl2t}{existingViewModelTypeNames[i]},{nlt}";
            }

            for (int i = 0; i < obsoletViewModelTypeNames.Length; ++i)
            {
                obsoletViewModelTypeNames[i] = $"[InspectorName(\"<Removed>\")]{nl2t}{obsoletViewModelTypeNames[i]},{nlt}";
            }

            string delimiter = obsoletViewModelTypeNames.Length > 0 ? t : string.Empty;
            string content =
                "using UnityEngine;" + nl +
                "namespace Battlehub.UIElements.Bindings" + nl +
                "{" + nlt +
                    "[KnownViewModels]" + nlt +
                    "public enum KnownViewModels " + nlt +
                    "{" + nl2t +
                        $"[InspectorName(\"<None>\")]" + nl2t +
                        $"None," + nl2t +
                        $"{string.Join(t, existingViewModelTypeNames)}" + delimiter +
                        $"{string.Join(t, obsoletViewModelTypeNames)}" +
                    "}" + nl +
                "}";


            string[] generatedAssetsPath = { UIElementsBindingsPathUtil.Instance.GeneratedAssetsPath };
            foreach (string asset in AssetDatabase.FindAssets("", generatedAssetsPath))
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                AssetDatabase.DeleteAsset(path);
            }

            UIElementsBindingsPathUtil.Instance.EnsureGeneratedAssetFolderExists();

            string fullPath = $"{UIElementsBindingsPathUtil.Instance.GeneratedAssetsPath}/KnownViewModels.cs";
            File.WriteAllText(fullPath, content);
            AssetDatabase.ImportAsset(fullPath);
            AssetDatabase.SaveAssets();
        }

        private static (object, InspectorNameAttribute)[] GetEnumValuesWithInspectorNameAttribute(Type enumType)
        {
            return Enum.GetValues(enumType).Cast<object>().Select(v => (v, GetAttribute<InspectorNameAttribute>(v))).ToArray();
        }

        private static bool ShouldUpdateKnownViewModelsEnum(out string[] outExistingViewModelTypeNames, out string[] outObsoleteViewModelTypeNames)
        {
            var exisitingViewModelTypes = TypeCache.GetTypesWithAttribute<BindingAttribute>();

            Type knownViewModelsEnumType = TypeCache.GetTypesWithAttribute<KnownViewModelsAttribute>().FirstOrDefault();
            string[] viewModelTypeNamesFromEnum;
            (object EnumValue, InspectorNameAttribute Attribute)[] enumValues;
            if (knownViewModelsEnumType != null) 
            {
                enumValues = GetEnumValuesWithInspectorNameAttribute(knownViewModelsEnumType);
                viewModelTypeNamesFromEnum = enumValues
                    .Where(enumValue => enumValue.Attribute.displayName != "<Removed>" && enumValue.Attribute.displayName != "<None>")
                    .Select(enumValue => Enum.GetName(knownViewModelsEnumType, enumValue.EnumValue)).ToArray();
            }
            else
            {
                enumValues = new (object EnumValue, InspectorNameAttribute Attribute)[0];
                viewModelTypeNamesFromEnum = new string[0];
            }

            
            string[] existingViewModelTypeNames = exisitingViewModelTypes
                    .Select(type => KnownViewModelsUtils.TypeToEnumName($"{type.FullName},{type.Assembly.GetName().Name}"))
                    .ToArray();

            bool shouldUpdate = false;
            if (viewModelTypeNamesFromEnum.Length != exisitingViewModelTypes.Count)
            {
                shouldUpdate = true;
            }
            else
            {
                for (int i = 0; i < viewModelTypeNamesFromEnum.Length; ++i)
                {
                    if (viewModelTypeNamesFromEnum[i] != existingViewModelTypeNames[i])
                    {
                        shouldUpdate = true;
                        break;
                    }
                }
            }

            if(!shouldUpdate)
            {
                outExistingViewModelTypeNames = new string[0];
                outObsoleteViewModelTypeNames = new string[0];
                return false;
            }

            string[] newObsoleteViewModelTypeNames = viewModelTypeNamesFromEnum
                .Where(typeName => Array.IndexOf(existingViewModelTypeNames, typeName) < 0)
                .ToArray();

            string[] existingObsoleteViewModelTypeNamesFromEnum = enumValues
                .Where(enumValue => enumValue.Attribute.displayName == "<Removed>")
                .Select(enumValue => Enum.GetName(knownViewModelsEnumType, enumValue.EnumValue)).ToArray();

            outExistingViewModelTypeNames = existingViewModelTypeNames;
            outObsoleteViewModelTypeNames = newObsoleteViewModelTypeNames
                .Union(existingObsoleteViewModelTypeNamesFromEnum)
                .Except(existingViewModelTypeNames)
                .Distinct()
                .ToArray();

            return true;
        }
    }
}

