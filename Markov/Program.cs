using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markov
{
    class Program
    {
        static Random _rnd = new Random();
        static Dictionary<Tuple<string, string>, List<string>> _graph = new Dictionary<Tuple<string, string>, List<string>>();

        static void Acquire (Stream stream)
        {
            using (var sr = new StreamReader(stream))
                Acquire(sr.ReadToEnd());
        }

        static void Acquire (string text)
        {
            string[] words = text.Split(new char[] { ' ', '\t', '\n', '\r' });

            for (int i = 1; i < words.Length - 2; i += 1)
            {
                var key = Tuple.Create (words[i], words[i + 1]);

                List<string> list;
                if (!_graph.TryGetValue(key, out list))
                {
                    list = new List<string>();
                    _graph[key] = list;
                }

                list.Add(words[i + 2]);
            }

            foreach (var list in _graph.Values)
                list.Sort();
        }

        static IEnumerable<string> Generate (int size)
        {
            Tuple<string, string> current = null;

            while (current == null)
            {
                current = _graph.Keys
                                .Skip(_rnd.Next(0, _graph.Keys.Count - 1))
                                .FirstOrDefault(t => t.Item1.EndsWith("."));
            }

            yield return current.Item2;

            for (int i = 0; i < size; i++)  
            {
                List<string> words = _graph[current];
                string word = words[_rnd.Next (words.Count - 1)];

                yield return word;

                current = Tuple.Create(current.Item2, word);
            }
        }

        static void Main(string[] args)
        {
            Acquire(File.OpenRead(@"quijote.txt"));
            Console.WriteLine(string.Join(" ", Generate(150).ToArray()));
            Console.ReadKey ();
        }
    }
}
