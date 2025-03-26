using System.Collections.Generic;
using System.Linq;

namespace Application.Tools
{
    public class Tools
    {
        public static void Shuffle<T>(List<T> list) where T : class
        {
            var count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var item = list[i];
                var randomIndex = UnityEngine.Random.Range(0, count);
                list[i] = list[randomIndex];
                list[randomIndex] = item;
            }
        }

        public static void Shuffle<T>(Stack<T> stack) where T : class
        {
            var list = stack.ToList();
            Shuffle(list);
            stack.Clear();
            foreach (var item in list)
                stack.Push(item);
        }
    }
}