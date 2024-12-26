// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.Reactor.ScriptableObjects;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Runtime;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using UnityEditor;
namespace Doozy.Editor.Reactor.Automation.Generators
{
    public class ProgressorIdExtensionGenerator
    {
        private static string templateName => nameof(ProgressorIdExtensionGenerator).Replace("Generator", "");
        private static string templateNameWithExtension => $"{templateName}.cst";
        private static string templateFilePath => $"{EditorPath.path}/Reactor/Automation/Templates/{templateNameWithExtension}";
        
        private static string targetFileNameWithExtension => $"{templateName}.cs";
        private static string targetFilePath => $"{RuntimePath.path}/Reactor/Ids/{targetFileNameWithExtension}";
        
        public static bool Run(bool saveAssets = true, bool refreshAssetDatabase = false, bool silent = false)
        {
            string data = FileGenerator.GetFile(templateFilePath);
            if (data.IsNullOrEmpty()) return false;
            ProgressorIdDataGroup dataGroup = ProgressorIdDatabase.instance.database;
            if (!ProgressorIdDatabase.instance.database.isEmpty)
                data = InjectContent(data, dataGroup.GetCategories, category => dataGroup.GetNames(category));
            bool result = FileGenerator.WriteFile(targetFilePath, data, silent);
            if (!result) return false;
            if (saveAssets) AssetDatabase.SaveAssets();
            if (refreshAssetDatabase) AssetDatabase.Refresh();
            return true;
        }
        
         private static string InjectContent(string templateData, Func<IEnumerable<string>> getCategories, Func<string, IEnumerable<string>> getNames)
        {
            var accessorStringBuilder = new StringBuilder();
            var dataStringBuilder = new StringBuilder();
            var categories = getCategories.Invoke().ToList();
            int categoriesCount = categories.Count;
            for (int categoryIndex = 0; categoryIndex < categories.Count; categoryIndex++)
            {
                string category = categories[categoryIndex];
                if (category.Equals(CategoryNameItem.k_DefaultCategory)) continue;
                var names = getNames.Invoke(category).ToList();

                //ACCESSOR//
                {
                    accessorStringBuilder.AppendLine($"        public static IEnumerable<{nameof(Progressor)}> GetProgressors({nameof(ProgressorId)}.{category} id) => GetProgressors(nameof({nameof(ProgressorId)}.{category}), id.ToString());");
                    if (categoryIndex < categoriesCount - 2) accessorStringBuilder.AppendLine();
                }

                //DATA//
                {
                    dataStringBuilder.AppendLine($"        public enum {category}");
                    dataStringBuilder.AppendLine("        {");
                    for (int nameIndex = 0; nameIndex < names.Count; nameIndex++)
                    {
                        string name = names[nameIndex];
                        dataStringBuilder.AppendLine($"            {name}{(nameIndex < names.Count - 1 ? "," : "")}");
                    }
                    dataStringBuilder.AppendLine("        }");
                    if (categoryIndex < categoriesCount - 2) dataStringBuilder.AppendLine();
                }
            }

            templateData = templateData.Replace("//ACCESSOR//", accessorStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            templateData = templateData.Replace("//DATA//", dataStringBuilder.ToString().RemoveLast(Environment.NewLine.Length));
            return templateData;
        }
    }
}
