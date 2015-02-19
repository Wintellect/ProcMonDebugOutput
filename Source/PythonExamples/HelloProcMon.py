__author__ = 'Justin Dearing <zippy1981@gmail.com>'
__copyright__ = "Copyright 2014, Justin Dearing"
__credits__ = ["Justin Dearing", "John Robbins", "Mark Russinovich"]
__version__ = "1.0.0"
__status__ = "Prototype"

# Tested on Python 3.4

import win32file
import pywintypes

GENERIC_WRITE = 0x40000000
OPEN_EXISTING = 3
FILE_WRITE_ACCESS = 0x0002
FILE_SHARE_WRITE = 0x00000002
FILE_ATTRIBUTE_NORMAL = 0x00000080
METHOD_BUFFERED = 0
FILE_DEVICE_PROCMON_LOG = 0x00009535
PROCMON_DEBUGGER_HANDLER = r"\\.\Global\ProcmonDebugLogger"
IOCTL_EXTERNAL_LOG_DEBUGOUT = 2503311876 # Why: https://github.com/zippy1981/ProcMon.LINQpad/blob/master/ProcMonDebugOutput.linq

msg = bytes("Hello ProcMon from python with pywin32!", 'UTF-16')
msgLen = len(msg)
handle = win32file.CreateFile(PROCMON_DEBUGGER_HANDLER, GENERIC_WRITE, FILE_SHARE_WRITE, None, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL,0)
if handle == -1: raise RuntimeWarning("ProcMon doesn't appear to be running")
else:
    try:
        win32file.DeviceIoControl(handle, IOCTL_EXTERNAL_LOG_DEBUGOUT, msg, None)
    except pywintypes.error as e:
        if (e.winerror != 87): raise # Error 87 means ProcMon simply isn't running

    win32file.CloseHandle(handle)
