// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Modules
{
	/// <summary> Mody module used to control an AudioSource </summary>
	[AddComponentMenu("Mody/AudioSource Module")]
	public class AudioSourceModule : ModyModule
	{
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/Mody/AudioSource Module", false, 8)]
		private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
		{
			GameObjectUtils.AddToScene<AudioSourceModule>("AudioSource Module", false, true);
		}
		#endif
		
		/// <summary> Default module name </summary>
		public const string k_DefaultModuleName = "AudioSource";
		
		/// <summary> AudioSource reference </summary>
		public AudioSource Source;
		
		/// <summary> Check if this module has an AudioSource reference </summary>
		public bool hasSource => Source != null;

		/// <summary> Simple action to Play sound </summary>
		public SimpleModyAction Play;
		
		/// <summary> Simple action to Stop playing sound </summary>
		public SimpleModyAction Stop;
		
		/// <summary> Simple action to Mute sound </summary>
		public SimpleModyAction Mute;
		
		/// <summary> Simple action to Unmute sound </summary>
		public SimpleModyAction Unmute;
		
		/// <summary> Simple action to Pause playing sound </summary>
		public SimpleModyAction Pause;
		
		/// <summary> Simple action to Unpause playing sound </summary>
		public SimpleModyAction Unpause;
		
		/// <summary> Construct an AudioSource module with the default name </summary>
		public AudioSourceModule() : this(k_DefaultModuleName) { }

		/// <summary> Construct an AudioSource module with the given AudioSource reference </summary>
		/// <param name="audioSource"> AudioSource reference </param>
		public AudioSourceModule(AudioSource audioSource) : this(k_DefaultModuleName, audioSource) { }

		/// <summary> Construct an AudioSource module with the given module name and AudioSource reference </summary>
		/// <param name="moduleName"> Module name </param>
		/// <param name="audioSource"> AudioSource reference </param>
		public AudioSourceModule(string moduleName, AudioSource audioSource) : this(moduleName.IsNullOrEmpty() ? k_DefaultModuleName : moduleName)
		{
			Source = audioSource;
		}

		/// <summary> Construct an AudioSource module with the given module name </summary>
		/// <param name="moduleName"> Module name </param>
		public AudioSourceModule(string moduleName) : base(moduleName) { }

		/// <summary> Initialize the actions </summary>
		protected override void SetupActions()
		{
			this.AddAction(Play ??= new SimpleModyAction(this, nameof(Play), ExecuteSourcePlay));
			this.AddAction(Stop ??= new SimpleModyAction(this, nameof(Stop), ExecuteSourceStop));
			this.AddAction(Mute ??= new SimpleModyAction(this, nameof(Mute), ExecuteSourceMute));
			this.AddAction(Unmute ??= new SimpleModyAction(this, nameof(Unmute), ExecuteSourceUnmute));
			this.AddAction(Pause ??= new SimpleModyAction(this, nameof(Pause), ExecutePauseSource));
			this.AddAction(Unpause ??= new SimpleModyAction(this, nameof(Unpause), ExecuteSourceUnpause));
		}

		/// <summary> Execute Play on the AudioSource </summary>
		public void ExecuteSourcePlay()
		{
			if (!hasSource) return;
			Source.Play();
		}

		/// <summary> Execute Stop on the AudioSource </summary>
		public void ExecuteSourceStop()
		{
			if (!hasSource) return;
			Source.Stop();
		}
		
		/// <summary> Mute the AudioSource </summary>
		public void ExecuteSourceMute()
		{
			if (!hasSource) return;
			Source.mute = true;
		}
		
		/// <summary> Unmute the AudioSource </summary>
		public void ExecuteSourceUnmute()
		{
			if (!hasSource) return;
			Source.mute = false;
		}
		
		/// <summary> Execute Pause on the AudioSource </summary>
		public void ExecutePauseSource()
		{
			if (!hasSource) return;
			Source.Pause();
		}
		
		/// <summary> Execute UnPause on the AudioSource </summary>
		public void ExecuteSourceUnpause()
		{
			if (!hasSource) return;
			Source.UnPause();
		}
	}
}