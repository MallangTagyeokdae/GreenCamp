// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;

namespace Doozy.Runtime.SceneManagement
{
    [Serializable]
    public struct SceneLoaderSignalData
    {
        public SceneLoader source { get; private set; }
        
        public SceneLoaderSignalData(SceneLoader sceneLoader) =>
            source = sceneLoader;

        public override string ToString() =>
            source == null 
                ? "Source is null!" 
                : $"({nameof(SceneLoader)}) [{source.name}] state: {source.currentState}";
    }
}
