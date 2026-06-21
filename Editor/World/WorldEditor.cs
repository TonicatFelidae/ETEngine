using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETEngine.Editor
{
    public class WorldEditorUtility
    {
        /// <summary>
        /// Creates or finds the World root GameObject in the scene.
        /// </summary>
        public static GameObject CreateWorld()
        {
            GameObject world = GameObject.Find("World");
            if (world == null)
            {
                world = new GameObject("World");
            }
            return world;
        }

        /// <summary>
        /// Creates a GameObject hierarchy under World using a path string (e.g., "parent/child/grandchild").
        /// Creates World and any missing intermediate parents as needed.
        /// </summary>
        public static GameObject CreateWorldObject(string path)
        {
            if (string.IsNullOrEmpty(path))
                return CreateWorld();

            string[] parts = path.Split('/');
            GameObject current = CreateWorld();

            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                Transform child = current.transform.Find(part);
                if (child == null)
                {
                    GameObject newObj = new GameObject(part);
                    newObj.transform.SetParent(current.transform);
                    current = newObj;
                }
                else
                {
                    current = child.gameObject;
                }
            }

            return current;
        }
    }
}