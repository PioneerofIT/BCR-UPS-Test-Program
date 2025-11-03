using System;
using System.Collections.Generic;
using VSP.COMMON.BASE_COMPONENT;

namespace VSP.COMMON.MigrationToolKit
{
    public class IniToJsonConverter
    {
        private readonly VSIniFile _ini;

        public IniToJsonConverter(string iniFilePath)
        {
            _ini = new VSIniFile(iniFilePath);
        }

        /// <summary>
        /// INI를 SYSTEM.JSON 구조로 변환합니다.
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> Convert()
        {
            var result = new Dictionary<string, Dictionary<string, object>>();
            _ini.ReadSections(out var sections);

            foreach (var section in sections)
            {
                _ini.ReadSectionValues(section, out var values);

                foreach (var (key, raw) in values)
                {
                    object value = TryParseValue(raw);
                    string type = GetValueType(value);

                    // 중복 키 처리
                    string finalKey = result.ContainsKey(key) ? $"{section}_{key}" : key;

                    // 중복 키 로그 출력
                    if (result.ContainsKey(key))
                    {
                        //UtilExtern.ShowInitialMessage($"[중복 키 감지] 기존 키 '{key}'가 존재하여 '{finalKey}'로 저장됩니다.");
                        // 또는 로그 시스템이 있다면: Log.Warn($"Duplicate key detected: {key}");
                    }

                    var entry = new Dictionary<string, object>
                    {
                        ["SECTION"] = section,
                        ["TYPE"] = type,
                        ["VALUE"] = value,
                        ["MINIMUM"] = type == "INTEGER" ? 0 : null,
                        ["MAXIMUM"] = type == "INTEGER" ? 10 : null,
                        ["UNIT"] = null,
                        ["DESCRIPTION"] = $"{key} 설정 항목",
                        ["HINT"] = $"{key} 값을 입력하세요.",

                        ["CATEGORY"] = "UNKNOWN", // 항상 UNKNOWN으로 설정
                        ["CATEGORYINDEX"] = "-1",
                        ["CATEGORYITEMS"] = GetCategoryItems(key, value)
                    };

                    result[finalKey] = entry;
                }
            }

            return result;
        }

        private object TryParseValue(string raw)
        {
            if (int.TryParse(raw, out var i)) return i;
            if (bool.TryParse(raw, out var b)) return b;
            return raw;
        }

        private string GetValueType(object value)
        {
            return value switch
            {
                int => "INTEGER",
                bool => "BOOLEAN",
                _ => "STRING"
            };
        }

        private List<string> GetCategoryItems(string key, object value)
        {
            if (key.Contains("PORT", StringComparison.OrdinalIgnoreCase))
            {
                return new List<string>
            {
                "COM1", "COM2", "COM3", "COM4", "COM5",
                "COM6", "COM7", "COM8", "COM9", "COM10"
            };
            }

            if (key.Contains("CIM_TYPE", StringComparison.OrdinalIgnoreCase))
            {
                return new List<string>
            {
                "NONE", "GEM", "GEM_LGIT", "CIM_SUNWODA",
                "CIM_COWELL", "CIM_NVT"
            };
            }

            return new List<string> { value?.ToString() ?? "UNKNOWN" };
        }
    }
}