// NativeTest.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "ProcMonDebugOutput.h"


int _tmain(void)
{
    WCHAR szText[100];
    for (int i = 0; i < 20; i++)
    {
        _stprintf_s(szText,
                    _countof(szText),
                    L"ProcMon Debug Out Test # %d",
                    i);
        BOOL bRet = ProcMonDebugOutput(szText);
        if (TRUE == bRet)
        {
            _tprintf(L"Wrote %d\n", i);
        }
        else
        {
            _tprintf(L"error 0x%x\n", GetLastError());
        }
        ::Sleep(500);
    }
    return (0);
}

