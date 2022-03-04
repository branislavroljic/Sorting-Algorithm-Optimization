#include<stdio.h>
#include<omp.h>
#include<math.h>
#include <stdlib.h>

#include <sys/sysinfo.h>


void merge(int [], int , int , int );
void mergeSort(int [], int , int );
void openMPMergeSort(int [], int , int , int );
void COopenMPMergeSort(int [], int [], int , int , int );
int isSorted(int* , int );
void exec(int);

void cacheOptimizedMerge(int [], int [], int , int , int ) ;
void cacheOptimizedSort(int [], int [], int , int );
void execSeqWithMergeCacheOptimization(int );

void merge(int array[], int l, int m, int r)
{
    int i, j, k;
    const int n1 = m - l + 1;
    const int n2 = r - m;

    int* L = (int*)malloc(n1 * sizeof(int));
    int* R = (int*)malloc(n2 * sizeof(int));
    for (i = 0; i < n1; i++)
        L[i] = array[l + i];
    for (j = 0; j < n2; j++)
        R[j] = array[m + 1 + j];

    i = 0; 
    j = 0; 
    k = l; 
    while (i < n1 && j < n2) {
        if (L[i] <= R[j]) {
            array[k] = L[i];
            i++;
        }
        else {
            array[k] = R[j];
            j++;
        }
        k++;
    }

    while (i < n1) {
        array[k] = L[i];
        i++;
        k++;
    }

    
    while (j < n2) {
        array[k] = R[j];
        j++;
        k++;
    }

    free(L);
    free(R);
}

void mergeSort(int array[], int l, int r)
{
    if (l < r) {
        int m = l + (r - l) / 2;

        // Sort first and second halves
        mergeSort(array, l, m);
        mergeSort(array, m + 1, r);

        merge(array, l, m, r);
    }
}

 void openMPMergeSort(int array[], int left, int right, int depth) {
            if (left < right) {
                
                int middle = left + (right - left) / 2;
                if (depth > 0) {
#pragma omp task shared(array)
                    openMPMergeSort(array, left, middle, depth - 1);
#pragma omp task shared(array)
                    openMPMergeSort(array, middle + 1, right, depth - 1);

#pragma omp taskwait
                    merge(array, left, middle, right);
                }
                else {
                    mergeSort(array, left, middle);
                    mergeSort(array, middle + 1, right);
 merge(array, left, middle, right);
                }
            }
        }

    
void cacheOptimizedMerge(int arrayay[], int tempArray[], int left, int right, int rightEnd) {

    int i, left_end, num_elements, tmp_pos;
    left_end = right - 1;
    tmp_pos = left;
    num_elements = rightEnd - left + 1;


    while ((left <= left_end) && (right <= rightEnd))

        if (arrayay[left] <= arrayay[right])
            tempArray[tmp_pos++] = arrayay[left++];
        else
            tempArray[tmp_pos++] = arrayay[right++];

    while (left <= left_end) 
        tempArray[tmp_pos++] = arrayay[left++];

    while (right <= rightEnd)
        tempArray[tmp_pos++] = arrayay[right++];

    for (i = 1; i <= num_elements; i++, rightEnd--)
        arrayay[rightEnd] = tempArray[rightEnd];

}


void cacheOptimizedSort(int array[], int tempArray[], int left, int right) {

    int center;
    if (left < right) {

        center = (left + right) / 2;
        cacheOptimizedSort(array, tempArray, left, center);
        cacheOptimizedSort(array, tempArray, center + 1, right);
        cacheOptimizedMerge(array, tempArray, left, center + 1, right);
    }
}

void COopenMPMergeSort(int array[], int tempArray[], int l, int r, int depth)
{
    int m;
    if (l < r) {

        m = l + (r - l)/2;
        if (depth > 0) {
            // Sort first and second halves
#pragma omp task shared(array, tempArray)
            COopenMPMergeSort(array,tempArray, l, m, depth  - 1);
#pragma omp task shared(array, tempArray)
            COopenMPMergeSort(array,tempArray, m + 1, r, depth - 1);

#pragma omp taskwait
            cacheOptimizedMerge(array,tempArray, l, m + 1, r);
        }
        else {
            cacheOptimizedSort(array, tempArray, l, m);
            cacheOptimizedSort(array, tempArray, m + 1, r);
                cacheOptimizedMerge(array,tempArray, l, m + 1, r);
        }
    }
}

int isSorted(int* a, int size) {
    for (int i = 0; i < size - 1; i++)
        if (a[i] > a[i + 1])
            return 0;
    return 1;
}

void printArray(int *a, int size){
   for(int i = 0; i < size; i++)
       printf("%d ", a[i]);
   printf("\n");
}

void exec(int size) {


FILE *fpt;
fpt = fopen("Results.csv", "a");
fprintf(fpt,"Number of elements, %d\n", size);

printf("================================================");
printf("\nNumber of elements: %d\n", size);
    int* randArray = (int*)malloc(size * sizeof(int));
    int* SSArray = (int*)malloc(size * sizeof(int));
    int* COArray = (int*)malloc(size * sizeof(int));
    int* temp_COArray = (int*)malloc(size * sizeof(int));
    int* OMPArray = (int*)malloc(size * sizeof(int));
    int* OMPCOArray = (int*)malloc(size * sizeof(int));
    int* temp_OMPCOArray = (int*)malloc(size * sizeof(int));

    for (int i = 0; i < size; i++)
        randArray[i] = rand();
    
    for (int i = 0; i < size; i++)
    {
        SSArray[i] = randArray[i];
        COArray[i] = randArray[i];
        OMPArray[i] = randArray[i];
        OMPCOArray[i] = randArray[i];
    }

//Standard MegreSort
    double start_time = omp_get_wtime();
    mergeSort(SSArray, 0, size - 1);
    double run_time = omp_get_wtime() - start_time;
    printf(" Standard Merge Sort : %lfs\n ", run_time);
    fprintf(fpt,"Standard Merge Sort, %lf\n", run_time);
    if(!isSorted(SSArray, size))
    printf("Standard Merge Sort failed!!!\n");

//OpenMP MergeSort
        start_time = omp_get_wtime();
    #pragma omp parallel
        {
            #pragma omp single
            openMPMergeSort(OMPArray, 0, size - 1, get_nprocs());
        }
        run_time = omp_get_wtime() - start_time;
        printf("OpenMP Merge Sort : %lfs\n ", run_time);
        fprintf(fpt,"OpenMP Merge Sort, %lf\n", run_time);
    if(!isSorted(OMPArray, size))
        printf("OpenMP Merge Sort failed!!!\n");


fprintf(fpt," , \n");
fclose(fpt);


//CacheOptimized MergeSort
//        start_time = omp_get_wtime();
//        cacheOptimizedSort(COArray, temp_COArray,  0, size - 1);
//        run_time = omp_get_wtime() - start_time;
//        printf("Cache optimized Merge Sort : %lfs\n ", run_time);
//    if(!isSorted(COArray, size))
//        printf("Cache optimized Merge Sort failed!!!\n");

//OpenMP + CacheOptimized MergeSort
        start_time = omp_get_wtime();
    #pragma omp parallel
        {
    #pragma omp single
            COopenMPMergeSort(OMPCOArray, temp_OMPCOArray, 0, size - 1, get_nprocs());
        }
       run_time = omp_get_wtime() - start_time;
        printf("OpenMP Cache optimized Merge Sort : %lfs\n ", run_time);
        if(!isSorted(OMPCOArray, size))
        printf("OpenMP Cache optimized Merge Sort failed!!");
    
}

void execSeqWithMergeCacheOptimization(int size) {


    int* array = (int*)malloc(size * sizeof(int));
    for (int i = 0; i < size; i++)
            array[i] = rand();

    int* SSArray = (int*)malloc(size * sizeof(int));
    for (int i = 0; i < size; i++)
    {
        SSArray[i] = array[i];
    }

    double start_time = omp_get_wtime();
    mergeSort(SSArray, 0, size - 1);
    double run_time = omp_get_wtime() - start_time;
    printf(" Standard Merge Sort : %lfs\n ", run_time);
    if(!isSorted(SSArray, size))
    printf("Standard Merge Sort failed!!!\n");
    

    int* tempArray = (int*)malloc(size * sizeof(int));
    start_time = omp_get_wtime();
    cacheOptimizedSort(array, tempArray, 0, size - 1);
    run_time = omp_get_wtime() - start_time;
    printf("\n Cache optimized Merge Sort : %lf s\n ", run_time);
    if(!isSorted(array, size))
    printf("Cache optimized Merge sort failed!!!\n");


}

int main()
{
remove("Results.csv");

   //execSeqWithMergeCacheOptimization(1000000);
exec(1000000);
exec(3000000);
exec(5000000);
exec(10000000);
exec(30000000);
    return 0;
}
