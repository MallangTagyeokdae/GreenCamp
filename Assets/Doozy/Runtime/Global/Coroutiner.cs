// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable UnusedMember.Global
// ReSharper disable Unity.IncorrectMethodSignature
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Global
{
    /// <inheritdoc />
    /// <summary> Special class used to run Coroutines on. When using any of its public static methods, it will instantiate itself and run any number of coroutines </summary>
    public class Coroutiner : SingletonBehaviour<Coroutiner>
    {
        /// <summary>
        /// Start a Coroutine.
        /// Coroutine will run on the Coroutiner GameObject
        /// </summary>
        /// <param name="enumerator"> The enumerator </param>
        public Coroutine StartLocalCoroutine(IEnumerator enumerator) =>
            StartCoroutine(enumerator);

        /// <summary> Stop the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="coroutine"> The coroutine </param>
        public void StopLocalCoroutine(Coroutine coroutine) =>
            StopCoroutine(coroutine);

        /// <summary> Stop the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="enumerator"> The enumerator </param>
        public void StopLocalCoroutine(IEnumerator enumerator) =>
            StopCoroutine(enumerator);

        /// <summary> Stop all Coroutines running on this behaviour </summary>
        public void StopAllLocalCoroutines() =>
            StopAllCoroutines();

        /// <summary>
        /// Start a Coroutine.
        /// Coroutine will run on the Coroutiner GameObject
        /// </summary>
        /// <param name="enumerator"> Target enumerator </param>
        public static Coroutine Start(IEnumerator enumerator) =>
            instance != null && enumerator != null
                ? instance.StartLocalCoroutine(enumerator)
                : null;

        /// <summary> Stop the first Coroutine named methodName, or the Coroutine stored in routine running on Coroutiner </summary>
        /// <param name="enumerator"> Target enumerator </param>
        public static void Stop(IEnumerator enumerator)
        {
            if (instance == null || enumerator == null) return;
            instance.StopLocalCoroutine(enumerator);
        }

        /// <summary> Stop the first Coroutine named methodName, or the Coroutine stored in routine running on Coroutiner </summary>
        /// <param name="coroutine"> The coroutine </param>
        public static void Stop(Coroutine coroutine)
        {
            if (instance == null || coroutine == null) return;
            instance.StopLocalCoroutine(coroutine);
        }

        /// <summary> Stop all Coroutines running on Coroutiner </summary>
        public static void StopAll()
        {
            if (instance == null) return;
            instance.StopAllLocalCoroutines();
        }

        /// <summary>
        /// Execute the given callback after the set time delay (in seconds).
        /// Coroutine will run on the Coroutiner GameObject
        /// </summary>
        /// <param name="callback"> Callback </param>
        /// <param name="delay"> Time delay in seconds </param>
        public static Coroutine ExecuteLater(UnityAction callback, float delay) =>
            Start(DelayExecution(callback, delay));

        /// <summary>
        /// Execute the given callback after the set number of frames has passed.
        /// Coroutine will run on the Coroutiner GameObject
        /// </summary>
        /// <param name="callback"> Callback </param>
        /// <param name="numberOfFrames"> Number of frames to wait until the callback is executed </param>
        public static Coroutine ExecuteLater(UnityAction callback, int numberOfFrames) =>
            Start(DelayExecution(callback, numberOfFrames));

        /// <summary> Delay callback execution with for set time duration (in seconds) </summary>
        /// <param name="callback"> Callback </param>
        /// <param name="delay"> Time delay in seconds </param>
        public static IEnumerator DelayExecution(UnityAction callback, float delay)
        {
            delay = delay < 0 ? 0 : delay;                  //sanity check
            yield return new WaitForSecondsRealtime(delay); //wait
            callback?.Invoke();                             //execute
        }

        /// <summary> Delay callback execution with for set number of frames </summary>
        /// <param name="callback"> Callback </param>
        /// <param name="numberOfFrames"> Number of frames to wait until the callback is executed </param>
        public static IEnumerator DelayExecution(UnityAction callback, int numberOfFrames)
        {
            numberOfFrames = numberOfFrames < 0 ? 0 : numberOfFrames; //sanity check
            while (numberOfFrames > 0)                                //wait
            {
                yield return null;
                numberOfFrames--;
            }
            callback?.Invoke(); //execute
        }
    }
}
