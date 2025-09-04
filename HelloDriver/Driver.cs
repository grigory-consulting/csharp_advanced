#include <ntddk.h>

#define DEVICE_NAME     L"\\Device\\HelloWorldDevice"
#define SYMBOLIC_NAME   L"\\DosDevices\\HelloWorldDevice"

// Define a simple IOCTL code (arbitrary, just for demo) 0x800 -> method
#define IOCTL_HELLO CTL_CODE(FILE_DEVICE_UNKNOWN, 0x800, METHOD_BUFFERED, FILE_ANY_ACCESS)

PDEVICE_OBJECT gDeviceObject = NULL;

// Unload function
VOID DriverUnload(PDRIVER_OBJECT pDriverObject)
{
    UNREFERENCED_PARAMETER(pDriverObject);
    UNICODE_STRING symLink = RTL_CONSTANT_STRING(SYMBOLIC_NAME);
    IoDeleteSymbolicLink(&symLink);
    if (gDeviceObject) IoDeleteDevice(gDeviceObject);
    DbgPrint("HelloWorldDriver: Unloading.\n");
}

// Major function for Create/Close
NTSTATUS HelloCreateClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    UNREFERENCED_PARAMETER(DeviceObject);
    Irp->IoStatus.Status = STATUS_SUCCESS;
    Irp->IoStatus.Information = 0;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return STATUS_SUCCESS;
}

// Major function for Device Control (IOCTLs)
NTSTATUS HelloDeviceControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
    PIO_STACK_LOCATION  irpSp;
    NTSTATUS            status = STATUS_INVALID_DEVICE_REQUEST;
    ULONG               outLen = 0;

    UNREFERENCED_PARAMETER(DeviceObject);
    irpSp = IoGetCurrentIrpStackLocation(Irp);

    if (irpSp->Parameters.DeviceIoControl.IoControlCode == IOCTL_HELLO)
    {
        DbgPrint("HelloWorldDriver: IOCTL_HELLO received from usermode.\n");

        // Optionally write something back to user
        const char* reply = "Hello from kernel driver!";
        size_t replyLen = strlen(reply) + 1;
        if (irpSp->Parameters.DeviceIoControl.OutputBufferLength >= replyLen)
        {
            RtlCopyMemory(Irp->AssociatedIrp.SystemBuffer, reply, replyLen);
            outLen = (ULONG)replyLen;
            status = STATUS_SUCCESS;
        }
        else
        {
            status = STATUS_BUFFER_TOO_SMALL;
        }
    }

    Irp->IoStatus.Status = status;
    Irp->IoStatus.Information = outLen;
    IoCompleteRequest(Irp, IO_NO_INCREMENT);
    return status;
}

// Driver entry point
NTSTATUS DriverEntry(PDRIVER_OBJECT pDriverObject, PUNICODE_STRING pRegistryPath)
{
    KdPrint(("DriverEntry called!\n"));
    UNICODE_STRING devName = RTL_CONSTANT_STRING(DEVICE_NAME);
    UNICODE_STRING symLink = RTL_CONSTANT_STRING(SYMBOLIC_NAME);
    NTSTATUS status;

    UNREFERENCED_PARAMETER(pRegistryPath);

    // Create device
    status = IoCreateDevice(
        pDriverObject,
        0,              // no device extension
        &devName,
        FILE_DEVICE_UNKNOWN,
        0,
        FALSE,
        &gDeviceObject);

    if (!NT_SUCCESS(status)) return status;

    // Create symbolic link
    status = IoCreateSymbolicLink(&symLink, &devName);
    if (!NT_SUCCESS(status)) {
        IoDeleteDevice(gDeviceObject);
        return status;
    }

    // Set dispatch routines
    pDriverObject->MajorFunction[IRP_MJ_CREATE] = HelloCreateClose;
    pDriverObject->MajorFunction[IRP_MJ_CLOSE] = HelloCreateClose;
    pDriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = HelloDeviceControl;
    pDriverObject->DriverUnload = DriverUnload;

    DbgPrint("HelloWorldDriver: Loaded and ready!\n");
    return STATUS_SUCCESS;
}
