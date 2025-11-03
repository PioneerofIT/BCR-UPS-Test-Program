using System;

using System.Runtime.InteropServices;
using System.Diagnostics;

public static class LogHelper
{
    [DllImport("VsLogLib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void AddVsLogFormat(string messageType, string format, params object[] args);

    [DllImport("VsLogLib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void AddVsLog(string messageType, string logMsg);

    [DllImport("VsLogLib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void AddVsLogFormatA(string messageType, string format, params object[] args);

    [DllImport("VsLogLib.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void AddVsLogA(string messageType, string logMsg);

    public static void LOG_PRINTF(string messageType, string format, params object[] args)
    {
        var stackFrame = new StackFrame(1, true);
        string fileName = stackFrame.GetFileName();
        int lineNumber = stackFrame.GetFileLineNumber();
        string functionName = stackFrame.GetMethod().Name;

        AddVsLogFormat(messageType, $"{fileName}:{lineNumber} {functionName} | {format}", args);
    }

    public static void LOG_STR(string messageType, string message)
    {
        var stackFrame = new StackFrame(1, true);
        string fileName = stackFrame.GetFileName();
        int lineNumber = stackFrame.GetFileLineNumber();
        string functionName = stackFrame.GetMethod().Name;

        AddVsLog(messageType, $"{fileName}:{lineNumber} {functionName} | {message}");
    }

    public static void LOG_PRINTFA(string messageType, string format, params object[] args)
    {
        var stackFrame = new StackFrame(1, true);
        string fileName = stackFrame.GetFileName();
        int lineNumber = stackFrame.GetFileLineNumber();
        string functionName = stackFrame.GetMethod().Name;

        AddVsLogFormatA(messageType, $"{fileName}:{lineNumber} {functionName} | {format}", args);
    }

    public static void LOG_STRA(string messageType, string message)
    {
        var stackFrame = new StackFrame(1, true);
        string fileName = stackFrame.GetFileName();
        int lineNumber = stackFrame.GetFileLineNumber();
        string functionName = stackFrame.GetMethod().Name;

        AddVsLogA(messageType, $"{fileName}:{lineNumber} {functionName} | {message}");
    }
}