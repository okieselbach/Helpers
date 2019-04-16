#pragma once

// Including SDKDDKVer.h defines the highest available Windows platform.

// If you wish to build your application for a previous Windows platform, include WinSDKVer.h and
// set the _WIN32_WINNT macro to the platform you wish to support before including SDKDDKVer.h.

// added to ensure support for min. WinPE 2.0 (based on Vista)
// WinPE 2.0 is based on Vista
// WinPE 3.0 is based on Windows 7
// WinPE 3.1 is based on Windows 7 with Service Pack 1
// WinPE 4.0 is based on Windows 8
#include <WinSDKVer.h>
#define _NEW_WIN32_WINNT _WIN32_WINNT_VISTA

#include <SDKDDKVer.h>