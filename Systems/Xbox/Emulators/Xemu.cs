using EmuHelp.HelperBase;
using EmuHelp.Logging;
using EmuHelp.Systems.Xbox;
using EmuHelp.Systems.Xbox.Emulators;
using JHelper.Common.MemoryUtils;
using JHelper.Common.ProcessInterop;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace EmuHelp.Systems.Xbox.Emulators;

internal class Xemu : XboxEmulator
{
    private IntPtr addr_base;

    internal Xemu()
        : base()
    {
        Log.Info("  => Attached to emulator: Xemu");
    }

    public override bool FindRAM(ProcessMemory _process)
    {
        if (_process.Is64Bit)
        {
            addr_base = _process.Scan(new MemoryScanPattern(3, "48 8B ?? ?? 45 89") { OnFound = addr => addr + 0x4 + _process.Read<int>(addr) });
            if (addr_base == IntPtr.Zero)
                return false;

            if (!_process.Read(addr_base, out IntPtr ptr))
                return false;
            RamBase = ptr;
        }
        else
        {
            addr_base = new MemoryScanPattern[]
            {
                new(2, "8B ?? ?? 45 89 ?? ?? 45 8D") { OnFound = _process.ReadPointer },
            }
            .Select(_process.Scan).FirstOrDefault(addr => addr != IntPtr.Zero);

            if (addr_base == IntPtr.Zero)
                return false;

            if (!_process.Read(addr_base, out IntPtr ptr))
                return false;
            RamBase = ptr;
        }

        if (RamBase != IntPtr.Zero)
            Log.Info($"  => RAM address found at 0x{RamBase.ToString("X")}");
        return true;
    }

    public override bool KeepAlive(ProcessMemory process)
    {
        if (!process.Read(addr_base, out IntPtr ptr))
            return false;
        RamBase = ptr;

        return true;
    }
}
