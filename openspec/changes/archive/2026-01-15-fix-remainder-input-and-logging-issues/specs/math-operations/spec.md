## ADDED Requirements

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

### Requirement: 除法策略的 intBits 使用方式（待讨论）
(SHALL) 各题型生成策略对 intBits 参数的使用方式应保持一致。当前存在以下不一致：

- **加减乘法策略**：使用 `min = 10^(intBits-1)`, `max = 10^intBits`
  - intBits = 2 → 10-99（两位数）
- **除法策略**：使用 `max = 10^(intBits-1)`
  - intBits = 2 → 1-9（个位数）

**临时修复**：DivisionNoRemainderStrategy 当前硬编码生成 1-9 范围的除法，忽略 intBits 参数。

**需要进一步讨论**：
- 除法题目是否应该与加减乘法使用相同的 intBits 解释？
- 除法题目的数值范围应该如何定义？
- 除法题目的难度应该如何与 intBits 关联？

#### Scenario: 统一 intBits 使用方式（待实现）
- **GIVEN** intBits = 2
- **WHEN** 生成除法题目
- **THEN** 除数和商的范围应该明确定义（当前为 1-9）

#### Scenario: 除法题目生成失败（待调查）
- **GIVEN** LV4 难度配置（RequireAbove100 = false, IntegerBits = 2）
- **WHEN** 随机选择到某个题型并尝试生成题目
- **THEN** 应在合理次数内（<100）生成满足条件的题目，而不是抛出异常
