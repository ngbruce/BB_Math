# 实现任务清单

## 1. 修复分题型统计（save() 方法）

- [x] 1.1 在 `Form1.save()` 的 `foreach` 循环前，为每个 `ExamObject` 设置 `SumQty = CorrectQty + WrongQty`
- [x] 1.2 将分题型统计段的输出格式从"剩余：X, 正确：Y, 错误：Z"改为"总数：X, 正确：Y, 错误：Z"（X = SumQty）

## 2. 修复答错惩罚的题型池行为

- [x] 2.1 在 `MainFormPresenter.HandleWrongAnswer()` 中移除 `ReinitializeExamTypePool()` 调用
- [x] 2.2 在 `ExamTypePool` 中添加 `AddToType()` 方法，答错时调用 `_examTypePool.AddToType(type, punishment)`

## 3. 日志增强

- [x] 3.1 练习完成时输出分题型统计到日志（`_logger.Info` 每个有参与的类型）
- [x] 3.2 创建 `tail-log.ps1` 脚本实时查看日志

## 4. 验证

- [x] 4.1 编译项目（用户已在 VS 2019 中完成）
- [x] 4.2 手动测试：完成一次含答错的练习，验证惩罚题目仅增加到错题型且从错题型抽取
- [x] 4.3 手动测试：验证 bbmath.dat 和日志中分题型统计正确输出
- [ ] 4.4 运行所有单元测试（需在 VS 2019 中完成）
