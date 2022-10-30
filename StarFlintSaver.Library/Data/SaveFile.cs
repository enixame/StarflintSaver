using System;

namespace StarFlintSaver.Library.Data
{
    public class SaveFile
    {
        public string FileName { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Description { get; set; }
    }
}
