using System.Collections.Generic;
using System.Linq;
using SoulCollector;
using UnityEngine;

namespace John {

    public static class Algorithms {

        /// <summary>
        /// Takes in an array of integers and sorts them in ascending order using the Bubble Sort method.
        /// </summary>
        /// <param name="array">The array to be sorted.</param>
        /// <returns>The sorted array.</returns>
        public static int[] BubbleSort(int[] array) {

            int n = array.Length;
            bool swapped = false;

            // Loop for n times (the length of the array).
            for (int i = 0; i < n - 1; i++) {

                // At the start of each sort loop, reset swapped to false.
                swapped = false;

                // Iterate through the array and compare each element with the adjacent element.
                for (int j = 0; j < n - 1; j++) {

                    // If the current element is larger than the next, swap them and set swapped
                    // to true.
                    if (array[j] > array[j + 1]) {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                        swapped = true;
                    }
                }

                // If no values were swapped in the last iteration, the collection has been sorted
                // and we can end.
                if (!swapped) break;
            }

            return array;

        }

        /// <summary>
        /// Takes in a List of integers and sorts them into ascending order using the Insertion Sort method.
        /// </summary>
        /// <param name="list">List of ints to be sorted.</param>
        /// <returns>Sorted list of ints.</returns>
        public static List<int> InsertionSort(List<int> list) {

            int n = list.Count;

            // The first list item has nothing before it, so we start iteration from the second item.
            for (int i = 1; i < n; i++) {

                // Cache the current iteration's value and the initialise the inner loops iterator as our
                // current iteration minus 1.
                int key = list[i];
                int j = i - 1;

                // Loop until either the inner loop iterator is at zero, or the list item being checked is
                // greater than the key we cached above.
                while (j >= 0 && list[j] > key) {

                    // If the current item is less than the key, move it up one and decrement the inner loop
                    // iterator.
                    list[j + 1] = list[j];
                    j--;

                }

                // Once we've reached the end of the inner loop, set the key to our last value before moving on.
                list[j + 1] = key;
            }

            return list;

        }

        /// <summary>
        /// Converts an array of integers into a comma delimited string.
        /// </summary>
        /// <param name="array">An array of integers.</param>
        /// <returns>A string containing the contents of the array, comma delimited.</returns>
        public static string ArrayToString(int[] array) => string.Join(", ", array);

        public static Tile[] BubbleSort(Dictionary<Vector2Int, Tile> tiles) {

            Tile[] array = new Tile[tiles.Count];
            tiles.Values.CopyTo(array, 0);
            int n = array.Length;
            bool swapped = false;

            // Loop for n times (the length of the array).
            for (int i = 0; i < n - 1; i++) {

                // At the start of each sort loop, reset swapped to false.
                swapped = false;

                // Iterate through the array and compare each element with the adjacent element.
                for (int j = 0; j < n - 1; j++) {

                    // If the current element is larger than the next, swap them and set swapped
                    // to true.
                    if (array[j].Health > array[j + 1].Health) {
                        Tile temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                        swapped = true;
                    }
                }

                // If no values were swapped in the last iteration, the collection has been sorted
                // and we can end.
                if (!swapped) break;
            }

            return array;

        }

        public static List<Tile> InsertionSort(Dictionary<Vector2Int, Tile> tiles) {

            List<Tile> list = tiles.Values.ToList<Tile>();

            int n = list.Count;

            // The first list item has nothing before it, so we start iteration from the second item.
            for (int i = 1; i < n; i++) {

                // Cache the current iteration's value and the initialise the inner loops iterator as our
                // current iteration minus 1.
                Tile key = list[i];
                int j = i - 1;

                // Loop until either the inner loop iterator is at zero, or the list item being checked is
                // greater than the key we cached above.
                while (j >= 0 && list[j].Health > key.Health) {

                    // If the current item is less than the key, move it up one and decrement the inner loop
                    // iterator.
                    list[j + 1] = list[j];
                    j--;

                }

                // Once we've reached the end of the inner loop, set the key to our last value before moving on.
                list[j + 1] = key;
            }

            return list;

        }

    }

}