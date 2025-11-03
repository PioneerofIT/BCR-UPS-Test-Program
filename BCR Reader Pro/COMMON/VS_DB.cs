using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using VSP.COMMON;
using VSP.COMMON.RECIPE_PARAM;
using VSP.GUI.SETTING;

namespace VSP.COMMON
{
    // ============================================================
    // Description : CLASS CUserInfo
    // ============================================================
    public class CUserInfo
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화 (Constructor / Initialization)
        // ============================================================
        #region ObjectLifecycle
        public CUserInfo()
        {
            grade = 0;
            UserId = string.Empty;
            UserName = string.Empty;
            Password = string.Empty;
            LastLogInTime = DateTime.Now;
        }
        #endregion

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties
        public int grade { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime LastLogInTime { get; set; }
        #endregion

        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        #region AccessorsComputation
        public string DisplayName => $"{UserName} ({UserId})";
        #endregion

        // ============================================================
        // Description : [4] Internal Logic / Validation ( 데이터 검증)
        // ============================================================
        #region InternalLogicValidation
        public bool IsLoginAvailable() => !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Password);
        #endregion

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        #region Unclassified
        // 추후 필요 시 확장 예정
        #endregion
    }

    // ============================================================
    // Description : CLASS CLogBuffer
    // ============================================================
    public class CLogBuffer
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화 (Constructor / Initialization)
        // ============================================================
        #region ObjectLifecycle
        public CLogBuffer()
        {
            Init();
        }

        public void Init()
        {
            AlarmNo = -1;
            LogMessage = string.Empty;
            StartTime = DateTime.Now;
            ResetTime = DateTime.Now;
            EndTime = DateTime.Now;
        }
        #endregion

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties
        public int AlarmNo { get; set; }
        public string LogMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ResetTime { get; set; }
        public DateTime EndTime { get; set; }
        #endregion

        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        #region AccessorsComputation
        public TimeSpan Elapsed => EndTime - StartTime;
        public bool IsActive => AlarmNo >= 0;
        #endregion

        // ============================================================
        // Description : [4] Internal Logic / Validation ( 데이터 검증)
        // ============================================================
        #region InternalLogicValidation
        public bool IsValidTimeRange() => StartTime <= ResetTime && ResetTime <= EndTime;
        #endregion

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        #region Unclassified
        // 추가 로직은 여기 작성
        #endregion
    }


    /* ==========================================================================
    Description	: DB public static hsjangstatic
    ========================================================================== */
    public static class Db
    {
        public static CVSDbManager Manager => CVSDbManager.Instance;
    }
    // ============================================================
    // Description : Db Class (Recipe,User,Log 관리)
    // ============================================================
    public class CVSDbManager : IDisposable
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화
        // ============================================================

        public static CVSDbManager Instance => instance ??= new CVSDbManager();

        private CVSDbManager()
        {
            connection = new SQLiteConnection($"Data Source={DbPath};Version=3;");
            connection.Open();

            Recipe = new RecipeManager(connection);  // 필요에 따라 다른 Manager도 연결
            User = new UserManager(connection);

        }

        public void Dispose()
        {
            Recipe?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }

        public void Initialize()
        {
            //생성자 내부에서 Initialize를 호출하면 안 돼요,
        }



        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties
        private static CVSDbManager instance;
        private readonly SQLiteConnection connection;
        private string DbPath = CGlobal.Instance.DataDir + "/VSP.db"; // 경로 고정

        public RecipeManager Recipe { get; private set; }
        public UserManager User { get; }
        #endregion

        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        #region AccessorsComputation

        #endregion

        // ============================================================
        // Description : [4] Internal Logic / Validation ( 데이터 검증)
        // ============================================================
        #region InternalLogicValidation
      
        #endregion

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        #region Unclassified
        // ...
        #endregion
    }


    // =======================================================================
    // Description : Class Recipe Manager
    // =======================================================================
    public class RecipeManager : IDisposable
    {
        // ============================================================
        // Description : [1] 객체 생명 주기 (Object Lifecycle) - 생성자 / 초기화
        // ============================================================
        #region ObjectLifecycle

        private SQLiteConnection connection;

        public RecipeManager(SQLiteConnection connection)
        {
            this.connection = connection;

            LoadRecipeLists();  // 초기 로딩

            LoadCleanRecipeItems();
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }

        #endregion

        // ============================================================
        // Description : [2] Properties (속성 및 설정 값)
        // ============================================================
        #region Properties
        private string DbPath = CGlobal.Instance.DataDir; // 경로 고정
        public readonly Dictionary<RecipeType, string> recipeNameMap = new();

        public List<string> CleanRecipeList { get; private set; } = new();
        public List<string> MotionRecipeList { get; private set; } = new();
        public List<RecipePair> RecipeList { get; private set; } = new();

        public Dictionary<string, TCleanParam> CleanRecipeParamMap { get; private set; } = new();


        public class RecipePair
        {
            public string Name { get; set; }         // Recipe 필드
            public string Clean { get; set; }        // 이게 없으면 CS0117 발생
            public string Motion { get; set; }

            public override string ToString() => $"🧼 {Clean} + 🤖 {Motion}";


        }

        #endregion

        // ============================================================
        // Description : [3] Accessors / Computation (데이터 조회 및 계산)
        // ============================================================
        #region AccessorsComputation

        public void AddRecipe(string recipe, string clean, string device)
        {
            if (connection.State != ConnectionState.Open)
                return;

            if (IsRegisteredRecipe(recipe))
                return;

            try
            {
                string sql = @"INSERT INTO RecipeTbl ([Recipe], [DateTime], [TYPE], [Clean], [Motion])
                       VALUES (@recipe, @dateTime, @type, @clean, @motion)";
                using var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@recipe", recipe);
                command.Parameters.AddWithValue("@dateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@type", RecipeType.RcpParam);
                command.Parameters.AddWithValue("@clean", clean);
                command.Parameters.AddWithValue("@motion", device);
                command.ExecuteNonQuery();


                // recipeNameMap에 중복 없이 등록
                if (!recipeNameMap.ContainsKey(RecipeType.RcpParam))
                {
                    recipeNameMap.Add(RecipeType.RcpParam, recipe);
                }
                else if (!recipeNameMap.Values.Contains(recipe))
                {
                    // 같은 타입에 다른 이름이 필요하다면 여러 개를 허용하도록 구조를 확장 가능
                    UtilExtern.ShowMsg($"[Info] 같은 타입에 중복되지 않은 레시피가 추가되었습니다: {recipe}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void EditRecipe(string oldRecipe, string newRecipe, string clean, string motion, int type = 0)
        {
            string sql = @"UPDATE RecipeTbl 
                       SET Recipe = @newRecipe, Clean = @clean, Motion = @motion, TYPE = @type, DateTime = datetime('now') 
                       WHERE Recipe = @oldRecipe";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@newRecipe", newRecipe);
            command.Parameters.AddWithValue("@clean", clean);
            command.Parameters.AddWithValue("@motion", motion);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@oldRecipe", oldRecipe);
            command.ExecuteNonQuery();
        }

        public void DeleteRecipe(string recipe)
        {
            string sql = "DELETE FROM RecipeTbl WHERE Recipe = @recipe";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@recipe", recipe);
            command.ExecuteNonQuery();
        }

        public List<string> GetAllRecipeNames()
        {
            var list = new List<string>();
            string sql = "SELECT Recipe FROM RecipeTbl ORDER BY DateTime DESC";
            using var command = new SQLiteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(reader["Recipe"]?.ToString() ?? "");
            }
            return list;
        }

        public bool IsRegisteredRecipe(string recipe)
        {
            string sql = "SELECT COUNT(*) FROM RecipeTbl WHERE Recipe = @recipe";
            using var command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@recipe", recipe);
            var count = Convert.ToInt32(command.ExecuteScalar());
            return count > 0;
        }

        private List<string> GetRecipeStringList(int recipeType)
        {
            var list = new List<string>();
            string sql = "SELECT Recipe FROM RecipeTbl WHERE TYPE = @type ORDER BY DateTime DESC";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@type", recipeType);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string recipe = reader["Recipe"]?.ToString();
                if (!string.IsNullOrWhiteSpace(recipe))
                    list.Add(recipe);
            }

            return list;
        }
        public bool IsRegisteredRcpParam(string recipe, RecipeType type)
        {
            return type switch
            {
                RecipeType.CleanParam => CleanRecipeList.Contains(recipe),
                RecipeType.MotionParam => MotionRecipeList.Contains(recipe),
                _ => false
            };
        }

        public bool AddRcpParam(string recipe, RecipeType type, object param)
        {
            bool bRet = false;
            if (connection == null && connection.State != ConnectionState.Open)
            {
                return bRet;
            }

            if (IsRegisteredRcpParam(recipe, type))
            {
                return bRet;
            }

            string fieldName = "";
            switch (type)
            {
                case RecipeType.CleanParam:
                    fieldName = "Clean";
                    break;

                case RecipeType.MotionParam:
                    fieldName = "Motion";
                    break;

                case RecipeType.RcpParam://혹시모르니 넣어놓자
                    fieldName = "Recipe";
                    return bRet;
                    break;
                default:
                    throw new ArgumentException("Invalid type");
            }

            string sql = $"INSERT INTO RecipeTbl ([Recipe], [DateTime], [TYPE], [{fieldName}]) VALUES (@recipe, @date, @type, @value)";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@recipe", recipe);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@type", (int)type);
            cmd.Parameters.AddWithValue("@value", recipe);

            try
            {
                if(cmd.ExecuteNonQuery() <= 0)//변경된 사항없음
                        return bRet;

                string filePath = "";

                if (type == RecipeType.CleanParam)
                {
                    if (param is TCleanParam cleanParam)
                    {
                        filePath = Path.Combine(DbPath, recipe + ".pls");
                        cleanParam.Save(filePath);
                    }
                }
                else if (param is TMotionParam motionParam)
                {
                    filePath = Path.Combine(DbPath, recipe + ".svr");
                    motionParam.Save(filePath);
                }

            }
            catch (Exception ex)
            {
                UtilExtern.ShowMsg(ex.Message);
                return bRet;
            }

            LoadRecipeLists();
            LoadCleanRecipeItems();

            return true;
        }

        public bool EditRcpParam(string oldRecipe, string newRecipe, RecipeType type, object param)
        {
            bool bRet = false;
            if (connection == null && connection.State != ConnectionState.Open)
            {
                return bRet;
            }

            string sql = @"UPDATE RecipeTbl 
                   SET Recipe = @newRecipe, DateTime = @date, TYPE = @type
                   WHERE Recipe = @oldRecipe AND TYPE = @type";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@newRecipe", newRecipe);
            cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@type", (int)type);
            cmd.Parameters.AddWithValue("@oldRecipe", oldRecipe);

            try
            {
                if (cmd.ExecuteNonQuery() <= 0)//변경된 사항없음
                    return bRet;
                Debug.WriteLine($"✏️ Edited recipe name: {oldRecipe} → {newRecipe}");

                String NewfilePath = "";
                String OldfilePath = "";
                String TargetPath = "";        

                if (type == RecipeType.CleanParam)
                {
                    if (param is TCleanParam cleanParam)
                    {
                        NewfilePath = Path.Combine(DbPath, newRecipe + ".pls");
                        OldfilePath = Path.Combine(DbPath, oldRecipe + ".pls");
                        //NewfilePath = Path.Combine(DbPath, NewfilePath + ".pls");
                        TargetPath = Path.Combine("Data/BACKUP/", oldRecipe + ".pls");
                        if(oldRecipe == newRecipe)
                            UtilExtern.CopyFile(newRecipe, TargetPath);

                        if (cleanParam.Save(NewfilePath) == true)
                        {
                            if (oldRecipe != newRecipe)
                                UtilExtern.MoveFile(OldfilePath, TargetPath);
                        }
                        else
                        {
                            //db저장성공햇으나 파일생성실패시 예외처리
                        }
                    }
                }
                else if (param is TMotionParam motionParam)
                {
                    OldfilePath = Path.Combine(DbPath, oldRecipe + ".svr");
                    NewfilePath = Path.Combine(DbPath, newRecipe + ".svr");
                    TargetPath = Path.Combine("Data/BACKUP/", OldfilePath);
                    if(motionParam.Save(NewfilePath))
                    {
                        UtilExtern.MoveFile(OldfilePath, TargetPath);
                    }

                }
            }
            catch (Exception ex)
            {
                UtilExtern.ShowMsg(ex.Message);
            }

            LoadRecipeLists();
            LoadCleanRecipeItems();
            return true;
        }

        public bool DelRcpParam(string recipe, RecipeType type)
        {
            bool bRet = false;
            if (connection == null && connection.State != ConnectionState.Open)
            {
                return bRet;
            }

            string sql = "DELETE FROM RecipeTbl WHERE Recipe = @recipe AND TYPE = @type";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@recipe", recipe);
            cmd.Parameters.AddWithValue("@type", (int)type);

            String filePath = "";
            String TargetPath = "";
            try
            {
                if (cmd.ExecuteNonQuery() <= 0)//변경된 사항없음
                    return bRet;

                Debug.WriteLine($"🗑️ Deleted recipe: {recipe}");

                if (type == RecipeType.CleanParam)
                {
                    filePath = Path.Combine(DbPath, recipe + ".pls");
                    TargetPath = Path.Combine("Data/REMOVE/", recipe + ".pls");
                    UtilExtern.MoveFile(filePath, TargetPath);
                }
                else if (type == RecipeType.MotionParam)
                {
                    filePath = Path.Combine(DbPath, recipe + ".svr");
                    TargetPath = Path.Combine("Data/REMOVE/", recipe + ".svr");
                    UtilExtern.MoveFile(filePath, TargetPath);
                }
            }
            catch (Exception ex)
            {
                UtilExtern.ShowMsg(ex.Message);
            }

            LoadRecipeLists();
            LoadCleanRecipeItems();
            return true;

        }

        #endregion

        // ============================================================
        // Description : [4] Internal Logic / Validation (데이터 검증)
        // ============================================================
        #region InternalLogicValidation

        public void LoadRecipeLists()
        {
            CleanRecipeList = LoadListByType((int)RecipeType.CleanParam);   // CLEAN_PARAM
            MotionRecipeList = LoadListByType((int)RecipeType.MotionParam);   // MOTION_PARAM
            RecipeList = LoadFullRecipeList();   // FULL_PARAM
        }

        private List<string> LoadListByType(int type)
        {
            var list = new List<string>();
            string sql = "SELECT Recipe, TYPE FROM RecipeTbl WHERE TYPE = @type ORDER BY DateTime DESC";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@type", type);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string name = reader["Recipe"]?.ToString();
                object typeObj = reader["TYPE"];
                int recipeType = typeObj != null ? Convert.ToInt32(typeObj) : -1;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    list.Add(name);
                   // UtilExtern.ShowInitialMessage($"📌 로드됨 → TYPE: {recipeType} / NAME: {name}");
                }
            }

            return list;
        }

        private List<RecipePair> LoadFullRecipeList()
        {
            var list = new List<RecipePair>();
            string sql = "SELECT Recipe, Clean, Motion FROM RecipeTbl WHERE TYPE = @type ORDER BY DateTime DESC";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@type", (int)RecipeType.RcpParam);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string name = reader["Recipe"]?.ToString()?.Trim();
                string clean = reader["Clean"]?.ToString()?.Trim();
                string motion = reader["Motion"]?.ToString()?.Trim();

                if (!string.IsNullOrWhiteSpace(clean) && !string.IsNullOrWhiteSpace(motion))
                {
                    list.Add(new RecipePair
                    {
                        Name = name,
                        Clean = clean,
                        Motion = motion
                    });

                   // UtilExtern.ShowInitialMessage($"📌 통합 레시피 로드됨 → NAME: {name}, CLEAN: {clean}, MOTION: {motion}");
                }
            }

            return list;
        }

        private void LoadCleanRecipeItems()
        {

            CleanRecipeParamMap.Clear();

            foreach (var recipeName in CleanRecipeList)
            {
                string filePath = Path.Combine(DbPath, recipeName + ".pls");

                var cleanParam = new TCleanParam();
                if (cleanParam.Load(filePath))
                {
                    CleanRecipeParamMap[recipeName] = cleanParam;
                }
                else
                {
                    Debug.WriteLine($"[오류] Load 실패: {filePath}");
                }
            }
        }

        #endregion

        // ============================================================
        // Description : [5] Unclassified (추후 정리 예정)
        // ============================================================
        #region Unclassified



        #endregion
    }
    // =======================================================================
    // Description : Class User Manager
    // =======================================================================
    public class UserManager
    {
        private readonly SQLiteConnection connection;

        public UserManager(SQLiteConnection conn)
        {
            connection = conn;
              
        }

        public void AddUser(string id, string name, string grade, string password)
        {
            string sql = @"INSERT INTO UserTbl (UserID, UserName, grade, Password, LastLogInTime)
                       VALUES (@id, @name, @grade,@password, datetime('now'))";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@grade", grade);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(string id)
        {
            string sql = @"DELETE FROM UserTbl WHERE UserID = @id";
            using var cmd = new SQLiteCommand(sql, connection);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public void EditUser(string id, string name, string grade, string password)
        {
            string sql = @"UPDATE UserTbl
                         SET 
                            UserName = @name,
                            grade    = @grade,
                            Password = @password
                         WHERE 
                            UserID   = @id";

            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@grade", grade);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }
        public CUserInfo QueryUser(string id)
        {
            string sql = @"SELECT UserID,UserName, grade, Password, LastLogInTime
                           FROM UserTbl
                           WHERE UserID = @id";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if(reader.Read())
            {
                return new CUserInfo
                {
                    UserId = reader.GetString(reader.GetOrdinal("UserID")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    grade = reader.GetInt16(reader.GetOrdinal("grade")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    LastLogInTime = UtilExtern.StrToDateTime(reader.GetString(reader.GetOrdinal("LastLogInTime")))
                };
            }
            else
            {
              
                return null;
            }


        }
        public void SaveUser(string id, string name, string grade, string password)
        {
            string sql = @"
                        INSERT INTO UserTbl
                            (UserID, UserName, grade, Password, LastLogInTime)
                        VALUES
                            (@id, @name, @grade, @password, datetime('now'))
                        ON CONFLICT(UserID) DO UPDATE SET
                            UserName       = @name,
                            grade          = @grade,
                            Password       = @password,
                            LastLogInTime  = datetime('now');
                        ";
            using var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@grade", grade);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.ExecuteNonQuery();
        }
        public List<CUserInfo> GetAllUsers()
        {
            var list = new List<CUserInfo>();
            string sql = "SELECT * FROM UserTbl";
            using var cmd = new SQLiteCommand(sql, connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CUserInfo
                {
                    UserId = reader.GetString(reader.GetOrdinal("UserID")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    grade = reader.GetInt32(reader.GetOrdinal("grade")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    LastLogInTime = UtilExtern.StrToDateTime(reader.GetString(reader.GetOrdinal("LastLogInTime")))
                });
            }
            return list;
        }

    }
}
