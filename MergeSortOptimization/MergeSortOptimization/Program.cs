using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MergeSortOptimization {

    static class Sort {

        public static int Depth { get; set; }
        //maksimalni broj niti za paralelizaciju
        public static int PROCESSOR_COUNT = Environment.ProcessorCount;

        //Spajanje dva niza, prvi je arr[l..m], drugi je arr[m+1..r]
        private static void Merge(int[] arr, int left, int m, int right) {
            
            //velicine dva podniza koji se spajaju
            int firstSubarraySize = m - left + 1;
            int secondSubarraySize = right - m;

            //temp nizovi
            int[] L = new int[firstSubarraySize];
            int[] R = new int[secondSubarraySize];
            int firstSubarrayCounter, secondSubarrayCounter;

            //kopiranje u temp nizove
            for (firstSubarrayCounter = 0; firstSubarrayCounter < firstSubarraySize; ++firstSubarrayCounter)
                L[firstSubarrayCounter] = arr[left + firstSubarrayCounter];
            for (secondSubarrayCounter = 0; secondSubarrayCounter < secondSubarraySize; ++secondSubarrayCounter)
                R[secondSubarrayCounter] = arr[m + 1 + secondSubarrayCounter];

           //spajanje temp nizova

          //pocetni indeksi prvog i drugog niza
            firstSubarrayCounter = 0;
            secondSubarrayCounter = 0;

            
            int k = left;
            while (firstSubarrayCounter < firstSubarraySize && secondSubarrayCounter < secondSubarraySize) {
                if (L[firstSubarrayCounter] <= R[secondSubarrayCounter]) {
                    arr[k] = L[firstSubarrayCounter];
                    firstSubarrayCounter++;
                }
                else {
                    arr[k] = R[secondSubarrayCounter];
                    secondSubarrayCounter++;
                }
                k++;
            }

            //kopiraj ostatak L[]
            while (firstSubarrayCounter < firstSubarraySize) {
                arr[k] = L[firstSubarrayCounter];
                firstSubarrayCounter++;
                k++;
            }

            //Kopiraj ostatak R[]
            while (secondSubarrayCounter < secondSubarraySize) {
                arr[k] = R[secondSubarrayCounter];
                secondSubarrayCounter++;
                k++;
            }
        }


        public static void StandardMergeSort(int[] array) {
            StandardMergeSort(array, 0, array.Length - 1);
        }
        private static void StandardMergeSort(int[] arr, int l, int r) {
            if (l < r) {
                
                //sredina
                int m = l + (r - l) / 2;

                //sortiraj prvi i drugi dio
                StandardMergeSort(arr, l, m);
                StandardMergeSort(arr, m + 1, r);

                // spoji dijelove
                Merge(arr, l, m, r);
            }
        }

        
        public static void ParallelInvokeMergeSort(int[] array) {
            Depth = PROCESSOR_COUNT;
            ParallelInvokeSort(array, 0, array.Length - 1);
        }
        private static void ParallelInvokeSort(int[] array, int left, int right) {
            if (left < right) {
                

                int m = left + (right - left) / 2;

                //Dok postoje "slobodne" niti, posao se rasporedjuje na njih
                if (Depth > 0) {
                    Depth -= 1;
                    Parallel.Invoke(
                 () => ParallelInvokeSort(array, left, m),
                 () => ParallelInvokeSort(array, m + 1, right));
                }
                //ako nema slobodnih niti, posao se obavlja serijski
                else {
                    StandardMergeSort(array, left, m);
                    StandardMergeSort(array, m + 1, right);
                }

                Merge(array, left, m, right);
            }
        }

        public static void TaskMergeSort(int[] array) {
            Depth = PROCESSOR_COUNT;
            TaskSort(array, 0, array.Length - 1);
        }
        private static void TaskSort(int[] array, int left, int right) {
            if (left < right) {
                
                int m = left + (right - left) / 2;

                //Dok postoje "slobodne" niti, posao se rasporedjuje na njih
                if (Depth > 0) {
                    Depth -= 1;
                    List<Task> tasks = new List<Task> {
                        Task.Factory.StartNew(() => TaskSort(array, left, m)),
                        Task.Factory.StartNew(() => TaskSort(array, m+1, right))
                    };

                    Task.WaitAll(tasks.ToArray());
                }
                //ako nema slobodnih niti, posao se obavlja serijski
                else {
                    StandardMergeSort(array, left, m);
                    StandardMergeSort(array, m + 1, right);
                }

                Merge(array, left, m, right);
            }
        }

        public static void CacheOptimizedMergeSort(int[] array) {
            int[] tempArray = new int[array.Length];
            CacheOptimizedSort(array, tempArray, 0, array.Length - 1);
        }
        private static void CacheOptimizedSort(int[] array, int[] tempArray, int left, int right) {

            int center;
            if (left < right) {

                center = (left + right) / 2;
                CacheOptimizedSort(array, tempArray, left, center);
                CacheOptimizedSort(array, tempArray, center + 1, right);
                CacheOptimizedMerge(array, tempArray, left, center + 1, right);
            }
        }

        private static void CacheOptimizedMerge(int[] array, int[] tempArray, int left, int right, int rightEnd) {

            int i, leftEnd, tempPos;
            leftEnd = right - 1;
            tempPos = left;
            int numOfElements = rightEnd - left + 1;


            while ((left <= leftEnd) && (right <= rightEnd))

                if (array[left] <= array[right])
                    tempArray[tempPos++] = array[left++];
                else
                    tempArray[tempPos++] = array[right++];

            //kopiranje ostatka prvog dijela
            while (left <= leftEnd)  
                tempArray[tempPos++] = array[left++];

            //kopiranje ostatka drugog dijela
            while (right <= rightEnd) 
                tempArray[tempPos++] = array[right++];

            //kopiraj tempArray u array
            for (i = 1; i <= numOfElements; i++, rightEnd--)
                array[rightEnd] = tempArray[rightEnd];

        }

        //kombinacija cache optimizacije sa Parallel.Invoke
        public static void ParallelInvokeCacheOptimizedMergeSort(int[] array) {
            Depth = PROCESSOR_COUNT;
            int[] tempArray = new int[array.Length];
            ParallelInvokeCacheOptimizedSort(array, tempArray, 0, array.Length - 1);
        }
        private static void ParallelInvokeCacheOptimizedSort(int[] array, int[] tempArray, int left, int right) {

            int center;
            if (left < right) {

                center = (left + right) / 2;

                if (Depth > 0) {
                    Depth -= 1;
                    Parallel.Invoke(
                        () => ParallelInvokeCacheOptimizedSort(array, tempArray, left, center),
                        () => ParallelInvokeCacheOptimizedSort(array, tempArray, center + 1, right)
                        );
                }
                else {
                    CacheOptimizedSort(array, tempArray, left, center);
                    CacheOptimizedSort(array, tempArray, center + 1, right);
                }
                CacheOptimizedMerge(array, tempArray, left, center + 1, right);
            }
        }

        //Quick Sort
        private static int Partition(int[] array, int left, int right) {
            int midPos = left + (right - left) / 2;
            int pivot = array[midPos];
            Swap(midPos, right - 1, array);
            int newPivot = left;
            for (int i = left; i < right - 1; i++)
                if (array[i].CompareTo(pivot) != 1) {
                    Swap(newPivot, i, array);
                    newPivot++;
                }

            Swap(newPivot, right - 1, array);
            return newPivot;
        }


        public static void StandardQuickSort(int[] array) {
            StandardQuickSort(array, 0, array.Length);
        }
        private static void StandardQuickSort(int[] arr, int left, int right) {
            int pivot;
            if (left < right) {
                //nadji pivot-a
                pivot = Partition(arr, left, right);
                //primjeni postupak na dobijenim dijelovima koje razdvaja pivot
                    StandardQuickSort(arr, left, pivot);
                    StandardQuickSort(arr, pivot + 1, right);
            }
        }

        public static void ParallelInvokeQuickSort(int[] array) {
            Depth = PROCESSOR_COUNT;
            ParallelQuickSort(array, 0, array.Length);
        }

        private static void ParallelQuickSort(int[] array, int left, int right) {
            int pivot;
            if (left < right) {

                pivot = Partition(array, left, right);

                if (Depth > 0) {
                    Depth -= 1;
                    Parallel.Invoke(
                        () => ParallelQuickSort(array, left, pivot),
                        () => ParallelQuickSort(array, pivot + 1, right));
                }
                else {
                    StandardQuickSort(array, left, pivot);
                    StandardQuickSort(array, pivot + 1, right);
                }

            }
        }

        public static void TaskQuickSort(int[] array) {
            Depth = PROCESSOR_COUNT;
            TaskQuickSort(array, 0, array.Length);
        }

        private static void TaskQuickSort(int[] array, int left, int right) {

            int pivot;
            if (left < right) {

                pivot = Partition(array, left, right);

                if (Depth > 0) {
                    Depth -= 1;
                    List<Task> tasks = new List<Task> {
                        Task.Factory.StartNew(() => TaskQuickSort(array, left, pivot)),
                        Task.Factory.StartNew(() => TaskQuickSort(array, pivot+1, right))
                    };

                    Task.WaitAll(tasks.ToArray());
                }
                else {
                    StandardQuickSort(array, left, pivot);
                    StandardQuickSort(array, pivot + 1, right);
                }

            }
        }

        //Zakomentarisan jer nakon vise testiranja, algoritam ne pokazuje znacajnu optmizaciju
       /* public static void ThreeWayQuickSort(int[] array) {
            ThreeWayQuickSort(array, 0, array.Length - 1);
        }

        private static void ThreeWayQuickSort(int[] array, int left, int right) {
            if (left < right) {

                int[] pivotPos = ThreeWayPartition(array, left, right);
                ThreeWayQuickSort(array, left, pivotPos[0] - 1);
                ThreeWayQuickSort(array, pivotPos[1] + 1, right);
            }
        }

        private static int[] ThreeWayPartition(int[] input, int lowIndex, int highIndex) {

            int lt = lowIndex;
            int gt = highIndex;
            int i = lowIndex + 1;

            int pivotIndex = lowIndex;
            int pivotValue = input[pivotIndex];


            while (i <= gt) {

                if (input[i] < pivotValue) {
                    Swap(i++, lt++, input);
                }
                else if (pivotValue < input[i]) {
                    Swap(i, gt--, input);
                }
                else {
                    i++;
                }

            }
            return new int[] { lt, gt };
        }*/


        private static void Swap<T>(int firstPos, int secondPos, T[] array) {
            T temp = array[firstPos];
            array[firstPos] = array[secondPos];
            array[secondPos] = temp;
        }

        //Dual pivot optimizacija
        public static void DualPivotQuickSort(int[] array) {
            DualPivotQuickSort(array, 0, array.Length - 1);
        }


        private static void DualPivotQuickSort(int[] array, int left, int right) {

            if (left < right) {
                int pivot1 = array[left];
                int pivot2 = array[right];


                if (pivot1 > pivot2) {
                    Swap(left, right, array);
                    pivot1 = array[left];
                    pivot2 = array[right];
                }
                
                int i = left + 1;
                int lt = left + 1;
                int gt = right - 1;

                while (i <= gt) {

                    if (array[i] < pivot1) {
                        Swap(i++, lt++, array);
                    }
                    else if (pivot2 < array[i]) {
                        Swap(i, gt--, array);
                    }
                    else {
                        i++;
                    }
                }

                Swap(left, --lt, array);
                Swap(right, ++gt, array);

                DualPivotQuickSort(array, left, lt - 1);
                if (array[lt] < array[gt]) DualPivotQuickSort(array, lt + 1, gt - 1);
                DualPivotQuickSort(array, gt + 1, right);

            }
        }

        //Parallel.Invoke + DulaPivot optimizacija
        public static void ParallelInvokeDualPivotQuickSort(int[] array) {
            Depth = PROCESSOR_COUNT;
            ParallelInvokeDualPivotQuickSort(array, 0, array.Length - 1);
        }

        private static void ParallelInvokeDualPivotQuickSort(int[] array, int left, int right) {

            if (left < right) {
                int pivot1 = array[left];
                int pivot2 = array[right];


                if (pivot1 > pivot2) {
                    Swap(left, right, array);
                    pivot1 = array[left];
                    pivot2 = array[right];
                }

                int i = left + 1;
                int lt = left + 1;
                int gt = right - 1;

                while (i <= gt) {

                    if (array[i] < pivot1) {
                        Swap(i++, lt++, array);
                    }
                    else if (pivot2 < array[i]) {
                        Swap(i, gt--, array);
                    }
                    else {
                        i++;
                    }
                }

                Swap(left, --lt, array);
                Swap(right, ++gt, array);


                if(Depth > 0) {
                    Depth -= 3;
                    Parallel.Invoke(
                        () => ParallelInvokeDualPivotQuickSort(array, left, lt - 1),
                        () => { if (array[lt] < array[gt]) ParallelInvokeDualPivotQuickSort(array, lt + 1, gt - 1); },
                        () => ParallelInvokeDualPivotQuickSort(array, gt + 1, right)
                        );
                }
                else {
                    DualPivotQuickSort(array, left, lt - 1);
                    if (array[lt] < array[gt]) DualPivotQuickSort(array, lt + 1, gt - 1);
                    DualPivotQuickSort(array, gt + 1, right);
                }

            }
        }

    }
}

