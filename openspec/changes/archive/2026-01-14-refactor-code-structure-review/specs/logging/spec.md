## ADDED Requirements

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
