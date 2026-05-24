# Change: 统一 MVP 架构，消除双重逻辑混乱

## Why
当前代码中存在 MVP 模式（MainFormPresenter + IMainFormView）和遗留模式（直接调用 answer()）两条并行的执行路径，导致：

1. **维护困难**：`RecordCorrectAnswer()` 和 `RecordWrongAnswer()` 在两个位置实现但只在一个位置生效
2. **功能混淆**：Presenter 调用的接口方法未被执行，实际执行的是 `Form1.answer()` 中的遗留代码
3. **文档失效**：OpenSpec 规范中描述的行为与实际代码不一致
4. **未来隐患**：如果在 Presenter 中添加逻辑，会被遗留路径绕过

**重要说明**：当前的 `Form1.answer()` 方法（Form1.cs 第 104-389 行）经过充分测试，功能正常工作。所有答题逻辑（正确/错误处理、定时器控制、金币奖励、记录格式等）都已在该方法中正确实现。重构的目标是统一架构路径，而非改变功能行为。因此，**在重构时必须认真参照 answer() 方法的所有逻辑，确保 MVP 路径完全复现其功能，避免引入功能回归**。

这是架构重构历史遗留问题（根据 session-summary.md 记录，曾经尝试迁移但因功能问题回退）。

## What Changes
- **BREAKING** 移除 `Form1.answer()` 遗留方法中的业务逻辑
- **BREAKING** 修改 `button2_Click` 调用 Presenter 的 `ValidateAnswer()` 方法
- **MODIFIED** 统一答案验证流程为 MVP 模式
- **MODIFIED** 更新 OpenSpec 规范，明确 MVP 架构为唯一有效的执行路径
- **REMOVED** 删除 Presenter 中未生效的 `RecordCorrectAnswer()` 和 `RecordWrongAnswer()` 调用
- **REMOVED** 删除 `IMainFormView` 接口中废弃的记录方法

## Impact
- **Affected specs**:
  - `ui-architecture` (新建规范，定义 MVP 架构)
  - `answer-validation` (新建规范，定义答案验证流程)
  
- **Affected code**:
  - `UI/MainFormPresenter.cs` - 简化 Presenter 逻辑，移除未执行的接口调用
  - `Form1.cs` - 移除 `answer()` 方法，修改 `button2_Click`，保留接口实现
  - `Form1.Designer.cs` - 可能需要调整事件绑定（如适用）
  
- **User impact**:
  - ✅ 代码维护更清晰，只有一条执行路径
  - ✅ 符合项目整体架构设计
  - ⚠️ 需要全面测试确保功能不回归
  
- **Risk**:
  - 修改核心答题流程，可能引入功能回归
  - 需要确保所有边界情况都被覆盖（错误处理、定时器、暂停、完成等）
