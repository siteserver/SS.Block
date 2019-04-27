using System;
using Datory;

namespace SS.Block.Core
{
    [Table("ss_block")]
    public class BlockInfo : Entity
    {
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public DateTime BlockDate { get; set; }

        [TableColumn]
        public int BlockCount { get; set; }
    }
}
