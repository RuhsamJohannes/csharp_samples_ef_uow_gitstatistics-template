using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GitStat.Core.Entities;
using Utils;
using System.Linq;

namespace GitStat.ImportConsole
{
    public class ImportController
    {
        const string FilenameTxt = "commits.txt";
        const string FilenameCsv = "commits.csv";

        /// <summary>
        /// Liefert die Messwerte mit den dazugehörigen Sensoren
        /// </summary>
        public static Commit[] ReadFromCsv()
        {
            string[][] commitsArray = MyFile.ReadStringMatrixFromCsv(FilenameCsv, false);

            var devs = commitsArray.GroupBy(c => c[0])
                .Select(c => new Developer
                {
                    Name = c.Key,
                    Commits = new List<Commit>()

                }).ToDictionary(n => n.Name);

            var commits = commitsArray.Select(c => new Commit
            {
                Developer = devs[c[0]],
                Date = DateTime.Parse(c[1]),
                HashCode = c[2],
                Message = c[3],
                FilesChanges = int.Parse(c[4]),
                Insertions = int.Parse(c[5]),
                Deletions = int.Parse(c[6])
            }).ToArray();

            foreach (var item in commits)
            {
                item.Developer.Commits.Add(item);
            }

            return commits;
        }
    }
}
