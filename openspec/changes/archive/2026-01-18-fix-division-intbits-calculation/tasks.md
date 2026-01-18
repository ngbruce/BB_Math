# 实现任务清单

## 1. 修复 DivisionNoRemainderStrategy 的 intBits 计算
- [x] 1.1 修改 `Core/DivisionNoRemainderStrategy.cs` 第 35 行的 max 计算公式
- [x] 1.2 将 `max = (int)Math.Pow(10, intBits - 1)` 改为 `max = (int)Math.Pow(10, intBits)`
- [x] 1.3 验证修改后公式与 `DivisionWithRemainderStrategy` 一致

## 2. 修复 LV4 难度除法的 effectiveIntBits 调整
- [x] 2.1 在 `UI/MainFormPresenter.cs` 第 236-262 行添加除法的 effectiveIntBits 调整
- [x] 2.2 为 LV4 难度的除法（包括无余数除法和有余数除法）设置 effectiveIntBits = 1
- [x] 2.3 添加调试日志，记录 effectiveIntBits 调整

## 3. 验证所有难度的除法生成逻辑
### 2.1 验证 LV3 难度（个位数乘除法）
- [x] 2.1.1 运行 15-30 道除法题目，验证除数和商不相同
- [x] 2.1.2 验证除数和商都在 1-9 范围内
- [x] 2.1.3 确认除法题目能被整除（无余数）
- [x] 2.1.4 检查是否有相同除数和商的题目（应该是极少数）

### 2.2 验证 LV4 难度（100以内加减乘除带余数）
- [x] 2.2.1 运行 15-30 道除法题目，验证除数和商在 10-99 范围内
- [x] 2.2.2 验证除法题目能被整除
- [x] 2.2.3 确认除数和商不相同（大部分情况）

### 2.3 验证 LV5 难度（100以上加减乘除带余数）
- [x] 2.3.1 运行 15-30 道除法题目，验证除数和商在 10-99 范围内
- [x] 2.3.2 验证除法题目能被整除
- [x] 2.3.3 确认除数和商不相同（大部分情况）

## 3. 对比两种除法策略
- [x] 3.1 检查 `DivisionNoRemainderStrategy` 的 intBits 计算
- [x] 3.2 检查 `DivisionWithRemainderStrategy` 的 intBits 计算
- [x] 3.3 确认两者的 intBits 计算公式现在一致

## 4. 更新规范文档
- [x] 4.1 在 `openspec/changes/fix-division-intbits-calculation/specs/math-operations/spec.md` 中添加 intBits 计算要求
- [x] 4.2 添加场景描述：除法策略应正确使用 intBits 参数
- [x] 4.3 确保两种除法策略的 intBits 解释一致

## 5. 验证和测试
- [x] 5.1 运行 `openspec validate fix-division-intbits-calculation --strict` 验证变更
- [x] 5.2 观察日志，确认除法题目生成正常
- [x] 5.3 检查所有测试通过，问题已彻底解决
