using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace QuoridorFullCalc
{
    public static class Program
    {
        // The possible statuses that each intersection may have.
        private const char Empty = ' ';       // Intersection is empty.
        private const char Vertical = '|';    // Intersection has a vertical wall.
        private const char Horizontal = '-';  // Intersection has a horizontal wall.

        private static void AddScenarios(IDictionary<string, List<BigInteger>> nextScenarios, IList<BigInteger> totals,
            string scenarioKey, IList<BigInteger> values, int spots, int offset)
        {
            if(!nextScenarios.ContainsKey(scenarioKey))
            {
                nextScenarios[scenarioKey] = Enumerable.Repeat(new BigInteger(0), values.Count).ToList();
            }
            for(var count = 0; count <= spots; ++count)
            {
                nextScenarios[scenarioKey][count + offset] += values[count];
                totals[count + offset] += values[count];
            }
        }

        private static void QuoridorCalc(int rows, int columns)
        {
            var additions = 0;
            var totalSpots = rows * columns;
            var nextScenarios = new Dictionary<string, List<BigInteger>>();

            // Starting counts indicates 1 way to place 0 walls, 0 ways to place any other walls.
            var startingScenario = new string(Empty, columns);
            var startingCounts = Enumerable.Repeat(new BigInteger(0), totalSpots + 1).ToList();
            startingCounts[0] = new BigInteger(1);
            nextScenarios[startingScenario] = startingCounts;

            Console.WriteLine(",Ways to place walls,,Walls");
            Console.Write("Spots,0-20 Walls,Total");
            for (var count = 0; count <= totalSpots; ++count)
            {
                Console.Write("," + count);
            }
            Console.WriteLine();

            // From the scenarios, calculate the next scenarios.
            for (var spot = 0; spot < totalSpots; ++spot)
            {
                var column = spot % columns;
                var currentScenarios = nextScenarios;
                nextScenarios = new Dictionary<string, List<BigInteger>>();
                var totals = Enumerable.Repeat(new BigInteger(0), totalSpots + 1).ToList();

                foreach (var scenarioKvp in currentScenarios)
                {
                    var scenario = scenarioKvp.Key;
                    var spotAbove = scenario[column];
                    var spotLeft = column > 0 ? scenario[column - 1] : Empty;
                    var newScenario = new StringBuilder(string.Copy(scenario));

                    // Leave current spot empty.
                    newScenario[column] = Empty;
                    var scenarioKey = newScenario.ToString();
                    AddScenarios(nextScenarios, totals, scenarioKey, scenarioKvp.Value, spot, 0);
                    additions += spot + 1;

                    // Place a vertical wall in the current spot.
                    if(spotAbove != Vertical)
                    {
                        newScenario[column] = Vertical;
                        scenarioKey = newScenario.ToString();
                        AddScenarios(nextScenarios, totals, scenarioKey, scenarioKvp.Value, spot, 1);
                        additions += spot + 1;
                    }

                    // Place a horizontal wall in the current spot.
                    if (spotLeft != Horizontal)
                    {
                        newScenario[column] = Horizontal;
                        scenarioKey = newScenario.ToString();
                        AddScenarios(nextScenarios, totals, scenarioKey, scenarioKvp.Value, spot, 1);
                        additions += spot + 1;
                    }
                }

                var total20 = new BigInteger(0);
                var total = new BigInteger(0);
                for(var walls = 0; walls <= totalSpots; ++ walls)
                {
                    if(walls <= 20)
                    {
                        total20 += totals[walls];
                    }
                    total += totals[walls];
                }
                Console.Write(spot + 1 + ",\"=\"\"" + total20.ToString("N0") + "\"\"\",\"=\"\"" + total.ToString("N0") + "\"\"\"");
                for(var walls = 0; walls <= spot + 1; ++walls)
                {
                    Console.Write(",\"=\"\"" + totals[walls].ToString("N0") + "\"\"\"");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.Write("\"=\"\"" + additions.ToString("N0") + " additions\"\"\"");
        }

        public static void Main(string[] args)
        {
            var rows = 8;
            var columns = 8;

            if((args.Length >= 1) && int.TryParse(args[0], out rows))
            {
                columns = rows;
                if(args.Length >= 2)
                {
                    int.TryParse(args[1], out rows);
                }
            }

            QuoridorCalc(rows, columns);
        }
    }
}
