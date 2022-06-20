//---------------------------------------------------------------------//
//                    GNU GENERAL PUBLIC LICENSE                       //
//                       Version 2, June 1991                          //
//                                                                     //
// Copyright (C) Wells Hsu, wellshsu@outlook.com, All rights reserved. //
// Everyone is permitted to copy and distribute verbatim copies        //
// of this license document, but changing it is not allowed.           //
//                  SEE LICENSE.md FOR MORE DETAILS.                   //
//---------------------------------------------------------------------//
using System;
using System.Threading;
using UnityEngine;
using System.Collections;

namespace EP.U3D.LIBRARY.BASE
{
    public class Loom : MonoBehaviour
    {
        private static int maxThreads = 8;
        private static int numThreads;
        private static int mainThreadID;
        private Queue actions = new Queue();
        public static Loom Instance;

        void Awake()
        {
            Instance = this;
            mainThreadID = Thread.CurrentThread.ManagedThreadId;
            actions = Queue.Synchronized(actions);
        }

        void Update()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                object action = actions.Dequeue();
                if (action != null && action is Action)
                {
                    (action as Action)();
                }
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public static void Initialize(Transform root)
        {
            if (Instance == null)
            {
                var go = new GameObject("Loom");
                if (root) go.transform.SetParent(root);
                go.AddComponent<Loom>();
            }
        }

        public static Coroutine StartCR(IEnumerator cr)
        {
            return Instance.StartCoroutine(cr);
        }

        public static void StopCR(Coroutine cr_ret)
        {
            Instance.StopCoroutine(cr_ret);
        }

        public static bool IsInMainThread() { return Thread.CurrentThread.ManagedThreadId == mainThreadID; }

        public static void QueueInMainThread(Action action)
        {
            Instance.actions.Enqueue(action);
        }

        public static Thread RunAsync(Action action)
        {
            while (numThreads >= maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                try
                {
                    ((Action)obj)();
                }
                catch
                {
                }
                finally
                {
                    Interlocked.Decrement(ref numThreads);
                }
            }, action);
            return null;
        }
    }
}
