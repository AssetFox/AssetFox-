using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;
using OfficeOpenXml;

namespace BridgeCareCoreTests
{
    public static class ExcelPackageAsserts
    {
        public static void ValidExcelPackageData(Stream stream)
        {
            var _ = new ExcelPackage(stream); // will throw if the stream does not correspond to a valid ExcelPackage.
        }

        public static void ValidExcelPackageData(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            //File.WriteAllBytes("zzzzz.xlsx", bytes);
            ValidExcelPackageData(stream);
        }

        public static void ValidExcelPackageData(string packageContentsAsString)
        {
            var bytes = Convert.FromBase64String(packageContentsAsString);
            ValidExcelPackageData(bytes);
        }

        public static void ValidExcelPackageData(FileInfoDTO fileInfo)
        {
            var data = fileInfo.FileData;
            ValidExcelPackageData(data);
        }
    }
}
