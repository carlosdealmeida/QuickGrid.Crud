using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;

namespace QuickGrid.Crud.Helpers
{
    public static class List
    {
        public static void ExportToXml<T>(this List<T> data, string directory, string fileName)
        {
            var serializer = new XmlSerializer(typeof(List<T>));
            var filePath = Path.Combine(directory, fileName + ".xml");
            using (var streamWriter = new StreamWriter(filePath))
            {
                serializer.Serialize(streamWriter, data);
            }
        }
        public static void ExportToXml(this List<dynamic> data, string directory, string fileName)
        {
            var serializer = new XmlSerializer(data.GetType());
            var filePath = Path.Combine(directory, fileName + ".xml");
            using (var streamWriter = new StreamWriter(filePath))
            {
                serializer.Serialize(streamWriter, data);
            }
        }
        public static void ExportToXlsx<T>(this List<T> data, string directory, string fileName)
        {
            var filePath = Path.Combine(directory, fileName + ".xlsx");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Data");
                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = properties[i].Name;
                }
                worksheet.Cell(2, 1).InsertData(data);
                workbook.SaveAs(filePath);
            }
        }

        public static void ExportToXlsx(this List<dynamic> data, string directory, string fileName)
        {
            var filePath = Path.Combine(directory, fileName + ".xlsx");
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Data");
                if (data.Any())
                {
                    var dictData = data.First() as IDictionary<string, object>;
                    var columns = dictData.Keys.ToList();
                    for (int i = 0; i < columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = columns[i];
                    }
                    for (int i = 0; i < data.Count; i++)
                    {
                        var itemDict = data[i] as IDictionary<string, object>;
                        for (int j = 0; j < columns.Count; j++)
                        {
                            worksheet.Cell(i + 2, j + 1).Value = itemDict[columns[j]]?.ToString();
                        }
                    }
                }
                workbook.SaveAs(filePath);
            }
        }

        public static void ExportToCsv<T>(this List<T> data, string directory, string fileName)
        {
            var filePath = Path.Combine(directory, fileName + ".csv");
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(data);
            }
        }
        public static void ExportToCsv(this List<dynamic> data, string directory, string fileName)
        {
            var filePath = Path.Combine(directory, fileName + ".csv");
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(data);
            }
        }


        public static void ExportToTxt<T>(this List<T> data, string directory, string fileName)
        {
            var filePath = Path.Combine(directory, fileName + ".txt");
            using (var writer = new StreamWriter(filePath))
            {
                var properties = typeof(T).GetProperties();
                writer.WriteLine(string.Join(", ", properties.Select(p => p.Name)));

                foreach (var item in data)
                {
                    var values = properties.Select(p => p.GetValue(item)?.ToString() ?? "");
                    writer.WriteLine(string.Join(", ", values));
                }
            }
        }
        public static void ExportToTxt(this List<dynamic> data, string directory, string fileName)
        {
            var filePath = Path.Combine(directory, fileName + ".txt");
            using (var writer = new StreamWriter(filePath))
            {
                if (data.Count > 0)
                {
                    var properties = ((IDictionary<string, object>)data[0]).Keys;
                    writer.WriteLine(string.Join(", ", properties));

                    foreach (var item in data)
                    {
                        var values = properties.Select(p => ((IDictionary<string, object>)item)[p]?.ToString() ?? "");
                        writer.WriteLine(string.Join(", ", values));
                    }
                }
            }
        }
    }
}
