using System;
using System.Collections.Generic;
using System.Linq;
using GitStat.Core.Contracts;
using GitStat.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GitStat.Persistence
{
    public class CommitRepository : ICommitRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommitRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRange(Commit[] commits)
        {
            _dbContext.Commits.AddRange(commits);
        }

        public List<Commit> GetLatestCommits(int days)
        {
            DateTime lastCommitDate = _dbContext.Commits.OrderByDescending(c => c.Date)
                                                    .First()
                                                    .Date;

            List<Commit> latestCommits = _dbContext.Commits.Include(c => c.Developer)
                                                    .Where(c => lastCommitDate.AddDays(-days) <= c.Date && c.Date <= lastCommitDate)
                                                    .OrderBy(_ => _.Date)
                                                    .ToList();

            return latestCommits;
        }

        public Commit GetCommitById(int id) => _dbContext.Commits.Find(id);
    }
}