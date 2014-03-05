
#include "stdafx.h"
#include "ProcMonDebugOutput.h"

#define FILE_DEVICE_PROCMON_LOG     0x00009535
#define IOCTL_EXTERNAL_LOG_DEBUGOUT	(ULONG) CTL_CODE(FILE_DEVICE_PROCMON_LOG ,\
                                                     0x81                    ,\
                                                     METHOD_BUFFERED         ,\
                                                     FILE_WRITE_ACCESS        )

// The global file handle to the Process Monitor device.
static HANDLE g_hDevice = INVALID_HANDLE_VALUE;

HANDLE OpenProcessMonitorLogger(void)
{
    if (INVALID_HANDLE_VALUE == g_hDevice)
    {
        // I'm attempting the open every time because the user could start 
        // Process Monitor after their process.
        g_hDevice = ::CreateFile(L"\\\\.\\Global\\ProcmonDebugLogger",
                                 GENERIC_WRITE,
                                 FILE_SHARE_WRITE,
                                 NULL,
                                 OPEN_EXISTING,
                                 FILE_ATTRIBUTE_NORMAL,
                                 NULL);
    }
    return (g_hDevice);
}

void CloseProcessMonitorLogger(void)
{
    if (INVALID_HANDLE_VALUE != g_hDevice)
    {
        ::CloseHandle(g_hDevice);
        g_hDevice = INVALID_HANDLE_VALUE;
    }
}

PROCMONDEBUGOUTPUT_DLLINTERFACE __success(return == TRUE)
BOOL __stdcall ProcMonDebugOutput(__in LPCWSTR lpOutputString)
{
    BOOL bRet = FALSE;

    if (NULL == lpOutputString)
    {
        ::SetLastError(ERROR_INVALID_PARAMETER);
        bRet = FALSE;
    }
    else
    {
        HANDLE hProcMon = OpenProcessMonitorLogger();
        if (INVALID_HANDLE_VALUE != hProcMon)
        {
            DWORD iLen = (DWORD) _tcslen(lpOutputString) * sizeof (WCHAR);
            DWORD iOutLen = 0;
            bRet = ::DeviceIoControl(hProcMon,
                                     IOCTL_EXTERNAL_LOG_DEBUGOUT,
                                     (LPVOID) lpOutputString,
                                     iLen,
                                     NULL,
                                     0,
                                     &iOutLen,
                                     NULL);
            if (FALSE == bRet)
            {
                DWORD dwLastError = ::GetLastError();
                if (ERROR_INVALID_PARAMETER == dwLastError)
                {
                    // The driver is loaded but the user mode Process Monitor
                    // program is not running so turn the last error into a 
                    // write failure.
                    ::SetLastError(ERROR_WRITE_FAULT);
                }
            }
        }
        else
        {
            // Process Monitor isn't loaded.
            ::SetLastError(ERROR_BAD_DRIVER);
            bRet = FALSE;
        }
    }
    return (bRet);
}

BOOL APIENTRY DllMain(HMODULE /*hModule*/,
                      DWORD   ul_reason_for_call,
                      LPVOID  /*lpReserved*/)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    case DLL_PROCESS_DETACH:
        // Close the handle to the driver.
        CloseProcessMonitorLogger();
        break;
    }
    return (TRUE);
}

