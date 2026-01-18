# Change: 优化答案日志输出和全对奖励系统，修复LV5难度出题重试问题，修复答题后题型分布不均

## Why
在测试和调试过程中发现了以下问题需要修复：
1. 测试日志文件中只有部分题型打印了答案，加法和减法缺少答案输出，影响手动测试效率
2. 全对奖励金币的计算逻辑使用硬编码的除法 `/ 2`，不便于管理和修改
3. LV5难度下，除法和乘法题型出现无限重试问题（最多100次），导致出题失败
4. 答题后题型分布不均，15题测试中只有1道有余数除法（期望约3道），且 `randomIndex=4`（有余数除法）一次都没有生成

## What Changes

### 1. 修复测试日志打印答案问题
- **FIXED** 在Form1.cs的GenNum()方法中，为加法和减法添加答案日志输出
- **FIXED** 在UI/MainFormPresenter.cs的GenerateProblem()方法中，为所有题型添加统一的答案日志输出
- **FIXED** 修复变量名冲突问题（examObject -> currentExamObject）

**影响范围**：
- Form1.cs 第442行、469行
- UI/MainFormPresenter.cs 第225-249行

### 2. 优化全对奖励金币系统
- **ADDED** 在Core/AppConstants.cs中添加常量FullAnswerBonusCoefficient = 0.5
- **FIXED** 修改Form1.cs和UI/MainFormPresenter.cs中的奖励计算逻辑，使用常量而非硬编码
- **UPDATED** 更新文档openspec/specs/scoring-system/spec.md，反映新的计算逻辑

**计算公式变化**：
- 修改前：`int bonus = GameStateManager.examTtlRec / 2;`
- 修改后：`int bonus = (int)(GameStateManager.examTtlRec * AppConstants.FullAnswerBonusCoefficient);`

**影响范围**：
- Core/AppConstants.cs 第129-132行
- Form1.cs 第154行
- UI/MainFormPresenter.cs 第383行
- openspec/specs/scoring-system/spec.md 第29-32行

### 3. 修复LV5难度出题重试问题
- **FIXED** 修复Core/DivisionNoRemainderStrategy.cs中硬编码个位数的问题，使用intBits参数
- **FIXED** 修复Core/DivisionWithRemainderStrategy.cs中范围计算错误的问题
- **FIXED** 修改Core/DifficultyConfiguration.cs，移除LV5的RequireAbove100约束
- **FIXED** 修改UI/MainFormPresenter.cs中的验证逻辑：
  - LV5乘法使用两位数（effectiveIntBits=2），结果范围100-9801
  - 重构验证逻辑，根据IntegerBits区分处理（LV1-4检查<100，LV5仅检查加法≤2000）
- **UPDATED** 更新文档openspec/specs/math-operations/spec.md和difficulty-selection/spec.md

**根本原因分析**：
1. **除法策略问题**：
   - 无余数除法：硬编码个位数（1-9），完全忽略intBits参数
   - 有余数除法：max = 10^(intBits-1) 计算错误

2. **LV5配置矛盾**：
   - MinOperand = 1（允许1-999）vs RequireAbove100 = true（要求>=100）
   - 文档说除法生成"两位数"（10-99），但RequireAbove100要求至少一个数>=100
   - 导致除法无法生成有效题目

3. **验证逻辑设计缺陷**：
   - 原逻辑为二元：RequireAbove100=false（<100）vs true（>=100）
   - 实际需求是三元：LV1-4（<100）、LV5（允许100以上但有约束）

**修复后的行为**：
- LV1-LV4：操作数和结果<100（双重保险）
- LV5：
  - 加法：操作数100-999，结果≤2000
  - 减法：操作数100-999，结果0-899
  - 乘法：操作数10-99（两位数），结果100-9801
  - 除法：除数10-99（两位数），商10-99，结果100-9801

### 4. 修复答题后题型分布不均
- **FIXED** 将Form1.cs中答题正确和答题错误后的GenNum()调用改为_presenter.GenerateProblem()
- **FIXED** 确保所有题目生成都使用MainFormPresenter，保证题型均匀分布

**影响范围**：
- Form1.cs 第170行（答题正确后）
- Form1.cs 第219行（答题错误后）

**根本原因分析**：
1. **问题现象**：
   - 15题测试中只有1道有余数除法（randomIndex=4），期望约3道
   - `randomIndex=4`（有余数除法）一次都没有生成（15次随机选择中4一次都没出现）
   
2. **原因**：
   - Form1.cs的答题逻辑中调用了旧版 `GenNum()` 方法
   - `GenNum()` 方法使用旧的随机逻辑，不是均匀分布
   - 应该使用 `MainFormPresenter.GenerateProblem()`，它使用 `random.Next(0, 5)` 均匀选择题型

3. **解决方案**：
   - 将所有 `GenNum()` 调用改为 `_presenter.GenerateProblem()`
   - 确保所有题目生成使用统一的随机逻辑

**修复后的预期**：
- 题型分布应该接近均匀（各约20%）

**影响范围**：
- Core/DivisionNoRemainderStrategy.cs 第30-41行
- Core/DivisionWithRemainderStrategy.cs 第29-41行
- Core/DifficultyConfiguration.cs 第149-168行
- UI/MainFormPresenter.cs 第156-202行
- openspec/specs/math-operations/spec.md 第10行、第66-72行
- openspec/specs/difficulty-selection/spec.md 第12行

## Impact
- **Affected specs**: logging, scoring-system, math-operations, difficulty-selection
- **Affected code**:
  - Form1.cs - 答案日志输出、奖励计算
  - UI/MainFormPresenter.cs - 答案日志输出、奖励计算、题目生成和验证逻辑
  - Core/AppConstants.cs - 添加全对奖励系数常量
  - Core/DivisionNoRemainderStrategy.cs - 除法生成策略修复
  - Core/DivisionWithRemainderStrategy.cs - 除法生成策略修复
  - Core/DifficultyConfiguration.cs - LV5配置修复
- **Breaking changes**: None
- **Performance impact**: 轻微提升（减少重试次数）
- **User impact**:
  - ✅ 测试日志现在包含所有题型的答案，便于手动测试
  - ✅ 全对奖励金币便于通过常量调整
  - ✅ LV5难度所有题型都能正常生成，不再出现重试失败

## Test Plan
- [ ] 1.1 测试所有题型，确认测试日志中都有答案输出
- [ ] 1.2 测试全对奖励，确认奖励数量为题目数/2（向下取整）
- [ ] 2.1 测试LV5加法，确认能正常生成，结果≤2000
- [ ] 2.2 测试LV5减法，确认能正常生成，结果0-899
- [ ] 2.3 测试LV5乘法，确认使用两位数（10-99），结果100-9801
- [ ] 2.4 测试LV5无余数除法，确认除数和商都是两位数（10-99）
- [ ] 2.5 测试LV5有余数除法，确认除数和商都是两位数（10-99）
- [ ] 2.6 压力测试：在LV5难度下运行100道题，验证所有题型都能正常生成，无重试失败
- [ ] 3.1 回归测试LV1-LV4难度，确认原有功能不受影响
- [ ] 3.2 检查日志输出，确认格式统一且清晰
