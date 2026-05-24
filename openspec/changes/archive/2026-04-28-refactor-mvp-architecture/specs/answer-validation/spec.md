## ADDED Requirements

### Requirement: 答案验证流程
系统 SHALL 提供完整的答案验证流程，包括输入验证、答案校验、奖励结算、记录更新等步骤。

#### Scenario: 正确答案处理流程
- **GIVEN** 用户输入正确答案
- **WHEN** Presenter 的 `ValidateAnswer()` 检测到答案正确
- **THEN** 停止定时器
- **AND** 更新正确计数（`GameStateManager.correct++`）
- **AND** 减少剩余题目数（`GameStateManager.examTtl--`）
- **AND** 奖励金币（`GameStateManager.coinTtl += awardCoin`）
- **AND** 记录正确答案到 UI（正确记录 TextBox）
- **AND** 调用 `View.UpdateStatistics()` 更新界面统计
- **AND** 如果未完成，调用 `Presenter.GenerateProblem()` 生成新题目
- **AND** 调用 `View.ClearAnswerInput()` 清空输入框

#### Scenario: 错误答案处理流程
- **GIVEN** 用户输入错误答案
- **WHEN** Presenter 的 `ValidateAnswer()` 检测到答案错误
- **WHEN** 停止定时器
- **AND** 更新错误计数（`GameStateManager.wrong++`）
- **AND** 增加题型用时惩罚（如需要）
- **AND** 记录错误答案到 UI（错误记录 TextBox）
- **AND** 显示错误答案对话框（`View.ShowErrorAnswerDialog()`）
- **AND** 调用 `View.UpdateStatistics()` 更新界面统计
- **AND** 对话框关闭后生成新题目

#### Scenario: 练习完成处理流程
- **GIVEN** 剩余题目数归零（`examTtl <= 0`）
- **WHEN** 用户答完最后一题
- **THEN** 标记练习完成（`GameStateManager.finished = true`）
- **AND** 计算正确率（`correct / (correct + wrong) * 100`）
- **AND** 如果全对，额外奖励金币（`examTtlRec * 0.5`）
- **AND** 调用 `View.DisableExamControls()` 禁用答题控件
- **AND** 调用 `View.UpdateCompletionTime()` 更新完成时间
- **AND** 调用 `View.UpdateAverageTimeCost()` 更新平均用时
- **AND** 保存练习记录到日志
- **AND** 显示"再练一次"按钮

### Requirement: 答案记录格式
系统 SHALL 以统一的格式记录正确和错误答案，支持有余数除法的特殊显示。

#### Scenario: 正确答案记录格式（普通题型）
- **GIVEN** 用户答对普通题型（加减乘、无余数除法）
- **WHEN** 记录正确答案
- **THEN** 记录格式为："算式 = 结果\r\n"
- **AND** 示例："10 + 20 = 30\r\n"
- **AND** 每条记录后添加换行符

#### Scenario: 正确答案记录格式（有余数除法）
- **GIVEN** 用户答对有余数除法
- **WHEN** 记录正确答案
- **THEN** 记录格式为："被除数 ÷ 除数 = 商......余数\r\n"
- **AND** 示例："10 ÷ 3 = 3......1\r\n"
- **AND** 使用六个点表示余数（与旧代码保持一致）

#### Scenario: 错误答案记录格式（普通题型）
- **GIVEN** 用户答错普通题型
- **WHEN** 记录错误答案
- **THEN** 记录格式为："算式 = 用户答案 ( 正确答案 )\r\n"
- **AND** 示例："10 + 20 = 40 ( 30 )\r\n"
- **AND** 用户答案和正确答案之间有空格

#### Scenario: 错误答案记录格式（有余数除法）
- **GIVEN** 用户答错有余数除法
- **WHEN** 记录错误答案
- **THEN** 记录格式为："被除数 ÷ 除数 = 商余余数 ( 正确商余正确余数 )\r\n"
- **AND** 示例："10 ÷ 3 = 2余2 ( 3余1 )\r\n"
- **AND** 余数使用中文"余"字表示

### Requirement: 输入验证
系统 SHALL 在验证答案之前验证用户输入的有效性。

#### Scenario: 普通题型输入验证
- **GIVEN** 题型为普通题型（加减乘、无余数除法）
- **WHEN** 用户提交答案
- **THEN** 仅验证结果输入框（`tbResult`）
- **AND** 如果输入不是数字，提示错误
- **AND** 不进行答案验证

#### Scenario: 有余数除法输入验证
- **GIVEN** 题型为有余数除法
- **WHEN** 用户提交答案
- **THEN** 验证结果输入框（`tbResult`）和余数输入框（`tbRemainder`）
- **AND** 如果任意一个输入框不是数字，提示错误
- **AND** 不进行答案验证

### Requirement: 定时器处理
系统 SHALL 在答题过程中正确管理定时器的启动、停止和重置。

#### Scenario: 答题时停止定时器
- **GIVEN** 用户点击提交答案按钮
- **WHEN** 开始验证答案
- **THEN** 停止定时器（`timer1.Stop()`）
- **AND** 记录当前题型用时（`ElapsedTime += TimeLimit - counterTimeOut`）

#### Scenario: 生成新题目时重置定时器
- **GIVEN** 验证答案完成，需要生成新题目
- **WHEN** 调用 `View.ResetTimer()`
- **THEN** 将 `counterTimeOut` 重置为当前题型的 `TimeLimit`
- **AND** 启动定时器（`timer1.Start()`）
- **AND** 更新进度条和倒计时显示

#### Scenario: 练习完成时停止定时器
- **GIVEN** 用户答完所有题目（`examTtl <= 0`）
- **WHEN** 调用 `View.DisableExamControls()`
- **THEN** 停止定时器
- **AND** 禁用定时器相关控件

### Requirement: 题型池管理
系统 SHALL 在答案验证后正确更新题型池状态。

#### Scenario: 答对题目后更新题型池
- **GIVEN** 用户答对题目
- **WHEN** 调用 `Presenter.GenerateProblem()` 生成新题目
- **THEN** 从题型池中抽取新题型
- **AND** 减少题型池剩余题目数
- **AND** 如果题型池为空，回退到随机选择

#### Scenario: 答错题目后重新初始化题型池
- **GIVEN** 用户答错题目（惩罚机制增加题目数）
- **WHEN** 答错处理完成
- **THEN** 调用 `Presenter.ReinitializeExamTypePool()`
- **AND** 根据新的总题目数重新分配题型数量
- **AND** 确保题型分布均匀
