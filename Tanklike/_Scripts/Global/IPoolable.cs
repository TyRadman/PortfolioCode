using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;
using System;

namespace TankLike
{
    public interface IPoolable
    {
        Transform transform { get; }
        void Init(Action<IPoolable> OnRelease);
        void TurnOff();
        void OnRequest();
        /// <summary>
        /// Disposing the poolable once they're used to be ready to use again. Getting them ready to be used again.
        /// </summary>
        void OnRelease();
        /// <summary>
        /// When we no longer need this pool. Meaning this pool will probably never be used in this scene.
        /// </summary>
        void Clear();
    }
}
