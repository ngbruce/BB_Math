# Change: 修复每题完成后定时器未重置的问题

## Why
在测试中发现，每题答题完成后，生成新题时倒计时定时器不会重新开始计时，导致时间限制失效。

**问题现象**：
1. 每题有时间限制（常量定义为120秒）
2. 超过时间限制会惩罚（增加题目数量）
3. 答完一题后，出新题时，倒计时应该重新开始
4. 但现在答题后定时器不会重置，继续从上次剩余时间倒数

**根本原因**：
在 MVP 架构重构后，`MainFormPresenter.GenerateProblem()` 调用 `_view.DisplayProblem()` 显示新题目，但没有重置定时器。定时器只在以下地方重置：
- `btnStart_Click()` - 开始练习时
- `timer1_Tick()` - 超时时
- `GenNum()` - 旧的生成方法（已废弃）

由于现在统一使用 `MainFormPresenter.GenerateProblem()` 生成题目，而 Presenter 层无法直接访问 Form1 的定时器，需要通过 View 接口来重置定时器。

**影响范围**：
- 所有难度级别（LV1-LV5）
- 所有题型
- 任何使用了 Presenter 生成题目的场景

## What Changes

### 1. 扩展 IMainFormView 接口
- **ADDED** 在 `UI/MainFormPresenter.cs` 的 `IMainFormView` 接口中添加 `ResetTimer()` 方法
- **ADDED** 方法签名：`void ResetTimer()`
- **PURPOSE**：允许 Presenter 通知 View 重置定时器

### 2. 实现 ResetTimer() 方法
- **ADDED** 在 `Form1.cs` 中实现 `IMainFormView.ResetTimer()` 方法
- **IMPLEMENTATION**：重置 `counterTimeOut` 为当前题型的 `TimeLimit`，并重新启动 `timer1`
- **CODE**：
  ```csharp
  public void ResetTimer()
  {
      // 设置倒计时时间为当前题型的时间限制
      counterTimeOut = GameStateManager.lstExamObjects[GameStateManager.currentTypeIndex].TimeLimit;
      // 重启定时器
      timer1.Start();
  }
  ```

### 3. 在生成新题时调用定时器重置
- **MODIFIED** 在 `UI/MainFormPresenter.cs` 的 `GenerateProblem()` 方法中
- **CHANGE**：在调用 `_view.DisplayProblem()` 后，调用 `_view.ResetTimer()`
- **LOCATION**：第 316 行之后
- **CODE**：
  ```csharp
  // 更新视图
  _view.DisplayProblem(Equation);

  // 重置定时器
  _view.ResetTimer();
  ```

### 4. 更新相关规范文档
- **MODIFIED** 更新 `openspec/specs/time-management/spec.md`，添加关于定时器重置的要求
- **ADDED** 新的场景描述：每题生成后应重置定时器

## Impact

- **Affected specs**: time-management
- **Affected code**:
  - `UI/MainFormPresenter.cs` - 在 `IMainFormView` 接口中添加方法，在 `GenerateProblem()` 中调用
  - `Form1.cs` - 实现 `ResetTimer()` 方法

- **Breaking changes**: 无（添加新方法，不改变现有接口）

- **Performance impact**: 无影响

- **User impact**:
  - ✅ 每题完成后，倒计时正确重置为时间限制（120秒）
  - ✅ 时间限制机制正常工作，超时会惩罚加题
  - ✅ 用户体验改善，每题都有公平的时间限制

## Related Changes

- `fix-all-difficulty-random-generation-bug` - 修复了随机数生成器问题，与本次修复同时进行
- `2026-01-15-fix-remainder-input-and-logging-issues` - 之前的 MVP 架构重构，引入了 Presenter 模式

## Test Plan

- [ ] 1.1 测试 LV1 难度，完成一题后，验证倒计时重置为60秒
- [ ] 1.2 测试 LV2 难度，完成一题后，验证倒计时重置为60秒
- [ ] 1.3 测试 LV3 难度，完成一题后，验证倒计时重置为60秒
- [ ] 1.4 测试 LV4 难度，完成一题后，验证倒计时重置为60秒
- [ ] 1.5 测试 LV5 难度，完成一题后，验证倒计时重置为60秒

- [ ] 2.1 测试所有题型（加、减、乘、无余数除、有余数除），验证倒计时重置
- [ ] 2.2 答题超时，验证惩罚机制正常工作（增加题目数量）
- [ ] 2.3 超时后重新开始答题，验证倒计时正确重置

- [ ] 3.1 连续答题10题，验证每题倒计时都正确重置
- [ ] 3.2 观察日志，确认定时器重置调用时机正确
- [ ] 3.3 检查没有重复启动定时器的问题
