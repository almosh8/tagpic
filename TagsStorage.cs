﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TagPic
{
    public static class TagsStorage
    {
        private static Dictionary<string, int> _tags;

        static TagsStorage()
        {
            // Initialize the dictionary with tags from the text file
            _tags = new Dictionary<string, int>();

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
                }
            }
        }

        public static List<string> GetAllTags()
        {
            return new List<string>(_tags.Keys);
        }
        public static IEnumerable<string> GetTagsWithPrefix(string prefix)
        {
            return _tags.Keys.Where(tag => tag.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void AddTags(string tagString)
        {
            string[] tags = tagString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string tag in tags)
            {
                string normalizedTag = tag.Trim().ToLowerInvariant();

                if (!_tags.ContainsKey(normalizedTag))
                {
                    _tags.Add(normalizedTag, 1);
                }
                else
                {
                    _tags[normalizedTag]++;
                }
            }

            SaveTagsToFile();
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
