using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
//using VSP.GUI.COMMON;


    namespace VSP.COMMON
{
    /* ==========================================================================
    Description	: VS_UTILS public static hsjangstatic
    ========================================================================== */
    public static class UtilExtern
    {
        // ✅ 범위 검사 (제네릭 지원)
        // 범위 검사
        //bool check1 = VS_UTILS.InRange(5, 1, 10);  // ✅ true
        //bool check2 = VS_UTILS.InRange(20.5, 10.0, 30.0);  // ✅ true
        //bool check3 = VS_UTILS.InRange(DateTime.Now, DateTime.Today.AddDays(-7), DateTime.Today);  // ✅ true
        public static bool InRange<T>(T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }

        // ✅ 값 제한 (Clamp 기능)
        // 값 제한 (Clamp 기능)
        //int clampedValue = VS_UTILS.Clamp(15, 10, 20);  // ✅ 결과: 15
        //double clampedDouble = VS_UTILS.Clamp(25.7, 10.0, 20.0);  // ✅ 결과: 20.0
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : (value.CompareTo(max) > 0 ? max : value);
        }

        // ✅ JSON 변환 (객체 → 문자열)
        public static string ToJson<T>(T obj) where T : class
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        // ✅ JSON 역직렬화 (문자열 → 객체)
        public static T FromJson<T>(string json) where T : class
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        //// JSON 변환 예시
        //string jsonString = VS_UTILS.ToJson(new { Name = "Test", Value = 42 });
        //var obj = VS_UTILS.FromJson<dynamic>(jsonString);

        /// <summary>
        /// 문자열을 정수형으로 안전하게 변환합니다.
        /// 변환 실패 시 기본값(fallback)을 반환합니다.
        /// </summary>
        public static int StrToInt(string? s, int fallback = 0) =>
            int.TryParse(s, out var value) ? value : fallback;

        /// <summary>
        /// 문자열을 실수형(double)으로 안전하게 변환합니다.
        /// 변환 실패 시 기본값(fallback)을 반환합니다.
        /// </summary>
        public static double StrToDouble(string? s, double fallback = 0.0) =>
            double.TryParse(s, out var value) ? value : fallback;

        //String을 날짜로 변환 실패시 코드
        public static DateTime StrToDateTime(string dateTimeStr, DateTime? defaultValue = null)
        {
            // 기본값이 null이거나 유효하지 않으면 안전한 값(2000-01-01)으로 설정
            DateTime safeDefault = (defaultValue.HasValue && defaultValue.Value.Year > 1900)
                ? defaultValue.Value
                : new DateTime(2000, 1, 1);

            // 문자열을 DateTime으로 변환 시도
            if (DateTime.TryParse(dateTimeStr, out DateTime result))
            {
                return result; // 변환 성공 → 변환된 DateTime 반환
            }

            return safeDefault; // 변환 실패 → 안전한 기본값 반환
        }

        public static int StrToIntDef(string input, int defaultValue)
        {
            return int.TryParse(input, out var result) ? result : defaultValue;
        }


        public static ObservableCollection<T> ToObservable<T>(IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

        public static void MoveFile(string sourcePath, string targetDirectory)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    Debug.WriteLine($"❌ 원본 파일이 없습니다: {sourcePath}");
                    return;
                }

                if (!Directory.Exists(targetDirectory))
                    Directory.CreateDirectory(targetDirectory); // 대상 폴더가 없다면 생성

                string fileName = Path.GetFileName(sourcePath);
                string targetPath = Path.Combine(targetDirectory, fileName);

                // 기존 대상 파일이 있으면 삭제 후 이동
                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                File.Move(sourcePath, targetPath);
                Debug.WriteLine($"✅ 파일 이동 완료: {fileName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ 파일 이동 중 오류 발생: {ex.Message}");
            }
        }

        public static void CopyFile(string sourcePath, string targetDirectory)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    Debug.WriteLine($"❌ 원본 파일이 없습니다: {sourcePath}");
                    return;
                }

                if (!Directory.Exists(targetDirectory))
                    Directory.CreateDirectory(targetDirectory); // 대상 폴더가 없다면 생성

                string fileName = Path.GetFileName(sourcePath);
                string targetPath = Path.Combine(targetDirectory, fileName);

                // 기존 대상 파일이 있으면 삭제 후 복사
                if (File.Exists(targetPath))
                    File.Delete(targetPath);

                File.Copy(sourcePath, targetPath);
                Debug.WriteLine($"✅ 파일 복사 완료: {fileName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ 파일 복사 중 오류 발생: {ex.Message}");
            }
        }

        public static void CopyDirectory(string sourceDir, string targetDir)
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



        /* ==========================================================================
        Description	: Message Show
        ========================================================================== */

        //public static void ShowMsg(string info, DependencyObject caller = null)
        //{
        //    var popup = new QueryMsgDlg(info);
        //    var owner = caller != null ? Window.GetWindow(caller) : Application.Current.MainWindow;

        //    if (owner != null && owner != popup)
        //        popup.Owner = owner;

        //    popup.ShowDialog();
        //}

        //public static bool IsQueryOk(string question, DependencyObject caller = null)
        //{
        //    var popup = new QueryMsgDlg(question, QueryMsgDlg.DialogMode.YesNo);
        //    var owner = caller != null ? Window.GetWindow(caller) : Application.Current.MainWindow;

        //    if (owner != null && owner != popup)
        //        popup.Owner = owner;

        //    bool? result = popup.ShowDialog();
        //    return result == true && popup.Result;
        //}

        //public static void ShowInitialMessage(string message)
        //{
        //    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    ProgramProcessWindow.Show($"{timestamp} : {message}");
       
  
        //}

        

    }
    public static class BorderHelper
    {
        public static void UpdateColor<T>(
               T control,
               bool isOn,
               Brush onBrush,
               Brush offBrush,
               Action<T, Brush> brushSetter)
        {
            var brush = isOn ? onBrush : offBrush;
            brushSetter(control, brush);
        }
    }

}
