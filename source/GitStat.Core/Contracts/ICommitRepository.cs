﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitStat.Core.Entities;

namespace GitStat.Core.Contracts
{
    public interface ICommitRepository
    {
        void AddRange(Commit[] commits);
        List<Commit> GetLatestCommits(int days);
        Commit GetCommitById(int id);
    }
}
