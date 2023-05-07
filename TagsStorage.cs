using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagPic
{
    public static class TagsStorage
    {
        private static Dictionary<string, int> _tags;
        private static List<string> _tagsList;

        static TagsStorage()
        {
            // Initialize the dictionary with tags from the text file
            _tags = new Dictionary<string, int>();
            _tagsList = new List<string>();

            string tagsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "tags.txt");

            if (!File.Exists(tagsFilePath))
            {
                File.Create(tagsFilePath).Close();
            }

            string[] lines = File.ReadAllLines(tagsFilePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string tag = parts[0].Trim();
                    int count = int.Parse(parts[1].Trim());
                    _tags[tag] = count;
                    _tagsList.Add(tag);
                }
            }
        }

        public static List<string> GetAllTags()
        {
            return _tagsList;
        }
        public static IEnumerable<string> GetTagsWithPrefix(string prefix)
        {
            return _tagsList.Where(tag => tag.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void AddTags(string tagString)
        {
            string[] tags = tagString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string tag in tags)
            {
                AddTag(tag);

                SaveTagsToFile();
            }
        }

        public static void AddTag(string tag)
        {
            string normalizedTag = tag.Trim().ToLowerInvariant();

            if (!_tags.ContainsKey(normalizedTag))
            {
                _tags.Add(normalizedTag, 1);
                _tagsList.Add(normalizedTag);
            }
            else
            {
                _tags[normalizedTag]++;
            }
        }

        private static void SaveTagsToFile()
        {
            string tagsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "tags.txt");

            List<string> lines = new List<string>();

            foreach (var tag in _tags.OrderBy(x => x.Key))
            {
                lines.Add(tag.Key + ": " + tag.Value);
            }

            File.WriteAllLines(tagsFilePath, lines);
        }
    }
}
