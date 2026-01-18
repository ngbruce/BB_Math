# BB Math API 参考文档

**版本**: v7.1.1
**生成日期**: 2026-01-18
**开发环境**: Visual Studio 2019, C# (.NET Framework), Windows Forms, MSTest

---

## 目录
1. [核心模块 (BBMath.Core)](#核心模块-bbmathcore)
2. [配置管理 (BBMath.Configuration)](#配置管理-bbmathconfiguration)
3. [UI 模块 (BBMath.UI)](#ui-模块-bbmathui)
4. [应用模块 (BBMath.Application)](#应用模块-bbmathapplication)

---

## 核心模块 (BBMath.Core)

### 常量类

#### AppConstants
集中管理所有应用程序常量。

**命名空间**: `BBMath.Core`

**常量字段**:

##### 文件名常量
- `const string ConfigFileName = "bbmath.cfg"` - 配置文件名
- `const string LogDataFileName = "bbmath.dat"` - 日志数据文件名
- `const string LogFilePrefix = "bbmath"` - 日志文件前缀

##### 默认游戏状态值
- `const int DefaultExamTotal = 15` - 默认习题总数
- `const int DefaultCoinTotal = 10` - 默认初始金币数
- `const int DefaultCorrectCount = 0` - 默认正确数量
- `const int DefaultWrongCount = 0` - 默认错误数量

##### 暂停机制常量
- `const int DefaultPauseType = 1` - 默认暂停类型（1=限时间暂停）
- `const int DefaultAllowPauseCount = 5` - 默认可用暂停次数
- `const int DefaultPauseSecondsLeft = 600` - 默认剩余暂停时间（秒）

##### 金币奖励与消费常量
- `const int DefaultAwardCoinPerCorrect = 3` - 答对一题默认奖励金币数
- `const double FullAnswerBonusCoefficient = 0.5` - 全对通关额外奖励系数（额外金币 = 初始题数 × 此系数，向下取整）
- `const int DefaultCostCoinCheck = 1` - 默认检查辅助框花费金币（已废弃）
- `const int DefaultCostCoinGive = 3` - 默认给出答案花费金币（已废弃）

##### 惩罚系统常量
- `const int DefaultPunishmentAddQuestions = 2` - 默认做错惩罚加题数量
- `const int DefaultPunishmentTimeout = 1` - 默认超时惩罚加题数量

##### 日志系统常量
- `const long DefaultLogMaxFileSize = 10 * 1024 * 1024` - 默认日志文件最大大小（字节）
- `const int LogFileMaxRetries = 10` - 日志文件操作最大重试次数
- `const bool DefaultEnableDebugLogging = true` - 是否默认启用调试级别日志

---

### 枚举类型

#### ExamType
表示数学运算类型。

**命名空间**: `BBMath.Core`

**枚举值**:
- `Addition = 1` - 加法运算
- `Subtraction = 2` - 减法运算
- `Multiplication = 3` - 乘法运算
- `DivisionNoRemainder = 4` - 除尽除法
- `DivisionWithRemainder = 5` - 带余数除法

#### LogLevel
表示日志级别。

**命名空间**: `BBMath.Core`

**枚举值**:
- `Debug` - 调试信息，用于开发阶段
- `Info` - 一般信息，用于记录正常操作
- `Warning` - 警告信息，表示潜在问题
- `Error` - 错误信息，表示操作失败但应用可继续运行
- `Critical` - 严重错误，表示应用可能无法继续运行

#### PauseType
表示暂停类型。

**命名空间**: `BBMath.Core`

**枚举值**:
- `ByCount = 0` - 限次数暂停
- `ByTime = 1` - 限时间暂停

---

### 接口

#### IConfigurationService
配置管理服务接口。

**命名空间**: `BBMath.Configuration`

**方法**:
- `string ReadValue(string section, string key)` - 读取字符串配置项
- `int ReadInt(string section, string key)` - 读取整数配置项
- `bool ReadBool(string section, string key)` - 读取布尔配置项
- `void WriteValue(string section, string key, string value)` - 写入配置项
- `bool Exists(string section, string key)` - 检查配置项是否存在

**使用示例**:
```csharp
IConfigurationService config = new IniConfigurationService();
string value = config.ReadValue("APP", "coinTtl");
int count = config.ReadInt("APP", "allowPause");
config.WriteValue("APP", "coinTtl", "100");
```

---

#### IFileService
文件服务接口。

**命名空间**: `BBMath.Core`

**方法**:
- `bool FileExists(string filePath)` - 检查文件是否存在
- `string ReadAllText(string filePath)` - 读取文件所有文本
- `IEnumerable<string> ReadAllLines(string filePath)` - 读取文件所有行
- `void WriteAllText(string filePath, string content)` - 写入文本到文件（覆盖）
- `void AppendAllText(string filePath, string content)` - 追加文本到文件
- `void AppendAllLines(string filePath, IEnumerable<string> lines)` - 追加多行文本到文件
- `void EnsureDirectoryExists(string directoryPath)` - 创建目录（如果不存在）
- `string GetApplicationBaseDirectory()` - 获取应用程序基目录
- `string CombinePaths(params string[] paths)` - 组合路径
- `void CopyFile(string sourcePath, string destinationPath, bool overwrite = false)` - 复制文件
- `void MoveFile(string sourcePath, string destinationPath)` - 移动文件
- `void DeleteFile(string filePath)` - 删除文件
- `long GetFileSize(string filePath)` - 获取文件大小（字节）
- `DateTime GetFileLastWriteTime(string filePath)` - 获取文件最后修改时间
- `IEnumerable<string> GetFiles(string directoryPath, string searchPattern = "*.*")` - 获取目录中所有文件

**使用示例**:
```csharp
IFileService fileService = new FileService();
string content = fileService.ReadAllText("data.txt");
fileService.WriteAllText("output.txt", "Hello, World!");
```

---

#### ILoggerService
日志服务接口。

**命名空间**: `BBMath.Core`

**方法**:
- `void Debug(string message)` - 记录调试级别日志
- `void Info(string message)` - 记录信息级别日志
- `void Warning(string message)` - 记录警告级别日志
- `void Error(string message)` - 记录错误级别日志
- `void Critical(string message)` - 记录严重级别日志
- `void Exception(Exception ex, string message = null)` - 记录异常信息
- `void Log(LogLevel level, string format, params object[] args)` - 记录带有格式字符串的日志
- `bool IsEnabled(LogLevel level)` - 检查指定日志级别是否启用

**使用示例**:
```csharp
ILoggerService logger = new LoggerService();
logger.Info("应用程序启动");
logger.Error("文件读取失败");
logger.Exception(ex, "发生异常");
```

---

#### IMathProblemGenerator
数学题目生成器接口。

**命名空间**: `BBMath.Core`

**方法**:
- `MathProblem GenerateProblem(ExamType examType, int intBits = 0, int decBits = 0, bool allowNegativeResult = false)` - 生成数学题目
- `MathProblem GenerateRandomProblem(int intBits = 0, int decBits = 0, bool allowNegativeResult = false)` - 生成随机题目

**使用示例**:
```csharp
// ⚠️ 重要：应重用 Random 实例以避免随机数重复
Random random = new Random();
IMathProblemGenerator generator = new MathProblemGenerator(random);

// 生成题目
var problem = generator.GenerateProblem(ExamType.Addition, intBits: 3);
var randomProblem = generator.GenerateRandomProblem(intBits: 2);
```

**注意事项**:
- 必须传入 Random 实例并在应用程序生命周期内重用，避免每次生成题目时创建新的 Random 实例
- 重复创建 Random 实例会导致相同时间种子，产生重复的题目序列

---

#### IProblemValidator
题目验证器接口。

**命名空间**: `BBMath.Core`

**方法**:
- `bool ValidateAnswer(MathProblem problem, int userAnswer, int userRemainder = 0)` - 验证用户答案是否正确
- `ValidationResult ValidateWithDetails(MathProblem problem, int userAnswer, int userRemainder = 0)` - 验证用户答案，返回详细信息

**使用示例**:
```csharp
IProblemValidator validator = new ProblemValidator();
bool isCorrect = validator.ValidateAnswer(problem, 42);
var result = validator.ValidateWithDetails(problem, 42);
if (result.IsValid)
{
    Console.WriteLine("答案正确！");
}
```

---

### 类

#### MathProblem
数学题目信息类。

**命名空间**: `BBMath.Core`

**属性**:
- `ExamType Type { get; set; }` - 题目类型
- `int Operand1 { get; set; }` - 第一个操作数
- `int Operand2 { get; set; }` - 第二个操作数
- `int CorrectAnswer { get; set; }` - 正确答案（对于除法是商）
- `int Remainder { get; set; }` - 余数（仅适用于有余数除法）
- `string EquationString { get; set; }` - 算式字符串表示
- `int IntegerBits { get; set; }` - 整数位数限制
- `int DecimalBits { get; set; }` - 小数位数限制
- `bool AllowNegativeResult { get; set; }` - 是否允许负数结果

**方法**:
- `bool ValidateAnswer(int userAnswer, int userRemainder = 0)` - 验证用户答案是否正确
- `string ToString()` - 获取完整的题目描述

**使用示例**:
```csharp
var problem = new MathProblem
{
    Type = ExamType.Addition,
    Operand1 = 10,
    Operand2 = 20,
    CorrectAnswer = 30,
    EquationString = "10 + 20 = ?"
};

bool isCorrect = problem.ValidateAnswer(30);
```

---

#### ValidationResult
验证结果类。

**命名空间**: `BBMath.Core`

**属性**:
- `bool IsValid { get; set; }` - 是否验证通过
- `string Message { get; set; }` - 验证消息
- `int CorrectAnswer { get; set; }` - 正确答案
- `int CorrectRemainder { get; set; }` - 正确余数（仅适用于有余数除法）

**静态方法**:
- `static ValidationResult Success(MathProblem problem)` - 创建成功的验证结果
- `static ValidationResult Failure(MathProblem problem, string reason)` - 创建失败的验证结果

**使用示例**:
```csharp
var result = ValidationResult.Success(problem);
var failureResult = ValidationResult.Failure(problem, "计算错误");
```

---

#### FileService
文件服务实现类。

**命名空间**: `BBMath.Core`

**构造函数**:
- `FileService()` - 默认构造函数

**实现接口**: `IFileService`

**使用示例**:
```csharp
IFileService fileService = new FileService();
fileService.WriteAllText("test.txt", "Hello");
var content = fileService.ReadAllText("test.txt");
```

---

#### LoggerService
日志服务实现类。

**命名空间**: `BBMath.Core`

**构造函数**:
- `LoggerService()` - 默认构造函数，使用应用程序根目录下的 `log` 子目录
- `LoggerService(string logDirectory, string logFilePrefix, long maxFileSize, bool enableDebug)` - 自定义构造函数
- `LoggerService(IFileService fileService, string logDirectory, string logFilePrefix, long maxFileSize, bool enableDebug)` - 带依赖注入的构造函数

**实现接口**: `ILoggerService`

**特性**:
- ✅ 自动创建日志目录（如果不存在）
- ✅ 支持日志文件按日期轮转
- ✅ 单个日志文件大小限制（默认 10 MB）
- ✅ 支持多级别日志（Debug、Info、Warning、Error、Critical）

**使用示例**:
```csharp
ILoggerService logger = new LoggerService();
logger.Info("应用程序启动");
logger.Error("发生错误");
```

---

#### MathProblemGenerator
数学题目生成器实现类，采用策略模式实现可扩展的题目生成。

**命名空间**: `BBMath.Core`

**构造函数**:
- `MathProblemGenerator(Random random = null)` - 构造函数，可传入 Random 实例

**实现接口**: `IMathProblemGenerator`

**重要说明**:
- 应传入 Random 实例并在整个应用程序生命周期内重用
- 避免每次生成题目时创建新的 Random 实例，这会导致随机数重复（相同时间种子）

**使用示例**:
```csharp
// ✅ 正确：重用 Random 实例
Random random = new Random();
IMathProblemGenerator generator = new MathProblemGenerator(random);

// 生成多道题目，每次都使用同一个 generator 实例
var problem1 = generator.GenerateProblem(ExamType.Addition, 3, 0, false);
var problem2 = generator.GenerateProblem(ExamType.Subtraction, 2, 0, false);

// ❌ 错误：每次创建新实例会导致随机数重复
var generator2 = new MathProblemGenerator(); // 不推荐
```

---

#### ExamTypePool
题型池，用于管理和分配题型。

**命名空间**: `BBMath.Core`

**构造函数**:
- `ExamTypePool()` - 默认构造函数

**属性**:
- `int RemainingCount { get; }` - 获取题型池中剩余的题型数量总和

**方法**:
- `void Initialize(int totalQuestions, List<ExamType> supportedTypes)` - 初始化题型池，分配题型数量
- `ExamType? DrawType()` - 从题型池中随机抽取一个题型，如果题型池为空则返回 null
- `Dictionary<ExamType, int> GetTypeCounts()` - 获取所有题型及其剩余数量（用于调试）
- `void Clear()` - 清空题型池

**使用示例**:
```csharp
var pool = new ExamTypePool();

// 初始化题型池（15题，5种题型）
pool.Initialize(15, new List<ExamType>
{
    ExamType.Addition,
    ExamType.Subtraction,
    ExamType.Multiplication,
    ExamType.DivisionNoRemainder,
    ExamType.DivisionWithRemainder
});

// 从题型池抽取题型
var examType = pool.DrawType();
if (examType.HasValue)
{
    Console.WriteLine($"抽取到题型: {examType.Value}");
}

// 获取剩余题目数
int remaining = pool.RemainingCount;
Console.WriteLine($"剩余题目数: {remaining}");
```

---

#### ProblemValidator
题目验证器实现类。

**命名空间**: `BBMath.Core`

**构造函数**:
- `ProblemValidator()` - 默认构造函数

**实现接口**: `IProblemValidator`

**使用示例**:
```csharp
IProblemValidator validator = new ProblemValidator();
bool isCorrect = validator.ValidateAnswer(problem, userAnswer);
```

---

#### InputValidator
输入验证静态类。

**命名空间**: `BBMath.Core`

**静态方法**:
- `static void ValidateNotEmptyOrWhitespace(string value, string parameterName)` - 验证字符串是否为空或空白
- `static void ValidateStringLength(string value, string parameterName, int minLength, int maxLength)` - 验证字符串长度
- `static void ValidateRange(int value, string parameterName, int minValue, int maxValue)` - 验证数值范围
- `static void ValidatePositive(int value, string parameterName)` - 验证数值是否为正数
- `static void ValidateNonNegative(int value, string parameterName)` - 验证数值是否为非负数
- `static void ValidatePercentage(int value, string parameterName)` - 验证数值是否在百分比范围内（0-100）

**使用示例**:
```csharp
InputValidator.ValidateNotEmptyOrWhitespace(username, "username");
InputValidator.ValidateRange(count, "count", 1, 100);
InputValidator.ValidatePositive(price, "price");
```

---

#### GameStateManager
游戏状态管理器（静态类）。

**命名空间**: `BBMath.Core`

**公共字段**:
- `static List<ExamObject> lstExamObjects` - 题目对象列表
- `static int currentTypeIndex` - 当前题型索引
- `static bool finished` - 标记是否已完成
- `static int examTtl` - 习题总数
- `static int examTtlRec` - 保存初始习题总数
- `static int punishment` - 做错加题数量设置
- `static int correct` - 总正确数量统计
- `static int wrong` - 总错误数量统计
- `static bool helpBox` - 是否允许辅助框（已废弃）
- `static bool helpBoxFree` - 辅助框免费使用（已废弃）
- `static int coinTtl` - 箱子内金币数
- `static int costCoinCheck` - 辅助框检查花费金币（已废弃）
- `static int costCoinGive` - 辅助框给出结果花费金币（已废弃）
- `static int awardCoin` - 答对一题增加金币
- `static int punishmentTimeOut` - 超时加题数量
- `static string PSW` - 管理员密码
- `static bool flagPause` - 是否暂停状态
- `static int allowPause` - 可用暂停次数
- `static int errorMsgShowTime` - 错误框显示时间
- `static int pauseType` - 暂停类型（0=限次数，1=限时间）

**公共属性**:
- `static AppSettings Settings { get; }` - 应用程序配置实例

**公共方法**:
- `static PauseType GetPauseTypeEnum()` - 获取暂停类型的枚举值

**使用示例**:
```csharp
var settings = GameStateManager.Settings;
int coin = GameStateManager.coinTtl;
GameStateManager.correct++;
```

---

#### LoggerHelper
日志帮助类（静态类）。

**命名空间**: `BBMath.Core`

**公共方法**:
- `static void Initialize()` - 初始化日志系统
- `static void Shutdown()` - 关闭日志系统
- `static void Debug(string message)` - 记录调试日志
- `static void Info(string message)` - 记录信息日志
- `static void Warning(string message)` - 记录警告日志
- `static void Error(string message)` - 记录错误日志
- `static void Critical(string message)` - 记录严重日志
- `static void Exception(Exception ex, string message = null)` - 记录异常

**使用示例**:
```csharp
LoggerHelper.Initialize();
LoggerHelper.Info("应用程序启动");
LoggerHelper.Exception(ex, "发生错误");
LoggerHelper.Shutdown();
```

---

#### GlobalExceptionHandler
全局异常处理器。

**命名空间**: `BBMath.Core`

**公共方法**:
- `static void Initialize()` - 初始化全局异常处理器
- `static void Cleanup()` - 清理全局异常处理器

**使用示例**:
```csharp
GlobalExceptionHandler.Initialize();
// 应用程序运行...
GlobalExceptionHandler.Cleanup();
```

---

## 配置管理 (BBMath.Configuration)

### AppSettings
强类型应用程序配置类。

**命名空间**: `BBMath.Configuration`

**构造函数**:
- `AppSettings(IConfigurationService configService)` - 构造函数
- `AppSettings()` - 默认构造函数，使用默认的INI文件配置服务

**属性**:
- `int CoinTotal { get; set; }` - 金币总数
- `string Password { get; set; }` - 管理员密码
- `bool HelpBox { get; set; }` - 是否启用辅助框（已废弃）
- `bool HelpBoxFree { get; set; }` - 辅助框是否免费使用（已废弃）
- `int PauseType { get; set; }` - 暂停类型：0=限次数暂停，1=限时间暂停
- `int PauseSecondsLeft { get; set; }` - 剩余暂停时间（秒）
- `int AllowPauseCount { get; set; }` - 可用暂停次数
- `int ErrorMessageShowTime { get; set; }` - 错误消息显示时间（秒）
- `int AwardCoinPerCorrect { get; set; }` - 答对奖励金币数
- `int CostCoinCheck { get; set; }` - 检查辅助框花费金币（已废弃）
- `int CostCoinGive { get; set; }` - 给出答案花费金币（已废弃）
- `int PunishmentAddQuestions { get; set; }` - 惩罚加题数量
- `int PunishmentTimeout { get; set; }` - 超时惩罚加题数量

**方法**:
- `void Save()` - 保存所有配置变更
- `void Reload()` - 重新加载配置

**使用示例**:
```csharp
var settings = new AppSettings();
int coins = settings.CoinTotal;
settings.CoinTotal = 100;
settings.Save();
```

---

### IniConfigurationService
INI 文件配置服务实现，带有内存缓存。

**命名空间**: `BBMath.Configuration`

**构造函数**:
- `IniConfigurationService(string iniPath)` - 构造函数，指定 INI 文件路径
- `IniConfigurationService()` - 默认构造函数（使用应用程序目录下的默认配置文件）

**实现接口**: `IConfigurationService`

**额外方法**:
- `void ReloadCache()` - 清除缓存并重新从文件加载
- `bool ExistINIFile()` - 验证文件是否存在
- `void IniWriteValue(string section, string key, string value)` - 写入 INI 文件（兼容旧方法）
- `int IniReadValue(string section, string key, ref string value)` - 读取 INI 文件值（兼容旧方法）

**使用示例**:
```csharp
IConfigurationService config = new IniConfigurationService();
string value = config.ReadValue("APP", "coinTtl");
config.WriteValue("APP", "coinTtl", "100");
```

---

## UI 模块 (BBMath.UI)

### IMainFormView
主窗体视图接口。

**命名空间**: `BBMath.UI`

**方法**:
- `void DisplayProblem(MathProblem problem)` - 显示题目
- `void UpdateStatistics(int correct, int wrong)` - 更新统计信息
- `void RecordCorrectAnswer()` - 记录正确答案
- `void RecordWrongAnswer()` - 记录错误答案
- `void ShowErrorAnswerDialog()` - 显示错误答案对话框
- `void ClearAnswerInput()` - 清除答案输入
- `void DisableExamControls()` - 禁用考试控件
- `void UpdateCompletionTime(TimeSpan time)` - 更新完成时间
- `void UpdateAverageTimeCost(double avgTime)` - 更新平均用时
- `void ShowBonusDialog(int bonus)` - 显示奖励对话框
- `void UpdateExamTypeDisplay(string typeName)` - 更新题型显示
- `void UpdatePauseState(bool paused)` - 更新暂停状态
- `void UpdateCoinDisplay(int coins)` - 更新金币显示
- `void ShowHelperBox(int answer)` - 显示辅助框（已废弃）
- `void ShowError(string message)` - 显示错误

---

### BaseForm
基础表单类，提供通用功能。

**命名空间**: `BBMath.UI`

**方法**:
- `void SafeExecute(Action action)` - 安全执行方法，捕获异常

**使用示例**:
```csharp
public class MyForm : BaseForm
{
    public void DoSomething()
    {
        SafeExecute(() =>
        {
            // 可能抛出异常的代码
        });
    }
}
```

---

### MainFormPresenter
主窗体表现器。

**命名空间**: `BBMath.UI`

**构造函数**:
- `MainFormPresenter(IMainFormView view)` - 构造函数

**方法**:
- `void GenerateProblem()` - 生成题目（使用题型池抽取题型）
- `void ValidateAnswer(int userAnswer, int userRemainder = 0)` - 验证答案
- `void StartExam(int questionCount)` - 开始考试
- `void InitializeExamTypePool()` - 初始化题型池，在练习开始时调用
- `void ReinitializeExamTypePool()` - 重新初始化题型池（在答错加题后调用）
- `void PauseExam()` - 暂停考试
- `void ResumeExam()` - 恢复考试
- `void EndExam()` - 结束考试
- `void Cleanup()` - 清理资源

**使用示例**:
```csharp
var presenter = new MainFormPresenter(this);

// 开始考试前初始化题型池
presenter.InitializeExamTypePool();

// 生成题目（内部会从题型池抽取题型）
presenter.GenerateProblem();

// 验证答案
presenter.ValidateAnswer(userAnswer);

// 答错后重新初始化题型池（加题后）
presenter.ReinitializeExamTypePool();
```

---

### FormDataService
表单数据传递服务。

**命名空间**: `BBMath.UI`

**方法**:
- `void SetData(string key, object value)` - 设置数据
- `T GetData<T>(string key)` - 获取数据
- `void Clear()` - 清除所有数据

**使用示例**:
```csharp
var dataService = new FormDataService();
dataService.SetData("username", "John");
string username = dataService.GetData<string>("username");
```

---

## 应用模块 (BBMath.Application)

### Program
应用程序主入口。

**命名空间**: `BBMath.Application`

**方法**:
- `static void Main()` - 应用程序主入口方法

**使用示例**:
应用程序启动时会自动调用 `Main()` 方法。

---

## 附录

### 常见使用场景

#### 场景 1: 读取和写入配置
```csharp
var config = new IniConfigurationService();
int coinTotal = config.ReadInt("APP", "coinTtl");
config.WriteValue("APP", "coinTtl", "100");
```

#### 场景 2: 记录日志
```csharp
LoggerHelper.Initialize();
LoggerHelper.Info("应用程序启动");
LoggerHelper.Error("发生错误");
LoggerHelper.Shutdown();
```

#### 场景 3: 生成和验证题目
```csharp
IMathProblemGenerator generator = new MathProblemGenerator();
var problem = generator.GenerateProblem(ExamType.Addition, 3);

IProblemValidator validator = new ProblemValidator();
bool isCorrect = validator.ValidateAnswer(problem, userAnswer);
```

#### 场景 4: 文件操作
```csharp
IFileService fileService = new FileService();
var content = fileService.ReadAllText("data.txt");
fileService.WriteAllText("output.txt", content);
```

#### 场景 5: 输入验证
```csharp
InputValidator.ValidateNotEmptyOrWhitespace(username, "username");
InputValidator.ValidateRange(count, "count", 1, 100);
InputValidator.ValidatePositive(price, "price");
```


### 项目结构

```
BB_Math/
├── Core/                    # 核心业务逻辑
│   ├── AppConstants.cs      # 常量定义
│   ├── GameStateManager.cs  # 游戏状态管理
│   ├── FileService.cs       # 文件服务
│   ├── LoggerService.cs     # 日志服务
│   ├── InputValidator.cs    # 输入验证
│   ├── MathProblemGenerator.cs # 题目生成器
│   ├── ExamTypePool.cs      # 题型池管理 ✨（新增）
│   └── ...
├── UI/                      # UI 层（MVP 模式）
│   ├── BaseForm.cs          # 基础表单类
│   ├── MainFormPresenter.cs # 主窗体表现器
│   └── FormDataService.cs   # 表单数据传递服务
├── BBMath.Tests/            # 单元测试
├── AppSettings.cs           # 强类型配置类 ✨（配置管理）
├── IConfigurationService.cs      # 配置服务接口 ✨（配置管理）
├── IniConfigurationService.cs    # INI 配置服务实现 ✨（配置管理）
├── Form1.cs                 # 主窗体
└── Program.cs               # 程序入口
```

**说明**：配置管理相关的类（`AppSettings`、`IConfigurationService`、`IniConfigurationService`）直接位于项目根目录，不在单独的 Configuration 文件夹中。

---

**文档版本**: 1.0
**最后更新**: 2026-01-15
