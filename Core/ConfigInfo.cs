using System.Collections.Generic;

namespace SS.Block.Core
{
    public class ConfigInfo
    {
        public bool IsBlock { get; set; }

        public bool IsBlockAll { get; set; }

        public List<int> BlockAreas { get; set; }

        public string BlockMethod { get; set; }

        public string RedirectUrl { get; set; }

        public string Warning { get; set; }

        public string Password { get; set; }
    }
}