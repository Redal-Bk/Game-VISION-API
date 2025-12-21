using System;
using System.Collections.Generic;
using System.Text;

namespace Game_Vision.Domain.Basic
{
    public class CpuList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GpuList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class OsList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class RamList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class StorageList
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
