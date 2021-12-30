using System;

namespace Framework.Mobile
{
    public struct FileStruct
    {
        public string Flags;
        public string Owner;
        public string Group;
        public int FileSize;
        public bool IsDirectory;
        public DateTime CreateTime;
        public string Name;
    }
}