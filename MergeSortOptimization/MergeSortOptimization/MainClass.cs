using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace MergeSortOptimization {
    class MainClass {
        private static readonly string resultsfilePath = "F:\\Arhitektura racunara projekti\\MergeSort_Optimization\\MergeSortOptimization\\AlgorithmResult.csv";
        private static readonly Random random = new Random();
        private static readonly Stopwatch sw = new Stopwatch();
        private static int[] standardSortArray;
        private static int[] PIArray;
        private static int[] TSArray;
        private static int[] COArray;
        private static int[] PICOArray;
        
        private static int[] QSArray;
        private static int[] PIQSArray;
        private static int[] TSQSArray;
        //private static int[] TWQSArray;
        private static int[] DPQSArray;
        private static int[] PIDPQSArray;


        static void Execute(uint arrayLenght) {

            Console.WriteLine("---------------------------------");
            Console.WriteLine("Number of elements: " + arrayLenght + "\n");
            List<int> randomList = new List<int>();

            for (int i = 0; i < arrayLenght; i++)
                randomList.Add(random.Next());

            standardSortArray = randomList.ToArray();
            PIArray = randomList.ToArray();
            TSArray = randomList.ToArray();
            COArray = randomList.ToArray();
            PICOArray = randomList.ToArray();
            PIDPQSArray = randomList.ToArray();
            QSArray = randomList.ToArray();
            PIQSArray = randomList.ToArray();
            TSQSArray = randomList.ToArray();
          //  TWQSArray = randomList.ToArray();
            DPQSArray = randomList.ToArray();

            Console.WriteLine("{0,-50} {1,5}\n", "Algorithm", "Seconds");
            List<string> algorithmNames = new List<string>();
            List<double> algorithmTimes = new List<double>();


            sw.Start();
            Sort.StandardMergeSort(standardSortArray);
            sw.Stop();
            algorithmNames.Add("Standard Merge Sort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(standardSortArray))
                Console.WriteLine("Standard Mege Sort failed!!!");

            sw.Reset();
            sw.Start();
            Sort.ParallelInvokeMergeSort(PIArray);
            sw.Stop();
            algorithmNames.Add("Parallel Invoke Merge Sort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(PIArray))
                Console.WriteLine("Parallel Invoke Merge Sort failed!!!");

            sw.Reset();
            sw.Start();
            Sort.TaskMergeSort(TSArray);
            sw.Stop();
            algorithmNames.Add("Task  Merge Sort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(TSArray))
                Console.WriteLine("Task  Merge Sort failed!!!");


            sw.Reset();
            sw.Start();
            Sort.CacheOptimizedMergeSort(COArray);
            sw.Stop();
            algorithmNames.Add("Cache optimized  Merge Sort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(COArray))
                Console.WriteLine("Cache optimized  Merge Sort failed!!!");


            sw.Reset();
            sw.Start();
            Sort.ParallelInvokeCacheOptimizedMergeSort(PICOArray);
            sw.Stop();
            algorithmNames.Add("Parallel Invoke Cache optimized Merge Sort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(PICOArray))
                Console.WriteLine("Parallel Invoke Cache optimized Merge Sort failed!!!");

            sw.Reset();
            sw.Start();
            Sort.StandardQuickSort(QSArray);
            sw.Stop();
            algorithmNames.Add("Standard QuickSort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));

            if (!IsSorted(QSArray))
                Console.WriteLine("Standard QuickSort failed!!!");

            sw.Reset();
            sw.Start();
            Sort.ParallelInvokeQuickSort(PIQSArray);
            sw.Stop();
            algorithmNames.Add("ParallelInvoke QuickSort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(PIQSArray))
                Console.WriteLine("ParallelInvoke QuickSort failed!!!");

            sw.Reset();
            sw.Start();
            Sort.TaskQuickSort(TSQSArray);
            sw.Stop();
            algorithmNames.Add("Task QuickSort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(TSQSArray))
                Console.WriteLine("Task QuickSort failed!!!");

           /* sw.Reset();
            sw.Start();
            Sort.ThreeWayQuickSort(TWQSArray);
            sw.Stop();
            algorithmNames.Add("ThreeWay QuickSort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(TWQSArray))
                Console.WriteLine("ThreeWay QuickSort failed!!!");*/

            sw.Reset();
            sw.Start();
            Sort.DualPivotQuickSort(DPQSArray);
            sw.Stop();
            algorithmNames.Add("DualPivot QuickSort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(DPQSArray))
                Console.WriteLine("DualPivot QuickSort failed!!!");

            sw.Reset();
            sw.Start();
            Sort.ParallelInvokeDualPivotQuickSort(PIDPQSArray);
            sw.Stop();
            algorithmNames.Add("ParallelInvoke DualPivot QuickSort");
            algorithmTimes.Add((sw.ElapsedMilliseconds / 1000.0));
            if (!IsSorted(PIDPQSArray))
                Console.WriteLine("ParallelInvokeDualPivotQuickSort failed!!!");


            using (var w = new StreamWriter(resultsfilePath, append: true)) {
                var firstLine = string.Format("{0},{1}", "Number of elements", arrayLenght);
                w.WriteLine(firstLine);
                w.Flush();
                for (int ctr = 0; ctr < algorithmNames.Count; ctr++) {
                    Console.WriteLine("{0,-50} {1,5:N5}", algorithmNames[ctr], algorithmTimes[ctr]);
                    var line = string.Format("{0},{1}", algorithmNames[ctr], algorithmTimes[ctr]);
                    w.WriteLine(line);
                    w.Flush();
                }
                w.WriteLine(" , ");
                w.WriteLine(" , ");
                w.Flush();
            }

            Console.WriteLine();
        }


        public static bool IsSorted(int[] array) {
            for (int i = 0; i < array.Length - 1; i++)
                if (array[i] > array[i + 1])
                    return false;
            return true;
        }


        public static void Main(String[] args) {
            File.Delete(resultsfilePath);

            Execute(1_000_000);
            Execute(3_000_000);
            Execute(5_000_000);
            Execute(10_000_000);
            Execute(30_000_000);

        }

    }
}
