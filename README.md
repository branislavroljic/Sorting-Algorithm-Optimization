# Sorting-Algorithm-Optimization

The MergeSort and QuickSort algorithms were optimized in the C and C # programming languages. Due to the limitations of the VisualStudio framework, optimization using multicore processor parallelization (OpenMP) was implemented in the C programming language, the program was compiled and run in LinuxOS (testing was performed in a virtual machine) while all other optimizations were implemented in C # in VisualStudio.

_Parallel Invoke Merge Sort_ optimization implies parallelization of Merge Sort algorithm using Parallel.Invoke methods of C # programming language.

_Task Merge Sort_ involves optimizing the Merge Sort algorithm using the Task.Factory.StartNew () method of the C # programming language.

Both of these optimizations allocate part of the work to more (hardware) threads, and thus significantly speed up the execution of the algorithm, compared to serial execution.

As the & quot; depth & quot; up to which the optimization is performed (ie the maximum number of threads on which the work is allocated) the number of physical cores was used.

_Cache optimized MergeSort_ represents cache optimization. After testing and graphical representation using Valgrind and KCachegrind tools, cache optimization (although constant) is not significant.

<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/first.png">

The following figure shows how the Cache optimized Merge Sort algorithm works. (Taken from Stack overflow)

<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/second.png">

_Parallel Invoke Cache Optimized Merge Sort_ is a combination of the two optimization methods described above. Sorting time is significantly reduced, as expected.

In addition to Merge Sort optimization, the QuickSort algorithm was also optimized. The main goal is to demonstrate cache optimization, which is represented by the _Dual Pivot Quick Sort_ optimization algorithm, while _Parallel Invoke and Task Quick Sort_ are listed for comparison and the main idea is the same as with Merge Sort.

_ParallelInvoke DualtPivot QuickSort_ is a combination of DualPivot (optimization bag) and parallelization on a multi-core processor.

In all examples, we can observe the application of Amdahl's law, because the maximum improvement in performance is limited by the part of the algorithm that cannot be parallelized.

Cache optimization using DualPivot QuickSort is noticeably more significant, as the following figure shows.

<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/third.png">

_ ** Note: in both cases, testing was performed on a string of 1,000,000, and the results refer to L1 Data Read Miss ** _

** The following figures represent the results of measuring the execution time of sorting algorithms. TIME IS MEASURED IN SECONDS. **

<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/fourth.png">

<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/fifth.png">
<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/sixth.png">
<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/seventh.png">
<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/eight.png">

The OpenMP optimization of the Merge Sort algorithm was written in C and run in LinuxOS, with the program compiled with the gcc compiler.

The ** # ** _ ** pragma omp task was used for optimization. ** _ Similar to Parallel.Invoke and Task.Factory, here too the maximum number of threads to which work can be allocated is limited to the number of cores returned f- jom _get \ _nprocs () ._

** Attached are the results of measuring the execution time of sorting using the & quot; standard & quot; MergeSort algorithm, and using OpenMP optimization of MergeSort algorithm. **

** Results are given in seconds. **

<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/nineth.png">
<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/tenth.png">
<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/eleventh.png">
<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/twelveth.png">
<img src="https://github.com/branislavroljic/Sorting-Algorithm-Optimization/blob/master/MergeSortOptimization/readme/thirteenth.png">
