using System;
using System.Collections.Generic;
using BBMath.Core;
using BBMath.Configuration;

namespace BBMath.UI
{
    /// <summary>
    /// 主窗体表现器（Presenter）
    /// 负责处理业务逻辑，与视图（Form1）解耦
    /// </summary>
    public class MainFormPresenter
    {
        private readonly IMainFormView _view;
        private readonly ILoggerService _logger;
        private readonly IConfigurationService _configService;

        /// <summary>
        /// 当前题型索引
        /// </summary>
        public int CurrentExamTypeIndex { get; set; }

        /// <summary>
        /// 当前算式的数字
        /// </summary>
        public int Num1 { get; set; }
        public int Num2 { get; set; }
        public int Num3 { get; set; }
        public int Result { get; set; }
        public int NumHalfResult { get; set; }

        /// <summary>
        /// 当前算式字符串
        /// </summary>
        public string Equation { get; set; }

        /// <summary>
        /// 随机数生成器
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// 练习开始时间
        /// </summary>
        private DateTime _startTime;

        /// <summary>
        /// 题型池，用于管理题型分配
        /// </summary>
        private ExamTypePool _examTypePool;

        /// <summary>
        /// 数学题目生成器（重用单一实例，避免 Random 重复初始化）
        /// </summary>
        private readonly MathProblemGenerator _mathProblemGenerator;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="view">视图接口</param>
        /// <param name="logger">日志服务</param>
        /// <param name="configService">配置服务</param>
        public MainFormPresenter(IMainFormView view, ILoggerService logger, IConfigurationService configService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));

            _random = new Random();
            _examTypePool = new ExamTypePool();
            _mathProblemGenerator = new MathProblemGenerator(_random);
            CurrentExamTypeIndex = 0;

            Initialize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Initialize()
        {
            // 加载配置
            LoadConfiguration();
            
            // 初始化游戏状态
            InitializeGameState();
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadConfiguration()
        {
            try
            {
                // 使用 AppSettings 类加载配置
                var settings = new AppSettings(_configService);
                GameStateManager.coinTtl = settings.CoinTotal;
                GameStateManager.PSW = settings.Password;

                _logger.Info($"配置加载成功，金币：{GameStateManager.coinTtl}");
            }
            catch (Exception ex)
            {
                _logger.Error($"加载配置失败：{ex.Message}");
                _logger.Exception(ex);
            }
        }

        /// <summary>
        /// 初始化游戏状态
        /// </summary>
        private void InitializeGameState()
        {
            GameStateManager.flagPause = false;
            GameStateManager.finished = false;
            GameStateManager.correct = 0;
            GameStateManager.wrong = 0;
            // examTtl 不在这里初始化，保持默认值 DefaultExamTotal
            // 用户点击"开始"按钮时会从输入框读取用户输入的值
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// 初始化题型池，在练习开始时调用
        /// </summary>
        public void InitializeExamTypePool()
        {
            try
            {
                // 获取当前难度配置
                var difficultyConfig = DifficultyConfigurationManager.GetConfig(GameStateManager.currentDifficulty);

                // 使用题型池分配题型数量
                _examTypePool.Initialize(GameStateManager.examTtl, difficultyConfig.SupportedOperations);

                // 打印题型分配情况（调试用）
                var typeCounts = _examTypePool.GetTypeCounts();
                _logger.Info($"题型池初始化完成 - 总题目数: {GameStateManager.examTtl}, 题型种类数: {difficultyConfig.SupportedOperations.Count}");
                foreach (var kvp in typeCounts)
                {
                    _logger.Debug($"  {kvp.Key}: {kvp.Value} 题");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"初始化题型池失败：{ex.Message}");
                _logger.Exception(ex);
            }
        }

        /// <summary>
        /// 重新初始化题型池（在答错加题后调用）
        /// </summary>
        public void ReinitializeExamTypePool()
        {
            try
            {
                // 获取当前难度配置
                var difficultyConfig = DifficultyConfigurationManager.GetConfig(GameStateManager.currentDifficulty);

                // 记录重新初始化前的题型池状态
                var oldTypeCounts = _examTypePool.GetTypeCounts();
                _logger.Info($"重新初始化题型池 - 加题后总题目数: {GameStateManager.examTtl}");

                // 重新分配题型数量
                _examTypePool.Initialize(GameStateManager.examTtl, difficultyConfig.SupportedOperations);

                // 打印重新分配后的题型情况
                var newTypeCounts = _examTypePool.GetTypeCounts();
                _logger.Debug($"题型池重新分配完成 - 题型种类数: {difficultyConfig.SupportedOperations.Count}");
                foreach (var kvp in newTypeCounts)
                {
                    _logger.Debug($"  {kvp.Key}: {kvp.Value} 题");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"重新初始化题型池失败：{ex.Message}");
                _logger.Exception(ex);
            }
        }

        /// <summary>
        /// 生成新的数学题目
        /// </summary>
        public void GenerateProblem()
        {
            try
            {
                // 获取当前难度配置
                var difficultyConfig = DifficultyConfigurationManager.GetConfig(GameStateManager.currentDifficulty);

                // 从题型池中抽取题型
                ExamType examType;
                var selectedType = _examTypePool.DrawType();

                if (selectedType.HasValue)
                {
                    examType = selectedType.Value;
                    _logger.Debug($"从题型池抽取题型: {examType}, 剩余题目数: {_examTypePool.RemainingCount}");
                }
                else if (difficultyConfig.SupportedOperations.Count > 0)
                {
                    // 如果题型池为空，回退到随机选择（兼容旧逻辑）
                    int randomIndex = _random.Next(0, difficultyConfig.SupportedOperations.Count);
                    examType = difficultyConfig.SupportedOperations[randomIndex];
                    _logger.Debug($"题型池为空，随机选择题型: randomIndex={randomIndex}, examType={examType}");
                }
                else
                {
                    // 如果没有支持的运算类型，使用默认逻辑
                    var examObject = GameStateManager.lstExamObjects[CurrentExamTypeIndex];
                    examType = examObject.Examtype;
                    _logger.Debug($"使用默认逻辑选择题型: examType={examType}");
                }

                // 使用循环而不是递归，避免大量重复日志
                MathProblem problem;
                int maxAttempts = 100;
                int attemptCount = 0;

                do
                {
                    attemptCount++;
                    if (attemptCount > maxAttempts)
                    {
                        _logger.Error($"生成题目失败：尝试生成题目次数超过最大限制({maxAttempts})");
                        _logger.Error($"题型: {examType}, IntegerBits: {difficultyConfig.IntegerBits}, RequireAbove100: {difficultyConfig.RequireAbove100}");
                        throw new InvalidOperationException($"尝试生成题目次数超过最大限制({maxAttempts})");
                    }

                    // 使用重用的生成器生成题目（避免 Random 重复初始化）
                    // MathProblemGenerator 在构造函数中初始化并传入 _random 实例

                    // 根据题型和难度调整 intBits
                    int effectiveIntBits = difficultyConfig.IntegerBits;
                    if (examType == ExamType.Multiplication)
                    {
                        if (difficultyConfig.Difficulty == Difficulty.LV4)
                        {
                            // LV4 难度下，乘法应生成个位数乘法，确保结果 < 100
                            effectiveIntBits = 1; // 生成 1-9 的个位数乘法
                            _logger.Debug($"LV4 乘法使用 effectiveIntBits={effectiveIntBits}（个位数），避免结果 >= 100");
                        }
                        else if (difficultyConfig.Difficulty == Difficulty.LV5)
                        {
                            // LV5 难度下，乘法应生成两位数乘法，结果在 100-9801 之间
                            effectiveIntBits = 2; // 生成 10-99 的两位数乘法
                            _logger.Debug($"LV5 乘法使用 effectiveIntBits={effectiveIntBits}（两位数），结果范围 100-9801");
                        }
                    }
                    else if (examType == ExamType.DivisionNoRemainder || examType == ExamType.DivisionWithRemainder)
                    {
                        if (difficultyConfig.Difficulty == Difficulty.LV4)
                        {
                            // LV4 难度下，除法应生成个位数，确保被除数 < 100
                            // 使用两位数（10-99）会导致被除数 >= 100（10*10=100），触发无限重试
                            effectiveIntBits = 1; // 生成 1-9 的个位数除法
                            _logger.Debug($"LV4 除法使用 effectiveIntBits={effectiveIntBits}（个位数），避免被除数 >= 100");
                        }
                    }

                    problem = _mathProblemGenerator.GenerateProblem(
                        examType,
                        effectiveIntBits,
                        0,
                        difficultyConfig.AllowNegativeResult
                    );

                    // 根据整数位数验证题目范围
                    bool shouldRetry = false;
                    string retryReason = "";

                    // LV1/LV2/LV3/LV4 (1-2位)：要求操作数和结果都在100以内
                    if (difficultyConfig.IntegerBits <= 2)
                    {
                        if (problem.Operand1 >= 100 || problem.Operand2 >= 100 || problem.CorrectAnswer >= 100)
                        {
                            shouldRetry = true;
                            retryReason = "不符合<100条件";
                        }
                    }
                    // LV5 (3位)：允许100以上，但加法结果不能过大
                    else if (difficultyConfig.IntegerBits == 3)
                    {
                        // 加法结果限制在2000以内
                        if (examType == ExamType.Addition && problem.CorrectAnswer > 2000)
                        {
                            shouldRetry = true;
                            retryReason = "加法结果>2000";
                        }
                    }

                    if (shouldRetry)
                    {
                        _logger.Debug($"重试题目（{retryReason}）：{problem.EquationString}, Op1={problem.Operand1}, Op2={problem.Operand2}, Result={problem.CorrectAnswer}");
                        continue;  // 继续重试
                    }

                    // 题目满足要求，退出循环
                    break;

                } while (true);

                // 同步 GameStateManager.currentTypeIndex 到当前选择的题型（只在最终确定题型后同步一次）
                for (int i = 0; i < GameStateManager.lstExamObjects.Count; i++)
                {
                    if (GameStateManager.lstExamObjects[i].Examtype == examType)
                    {
                        GameStateManager.currentTypeIndex = i;
                        CurrentExamTypeIndex = i;
                        _logger.Debug($"同步 currentTypeIndex: {i}, 题型: {examType}, 描述: {GameStateManager.lstExamObjects[i].Description}");
                        break;
                    }
                }

                // 设置题目数据
                Num1 = problem.Operand1;
                Num2 = problem.Operand2;
                Result = problem.CorrectAnswer;
                NumHalfResult = problem.Remainder;
                Equation = problem.EquationString;

                // 更新视图
                _view.DisplayProblem(Equation);

                // 重置定时器，确保每题都有完整的时间限制
                _view.ResetTimer();

                // 同步题目数据到 Form1 的字段，确保 answer() 方法使用正确的值
                _view.SyncProblemData(Num1, Num2, Result, NumHalfResult, Equation);

                _logger.Debug($"生成题目难度：{GameStateManager.currentDifficulty}  {Equation}");

                // 打印答案到日志（与 Form1.GenNum() 中的格式保持一致）
                var currentExamObject = GameStateManager.lstExamObjects[CurrentExamTypeIndex];
                string answerLog;
                switch (currentExamObject.Examtype)
                {
                    case ExamType.Addition:
                        answerLog = $"Addition:{Num1} + {Num2} = {Result}\r\n";
                        break;
                    case ExamType.Subtraction:
                        answerLog = $"Subtraction:{Num1} - {Num2} = {Result}\r\n";
                        break;
                    case ExamType.Multiplication:
                        answerLog = $"Multiplication:{Num1} × {Num2} = {Result}\r\n";
                        break;
                    case ExamType.DivisionWithRemainder:
                        answerLog = $"DivisionWithRemainder:{Num1} ÷ {Num2} = {Result}......{NumHalfResult}\r\n";
                        break;
                    case ExamType.DivisionNoRemainder:
                        answerLog = $"DivisionNoRemainder:{Num1} ÷ {Num2} = {Result}\r\n";
                        break;
                    default:
                        answerLog = $"{Equation} = {Result}\r\n";
                        break;
                }
                _logger.Info(answerLog);
            }
            catch (Exception ex)
            {
                _logger.Error($"生成题目失败：{ex.Message}");
                _logger.Exception(ex);
                _view.ShowError($"生成题目失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 验证答案
        /// </summary>
        /// <param name="userResult">用户答案</param>
        /// <param name="userRemainder">用户输入的余数（除法时使用）</param>
        /// <returns>是否正确</returns>
        public bool ValidateAnswer(int userResult, int userRemainder = 0)
        {
            try
            {
                var examObject = GameStateManager.lstExamObjects[CurrentExamTypeIndex];
                var validator = new ProblemValidator();

                // 创建 MathProblem 对象进行验证
                var problem = new MathProblem
                {
                    Type = examObject.Examtype,
                    CorrectAnswer = Result,
                    Remainder = NumHalfResult,
                    EquationString = Equation
                };

                bool isCorrect = validator.ValidateAnswer(problem, userResult, userRemainder);

                if (isCorrect)
                {
                    _logger.Info($"答案正确：{Equation} = {userResult}");
                    HandleCorrectAnswer();
                }
                else
                {
                    _logger.Info($"答案错误：{Equation} ≠ {userResult}（正确答案：{Result}）");
                    HandleWrongAnswer(userResult, userRemainder);
                }

                return isCorrect;
            }
            catch (Exception ex)
            {
                _logger.Error($"验证答案失败：{ex.Message}");
                _logger.Exception(ex);
                return false;
            }
        }

        /// <summary>
        /// 处理正确答案
        /// </summary>
        private void HandleCorrectAnswer()
        {
            GameStateManager.correct++;
            GameStateManager.examTtl--;

            // 奖励金币
            if (GameStateManager.awardCoin > 0)
            {
                GameStateManager.coinTtl += GameStateManager.awardCoin;
            }

            // 更新视图
            _view.UpdateStatistics(
                GameStateManager.correct,
                GameStateManager.wrong,
                GameStateManager.examTtl,
                GameStateManager.coinTtl
            );

            _view.RecordCorrectAnswer(Equation, Result, NumHalfResult);

            // 检查是否完成
            if (GameStateManager.examTtl <= 0)
            {
                HandleExamCompletion();
            }
            else
            {
                GenerateProblem();
                _view.ClearAnswerInput();
            }
        }

        /// <summary>
        /// 处理错误答案
        /// </summary>
        /// <param name="userResult">用户答案</param>
        /// <param name="userRemainder">用户输入的余数</param>
        private void HandleWrongAnswer(int userResult, int userRemainder)
        {
            GameStateManager.wrong++;

            // 惩罚机制：加时
            if (GameStateManager.lstExamObjects[CurrentExamTypeIndex].TimeLimit > 0)
            {
                var examObject = GameStateManager.lstExamObjects[CurrentExamTypeIndex];
                // 增加当前题型的用时
                // 这里可以根据实际需求实现具体的惩罚逻辑
            }

            // 更新视图
            _view.UpdateStatistics(
                GameStateManager.correct,
                GameStateManager.wrong,
                GameStateManager.examTtl,
                GameStateManager.coinTtl
            );

            _view.RecordWrongAnswer(Equation, userResult, userRemainder, Result, NumHalfResult);
            _view.ShowErrorAnswerDialog(Equation, userResult, userRemainder, Result, NumHalfResult);
        }

        /// <summary>
        /// 处理考试完成
        /// </summary>
        private void HandleExamCompletion()
        {
            GameStateManager.finished = true;

            // 计算统计信息
            int totalQuestions = GameStateManager.correct + GameStateManager.wrong;
            double accuracy = totalQuestions > 0 ? (double)GameStateManager.correct / totalQuestions * 100 : 0;

            // 全对额外奖励（题目数量的1/2，向下取整）
            if (GameStateManager.wrong == 0 && GameStateManager.examTtlRec > 0)
            {
                int bonus = (int)(GameStateManager.examTtlRec * AppConstants.FullAnswerBonusCoefficient);
                GameStateManager.coinTtl += bonus;
                _view.ShowBonusDialog(bonus);
            }

            _view.DisableExamControls();
            _view.UpdateCompletionTime();
            _view.UpdateAverageTimeCost(totalQuestions);

            // 保存练习记录
            SavePracticeRecord();

            _logger.Info($"练习完成 - 正确：{GameStateManager.correct}，错误：{GameStateManager.wrong}，正确率：{accuracy:F2}%");
        }

        /// <summary>
        /// 保存练习记录
        /// </summary>
        private void SavePracticeRecord()
        {
            try
            {
                var record = new PracticeRecord
                {
                    StartTime = _startTime,
                    EndTime = DateTime.Now,
                    TotalSeconds = (int)(DateTime.Now - _startTime).TotalSeconds,
                    ExamType = GameStateManager.lstExamObjects[CurrentExamTypeIndex].Examtype,
                    TotalQuestions = GameStateManager.correct + GameStateManager.wrong,
                    CorrectCount = GameStateManager.correct,
                    WrongCount = GameStateManager.wrong,
                    CoinsEarned = GameStateManager.correct * GameStateManager.awardCoin
                };

                record.CalculateAccuracy();

                var historyService = new HistoryRecordService();
                historyService.SavePracticeRecord(record);

                _logger.Info($"练习记录已保存：{record.Id}");
            }
            catch (Exception ex)
            {
                _logger.Error($"保存练习记录失败：{ex.Message}");
                _logger.Exception(ex);
            }
        }

        /// <summary>
        /// 切换题型
        /// </summary>
        /// <param name="index">题型索引</param>
        public void SwitchExamType(int index)
        {
            if (index >= 0 && index < GameStateManager.lstExamObjects.Count)
            {
                CurrentExamTypeIndex = index;
                _view.UpdateExamTypeDisplay(index);
                _logger.Info($"切换题型：{GameStateManager.lstExamObjects[index].Examtype}");
            }
        }

        /// <summary>
        /// 暂停/恢复
        /// </summary>
        public void TogglePause()
        {
            GameStateManager.flagPause = !GameStateManager.flagPause;
            _view.UpdatePauseState(GameStateManager.flagPause);
            
            _logger.Info(GameStateManager.flagPause ? "暂停" : "恢复");
        }

        /// <summary>
        /// 使用辅助功能
        /// </summary>
        /// <param name="type">辅助类型</param>
        /// <returns>是否成功</returns>
        public bool UseHelper(string type)
        {
            try
            {
                if (type == "check")
                {
                    // 检查答案
                    if (GameStateManager.coinTtl >= GameStateManager.costCoinCheck)
                    {
                        GameStateManager.coinTtl -= GameStateManager.costCoinCheck;
                        _view.UpdateCoinDisplay(GameStateManager.coinTtl);
                        return true;
                    }
                }
                else if (type == "give")
                {
                    // 给出答案
                    if (GameStateManager.coinTtl >= GameStateManager.costCoinGive)
                    {
                        GameStateManager.coinTtl -= GameStateManager.costCoinGive;
                        _view.UpdateCoinDisplay(GameStateManager.coinTtl);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.Error($"使用辅助功能失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 显示辅助框
        /// </summary>
        public void ShowHelperBox()
        {
            if (GameStateManager.helpBox)
            {
                _view.ShowHelperBox(true);
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Cleanup()
        {
            // 保存配置
            try
            {
                var settings = new AppSettings(_configService)
                {
                    CoinTotal = GameStateManager.coinTtl,
                    Password = GameStateManager.PSW
                };
                settings.Save();
                _logger.Info("配置已保存");
            }
            catch (Exception ex)
            {
                _logger.Error($"保存配置失败：{ex.Message}");
            }
        }
    }

    /// <summary>
    /// 主窗体视图接口
    /// </summary>
    public interface IMainFormView
    {
        /// <summary>
        /// 显示题目
        /// </summary>
        /// <param name="equation">算式</param>
        void DisplayProblem(string equation);

        /// <summary>
        /// 更新统计信息
        /// </summary>
        void UpdateStatistics(int correct, int wrong, int remaining, int coins);

        /// <summary>
        /// 记录正确答案
        /// </summary>
        void RecordCorrectAnswer(string equation, int result, int remainder);

        /// <summary>
        /// 记录错误答案
        /// </summary>
        void RecordWrongAnswer(string equation, int userResult, int userRemainder, int correctResult, int correctRemainder);

        /// <summary>
        /// 显示错误答案对话框
        /// </summary>
        void ShowErrorAnswerDialog(string equation, int userResult, int userRemainder, int correctResult, int correctRemainder);

        /// <summary>
        /// 清空答案输入框
        /// </summary>
        void ClearAnswerInput();

        /// <summary>
        /// 禁用考试控件
        /// </summary>
        void DisableExamControls();

        /// <summary>
        /// 更新完成时间
        /// </summary>
        void UpdateCompletionTime();

        /// <summary>
        /// 更新平均用时
        /// </summary>
        void UpdateAverageTimeCost(int totalQuestions);

        /// <summary>
        /// 显示奖励对话框
        /// </summary>
        void ShowBonusDialog(int bonus);

        /// <summary>
        /// 更新题型显示
        /// </summary>
        void UpdateExamTypeDisplay(int index);

        /// <summary>
        /// 更新暂停状态
        /// </summary>
        void UpdatePauseState(bool isPaused);

        /// <summary>
        /// 更新金币显示
        /// </summary>
        void UpdateCoinDisplay(int coins);

        /// <summary>
        /// 显示/隐藏辅助框
        /// </summary>
        void ShowHelperBox(bool show);

        /// <summary>
        /// 显示错误
        /// </summary>
        void ShowError(string message);

        /// <summary>
        /// 从 Presenter 同步题目数据到视图
        /// </summary>
        /// <param name="num1">第一个操作数</param>
        /// <param name="num2">第二个操作数</param>
        /// <param name="result">正确答案</param>
        /// <param name="numHalfResult">余数（除法时使用）</param>
        /// <param name="equation">算式字符串</param>
        void SyncProblemData(int num1, int num2, int result, int numHalfResult, string equation);

        /// <summary>
        /// 重置答题定时器
        /// </summary>
        void ResetTimer();
    }
}
