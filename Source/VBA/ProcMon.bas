Option Explicit

' Win32 API Constants
Private Const GENERIC_WRITE As Long = &H40000000
Private Const OPEN_EXISTING As Long = &O3&
Private Const FILE_WRITE_ACCESS As Long = &H2
Private Const FILE_SHARE_WRITE As Long = &H2
Private Const FILE_ATTRIBUTE_NORMAL As Long = &H80
Private Const METHOD_BUFFERED As Long = 0
Private Const FORMAT_MESSAGE_FROM_SYSTEM = &H1000
Private Const LANG_NEUTRAL = &H0


' Process Monitor Constants
Private Const FILE_DEVICE_PROCMON_LOG As Long = &H9535
Private Const PROCMON_DEBUGGER_HANDLER As String = "\\.\Global\ProcmonDebugLogger"
Private Const IOCTL_EXTERNAL_LOG_DEBUGOUT As Long = -1791655420

Dim hProcMon As LongPtr

Private Declare Function CreateFile Lib "kernel32" Alias "CreateFileW" _
    (ByVal lpFileName As LongPtr, _
     Optional ByVal dwDesiredAccess As Long = GENERIC_WRITE, _
     Optional ByVal dwShareMode As Long = FILE_SHARE_WRITE, _
     Optional lpSecurityAttributes As LongPtr = 0, _
     Optional ByVal dwCreationDisposition As Long = OPEN_EXISTING, _
     Optional ByVal dwFlagsAndAttributes As Long = FILE_ATTRIBUTE_NORMAL, _
     Optional ByVal hTemplateFile As LongPtr = 0) As LongPtr
     
Private Declare Function DeviceIoControl Lib "kernel32" _
    (ByVal hDevice As LongPtr, _
     ByVal dwIoControlCode As Long, _
     ByVal lpInBuffer As LongPtr, _
     ByVal nInBufferSize As Long, _
     Optional lpOutBuffer As LongPtr, _
     Optional ByVal nOutBufferSize As Long, _
     Optional lpBytesReturned As Long, _
     Optional ByVal lpOverlapped As LongPtr) As Long

Private Declare Function GetLastError Lib "kernel32" () As LongPtr
Private Declare Sub SetLastError Lib "kernel32" (ByVal dwErrCode As LongPtr)
Private Declare Function FormatMessage Lib "kernel32" Alias "FormatMessageA" _
    (ByVal dwFlags As Long, lpSource As Any, ByVal dwMessageId As Long, _
     ByVal dwLanguageId As Long, ByVal lpBuffer As String, ByVal nSize As Long, _
     Arguments As Long) As Long


Public Function ProcMonDebugOutput(message As String) As LongPtr
    ProcMonDebugOutput = False
    Dim outLen As Long
    outLen = 0

    If hProcMon = 0 Then
        hProcMon = CreateFile(StrPtr(PROCMON_DEBUGGER_HANDLER))
    End If
    If hProcMon = -1 Then
        Dim Buffer As String
        'Create a string buffer
        Buffer = Space(200)
        FormatMessage FORMAT_MESSAGE_FROM_SYSTEM, ByVal 0&, Err.LastDllError, LANG_NEUTRAL, Buffer, 200, ByVal 0&
        Err.Raise Buffer
    End If

    ProcMonDebugOutput = DeviceIoControl _
        (hProcMon, IOCTL_EXTERNAL_LOG_DEBUGOUT, _
        StrPtr(message), Len(message) * 2)
End Function

