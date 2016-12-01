// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;

public class ApiDrive
{
    /// <summary>
    /// Открывает файл и возвращает альтернативный файловый поток NTFS с запретом совместного использования
    /// </summary>
    /// <param name="fileName">Путь к файлу</param>
    /// <param name="streamName">Имя альтернативного потока</param>
    /// <param name="mode">Режим открытия фйла</param>
    /// <param name="access">Режим доступа к файлу</param>
    /// <returns></returns>
    public FileStream OpenWithStream(string fileName, string streamName, FileMode mode, FileAccess access)
    {
        return OpenWithStream(fileName, streamName, mode, access, FileShare.None);
    }

    /// <summary>
    /// Открывает файл и возвращает альтернативный файловый поток NTFS с заданым режимом совместного использования
    /// </summary>
    /// <param name="fileName">Путь к файлу</param>
    /// <param name="streamName">Имя альтернативного потока</param>
    /// <param name="mode">Режим открытия фйла</param>
    /// <param name="access">Режим доступа к файлу</param>
    /// <param name="share">Режим совместного использования</param>
    /// <returns></returns>
    public FileStream OpenWithStream(string fileName, string streamName, FileMode mode, FileAccess access, FileShare share)
    {
        uint desiredAccess = (access & FileAccess.Read) == FileAccess.Read ? GenericRead : 0;
        desiredAccess |= (access & FileAccess.Write) == FileAccess.Write ? GenericWrite : 0;

        SafeFileHandle fileHandle = CreateFile(
            streamName != String.Empty ? (fileName + ":" + streamName) : fileName,
            desiredAccess,
            share,
            IntPtr.Zero,
            mode,
            FileAttributes.Normal,
            IntPtr.Zero
        );
        if (fileHandle.IsInvalid)
        {
            int hresult = Marshal.GetHRForLastWin32Error();
            Exception ex = Marshal.GetExceptionForHR(hresult);
            throw ex ?? new Win32Exception(hresult, string.Format("Failed to open file with specified stream: '{0}'", fileName));
        }
        return new FileStream(fileHandle, access);
    }

    const uint GenericRead = 0x80000000;
    const uint GenericWrite = 0x40000000;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern SafeFileHandle CreateFile(
        string fileName,
        uint desiredAccess,
        [MarshalAs(UnmanagedType.U4)] FileShare share,
        IntPtr lpSecurityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
        IntPtr hTemplateFile
    );
}