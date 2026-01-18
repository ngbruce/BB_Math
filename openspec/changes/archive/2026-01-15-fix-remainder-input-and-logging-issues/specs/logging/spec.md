## ADDED Requirements

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
