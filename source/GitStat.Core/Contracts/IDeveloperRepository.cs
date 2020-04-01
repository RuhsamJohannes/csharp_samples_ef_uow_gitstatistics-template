using GitStat.Core.Entities;
using System;
using System.Collections.Generic;

namespace GitStat.Core.Contracts
{
    public interface IDeveloperRepository
    {
        IEnumerable<Tuple<string, int, int, int, int>> GetDevStats();
    }
}
