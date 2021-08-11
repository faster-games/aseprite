using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace FasterGames.Aseprite.Editor
{
    /// <summary>
    /// Internal editor utilities
    /// </summary>
    internal static class EditorUtils
    {   
        /// <summary>
        /// <see cref="EditorApplication"/> task runner
        /// </summary>
        /// <remarks>
        /// This allows us to schedule work to be run once on Editor update
        /// </remarks>
        /// <param name="cb">task to run</param>
        public static void Once(Action cb)
        {
            OnceTasks.Enqueue(cb);
            EditorApplication.update += OnceRunner;
        }

        /// <summary>
        /// Storage for tasks. See <see cref="Once"/>
        /// </summary>
        private static readonly Queue<Action> OnceTasks = new Queue<Action>();

        /// <summary>
        /// Worker job that runs on Editor update
        /// </summary>
        /// <remarks>
        /// This checks if any jobs need to be run, and if so, runs them.
        /// </remarks>
        private static void OnceRunner()
        {
            while (OnceTasks.Count > 0)
            {
                OnceTasks.Dequeue()();
            }

            EditorApplication.update -= OnceRunner;
        }
    }
}