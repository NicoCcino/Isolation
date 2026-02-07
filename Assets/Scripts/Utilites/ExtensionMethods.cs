using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class ExtensionMethods
{
    public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
    {
        var tcs = new TaskCompletionSource<object>();
        asyncOp.completed += obj => { tcs.SetResult(null); };
        return ((Task)tcs.Task).GetAwaiter();
    }

    public static void Shuffle<T>(this Stack<T> stack)
    {
        Shuffle<T>(stack, 0);
    }

    public static void Shuffle<T>(this Stack<T> stack, int seed)
    {
        List<T> list = stack.ToList();
        list.Shuffle(seed);
        stack = list.ToStack();
    }

    public static void Shuffle<T>(this List<T> list)
    {
        Shuffle<T>(list, 0);
    }

    public static void Shuffle<T>(this List<T> list, int seed)
    {
        for (int i = 0; i < list.Count; i++)
        {
            System.Random rnd;

            if (seed == 0) rnd = new System.Random();
            else rnd = new System.Random(seed);
            int num = rnd.Next(list.Count);
            T temp = list[i];
            list[i] = list[num];
            list[num] = temp;
        }
    }

    public static Stack<T> ToStack<T>(this List<T> list)
    {
        Stack<T> stack = new Stack<T>();
        foreach (T t in list)
            stack.Push(t);

        return stack;
    }

    private static IEnumerator __ExecAtNextFrame(UnityEngine.Events.UnityAction call)
    {
        yield return null;
        call?.Invoke();
    }

    /// <summary>
    /// Call the delegate call at the end of frame
    /// </summary>
    /// <param name="call"></param>
    public static void ExecuteAtNextFrame(this MonoBehaviour monoBehaviour, UnityEngine.Events.UnityAction call)
    {
        monoBehaviour.StartCoroutine(__ExecAtNextFrame(call));
    }
}
