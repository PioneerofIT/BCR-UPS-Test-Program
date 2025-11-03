
using System.Text;
using System.IO;
using VSP.COMMON;
using System.Text.RegularExpressions;
using VSP.COMMON.BASE_COMPONENT;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using VSP.GUI.BASE_FRAME;
using VSP.COMMON;



namespace VSP.COMMON
{
    /* ==========================================================================
    Description	: Lang public static hsjangstatic
    ========================================================================== */
    public static class Lang
    {
        public static CVSLanguageManager Manager => CVSLanguageManager.Instance;
        public static UILanguage UI => CVSLanguageManager.Instance.UILang;
        public static MotorLanguage MTR => CVSLanguageManager.Instance.MotorLang;
        public static AlarmLanguage Alarm => CVSLanguageManager.Instance.AlarmLang;
    }

    public sealed class CVSLanguageManager
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화
        // ============================================================
        #region ObjectLifecycle

        //private static CVSLanguageManager instance;

        private CVSLanguageManager()
        {
            UILang = new UILanguage();
            MotorLang = new MotorLanguage();
            AlarmLang = new AlarmLanguage();
            IoLang = new IoLanguage();
        }

        ~CVSLanguageManager()
        {
        }

       // public static CVSLanguageManager Instance => instance ??= new CVSLanguageManager();
        private static CVSLanguageManager? instance;

        public static CVSLanguageManager Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                try
                {
                    instance = new CVSLanguageManager();
                    return instance;
                }
                catch (Exception ex)
                {
                    // 로깅 또는 사용자 정의 예외 던지기
                    throw new InvalidOperationException("CVSLanguageManager 생성 실패", ex);
                }
            }
        }

        public void Initialize()
        {
            //생성자 내부에서 Initialize를 호출하면 안 돼요,
        }

        #endregion

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties
        //private readonly List<WeakReference<IVsFrame>> _viewObjects = new();
        public UILanguage UILang { get; private set; }
        public MotorLanguage MotorLang { get; private set; }
        public AlarmLanguage AlarmLang { get; private set; }
        public IoLanguage IoLang { get; private set; }

        #endregion

        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        #region Accessors
        public void ChangeLanguage(int LanguageType)
        {
            if (LanguageType == UILang.ChangeLanguage)
                return;


            UILang.ChangeLanguage = LanguageType;
            MotorLang.ChangeLanguage = LanguageType;
            AlarmLang.ChangeLanguage = LanguageType;
            IoLang.ChangeLanguage = LanguageType;



            if (CVS_VIEW_MANAGER.Instance != null)
                CVS_VIEW_MANAGER.Instance.LocalizeAll();

        }
        public void Load(string langCode)
        {
            //UILang.Load($"Lang/UI_{langCode}.INI");
            //MotorLang.Load($"Lang/MOTOR_{langCode}.INI");
            //AlarmLang.Load($"Lang/ALARM.mdb", langCode);
        }
        //// 뷰 오브젝트 등록
        //public void RegisterViewObject(IVsFrame obj)
        //{
        //    // 중복 등록 방지
        //    if (_viewObjects.Any(wr => wr.TryGetTarget(out var target) && target == obj))
        //        return;

        //    _viewObjects.Add(new WeakReference<IVsFrame>(obj));
        //}

        //// 언어 변경 통지
        //public void NotifyAllViewObjects()
        //{
        //    _viewObjects.RemoveAll(wr => !wr.TryGetTarget(out _));

        //    foreach (var wr in _viewObjects)
        //    {
        //        if (wr.TryGetTarget(out var target))
        //            target.Localize();
        //    }
        //} 



        #endregion

        // ============================================================
        // Description : [4] Internal Logic / Validation (데이터 검증)
        // ============================================================
        #region Internal

        // 추후 언어 유효성 검사 등 로직 필요시 여기에 작성

        #endregion

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        #region Unclassified

        // 필요 시 동적 언어 변경, 핫 리로드 기능 확장 가능

        #endregion
    }

    // ============================================================
    // Description : CLASS GUI LANGUAGE
    // ============================================================
    public class UILanguage
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화
        // ============================================================

        public UILanguage() 
        {
            Load();
        }

        public void Load()
        {
            EnsureLangFolderExists(); // <== Lang 폴더 없으면 복사
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.ENGLISH], "LANG/UI_ENG.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.KOREAN], "LANG/UI_KOR.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.CHINA_SIM], "LANG/UI_CHN_SIMPLIFIED.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.CHINA_TRAD], "LANG/UI_CHN_TRADITIONAL.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.VIETNAM], "LANG/UI_VTN.INI");
            //AnalyzeIniFile("Lang/UI_ENG.INI");
        }

        private void LoadLanguageFile(string langCode, string path)
        {
            if (!File.Exists(path))
            {
                UtilExtern.ShowInitialMessage($"📛 언어 파일이 존재하지 않습니다: {path}");
                return;
            }

            List<string> duplicateKeyWarnings = new();
            var langMap = new Dictionary<string, Dictionary<string, string>>();
            _languageMap[langCode] = langMap;

            try
            {
                string[] lines = File.ReadAllLines(path);
                int totalSections = 0;
                int totalKeys = 0;
                string currentSection = "";

                for (int i = 0; i < lines.Length; i++)
                {
                    string rawLine = lines[i];
                    string line = rawLine.TrimStart('\uFEFF').Trim();

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#"))
                    {
                        //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ❎ 주석/빈 줄: {line}");
                        continue;
                    }

                    var sectionMatch = Regex.Match(line, @"^\s*\[(.+?)\]");
                    if (sectionMatch.Success)
                    {
                        string sectionName = sectionMatch.Groups[1].Value.Trim().ToUpperInvariant();
                        currentSection = sectionName;

                        if (!langMap.ContainsKey(currentSection))
                        {
                            langMap[currentSection] = new(StringComparer.OrdinalIgnoreCase);
                            totalSections++;
                        }

                        //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ✅ 섹션: [{currentSection}]");
                        continue;
                    }

                    int idx = line.IndexOf('=');
                    if (idx > 0)
                    {
                        string key = line[..idx].Trim();
                        string value = line[(idx + 1)..].Trim();

                        if (!langMap.ContainsKey(currentSection))
                        {
                            langMap[currentSection] = new(StringComparer.OrdinalIgnoreCase);
                            totalSections++;
                        }

                        string fullKey = $"[{currentSection}]:{key}";
                        var sectionMap = langMap[currentSection];
                        if (!sectionMap.TryAdd(key, value))
                        {
                            duplicateKeyWarnings.Add(fullKey);
                            //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ⚠️ 중복 키: {fullKey}");
                        }
                        else
                        {
                            //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ✅ 키: {key} = {value}");
                            totalKeys++;
                        }
                    }
                    else
                    {
                        UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ⚠️ 유효하지 않은 구문: {line}");
                    }
                }

               // UtilExtern.ShowInitialMessage($"📄 [{langCode}] {Path.GetFileName(path)} → 섹션: {totalSections}, 키: {totalKeys}");
            }
            catch (Exception ex)
            {
                UtilExtern.ShowInitialMessage($"[LANG] 파싱 오류 - 파일: {path}\n{ex.Message}");
                return;
            }

            if (duplicateKeyWarnings.Count > 0)
            {
                string message = $"다음 항목에 중복된 키가 존재합니다:\n\n{string.Join("\n", duplicateKeyWarnings.Distinct())}";
                UtilExtern.ShowInitialMessage(message);
            }

            //if (_languageMap.TryGetValue(langCode, out var sectionMap))
            //{
            //    foreach (var section in sectionMap)
            //    {
            //        UtilExtern.ShowInitialMessage($"📘 [{langCode}][{section.Key}] → {section.Value.Count}개 키 로드됨");

            //        // (선택) 키 전체 보기
            //        foreach (var kvp in section.Value)
            //            UtilExtern.ShowInitialMessage($"    🔑 {kvp.Key} = {kvp.Value}");
            //    }
            //}

        }
        


        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties

        // 3중 맵: 언어 → 섹션 → 키 → 값
        //EX )    한국어->[QUERY]-> QryAdd-> Do you want to add this data?
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _languageMap = new();

        // 현재 선택된 언어 코드 (예: "ENG", "KOR", "CHN")
        public int ChangeLanguage { get; set; } = (int)LanguageType.KOREAN;

        public string Get(string section, string key, string fallback = "")
        {
            section = section.ToUpper();

            if (_languageMap.TryGetValue(LanguageNameType.Name[ChangeLanguage], out var langMap))
            {
                if (langMap.TryGetValue(section, out var sectionMap))
                {
                    if (sectionMap.TryGetValue(key, out var result))
                    {
                        return result;
                    }
                }
            }

            return fallback;
        }

        public string GetQry(string key, string fallback = "") => Get("QUERY", key, fallback);
        public string GetGui(string key, string fallback = "") => Get("GUI", key, fallback);
        public string GetMsg(string key, string fallback = "") => Get("MSG", key, fallback);
        public string GetManual(string key, string fallback = "") => Get("MANUAL", key, fallback);
        public string GetSeq(string key, string fallback = "") => Get("SEQ", key, fallback);


        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        #region Accessors

        #endregion

        // ============================================================
        // Description : [4] Internal Logic / Validation ( 데이터 검증)
        // ============================================================
        #region Internal

        #endregion

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        private void EnsureLangFolderExists()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string exeLangPath = Path.Combine(exePath, "LANG");

            if (!Directory.Exists(exeLangPath))
            {
                string projectLangPath = Path.Combine(exePath, @"..\..\..\..\LANG");
                string fullSourcePath = Path.GetFullPath(projectLangPath);

                if (Directory.Exists(fullSourcePath))
                {
                    UtilExtern.CopyDirectory(fullSourcePath, exeLangPath);
                }
                else
                {
                    UtilExtern.ShowInitialMessage("[LANG] LANG 폴더가 실행경로에도, 프로젝트에도 없습니다.");
                    //Console.WriteLine("[Lang] Lang 폴더가 실행경로에도, 프로젝트에도 없습니다.");
                }
            }
        }

   
        // TODO: 향후 다국어 hot-reload, 누락 Key 검사, 로그 확장 등
    }
    // ============================================================
    // Description : CLASS IO LANGUAGE
    // ============================================================
    public class IoLanguage
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화
        // ============================================================

        public IoLanguage()
        {
            Load();
        }

        public void Load()
        {
            EnsureLangFolderExists(); // <== Lang 폴더 없으면 복사
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.ENGLISH], "LANG/IO_ENG.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.KOREAN], "LANG/IO_KOR.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.CHINA_SIM], "LANG/IO_CHN_SIMPLIFIED.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.CHINA_TRAD], "LANG/IO_CHN_TRADITIONAL.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.VIETNAM], "LANG/IO_VTN.INI");
        }

        private void LoadLanguageFile(string langCode, string path)
        {
            if (!File.Exists(path))
            {
                UtilExtern.ShowInitialMessage($"📛 언어 파일이 존재하지 않습니다: {path}");
                return;
            }

            List<string> duplicateKeyWarnings = new();
            var langMap = new Dictionary<string, Dictionary<string, string>>();
            _languageMap[langCode] = langMap;

            try
            {
                string[] lines = File.ReadAllLines(path);
                int totalSections = 0;
                int totalKeys = 0;
                string currentSection = "";

                for (int i = 0; i < lines.Length; i++)
                {
                    string rawLine = lines[i];
                    string line = rawLine.TrimStart('\uFEFF').Trim();

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#"))
                    {
                        //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ❎ 주석/빈 줄: {line}");
                        continue;
                    }

                    var sectionMatch = Regex.Match(line, @"^\s*\[(.+?)\]");
                    if (sectionMatch.Success)
                    {
                        string sectionName = sectionMatch.Groups[1].Value.Trim().ToUpperInvariant();
                        currentSection = sectionName;

                        if (!langMap.ContainsKey(currentSection))
                        {
                            langMap[currentSection] = new(StringComparer.OrdinalIgnoreCase);
                            totalSections++;
                        }

                        //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ✅ 섹션: [{currentSection}]");
                        continue;
                    }

                    int idx = line.IndexOf('=');
                    if (idx > 0)
                    {
                        string key = line[..idx].Trim();
                        string value = line[(idx + 1)..].Trim();

                        if (!langMap.ContainsKey(currentSection))
                        {
                            langMap[currentSection] = new(StringComparer.OrdinalIgnoreCase);
                            totalSections++;
                        }

                        string fullKey = $"[{currentSection}]:{key}";
                        var sectionMap = langMap[currentSection];
                        if (!sectionMap.TryAdd(key, value))
                        {
                            duplicateKeyWarnings.Add(fullKey);
                            //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ⚠️ 중복 키: {fullKey}");
                        }
                        else
                        {
                            //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ✅ 키: {key} = {value}");
                            totalKeys++;
                        }
                    }
                    else
                    {
                        UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ⚠️ 유효하지 않은 구문: {line}");
                    }
                }

                 UtilExtern.ShowInitialMessage($"📄 [I/O{langCode}] {Path.GetFileName(path)} → 섹션: {totalSections}, 키: {totalKeys}");
            }
            catch (Exception ex)
            {
                UtilExtern.ShowInitialMessage($"[LANG] 파싱 오류 - 파일: {path}\n{ex.Message}");
                return;
            }

            if (duplicateKeyWarnings.Count > 0)
            {
                string message = $"다음 항목에 중복된 키가 존재합니다:\n\n{string.Join("\n", duplicateKeyWarnings.Distinct())}";
                UtilExtern.ShowInitialMessage(message);
            }

            //if (_languageMap.TryGetValue(langCode, out var sectionMap))
            //{
            //    foreach (var section in sectionMap)
            //    {
            //        UtilExtern.ShowInitialMessage($"📘 [{langCode}][{section.Key}] → {section.Value.Count}개 키 로드됨");

            //        // (선택) 키 전체 보기
            //        foreach (var kvp in section.Value)
            //            UtilExtern.ShowInitialMessage($"    🔑 {kvp.Key} = {kvp.Value}");
            //    }
            //}
        }

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties

        // 3중 맵: 언어 → 섹션 → 키 → 값
        //EX )    한국어->[QUERY]-> QryAdd-> Do you want to add this data?
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _languageMap = new();

        // 현재 선택된 언어 코드 (예: "ENG", "KOR", "CHN")
        public int ChangeLanguage { get; set; } = (int)LanguageType.KOREAN;

        public string Get(string section, string key, string fallback = "")
        {
            section = section.ToUpper();

            if (_languageMap.TryGetValue(LanguageNameType.Name[ChangeLanguage], out var langMap))
            {
                if (langMap.TryGetValue(section, out var sectionMap))
                {
                    if (sectionMap.TryGetValue(key, out var result))
                    {
                        return result;
                    }
                }
            }

            return fallback;
        }

        public string GetInput(string key, string fallback = "") => Get("INPUT", key, fallback);
        public string GetOutPut(string key, string fallback = "") => Get("OUTPUT", key, fallback);
       

        #endregion

        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        #region Accessors

        #endregion

        // ============================================================
        // Description : [4] Internal Logic / Validation ( 데이터 검증)
        // ============================================================
        #region Internal

        #endregion

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        private void EnsureLangFolderExists()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string exeLangPath = Path.Combine(exePath, "LANG");

            if (!Directory.Exists(exeLangPath))
            {
                string projectLangPath = Path.Combine(exePath, @"..\..\..\..\LANG");
                string fullSourcePath = Path.GetFullPath(projectLangPath);

                if (Directory.Exists(fullSourcePath))
                {
                    UtilExtern.CopyDirectory(fullSourcePath, exeLangPath);
                }
                else
                {
                    UtilExtern.ShowInitialMessage("[LANG] LANG 폴더가 실행경로에도, 프로젝트에도 없습니다.");
                }
            }
        }
    }

    // ============================================================
    // Description : CLASS Motor LANGUAGE
    // ============================================================
    public class MotorLanguage
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화
        // ============================================================


        public MotorLanguage()
        {
            Load();
        }

        public void Load()
        {
            EnsureLangFolderExists(); // <== CONFIG 폴더 없으면 복사

            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.ENGLISH], "LANG/MTR_ENG.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.KOREAN], "LANG/MTR_KOR.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.VIETNAM], "LANG/MTR_VTN.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.CHINA_SIM], "LANG/MTR_CHN_SIMPLIFIED.INI");
            LoadLanguageFile(LanguageNameType.Name[(int)LanguageType.CHINA_TRAD], "LANG/MTR_CHN_TRADITIONAL.INI");
            //LoadMotorFile("CHN_TRADITIONAL", "CONFIG/MOTOR_CHN_TRADITIONAL.INI");
            //AnalyzeMotorFile("CONFIG/MOTOR_ENG.INI");
        }

        private void LoadLanguageFile(string langCode, string path)
        {
            if (!File.Exists(path))
            {
                UtilExtern.ShowInitialMessage($"📛 모터 파일이 존재하지 않습니다: {path}");
                return;
            }

            List<string> duplicateKeyWarnings = new();
            var langMap = new Dictionary<string, Dictionary<string, string>>();
            _motorMap[langCode] = langMap;

            try
            {
                string[] lines = File.ReadAllLines(path);
                string currentSection = "";
                int totalSections = 0;
                int totalKeys = 0;

                for (int i = 0; i < lines.Length; i++)
                {
                    string rawLine = lines[i];
                    string line = rawLine.TrimStart('\uFEFF').Trim();

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("#"))
                        continue;

                    var sectionMatch = Regex.Match(line, @"^\s*\[(.+?)\]");
                    if (sectionMatch.Success)
                    {
                        string sectionName = sectionMatch.Groups[1].Value.Trim().ToUpperInvariant();
                        currentSection = sectionName;

                        if (!langMap.ContainsKey(currentSection))
                        {
                            langMap[currentSection] = new(StringComparer.OrdinalIgnoreCase);
                            totalSections++;
                        }

                        continue;
                    }

                    int idx = line.IndexOf('=');
                    if (idx > 0)
                    {
                        string key = line[..idx].Trim();
                        string value = line[(idx + 1)..].Trim();

                        if (!langMap.ContainsKey(currentSection))
                        {
                            langMap[currentSection] = new(StringComparer.OrdinalIgnoreCase);
                            totalSections++;
                        }

                        var sectionMap = langMap[currentSection];
                        string fullKey = $"[{currentSection}]:{key}";

                        if (!sectionMap.TryAdd(key, value))
                        {
                            duplicateKeyWarnings.Add(fullKey);
                            //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ⚠️ 중복 키: {fullKey}");
                        }
                        else
                        {
                            totalKeys++;
                            //UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ✅ 키: {key} = {value}");
                        }
                    }
                    else
                    {
                        UtilExtern.ShowInitialMessage($"[LINE {i + 1}] ⚠️ 유효하지 않은 구문: {line}");
                    }
                }

                UtilExtern.ShowInitialMessage($"📂 [{langCode}] {Path.GetFileName(path)} → 섹션: {totalSections}, 키: {totalKeys}");
            }
            catch (Exception ex)
            {
                UtilExtern.ShowInitialMessage($"[MOTOR] 파싱 오류 - 파일: {path}\n{ex.Message}");
            }

            if (duplicateKeyWarnings.Count > 0)
            {
                string msg = $"다음 항목에 중복된 키가 존재합니다:\n\n{string.Join("\n", duplicateKeyWarnings.Distinct())}";
                UtilExtern.ShowInitialMessage(msg);
            }
        }



        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties

        // 3중 맵: 언어 → 섹션 → 키 → 값
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _motorMap = new();

        // 현재 선택된 언어 코드 (예: "ENG", "KOR", "CHN")
        public int ChangeLanguage { get; set; } = (int)LanguageType.KOREAN;




        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================

        public string Get(string section, string key, string fallback = "")
        {
            section = section.ToUpper();

            if (_motorMap.TryGetValue(LanguageNameType.Name[ChangeLanguage], out var langMap))
            {
                if (langMap.TryGetValue(section, out var sectionMap))
                {
                    if (sectionMap.TryGetValue(key, out var result))
                    {
                        return result;
                    }
                }
            }

            return fallback;
        }
     

        public string GetMtrStr(string section, string ident, string fallback = "") =>Get(section, ident, fallback);

        public string GetMtrPosStr(int motorIndex, int posIndex, string fallback = "")
        {
            string section = $"MOTOR_{motorIndex:00}";
            string ident = $"Position_{posIndex:00}";
            return GetMtrStr(section, ident, fallback);
        }

        public string GetMtrName(int motorIndex, string fallback = "")
        {
            string section = $"MOTOR_{motorIndex:00}";
            return Get(section, "Name", fallback);
        }

        //public string GetMotorName(string motorKey, string fallback = "") =>
        //    Get(motorKey, "Name", fallback);

        //public string GetMotorPos(string motorKey, int posIndex, string fallback = "") =>
        //    Get(motorKey, $"Position_{posIndex:00}", fallback);

        public string GetTimer(int index, string fallback = "") =>
            Get("TIMER COUNTER", $"CntTmr_{index:00}", fallback);

        #endregion


        // ============================================================
        // Description : [4] Internal Logic / Validation ( 데이터 검증)
        // ============================================================
        #region Internal

        private void EnsureLangFolderExists()
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string exeConfigPath = Path.Combine(exePath, "LANG");

            if (!Directory.Exists(exeConfigPath))
            {
                string projectConfigPath = Path.Combine(exePath, @"..\..\..\..\LANG");
                string fullSourcePath = Path.GetFullPath(projectConfigPath);

                if (Directory.Exists(fullSourcePath))
                {
                    CopyDirectory(fullSourcePath, exeConfigPath);
                }
                else
                {
                    UtilExtern.ShowInitialMessage("[MOTOR] LANG 폴더가 실행경로에도, 프로젝트에도 없습니다.");
                }
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(targetDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir); // 재귀 복사
            }
        }

        #endregion


        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        #region Unclassified

        // TODO: 포지션 누락 감지, 타이머 유효성, 디버그 모드 표시 등

        #endregion
    }



    /// // ============================================================
    // Description : Class Alarm Language
    // ============================================================
    public class AlarmLanguage
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화
        // ============================================================
     

        public AlarmLanguage()
        {
            Load();
        }

        public void Load()
        {
            EnsureLangFolderExists();
            string DbPath = Path.Combine(CGlobal.Instance.DataDir, CongfigFileTypeNames.ERRDB);
            LoadErrorDatabase(DbPath);
        }

        private void LoadErrorDatabase(string dbPath)
        {
            if (!File.Exists(dbPath))
            {
                string OrgPath = Path.Combine(CGlobal.Instance.ExecuteDir, "DATA", CongfigFileTypeNames.ERRDB);

                if (File.Exists(OrgPath))
                {
                    UtilExtern.CopyFile(OrgPath, CGlobal.Instance.DataDir);
                }
                else
                {
                    UtilExtern.ShowInitialMessage($"The file does not exist in the source path either. {CongfigFileTypeNames.ERRDB} ");
                    return;
                }
            }

            try
            {
                using var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
                conn.Open();

                using var cmd = new SQLiteCommand("SELECT * FROM ErrDefine", conn);
                using var reader = cmd.ExecuteReader();

                // 언어별 초기화
                for (int i = 0; i < (int)LanguageType.Max; i++)
                {
                    string langName = LanguageNameType.Name[i];
                    _alarmMap[langName] = new Dictionary<int, Dictionary<string, string>>();
                }

                int totalCount = 0;

                while (reader.Read())
                {
                    if (reader["Code"] == DBNull.Value)
                        break;

                    int code = Convert.ToInt32(reader["Code"]);
                    string level = reader["Level"]?.ToString() ?? "";
                    string part = reader["EquipmentPart"]?.ToString() ?? "";
                    string picture = reader["ErrorPicture"]?.ToString() ?? "";

                    for (int i = 0; i < (int)LanguageType.Max; i++)
                    {
                        LanguageType langType = (LanguageType)i;
                        string langName = LanguageNameType.Name[i];

                        string suffix = langType switch
                        {
                            LanguageType.KOREAN => "",
                            LanguageType.ENGLISH => "_E",
                            LanguageType.VIETNAM => "_V",
                            LanguageType.CHINA_SIM => "_CHN_SIM",
                            LanguageType.CHINA_TRAD => "_CHN_TRAD",
                            _ => ""
                        };

                        var entry = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", reader[$"Name{suffix}"]?.ToString() ?? "" },
                { "Cause", reader[$"Cause{suffix}"]?.ToString() ?? "" },
                { "Action", reader[$"Action{suffix}"]?.ToString() ?? "" },
                { "Level", level },
                { "Part", part },
                { "Picture", picture }
            };

                        _alarmMap[langName][code] = entry;
                    }

                    totalCount++;
                }

                UtilExtern.ShowInitialMessage($"📂 오류 정의 DB → 항목 수: {totalCount}");
            }
            catch (Exception ex)
            {
                UtilExtern.ShowInitialMessage($"[ALARM] DB 로딩 오류: {ex.Message}");
            }
        }


        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================

        private readonly Dictionary<string, Dictionary<int, Dictionary<string, string>>> _alarmMap = new();

        public int ChangeLanguage { get; set; } = (int)LanguageType.KOREAN;



         public string Get(int code, string field, string fallback = "")
        {
            if (_alarmMap.TryGetValue(LanguageNameType.Name[ChangeLanguage], out var langMap))
            {
                if (langMap.TryGetValue(code, out var entry))
                {
                    if (entry.TryGetValue(field, out var value))
                    {
                        return value;
                    }
                }
            }

            return fallback;
        }

        public string Name(int code, string fallback = "") => Get(code, "Name", fallback);
        public string Cause(int code, string fallback = "") => Get(code, "Cause", fallback);
        public string Action(int code, string fallback = "") => Get(code, "Action", fallback);
        public string Level(int code, string fallback = "") => Get(code, "Level", fallback);
        public string Part(int code, string fallback = "") => Get(code, "Part", fallback);
        public string Picture(int code, string fallback = "") => Get(code, "Picture", fallback);

     


        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================



        // ============================================================
        // Description : [4] Internal Logic / Validation (데이터 검증)
        // ============================================================


        private void EnsureLangFolderExists()
        {
            string exePath = AppContext.BaseDirectory;
            string exeConfigPath = Path.Combine(exePath, "DATA");

            if (!Directory.Exists(exeConfigPath))
            {
                string projectConfigPath = Path.Combine(exePath, @"..\..\..\..\DATA");
                string fullSourcePath = Path.GetFullPath(projectConfigPath);

                if (Directory.Exists(fullSourcePath))
                    CopyDirectory(fullSourcePath, exeConfigPath);
                else
                    UtilExtern.ShowInitialMessage("[ALARM] DATA 폴더가 실행경로에도, 프로젝트에도 없습니다.");
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);

            foreach (var dir in Directory.GetDirectories(sourceDir))
                CopyDirectory(dir, Path.Combine(targetDir, Path.GetFileName(dir)));
        }


        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================

        // 추후 기능이 추가되면 이곳에 정리

   
    }
}
#endregion
