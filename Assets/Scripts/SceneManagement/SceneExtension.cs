using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace WiDiD.SceneManagement
{
    public static class SceneExtensions
    {
        public static List<T> FindAllObjectsOfTypeInScene<T>(Scene scene, int depth) where T : Component
        {
            List<T> results = new List<T>();
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                FindAllObjectsOfTypeInGameObject(rootGameObject, depth, results);
            }
            return results;
        }

        private static void FindAllObjectsOfTypeInGameObject<T>(GameObject gameObject, int depth, List<T> results) where T : Component
        {
            if (depth < 0) return;

            T component = gameObject.GetComponent<T>();
            if (component != null)
            {
                results.Add(component);
            }

            foreach (Transform child in gameObject.transform)
            {
                FindAllObjectsOfTypeInGameObject(child.gameObject, depth - 1, results);
            }
        }
    }
}