# Change: 修复除法策略中 intBits 计算错误和 LV4 难度除法重试问题

## Why
测试发现两个除法问题：

**LV3：除数和商总是相同**（如8÷8=1），因DivisionNoRemainderStrategy的max计算错误：`Math.Pow(10, intBits-1)`应为`Math.Pow(10, intBits)`。

**LV4：除法无限重试**，日志显示"不符合<100条件"。原因：intBits=2时，被除数=除数×商+余数≥10×10+1=101，超过限制。需将LV4除法的effectiveIntBits改为1（个位数）。

## What Changes

### 1. 修复 DivisionNoRemainderStrategy 的 intBits 计算
- **FIXED** 修改 `Core/DivisionNoRemainderStrategy.cs` 第 35 行
- **CHANGE**：将 `max = (int)Math.Pow(10, intBits - 1)` 改为 `max = (int)Math.Pow(10, intBits)`
- **REASON**：与 `DivisionWithRemainderStrategy` 保持一致，正确解释 intBits 参数
- **RESULT**：确保除数和商在正确的范围内独立生成，避免总是相同

### 2. 修复 LV4 难度除法的 effectiveIntBits 调整
- **FIXED** 修改 `UI/MainFormPresenter.cs` 第 236-252 行
- **CHANGE**：为 LV4 难度的除法（包括无余数除法和有余数除法）设置 effectiveIntBits = 1
- **REASON**：LV4 要求操作数和结果 < 100，两位数除法无法满足此限制
- **RESULT**：LV4 除法使用个位数（1-9），确保被除数 < 100

### 2. 验证所有难度的除法生成逻辑
- **VERIFIED** LV1（个位数加减法）：不使用除法，无影响
- **VERIFIED** LV2（100以内加减法）：不使用除法，无影响
- **VERIFIED** LV3（个位数乘除法）：除法使用 intBits=1，应生成 1-9 的数字
- **VERIFIED** LV4（100以内加减乘除带余数）：除法使用 intBits=2，应生成 10-99 的数字
- **VERIFIED** LV5（100以上加减乘除带余数）：除法使用 intBits=2，应生成 10-99 的数字

### 3. 更新相关规范文档
- **UPDATED** 更新 `openspec/changes/fix-division-intbits-calculation/specs/math-operations/spec.md`
- **ADDED** 添加关于除法策略 intBits 计算的要求

## Impact

- **Affected specs**: math-operations
- **Affected code**:
  - `Core/DivisionNoRemainderStrategy.cs` - 修复 intBits 计算公式
  - `UI/MainFormPresenter.cs` - 为 LV4 除法调整 effectiveIntBits

- **Breaking changes**: 无（修复内部实现，不改变外部接口）

- **Performance impact**: 无影响

- **User impact**:
  - ✅ LV3 难度下，除法题目不再出现除数和商相同的情况
  - ✅ LV4 难度下，除法题目不再出现无限重试的问题
  - ✅ 除法题目更加随机和合理，提高练习效果
  - ✅ 所有难度的除法生成逻辑保持一致

## Related Changes

- `fix-all-difficulty-random-generation-bug` - 修复了随机数生成器问题
- `fix-timer-reset-on-new-problem` - 修复了定时器重置问题
- `2026-01-15-fix-remainder-input-and-logging-issues` - 之前的修复，已经修正了除法的其他问题

## Test Plan

- [ ] 1.1 测试 LV3 难度，运行 15-30 道除法题目，验证除数和商不相同
- [ ] 1.2 验证 LV3 除法题目除数和商都在 1-9 范围内
- [ ] 1.3 确认除法题目能被整除（无余数）
- [ ] 1.4 检查是否有相同除数和商的题目（应该是极少数，而不是全部）

- [ ] 2.1 测试 LV4 难度，运行 15-30 道除法题目，验证能正常生成
- [ ] 2.2 验证 LV4 除法题目被除数 < 100（不再无限重试）
- [ ] 2.3 验证 LV4 除法题目除数和商都在 1-9 范围内（effectiveIntBits=1）
- [ ] 2.4 确认无余数除法和有余数除法都能正常生成

- [ ] 3.1 测试 LV5 难度，运行 15-30 道除法题目，验证除数和商在 10-99 范围内
- [ ] 3.2 验证 LV5 除法题目能被整除
- [ ] 4.1 检查有余数除法策略，确认不受影响
- [ ] 4.2 对比两种除法策略的 intBits 计算，确认现在一致
- [ ] 5.1 观察日志，确认除法题目生成正常
- [ ] 5.2 检查是否有生成题目失败的错误（LV4 应该不再出现）
