using GitStat.Core.Contracts;
using GitStat.Persistence;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GitStat.ImportConsole
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Import der Commits in die Datenbank");
            using (IUnitOfWork unitOfWorkImport = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWorkImport.DeleteDatabase();
                Console.WriteLine("Datenbank migrieren");
                unitOfWorkImport.MigrateDatabase();
                Console.WriteLine("Commits werden von commits.csv eingelesen");
                var commits = ImportController.ReadFromCsv();
                if (commits.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Commits eingelesen");
                    return;
                }
                Console.WriteLine(
                    $"  Es wurden {commits.Count()} Commits eingelesen, werden in Datenbank gespeichert ...");
                unitOfWorkImport.CommitRepository.AddRange(commits);
                int countDevelopers = commits.GroupBy(c => c.Developer).Count();
                int savedRows = unitOfWorkImport.SaveChanges();
                Console.WriteLine(
                    $"{countDevelopers} Developers und {savedRows - countDevelopers} Commits wurden in Datenbank gespeichert!");
                Console.WriteLine();
                var csvCommits = commits.Select(c =>
                    $"{c.Developer.Name};{c.Date};{c.Message};{c.HashCode};{c.FilesChanges};{c.Insertions};{c.Deletions}");
                File.WriteAllLines("commits.csv", csvCommits, Encoding.UTF8);
            }


            Console.WriteLine("\nDatenbankabfragen");
            Console.WriteLine("=================");
            using (IUnitOfWork unitOfWork = new UnitOfWork())
            {
                //Commits der letzten Vier Wochen vor dem letzten Commit
                int days = 28;
                var latestCommits = unitOfWork.CommitRepository.GetLatestCommits(days); //GetDevStats(28) => days since last commit

                Console.WriteLine($"\nCommits der letzten {days} Tage vor dem letzten Commit");
                Console.WriteLine($"---------------------------------------------------");
                Console.WriteLine($"{"Developer",-15}{"Date",-11}{"FileChanges", -13}{"Insertions", -12}{"Deletions", -11}");

                foreach (var item in latestCommits)
                {
                    Console.WriteLine($"{item.Developer.Name,-15}{item.Date.ToShortDateString(),-11}{item.FilesChanges, -13}{item.Insertions,-12}{item.Deletions,-11}");
                }

                //Commit mit id
                int id = 4;
                var IdCommit = unitOfWork.CommitRepository.GetCommitById(id);

                Console.WriteLine($"\nCommits mit ID {id}");
                Console.WriteLine($"----------------");
                Console.WriteLine($"{IdCommit.Developer.Name,-15}{IdCommit.Date.ToShortDateString(),-11}{IdCommit.FilesChanges,-13}{IdCommit.Insertions,-12}{IdCommit.Deletions,-11}");

                //Statistik der Commits der Developer
                var devStats = unitOfWork.DeveloperRepository.GetDevStats();

                Console.WriteLine("\nStatistik der Commits der Developer");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine($"{"Developer",-15}{"Commits",-11}{"FileChanges",-13}{"Insertions",-12}{"Deletions",-11}");

                foreach (var item in devStats)
                {
                    Console.WriteLine($"{item.Item1,-15}{item.Item2,-11}{item.Item3,-13}{item.Item4,-12}{item.Item5,-11}");
                }
            }
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }
    }
}
