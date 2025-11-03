using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace VSP.COMMON.BASE_COMPONENT
{
    public class VSJsonParser
    {
        public Dictionary<string, SystemOptionItem> OptionMap { get; set; } = new();

        /// <summary>
        /// SYSTEM.JSON 파일을 로드하여 OptionMap에 매핑합니다.
        /// </summary>
        public Dictionary<string, SystemOptionItem> LoadOptionItems(string path)
        {

            if (!File.Exists(path))
                throw new FileNotFoundException($"❌ 파일이 존재하지 않습니다: {path}");

            string json = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidDataException($"❌ JSON 파일이 비어 있습니다: {path}");

            JsonObject? root;
            try
            {
                var parsed = JsonNode.Parse(json);
                if (parsed is not JsonObject obj)
                    throw new InvalidDataException($"❌ JSON 루트가 객체(JsonObject)가 아닙니다: {path}");

                root = obj;
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"❌ JSON 파싱 중 오류 발생: {path}", ex);
            }

            var map = new Dictionary<string, SystemOptionItem>();

            foreach (var (key, node) in root)
            {
                var obj = node?.AsObject();
                if (obj == null) continue;

                map[key] = new SystemOptionItem
                {
                    Key = key,
                    Section = obj["SECTION"]?.ToString() ?? "",
                    Type = obj["TYPE"]?.ToString() ?? "",
                    Value = obj["VALUE"]?.ToString() ?? "",
                    Minimum = obj["MINIMUM"]?.ToString() ?? "",
                    Maximum = obj["MAXIMUM"]?.ToString() ?? "",
                    Unit = obj["UNIT"]?.ToString() ?? "",
                    Description = obj["DESCRIPTION"]?.ToString() ?? "",
                    Hint = obj["HINT"]?.ToString() ?? "",
                    CategoryItems = ParseStringList(obj["CATEGORYITEMS"]),
                    Category = obj["CATEGORY"]?.ToString() ?? "",
                    CategoryIndex = obj["CATEGORYINDEX"]?.ToString() ?? ""
                };
            }

            OptionMap = map;
            return map;
        }

        /// <summary>
        /// OptionMap을 SYSTEM.JSON 구조로 저장합니다.
        /// </summary>
        public void Save(string path)
        {
            Debug.WriteLine($"💾 CVSSystemOption.Save 시작 - {path}");

            var root = new JsonObject();

            foreach (var (key, item) in OptionMap)
            {
                var obj = new JsonObject
                {
                    ["SECTION"] = item.Section,
                    ["TYPE"] = item.Type,
                    ["VALUE"] = item.Value,
                    ["MINIMUM"] = item.Minimum,
                    ["MAXIMUM"] = item.Maximum,
                    ["UNIT"] = item.Unit,
                    ["DESCRIPTION"] = item.Description,
                    ["HINT"] = item.Hint,
                    ["CATEGORY"] = item.Category,
                    ["CATEGORYINDEX"] = item.CategoryIndex,
                    ["CATEGORYITEMS"] = BuildJsonArray(item.CategoryItems)

                };

                root[key] = obj;
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                // ❌ TypeInfoResolver 생략 (버전 호환)
            };

            // JsonNode를 직접 직렬화
            string json = JsonSerializer.Serialize(root, options);
            File.WriteAllText(path, json);

            Debug.WriteLine($"✅ CVSSystemOption 저장 완료 - 항목 수: {OptionMap.Count}");
        }

        /// <summary>
        /// JsonNode를 Dictionary<string, string>으로 변환합니다.
        /// </summary>
        private Dictionary<string, string> ParseStringMap(JsonNode? node)
        {
            var result = new Dictionary<string, string>();
            if (node is JsonObject obj)
            {
                foreach (var (k, v) in obj)
                    result[k] = v?.ToString() ?? "";
            }
            return result;
        }

        /// <summary>
        /// JsonNode를 List<string>으로 변환합니다.
        /// </summary>
        private List<string> ParseStringList(JsonNode? node)
        {
            var result = new List<string>();
            if (node is JsonArray arr)
            {
                foreach (var item in arr)
                    result.Add(item?.ToString() ?? "");
            }
            return result;
        }

        /// <summary>
        /// Dictionary<string, string>을 JsonObject로 변환합니다.
        /// </summary>
        private JsonObject BuildJsonObject(Dictionary<string, string> map)
        {
            var obj = new JsonObject();
            foreach (var (key, value) in map)
                obj[key] = value;
            return obj;
        }

        /// <summary>
        /// List<string>을 JsonArray로 변환합니다.
        /// </summary>
        private JsonArray BuildJsonArray(IEnumerable<string> list)
        {
            var arr = new JsonArray();
            foreach (var item in list)
                arr.Add(item);
            return arr;
        }
    }
}