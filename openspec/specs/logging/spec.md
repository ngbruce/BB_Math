## Purpose
日志系统记录每次练习的详细数据，包括开始时间、答题结果、用时、金币数等信息，便于后续分析和成绩追踪。
## Requirements
### Requirement: 日志记录
(SHALL) 系统应将每次练习的结果记录到日志文件，包括开始时间、答题结果、用时、金币数等信息。

#### Scenario: 记录练习结果
- **GIVEN** 用户完成 20 题练习，正确 18 题，错误 2 题，用时 5 分 30 秒
- **WHEN** 练习结束时
- **THEN** 日志文件（bbmath.dat）追加一条记录，包含开始时间、正确/错误数、用时、金币数

#### Scenario: 日志文件不存在时创建
- **GIVEN** 应用程序首次运行，日志文件不存在
- **WHEN** 第一次保存练习记录
- **THEN** 自动创建日志文件并完成写入

#### Scenario: 日志内容格式
- **GIVEN** 一次练习完成
- **WHEN** 查看日志文件
- **THEN** 日志包含：开始时间、正确数、错误数、未完成数、金币数、用时（时分秒）、题型统计、软件版本

#### Scenario: 查看历史日志
- **GIVEN** 日志文件已存在多条记录
- **WHEN** 点击查看日志按钮
- **THEN** 显示日志查看窗口，展示所有历史记录

#### Scenario: 应用关闭时自动保存
- **GIVEN** 用户正在答题，未正常完成
- **WHEN** 关闭应用程序窗口
- **THEN**: 自动保存当前进度到日志文件

### Requirement: 日志服务抽象接口
(SHALL) 系统应提供日志服务接口（ILoggerService），支持多级别日志记录（Debug、Info、Warning、Error），替代直接使用 Debug.Print()。

#### Scenario: 记录调试信息
- **GIVEN**: 调试模式开启
- **WHEN**: 调用 `logger.Debug("题目生成完成: {0}", equation)`
- **THEN**: 日志写入到调试输出和日志文件（如果启用文件日志）

#### Scenario: 记录普通信息
- **GIVEN**: 应用程序正常运行
- **WHEN**: 调用 `logger.Info("应用启动，版本: {0}", version)`
- **THEN**: 信息写入到日志文件，包含时间戳和日志级别

#### Scenario: 记录警告信息
- **GIVEN**: 检测到非关键问题（如配置文件损坏）
- **WHEN**: 调用 `logger.Warning("配置文件损坏，使用默认值")`
- **THEN**: 警告信息写入日志文件，可在日志查看器中筛选查看

#### Scenario: 记录错误信息
- **GIVEN**: 发生异常
- **WHEN**: 在 catch 块中调用 `logger.Error(ex, "保存记录时发生错误")`
- **THEN**: 错误信息、异常堆栈和发生时间写入日志文件

#### Scenario: 日志级别控制
- **GIVEN**: 配置文件中设置日志级别为 Warning
- **WHEN**: 调用 `logger.Debug()` 和 `logger.Info()`
- **THEN**: Debug 和 Info 日志被忽略，只记录 Warning 及以上级别

### Requirement: 结构化日志记录
(SHALL) 日志服务应支持结构化日志，便于日志分析和筛选。

#### Scenario: 记录带上下文的信息
- **GIVEN**: 用户正在答题，当前题目索引为 5
- **WHEN**: 调用 `logger.Info("用户提交答案", new { QuestionIndex = 5, UserAnswer = 42, CorrectAnswer = 43 })`
- **THEN**: 日志包含结构化数据，可解析为 JSON 格式

#### Scenario: 性能日志记录
- **GIVEN**: 记录题目生成耗时
- **WHEN**: 调用 `logger.Info("题目生成完成", new { Duration = 125, QuestionType = "Multiplication" })`
- **THEN**: 日志包含性能指标，可用于分析

### Requirement: 日志查看器集成
(SHALL) 系统应提供日志查看功能，支持在应用内查看和筛选日志。

#### Scenario: 查看所有日志
- **GIVEN**: 日志文件包含多条不同级别的日志
- **WHEN**: 打开日志查看窗口
- **THEN**: 按时间倒序列出所有日志条目

#### Scenario: 按级别筛选日志
- **GIVEN**: 日志文件包含 Debug、Info、Warning、Error 级别日志
- **WHEN**: 在查看器中选择只显示 Error 和 Warning
- **THEN**: 只显示 Error 和 Warning 级别的日志

#### Scenario: 按日期范围筛选
- **GIVEN**: 日志文件包含多天日志
- **WHEN**: 设置日期范围为最近 3 天
- **THEN**: 只显示最近 3 天内的日志

#### Scenario: 搜索日志内容
- **GIVEN**: 日志文件包含多条日志
- **WHEN**: 在搜索框中输入关键词 "错误"
- **THEN**: 显示包含该关键词的日志条目

### Requirement: 调试日志输出
(SHALL) 系统应在关键操作点输出调试日志，便于问题追踪和性能分析。

#### Scenario: 随机选择题型时输出日志
- **GIVEN** 在 MainFormPresenter.GenerateProblem() 中随机选择题型
- **WHEN** 选择完成
- **THEN** 应输出日志：`随机选择题型: randomIndex={index}, examType={type}`

#### Scenario: 同步 currentTypeIndex 时输出日志
- **GIVEN** 题型确定后
- **WHEN** 同步 GameStateManager.currentTypeIndex
- **THEN** 应输出日志：`同步 currentTypeIndex: {index}, 题型: {type}, 描述: {description}`

#### Scenario: 重试题目时输出详细日志
- **GIVEN** 生成的题目不符合难度要求
- **WHEN** 继续重试
- **THEN** 应输出日志包含：题目算式、操作数、结果、不符合条件的原因

#### Scenario: 生成题目失败时输出错误日志
- **GIVEN** 尝试次数超过最大限制（100）
- **WHEN** 抛出异常
- **THEN** 应输出错误日志包含：题型、IntegerBits、RequireAbove100 等关键信息

#### Scenario: lstExamObjects 初始化时输出日志
- **GIVEN** Form1_Load 执行
- **WHEN** lstExamObjects 初始化完成
- **THEN** 应输出日志包含：对象总数、每个对象的索引、题型和描述

---

### Requirement: 答题流程日志输出
(SHALL) 答题流程的日志输出应清晰反映执行顺序，避免混淆。

#### Scenario: 答题正确并生成新题目
- **GIVEN** 用户答题正确且还有剩余题目
- **WHEN** 开始生成新题目
- **THEN** 应输出日志：`答题正确，currentTypeIndex={index}。开始生成新题目...`

#### Scenario: 新题目界面更新完成
- **GIVEN** 新题目已生成并显示
- **WHEN** UpdateDisp() 执行完成
- **THEN** 应输出日志：`界面更新完成，新题型 currentTypeIndex={index}`

