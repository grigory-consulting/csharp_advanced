#include <stdio.h>

#if defined(_WIN32)
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

// Callback type: process array of ints
typedef void (*ArrayCallback)(int* data, int length);

static ArrayCallback g_cb = 0;

EXPORT void RegisterArrayCallback(ArrayCallback cb)
{
    g_cb = cb;
    printf("[C] Callback registered.\n");
}

EXPORT void TriggerArrayCallback()
{
    if (!g_cb) {
        printf("[C] No callback registered.\n");
        return;
    }

    int arr[5] = {1, 2, 3, 4, 5};
    printf("[C] Sending array {1,2,3,4,5} to managed...\n");
    g_cb(arr, 5); // C hands a raw pointer + length to managed

    printf("[C] After managed call, array = { ");
    for (int i = 0; i < 5; i++) {
        printf("%d ", arr[i]);
    }
    printf("}\n");
}


