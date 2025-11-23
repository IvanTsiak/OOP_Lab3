using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Lab3.Models;

namespace Lab3.Services
{
    public class JsonFileService
    {
        public async Task SaveToFileAsync(string filePath, List<Article> data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            using var stream = File.Create(filePath);
            await JsonSerializer.SerializeAsync(stream, data, options);
        }

        public async Task<List<Article>> LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new List<Article>();
            }

            using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<List<Article>>(stream);
        }
    }
}
