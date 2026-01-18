## Purpose
数学运算模块负责生成各种题型的数学题目，包括加法、减法、乘法、有余数除法、无余数除法，并提供答案验证功能。
## Requirements
### Requirement: 数学题目生成
(SHALL) 系统应能生成五种数学运算类型的题目：加法、减法、乘法、有余数除法、无余数除法。生成的题目应符合配置的数值范围限制。系统应根据选择的难度级别 (LV1/LV2/LV3/LV4/LV5) 调整题目参数,包括数值范围和可用的运算类型。为避免概率偏差，系统应在练习开始时预先分配每种题型的题目数量，并在出题过程中从分配的题型池中随机抽取，确保各题型在有限题目数内尽可能均匀分布。系统应使用单一的 Random 实例来生成所有随机数，避免在每次生成题目时创建新的 Random 实例，以确保随机数的真正随机性和避免重复生成相同的题目序列。难度定义如下:
- LV1 (个位数加减法): 操作数1-9，加法和减法同等概率，减法结果>=0
- LV2 (100以内加减法): 操作数1-99，加法和减法同等概率，加法结果<100，减法结果>=0
- LV3 (个位数乘除法): 操作数1-9，乘法和除法同等概率，除法保证能整除或有余数
- LV4 (100以内加减乘除带余数): 操作数1-99，五类运算同等概率（加、减、乘、无余数除、有余数除）
- LV5 (100以上加减乘除带余数): 操作数1-999，五类运算同等概率（加、减、乘、无余数除、有余数除）。注意："100以上"指操作数范围扩展到三位数（1-999），并非强制要求操作数>=100

#### Scenario: Random 实例生命周期管理
- **GIVEN** 应用程序启动
- **WHEN** 创建 MainFormPresenter 实例
- **THEN** 初始化单一的 Random 实例，并在整个应用程序生命周期内重用
- **AND** MathProblemGenerator 和所有策略类（加法、减法、乘法、除法）都使用这个 Random 实例
- **AND** 每次生成题目时不会创建新的 Random 实例

#### Scenario: 避免随机数重复
- **GIVEN** 需要重新生成题目（因为不符合验证条件）
- **WHEN** 调用题目生成方法
- **THEN** 使用已初始化的 Random 实例生成新的随机数
- **AND** 生成的操作数与上一次不同（在合理的随机性范围内）
- **AND** 不会出现连续多次生成完全相同题目的问题

#### Scenario: 练习开始时分配题型数量
- **GIVEN** 用户选择难度和题目总数，点击"开始"按钮
- **WHEN** 系统开始练习
- **THEN** 根据题目总数和当前难度的题型种类数，预先分配每种题型的数量
- **AND** 建立题型池，记录每种题型的剩余题目数
- **AND** 如果题目总数 < 题型种类数，随机选择题目总数种题型各分配 1 题

#### Scenario: 从题型池随机抽取题型
- **GIVEN** 题型池包含题型及其剩余数量
- **WHEN** 生成新题目
- **THEN** 从题型池中随机抽取一个题型（非零的题型）
- **AND** 抽取后该题型的剩余数量减 1
- **AND** 如果某题型数量变为 0，从池中移除该题型
- **AND** 使用抽取的题型和重用的 Random 实例生成具体的数学题目

#### Scenario: LV1 难度题型分配
- **GIVEN** 当前难度为 LV1（2种题型：加法、减法），题目总数为 15
- **WHEN** 系统分配题型数量
- **THEN** 加法和减法各分配 7-8 题（15 ÷ 2 = 7 余 1，余数随机分配给其中一种题型）
- **AND** 题型池初始状态为 {(加法, 7), (减法, 8)} 或 {(加法, 8), (减法, 7)}

#### Scenario: LV4 难度题型分配
- **GIVEN** 当前难度为 LV4（5种题型：加、减、乘、无余数除、有余数除），题目总数为 15
- **WHEN** 系统分配题型数量
- **THEN** 五种题型各分配 3 题（15 ÷ 5 = 3）
- **AND** 题型池初始状态为 {(加法, 3), (减法, 3), (乘法, 3), (无余数除, 3), (有余数除, 3)}

#### Scenario: LV5 难度题型分配
- **GIVEN** 当前难度为 LV5（5种题型：加、减、乘、无余数除、有余数除），题目总数为 15
- **WHEN** 系统分配题型数量
- **THEN** 五种题型各分配 3 题（15 ÷ 5 = 3）
- **AND** 题型池初始状态为 {(加法, 3), (减法, 3), (乘法, 3), (无余数除, 3), (有余数除, 3)}

### Requirement: 答案验证
(SHALL) 系统应能验证用户提交的答案是否正确，支持普通运算和有余数除法的验证。

#### Scenario: 验证加法答案正确
- **GIVEN** 题目：25 + 37 = 62
- **WHEN** 用户提交答案 62
- **THEN** 返回验证通过（true）

#### Scenario: 验证加法答案错误
- **GIVEN** 题目：25 + 37 = 62
- **WHEN** 用户提交答案 60
- **THEN** 返回验证失败（false），提供正确答案 62

#### Scenario: 验证有余数除法答案正确
- **GIVEN** 题目：25 ÷ 7 = 3 余 4
- **WHEN** 用户提交商 = 3，余数 = 4
- **THEN** 返回验证通过（true）

#### Scenario: 验证有余数除法答案错误（商正确，余数错误）
- **GIVEN** 题目：25 ÷ 7 = 3 余 4
- **WHEN** 用户提交商 = 3，余数 = 3
- **THEN** 返回验证失败（false），提供正确答案 商 = 3，余数 = 4

#### Scenario: 验证有余数除法答案错误（商和余数都错误）
- **GIVEN** 题目：25 ÷ 7 = 3 余 4
- **WHEN** 用户提交商 = 4，余数 = 3
- **THEN** 返回验证失败（false），提供正确答案

### Requirement: 余数输入框显示逻辑
(SHALL) 系统应根据当前题型决定是否显示余数输入框，**只**在题型为有余数除法(DivisionWithRemainder)时显示余数输入框(tbRemainder)和余数标签(lbRemainder)，其他情况下应全部隐藏。

#### Scenario: 有余数除法时显示余数输入框
- **GIVEN** 当前题型为 DivisionWithRemainder
- **WHEN** 显示题目界面
- **THEN** 余数输入框和余数标签都应可见，余数标签显示"余："

#### Scenario: 无余数除法时隐藏余数输入框
- **GIVEN** 当前题型为 DivisionNoRemainder
- **WHEN** 显示题目界面
- **THEN** 余数输入框和余数标签都应不可见

#### Scenario: 加减乘法时隐藏余数输入框
- **GIVEN** 当前题型为 Addition、Subtraction 或 Multiplication
- **WHEN** 显示题目界面
- **THEN** 余数输入框和余数标签都应不可见

#### Scenario: 余数输入框不应受 helpBox 影响
- **GIVEN** helpBox 设置为 true
- **WHEN** 当前题型为 Addition
- **THEN** 余数输入框应不可见（不受 helpBox 影响）

---

### Requirement: currentTypeIndex 同步
(SHALL) 系统在生成题目后，应同步更新 GameStateManager.currentTypeIndex 到当前题型在 lstExamObjects 中的索引。

#### Scenario: MainFormPresenter 生成题目时同步 currentTypeIndex
- **GIVEN** 在 MainFormPresenter.GenerateProblem() 中随机选择题型为 DivisionWithRemainder
- **WHEN** 生成题目完成
- **THEN** GameStateManager.currentTypeIndex 应更新为 lstExamObjects 中 DivisionWithRemainder 的索引

#### Scenario: Form1 生成题目时同步 currentTypeIndex
- **GIVEN** 在 Form1.GenNum() 中选择题型为 Addition
- **WHEN** 生成题目完成
- **THEN** GameStateManager.currentTypeIndex 应更新为 lstExamObjects 中 Addition 的索引

---

### Requirement: 除法策略的 intBits 计算方式
(SHALL) 各除法策略（DivisionNoRemainderStrategy 和 DivisionWithRemainderStrategy）对 intBits 参数的使用方式应保持一致。

- **除法策略**：使用 `max = 10^intBits`
  - intBits = 1 → 生成 1-9 的个位数
  - intBits = 2 → 生成 10-99 的两位数

#### Scenario: 统一 intBits 计算公式
- **GIVEN** intBits = 1
- **WHEN** 生成除法题目
- **THEN** 除数和商的范围为 1-9

#### Scenario: 两位数除法生成
- **GIVEN** intBits = 2
- **WHEN** 生成除法题目
- **THEN** 除数和商的范围为 10-99

#### Scenario: LV4 除法调整
- **GIVEN** LV4 难度（要求操作数和结果 < 100，IntegerBits = 2）
- **WHEN** 生成除法题目
- **THEN** 应使用 effectiveIntBits = 1，生成个位数除法（1-9）
- **THEN** 确保被除数 = 除数 × 商（或除数 × 商 + 余数）< 100

