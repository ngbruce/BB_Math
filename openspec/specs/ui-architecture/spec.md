## Purpose
UI 架构规范定义系统的 Model-View-Presenter (MVP) 架构模式，明确 View、Presenter、Model 的职责划分和交互方式，确保代码结构清晰，业务逻辑与界面显示分离。

## Requirements
### Requirement: MVP 架构模式
系统 SHALL 使用 Model-View-Presenter (MVP) 架构模式组织用户界面代码，实现业务逻辑与界面显示的清晰分离。

#### Scenario: Presenter 处理业务逻辑
- **GIVEN** 用户点击答题按钮
- **WHEN** 调用 `button2_Click` 事件处理
- **THEN** View 仅收集用户输入并委托给 Presenter
- **AND** Presenter 的 `ValidateAnswer()` 方法执行答案验证
- **AND** Presenter 更新游戏状态（金币、正确数、错误数等）
- **AND** Presenter 调用 View 接口方法更新界面

#### Scenario: View 只负责界面交互
- **GIVEN** 用户操作界面（点击按钮、输入文本等）
- **WHEN** View 收集到用户输入
- **THEN** View 不直接处理业务逻辑（验证答案、计算奖励等）
- **AND** View 仅调用 Presenter 的公共方法
- **AND** View 通过实现 `IMainFormView` 接口接收 Presenter 的更新指令

#### Scenario: 禁止 View 直接处理业务逻辑
- **GIVEN** 系统代码中存在 `Form1.answer()` 等遗留方法
- **WHEN** 开发者尝试在 View 中直接处理答题逻辑
- **THEN** 此方法必须标记为 `[Obsolete]`
- **AND** 新代码必须通过 Presenter 处理业务逻辑
- **AND** 旧代码路径（如直接调用 `answer()`）必须被禁用或移除

### Requirement: 答题流程的单一执行路径
系统 SHALL 确保答题功能只有一条有效的执行路径，避免多重逻辑导致维护困难。

#### Scenario: 答题入口统一
- **GIVEN** 用户点击"提交答案"按钮或按回车键
- **WHEN** 触发答题事件
- **THEN** 所有事件处理程序都调用 `_presenter.ValidateAnswer()`
- **AND** 不允许直接调用遗留的 `answer()` 方法
- **AND** 答题流程完全由 Presenter 控制

#### Scenario: 答题结果记录统一
- **GIVEN** 用户提交答案后验证完成
- **WHEN** 系统需要记录答题结果
- **THEN** Presenter 直接调用 View 的 TextBox 控件追加文本
- **AND** 使用 `\r\n` 作为换行符
- **AND** 不通过 `IMainFormView.RecordCorrectAnswer()` 等接口方法
- **AND** 接口中的记录方法标记为 `[Obsolete]`

#### Scenario: 遗留代码处理
- **GIVEN** 存在历史遗留代码（如 `Form1.answer()`）
- **WHEN** 重构代码以统一 MVP 架构
- **THEN** 遗留方法标记为 `[Obsolete]` 并保留供参考
- **AND** 遗留方法不再被调用
- **AND** 移除 `IMainFormView` 接口中未使用的记录方法

### Requirement: MVP 职责划分
系统 SHALL 明确 View、Presenter、Model 的职责范围，确保代码结构清晰。

#### Scenario: View 的职责
- **GIVEN** View 实现类 `Form1`
- **WHEN** 定义 View 的功能
- **THEN** View 负责界面显示和用户输入收集
- **AND** View 实现 `IMainFormView` 接口
- **AND** View 通过构造函数注入 Presenter 实例
- **AND** View 不包含业务逻辑（答案验证、金币计算等）

#### Scenario: Presenter 的职责
- **GIVEN** Presenter 实现类 `MainFormPresenter`
- **WHEN** 定义 Presenter 的功能
- **THEN** Presenter 负责业务逻辑处理
- **AND** Presenter 管理游戏状态（GameStateManager）
- **AND** Presenter 调用 Model 层服务（题目生成、答案验证等）
- **AND** Presenter 通过 View 接口更新 View 显示

#### Scenario: Model 的职责
- **GIVEN** Model 层包含各种服务和状态管理类
- **WHEN** 定义 Model 的功能
- **THEN** Model 提供数据和业务规则
- **AND** Model 不依赖 View（UI 层）
- **AND** Model 可独立进行单元测试
