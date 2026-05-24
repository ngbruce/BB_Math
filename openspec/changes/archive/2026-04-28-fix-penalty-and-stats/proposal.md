## 为什么

BB_Math 的练习记录（bbmath.dat）中，分题型统计段因 `ExamObject.SumQty` 从未赋值而从不输出，用户无法查看各题型的答题分布。同时，答错惩罚的题型池重新分配机制会丢弃已抽取历史，导致最终各题型数量分布不均衡。这两个问题影响用户审阅练习效果的正确性。

## 变更内容

- **修复** `ExamObject.SumQty` 未赋值导致分题型统计数据不输出的 bug
- **新增** 分题型统计输出中包含"总数"字段，便于用户了解各题型答题量
- **修改** 答错惩罚机制：惩罚题目仅追加到当前答错题型，不再清除整个题型池重新分配

## 功能 (Capabilities)

### 修改功能
- `scoring-system`: 练习记录输出的分题型统计段增加"总数"字段
- `punishment-system`: 答错惩罚题目仅增加到当前答错题型，不再重新初始化整个题型池

## 影响

- **代码**:
  - `Form1.cs` save() — 在保存前计算 `SumQty = CorrectQty + WrongQty`，增加总数输出
  - `MainFormPresenter.cs` HandleWrongAnswer() — 移除 `ReinitializeExamTypePool()` 调用，改为直接增加当前题型的 `TotalQty`
  - `Core/ExamTypePool.cs` — `ReinitializeExamTypePool()` 方法将不再被答错路径调用（保留供超时路径使用）
- **行为变更**: 答错后题型分布不再全量重新均分，而是仅增加做错题型的数量
- **无破坏性变更**: 配置文件格式和历史记录格式不变
