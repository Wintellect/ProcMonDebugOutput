__author__ = 'Justin Dearing <zippy1981@gmail.com>'
__copyright__ = "Copyright 2014, Justin Dearing"
__credits__ = ["Justin Dearing", "John Robbins", "Mark Russinovich"]
__version__ = "1.0.0"
__status__ = "Prototype"

import ctypes
from ctypes import windll, c_void_p
from ctypes import c_uint32
from ctypes import c_wchar_p
from ctypes import byref


GENERIC_WRITE = 0x40000000
OPEN_EXISTING = 3
FILE_WRITE_ACCESS = 0x0002
FILE_SHARE_WRITE = 0x00000002
FILE_ATTRIBUTE_NORMAL = 0x00000080
METHOD_BUFFERED = 0
FILE_DEVICE_PROCMON_LOG = 0x00009535
PROCMON_DEBUGGER_HANDLER = c_wchar_p(r"\\.\Global\ProcmonDebugLogger")
DW_IO_CONTROL_CODE = 2503311876

k32 = windll.kernel32

msg = bytes("Hello ProcMon from python with ctypes!", 'UTF-16')

handle = k32.CreateFileW(
    PROCMON_DEBUGGER_HANDLER,
    GENERIC_WRITE,
    FILE_SHARE_WRITE,
    0,
    OPEN_EXISTING,
    FILE_ATTRIBUTE_NORMAL,
    0
)
if handle == -1: raise RuntimeWarning("ProcMon doesn't appear to be running")

print ("Handle: %d" % handle)

k32.DeviceIoControl(
    handle,
    DW_IO_CONTROL_CODE,
    msg,
    len(msg) * 2,
    0,
    0,
    byref(c_void_p()), # So quoth the MSDN: If lpOverlapped is NULL, lpBytesReturned cannot be NULL.  http://msdn.microsoft.com/en-us/library/windows/desktop/aa363216.aspx
    None
)