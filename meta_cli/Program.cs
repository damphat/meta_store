using meta_store;
using System;
using System.Diagnostics;

class Program
{
    static long Perf(Action action)
    {
        var sw = new Stopwatch();
        sw.Start();
        int count = 0;
        while (sw.ElapsedTicks < TimeSpan.TicksPerSecond)
        {
            action();
            count++;
        }
        sw.Stop();
        Console.WriteLine($"Run {count} times in {sw.ElapsedMilliseconds} ms");
        return count;
    }

    static void Main(string[] args)
    {
        var a = Sigo.Freeze(Sigo.Parse(@"
            {
              user: {
                name: {
                  first: 'phat',
                  last: 'dam'
                },
                scores/1: [1,2,3,4,5,6,7,8,9],
                scores/2: [1,2,3,4,5,6,7,8,9]
                scores/3: [1,2,3,4,5,6,7,8,9]
              }
            }
        "));

        var b = Sigo.Freeze(Sigo.Parse(@"
            {0
              user: {0
                name: {0      
                  first: 'fat'
                  full: 'dam fat'
                },
                scores/1: [1,2,3,4,5,6,7,8,9,10],
                scores/2: {3}
                scores/3: {3, 2: {0}, 4: {0}, 6: {0}, 8: {0}}
              }
            }
            "
        ));

        Perf(() => Sigo.Freeze(Sigo.Parse(@"
            {0
              user: {0
                name: {0      
                  first: 'fat'
                  full: 'dam fat'
                },
                scores/1: [1,2,3,4,5,6,7,8,9,10],
                scores/2: {3}
                scores/3: {3, 2: {0}, 4: {0}, 6: {0}, 8: {0}}
              }
            }
        ")));

        Perf(() => Sigo.Merge(a, b));

        Console.WriteLine(Sigo.ToString(Sigo.Merge(a, b)));
    }
}

