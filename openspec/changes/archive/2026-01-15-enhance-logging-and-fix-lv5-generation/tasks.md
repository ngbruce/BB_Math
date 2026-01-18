# 实现任务清单

## 1. 修复测试日志打印答案问题
- [x] 1.1 在Form1.cs的GenNum()方法中，为加法添加答案日志输出
- [x] 1.2 在Form1.cs的GenNum()方法中，为减法添加答案日志输出
- [x] 1.3 在UI/MainFormPresenter.cs的GenerateProblem()方法中，为所有题型添加统一的答案日志输出
- [x] 1.4 修复变量名冲突问题（examObject -> currentExamObject）

## 2. 优化全对奖励金币系统
- [x] 2.1 在Core/AppConstants.cs中添加常量FullAnswerBonusCoefficient = 0.5
- [x] 2.2 修改Form1.cs中的奖励计算逻辑，使用常量而非硬编码的 / 2
- [x] 2.3 修改UI/MainFormPresenter.cs中的奖励计算逻辑，使用常量
- [x] 2.4 更新文档openspec/specs/scoring-system/spec.md，反映新的计算逻辑

## 3. 修复LV5难度出题重试问题
### 3.1 修复除法生成策略
- [x] 3.1.1 修改Core/DivisionNoRemainderStrategy.cs，使用intBits参数而非硬编码个位数
- [x] 3.1.2 修复Core/DivisionWithRemainderStrategy.cs中的范围计算错误

### 3.2 修复LV5配置和验证逻辑
- [x] 3.2.1 修改Core/DifficultyConfiguration.cs，移除LV5的RequireAbove100约束
- [x] 3.2.2 修改UI/MainFormPresenter.cs，LV5乘法使用两位数（effectiveIntBits=2）
- [x] 3.2.3 重构UI/MainFormPresenter.cs中的验证逻辑，根据IntegerBits区分处理
- [x] 3.2.4 确保LV1-LV4检查<100，LV5仅检查加法≤2000

### 3.3 更新相关文档
- [x] 3.3.1 更新openspec/specs/math-operations/spec.md，添加LV5乘法说明
- [x] 3.3.2 更新openspec/specs/difficulty-selection/spec.md，说明LV5"100以上"的含义

## 4. 修复答题后题型分布不均
- [x] 4.1 将Form1.cs中答题正确后的GenNum()调用改为_presenter.GenerateProblem()（第170行）
- [x] 4.2 将Form1.cs中答题错误后的GenNum()调用改为_presenter.GenerateProblem()（第219行）
- [x] 4.3 确认Form1.cs中没有其他地方调用GenNum()

## 5. 测试验证
- [x] 5.1 测试所有题型（加、减、乘、无余数除、有余数除），确认测试日志中都有答案输出
- [x] 5.2 测试全对奖励，确认奖励数量为题目数/2（向下取整）
- [x] 5.3 测试LV5加法，确认能正常生成，操作数100-999，结果≤2000
- [x] 5.4 测试LV5减法，确认能正常生成，操作数100-999，结果0-899
- [x] 5.5 测试LV5乘法，确认使用两位数（10-99），结果100-9801
- [x] 5.6 测试LV5无余数除法，确认除数和商都是两位数（10-99）
- [x] 5.7 测试LV5有余数除法，确认除数和商都是两位数（10-99）
- [x] 5.8 压力测试：在LV5难度下运行15-30题，验证题型分布接近均匀（各约20%）
- [x] 5.9 回归测试LV1-LV4难度，确认原有功能不受影响
- [x] 5.10 检查日志输出，确认格式统一且清晰

## 已完成的问题分析

### 问题1：测试日志缺少答案输出
**现象**：只有乘法、有余数除法、无余数除法打印答案，加法和减法没有
**原因**：Form1.GenNum()中只为部分题型添加了答案日志
**解决方案**：为所有题型添加答案日志输出

### 问题2：全对奖励金币不便管理
**现象**：使用硬编码的 / 2 计算奖励
**原因**：没有使用常量管理
**解决方案**：添加常量FullAnswerBonusCoefficient = 0.5

### 问题3：LV5除法重试失败
**现象**：无余数除法在LV5时无限重试
**原因**：
- DivisionNoRemainderStrategy硬编码个位数（1-9），忽略intBits参数
- RequireAbove100 = true要求至少一个数>=100，但除法生成两位数（10-99）
**解决方案**：
- 修复除法策略使用intBits
- 移除RequireAbove100约束

### 问题4：LV5乘法重试失败
**现象**：乘法在LV5时无限重试
**原因**：
- IntegerBits=3生成三位数（100-999）
- 两个三位数相乘结果可达998001
- 验证逻辑要求<100（因为RequireAbove100=false）
**解决方案**：
- LV5乘法使用两位数（effectiveIntBits=2）
- 重构验证逻辑，根据IntegerBits区分处理

### 问题5：验证逻辑设计缺陷
**现象**：无法正确处理LV5的约束
**原因**：原逻辑为二元（RequireAbove100），实际需求是三元
**解决方案**：根据IntegerBits区分处理（LV1-4检查<100，LV5特定约束）

### 问题6：答题后题型分布不均
**现象**：15题测试中只有1道有余数除法（randomIndex=4），期望约3道；`randomIndex=4`一次都没有生成
**根本原因**：
- Form1.cs的答题逻辑中调用了旧版 `GenNum()` 方法
- `GenNum()` 方法使用旧的随机逻辑，不是均匀分布
- 应该使用 `MainFormPresenter.GenerateProblem()`，它使用 `random.Next(0, 5)` 均匀选择题型
**解决方案**：
- 将答题正确和答题错误后的 `GenNum()` 调用改为 `_presenter.GenerateProblem()`
- 确保所有题目生成使用统一的随机逻辑
