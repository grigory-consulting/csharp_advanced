#include <stdio.h>

#if defined(_WIN32)
#define EXPORT __declspec(dllexport)
#else
#define EXPORT
#endif

typedef int (*Callback)(int);

static Callback g_cb = 0;

EXPORT void RegisterCallback(Callback cb)
{
    g_cb = cb;
    printf("[C] Callback registered.\n");
}

EXPORT void TriggerLoop()
{
    if (!g_cb) {
        printf("[C] No callback registered.\n");
        return;
    }

    for (int i = 1; i <= 5; i++) {
        printf("[C] Sending %d to managed...\n", i);
        int result = g_cb(i);   // call into C# delegate
        printf("[C] Managed returned %d\n", result);
    }
}
