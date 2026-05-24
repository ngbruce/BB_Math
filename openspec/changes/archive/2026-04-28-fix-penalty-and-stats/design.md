## 上下文

当前 `Form1.save()` 方法在输出练习记录时，有一个分题型统计循环（`foreach ExamObject`），但循环被 `if (obj.SumQty > 0)` 守卫，而 `SumQty` 从未被赋值，导致该段永远不输出。

当前 `MainFormPresenter.HandleWrongAnswer()` 在答错时调用 `ReinitializeExamTypePool()`，该方法用 `Clear()` 清空池后按剩余题数重新均分。此操作丢弃了已抽取的历史，如果已抽题分布不匀，最终分布会被锁定在不平衡状态。例如：若前几题连续抽到加法，答错后整个池被重置，加法的最终数量可能远高于其他题型。

## 目标 / 非目标

**目标：**
- 在 `save()` 执行前设置 `SumQty = CorrectQty + WrongQty`，使分题型统计正常输出
- 分题型统计段增加"总数"字段（`CorrectQty + WrongQty`）
- 答错惩罚改为仅向当前答错题型追加 `TotalQty`，不再清除和重新初始化题型池

**非目标：**
- 不改变 `PracticeRecord` 类或 XML 历史格式
- 不改变配置文件格式
- 不修改超时惩罚的题型池行为

## 决策

### Decision 1: 在 `save()` 中内联计算 SumQty 而非单独维护

在 `save()` 方法的 `foreach` 循环开始前，为每个 `ExamObject` 设置 `SumQty = CorrectQty + WrongQty`。这避免了需要在多处（Presenter 和 Form1）维护该字段的同步问题。

### Decision 2: 答错惩罚仅追加到当前题型

在 `HandleWrongAnswer()` 中移除 `ReinitializeExamTypePool()` 调用，改为通过已存在的 `examTtl += punishment` 和 `lstExamObjects[currentTypeIndex].TotalQty += punishment` 将惩罚题直接追加到当前错题型。

理由：惩罚的目的是让用户练习做错的题型类型，重置整个池稀释了惩罚效果且引入分布不均问题。

替代方案：
- **保留重置但跟踪历史消耗**：复杂度高，需要修改 ExamTypePool 的 Initialize 方法接受历史消耗参数
- **仅追加到错题型（选中）**：实现简单，语义清晰

### Decision 3: 分题型输出增加"总数"

在 `save()` 的分题型统计段中增加 `总数：<SumQty>`，使输出从：
```
<--题型-->   整数加法  , 	剩余：3, 正确：8, 错误：1
```
变为：
```
<--题型-->   整数加法  , 总数：9, 正确：8, 错误：1
```

## 风险 / 权衡

- [风险] 移除 `ReinitializeExamTypePool()` 后，答错不再重新均匀分布剩余题型 → **可接受**：惩罚本意是针对该题型，额外均匀分布会稀释针对性练习效果
- [风险] 连续在同一个题型上答错可能导致该题型占比过高 → **可接受**：这是合理的惩罚机制，且 `examTtl` 仍然正确跟踪总剩余题数

## 性能修复

- **`timer2` (进程检查) 已禁用**：`Process.GetProcesses()` + `MainWindowHandle` 访问在 UI 线程上阻塞，导致输入卡顿。原始 Kill 逻辑已被注释。如需恢复，必须改用后台线程（如 `System.Threading.Timer` 或 `Task.Run`）异步执行。
- **`timer1` 超时优化**：`ReinitializeExamTypePool()` (Clear + 全量重分配 + 多行日志 I/O) → `AddTimeoutPenalty()` (仅向当前题型 `AddToType(examType, 1)`)。

## 待定问题

无。
