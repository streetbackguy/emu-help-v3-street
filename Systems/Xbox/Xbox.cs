using EmuHelp.HelperBase;
using EmuHelp.Logging;
using EmuHelp.Systems.Xbox;
using EmuHelp.Systems.Xbox.Emulators;
using JHelper.Common.MemoryUtils;
using System;
using System.Runtime.InteropServices;

public class Xbox : HelperBase
{
    private const uint MINSIZE = 0x00010000;
    private const uint MAXSIZE = 0x08000000;

    private XboxEmulator? Xboxemulator
    {
        get => (XboxEmulator?)emulatorClass;
        set => emulatorClass = value;
    }

    public Xbox()
#if LIVESPLIT
        : this(true) { }

    public Xbox(bool generateCode)
        : base(generateCode)
#else
        : base()
#endif
    {
        Log.Info("  => Xbox Helper started");
    }

    internal override string[] ProcessNames { get; } =
    [
        "xemu.exe",
    ];

    public override bool TryGetRealAddress(ulong address, out IntPtr realAddress)
    {
        realAddress = default;

        if (Xboxemulator is null)
            return false;

        IntPtr baseRam = Xboxemulator.RamBase;

        if (baseRam == IntPtr.Zero)
            return false;


        if (address >= MINSIZE && address < MAXSIZE)
        {
            realAddress = (IntPtr)((ulong)baseRam + address);
            return true;
        }
        return false;
    }

    internal override Emulator? AttachEmuClass()
    {
        if (emulatorProcess is null)
            return null;

        return emulatorProcess.ProcessName switch
        {
            "xemu.exe" => new Xemu(),
            _ => null,
        };
    }
}
