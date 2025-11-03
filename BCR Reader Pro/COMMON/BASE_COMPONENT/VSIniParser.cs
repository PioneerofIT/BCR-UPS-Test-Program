using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VSP.COMMON.BASE_COMPONENT
{
    // 추상 클래스: INI 파일에 접근하는 기본 인터페이스 정의
    public abstract class CustomIniFile
    {
        public string FileName { get; }

        protected CustomIniFile(string fileName)
        {
            FileName = fileName;
        }

        public abstract bool SectionExists(string section);
        public abstract string ReadString(string section, string key, string defaultValue);
        public abstract void WriteString(string section, string key, string value);
        public abstract void ReadSection(string section, out string[] keys);
        public abstract void ReadSections(out string[] sections);
        public abstract void ReadSectionValues(string section, out (string key, string value)[] values);
        public abstract void EraseSection(string section);
        public abstract void DeleteKey(string section, string key);
        public abstract bool ValueExists(string section, string key);
        public abstract void UpdateFile();

        public virtual int ReadInteger(string section, string key, int defaultValue) =>
            int.TryParse(ReadString(section, key, defaultValue.ToString()), out var result) ? result : defaultValue;

        public virtual void WriteInteger(string section, string key, int value) =>
            WriteString(section, key, value.ToString());

        public virtual bool ReadBool(string section, string key, bool defaultValue) =>
            bool.TryParse(ReadString(section, key, defaultValue.ToString()), out var result) ? result : defaultValue;

        public virtual void WriteBool(string section, string key, bool value) =>
            WriteString(section, key, value.ToString());
    }

    // 구현 클래스: 실제 .ini 파일을 메모리에 로딩하고 저장
    public class VSIniFile : CustomIniFile
    {
        private readonly Dictionary<string, Dictionary<string, string>> _data;

        // 기본값을 true로 설정
        public bool AutoSave { get; set; } = true;

        public VSIniFile(string fileName) : base(fileName)
        {
            _data = new(StringComparer.OrdinalIgnoreCase);
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (!File.Exists(FileName))
                return;

            string currentSection = "";
            int totalSections = 0;
            int totalKeys = 0;
            List<string> duplicateKeyWarnings = new();

            var lines = File.ReadAllLines(FileName);

            for (int i = 0; i < lines.Length; i++)
            {
                string rawLine = lines[i];
                string line = rawLine.TrimStart('\uFEFF').Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#"))
                    continue;

                var match = Regex.Match(line, @"^\s*\[(.+?)\]");
                if (match.Success)
                {
                    string sectionName = match.Groups[1].Value.Trim();
                    currentSection = sectionName;

                    if (!_data.ContainsKey(currentSection))
                    {
                        _data[currentSection] = new(StringComparer.OrdinalIgnoreCase);
                        totalSections++;
                    }
                    continue;
                }

                int idx = line.IndexOf('=');
                if (idx > 0)
                {
                    string key = line[..idx].Trim();
                    string value = line[(idx + 1)..].Trim();

                    if (!_data.ContainsKey(currentSection))
                        _data[currentSection] = new(StringComparer.OrdinalIgnoreCase);

                    if (_data[currentSection].ContainsKey(key))
                        duplicateKeyWarnings.Add($"[{currentSection}]:{key}");

                    _data[currentSection][key] = value;
                    totalKeys++;
                }
                else
                {
                    Console.WriteLine($"[LINE {i + 1}] ⚠️ 키-값 형식이 아님: {line}");
                }
            }

            Console.WriteLine($"📄 {Path.GetFileName(FileName)} → 섹션: {totalSections}, 키: {totalKeys}");

            if (duplicateKeyWarnings.Count > 0)
            {
                Console.WriteLine("⚠️ 중복된 키가 존재합니다:\n" +
                                  string.Join("\n", duplicateKeyWarnings.Distinct()));
            }
        }

        public override bool SectionExists(string section) =>
            _data.ContainsKey(section);

        public override string ReadString(string section, string key, string defaultValue) =>
            _data.TryGetValue(section, out var sec) && sec.TryGetValue(key, out var val) ? val : defaultValue;

        public override void WriteString(string section, string key, string value)
        {
            if (!_data.ContainsKey(section))
                _data[section] = new(StringComparer.OrdinalIgnoreCase);

            _data[section][key] = value;

            if (AutoSave)
                UpdateFile();
        }

        public override int ReadInteger(string section, string key, int defaultValue)
        {
            string raw = ReadString(section, key, defaultValue.ToString());
            return int.TryParse(raw, out var result) ? result : defaultValue;
        }

        public override void WriteInteger(string section, string key, int value)
        {
            WriteString(section, key, value.ToString());
            if (AutoSave)
                UpdateFile();
        }

        public override bool ReadBool(string section, string key, bool defaultValue)
        {
            string raw = ReadString(section, key, defaultValue.ToString());
            return bool.TryParse(raw, out var result) ? result : defaultValue;
        }

        public override void WriteBool(string section, string key, bool value)
        {
            WriteString(section, key, value.ToString());
            if (AutoSave)
                UpdateFile();
        }

        public override void ReadSection(string section, out string[] keys)
        {
            keys = _data.TryGetValue(section, out var sec)
                ? new List<string>(sec.Keys).ToArray()
                : Array.Empty<string>();
        }

        public override void ReadSections(out string[] sections) =>
            sections = new List<string>(_data.Keys).ToArray();

        public override void ReadSectionValues(string section, out (string key, string value)[] pairs)
        {
            if (_data.TryGetValue(section, out var sec))
            {
                var result = new List<(string, string)>();
                foreach (var kv in sec)
                    result.Add((kv.Key, kv.Value));
                pairs = result.ToArray();
            }
            else
                pairs = Array.Empty<(string, string)>();
        }

        public override void EraseSection(string section) =>
            _data.Remove(section);

        public override void DeleteKey(string section, string key)
        {
            if (_data.TryGetValue(section, out var sec))
                sec.Remove(key);
        }

        public override bool ValueExists(string section, string key) =>
            _data.TryGetValue(section, out var sec) && sec.ContainsKey(key);

        public override void UpdateFile()
        {
            using var writer = new StreamWriter(FileName);
            foreach (var (section, entries) in _data)
            {
                writer.WriteLine($"[{section}]");
                foreach (var (key, value) in entries)
                    writer.WriteLine($"{key}={value}");
                writer.WriteLine();
            }
        }

        ~VSIniFile()
        {
            if (!AutoSave) // AutoSave가 꺼져 있을 때만 소멸자에서 저장
                UpdateFile();
        }
    }
}