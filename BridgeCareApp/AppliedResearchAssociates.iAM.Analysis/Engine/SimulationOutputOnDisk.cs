using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AppliedResearchAssociates.iAM.Analysis.Engine
{
    internal sealed class SimulationOutputOnDisk : IDisposable
    {
        public SimulationOutputOnDisk()
        {
            var tempPath = Path.GetTempPath();
            var folderName = Path.GetRandomFileName();
            var folderPath = Path.Combine(tempPath, folderName);
            Folder = Directory.CreateDirectory(folderPath);
            MainFilePath = Path.Combine(Folder.FullName, "main.json");

            AppDomain.CurrentDomain.ProcessExit += delegate
            {
                TryDeleteFolder();
            };
        }

        public void AddYearDetail(SimulationYearDetail yearDetail)
        {
            var yearFilePath = GetYearFilePath(yearDetail);
            Serialize(yearDetail, yearFilePath);
        }

        public void Clear()
        {
            foreach (var info in Folder.EnumerateFileSystemInfos())
            {
                info.Delete();
            }
        }

        public void Dispose()
        {
            TryDeleteFolder();
            GC.SuppressFinalize(this);
        }

        public SimulationOutput GetOutput()
        {
            using var reader = File.OpenText(MainFilePath);
            using JsonTextReader jsonReader = new(reader);
            var output = Serializer.Deserialize<SimulationOutput>(jsonReader);

            var yearFiles = Folder.GetFiles("year_*.json");
            foreach (var yearFile in yearFiles)
            {
                using var yearReader = yearFile.OpenText();
                using JsonTextReader yearJsonReader = new(yearReader);
                var year = Serializer.Deserialize<SimulationYearDetail>(yearJsonReader);
                output.Years.Add(year);
            }

            output.Years.Sort(static (y1, y2) => Comparer<int>.Default.Compare(y1.Year, y2.Year));

            return output;
        }

        public void Initialize(SimulationOutput output) => Serialize(output, MainFilePath);

        private static readonly JsonSerializer Serializer = new();

        private readonly DirectoryInfo Folder;

        private readonly string MainFilePath;

        private static void Serialize<T>(T t, string path)
        {
            using var writer = File.CreateText(path);
            using JsonTextWriter jsonWriter = new(writer);
            Serializer.Serialize(jsonWriter, t);
        }

        private string GetYearFilePath(SimulationYearDetail yearDetail) => Path.Combine(Folder.FullName, $"year_{yearDetail.Year}.json");

        private void TryDeleteFolder()
        {
            try
            {
                Folder?.Delete(true);
            }
            catch
            {
            }
        }
    }
}
