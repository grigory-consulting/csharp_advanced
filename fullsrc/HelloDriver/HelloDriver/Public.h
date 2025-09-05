/*++

Module Name:

    public.h

Abstract:

    This module contains the common declarations shared by driver
    and user applications.

Environment:

    user and kernel

--*/

//
// Define an Interface Guid so that apps can find the device and talk to it.
//

DEFINE_GUID (GUID_DEVINTERFACE_HelloDriver,
    0x89ca2df3,0x87f5,0x44b0,0x90,0xba,0xfb,0x30,0x03,0xf5,0x4a,0x90);
// {89ca2df3-87f5-44b0-90ba-fb3003f54a90}
