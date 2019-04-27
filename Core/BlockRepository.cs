using System;
using System.Collections.Generic;
using System.Linq;
using Datory;
using SiteServer.Plugin;

namespace SS.Block.Core
{
    public class BlockRepository
    {
        private readonly Repository<BlockInfo> _repository;

        public BlockRepository()
        {
            _repository = new Repository<BlockInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        public string TableName => _repository.TableName;

        private static class Attr
        {
            public const string SiteId = nameof(BlockInfo.SiteId);

            public const string BlockDate = nameof(BlockInfo.BlockDate);

            public const string BlockCount = nameof(BlockInfo.BlockCount);
        }

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public void AddBlock(int siteId)
        {
            var now = GetNow();
            if (_repository.Exists(Q.Where(Attr.SiteId, siteId).Where(Attr.BlockDate, now)))
            {
                _repository.Increment(Attr.BlockCount, Q.Where(Attr.SiteId, siteId).Where(Attr.BlockDate, now));
            }
            else
            {
                _repository.Insert(new BlockInfo
                {
                    SiteId = siteId,
                    BlockDate = now,
                    BlockCount = 1
                });
            }
        }

        private static DateTime GetNow()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day);
        }

        public List<KeyValuePair<string, int>> GetMonthlyBlockedList(int siteId)
        {
            var now = GetNow();
            var blockInfoList = _repository.GetAll(Q.Where(Attr.SiteId, siteId).WhereBetween(Attr.BlockDate, now.AddDays(-30), now.AddDays(1)));

            var blockedList = new List<KeyValuePair<string, int>>();
            for (var i = 30; i >= 0; i--)
            {
                var date = now.AddDays(-i).ToString("M-d");
                var blockInfo = blockInfoList.FirstOrDefault(x => x.BlockDate.ToString("M-d") == date);
                blockedList.Add(new KeyValuePair<string, int>(date, blockInfo?.BlockCount ?? 0));
            }

            return blockedList;
        }
    }
}
