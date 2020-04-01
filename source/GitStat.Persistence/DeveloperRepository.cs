using GitStat.Core.Contracts;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

namespace GitStat.Persistence
{
    public class DeveloperRepository : IDeveloperRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public DeveloperRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Tuple<string, int, int, int, int>> GetDevStats() => _dbContext.Developers.Include(d => d.Commits)
                                    .OrderByDescending(d => d.Commits.Count)
                                    .Select(d => new Tuple<string, int, int, int, int>(
                                        d.Name,
                                        d.Commits.Count,
                                        d.Commits.Sum(f => f.FilesChanges),
                                        d.Commits.Sum(i => i.Insertions),
                                        d.Commits.Sum(de => de.Deletions)
                                    )).ToList();
    }
}