// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.IO;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Common.Utils
{
    /// <summary> Contains methods for path manipulation, creation and deletion </summary>
    public static class PathUtils
    {
        /// <summary> Creates the given folder path </summary>
        /// <param name="path"> Target path </param>
        public static void CreatePath(string path)
        {
            Directory.CreateDirectory(ToAbsolutePath(path));
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);
            #endif
        }

        /// <summary> Clean the given path by adjusting the directory separators according to the OS </summary>
        /// <param name="path"> Target path </param>
        public static string CleanPath(string path) =>
            path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .Replace($"{Path.AltDirectorySeparatorChar}{Path.AltDirectorySeparatorChar}", Path.AltDirectorySeparatorChar.ToString());

        /// <summary> Convert the given path to data path </summary>
        /// <param name="path"> Target path </param>
        public static string ToAbsolutePath(string path) =>
            CleanPath
            (
                path.Contains(Application.dataPath)
                    ? path
                    : path.RemoveFirst("Assets".Length).AppendPrefixIfMissing(Application.dataPath)
            );

        /// <summary> Check if the given path is a data path </summary>
        /// <param name="path"> Target path </param>
        public static bool IsAbsolutePath(string path) =>
            path.StartsWith(Application.dataPath);

        /// <summary> Convert the given path to assets path </summary>
        /// <param name="path"> Target path </param>
        public static string ToRelativePath(string path) =>
            CleanPath
            (
                IsAbsolutePath(path)
                    ? "Assets" + path.Substring(Application.dataPath.Length)
                    : path
            );

        /// <summary> Check if the given path is an assets path </summary>
        /// <param name="path"> Target path </param>
        public static bool IsRelativePath(string path) =>
            !IsAbsolutePath(path);

        /// <summary> Get all available Resources directory paths within the current project </summary>
        public static string[] GetResourcesDirectories()
        {
            var result = new List<string>();
            var stack = new Stack<string>();
            stack.Push(Application.dataPath); // Add the root directory to the stack
            while (stack.Count > 0)           // While we have directories to process...
            {
                string currentDir = stack.Pop(); // Grab a directory off the stack
                try
                {
                    foreach (string dir in Directory.GetDirectories(currentDir))
                    {
                        if (Path.GetFileName(dir).Equals("Resources"))
                            result.Add(dir); // If one of the found directories is a Resources dir, add it to the result
                        stack.Push(dir);     // Add directories at the current level into the stack
                    }
                }
                catch
                {
                    Debug.LogError($"Directory {currentDir} couldn't be read from");
                }
            }
            return result.ToArray();
        }

        /// <summary> Check if the given path is a directory </summary>
        public static bool PathIsDirectory(string path) =>
            (File.GetAttributes(ToAbsolutePath(path)) & FileAttributes.Directory) == FileAttributes.Directory;

        /// <summary> Get the directory information for the specified path </summary>
        /// <param name="path"> Target path </param>
        public static string GetDirectoryName(string path) =>
            CleanPath(Path.GetDirectoryName(path));

        /// <summary> Get the file name and extension of the specified path string </summary>
        /// <param name="path"> Target path </param>
        public static string GetFileName(string path) =>
            CleanPath(Path.GetFileName(path));

        /// <summary> Get the file name of the specified path string without the extension </summary>
        /// <param name="path"> Target path </param>
        public static string GetFileNameWithoutExtension(string path) =>
            CleanPath(Path.GetFileNameWithoutExtension(path));

        /// <summary> Get the extension (including the period ".") of the specified path string </summary>
        /// <param name="path"> Target path </param>
        public static string GetExtension(string path) =>
            CleanPath(Path.GetExtension(path));

        /// <summary> Determine if the given path includes a file name extension </summary>
        /// <param name="path"> Target path </param>
        public static bool HasExtension(string path) =>
            Path.HasExtension(CleanPath(path));
    }
}
