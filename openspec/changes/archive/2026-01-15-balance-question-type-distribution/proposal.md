# Change: 均衡题型概率分布

## Why
当前出题逻辑采用每次从所有题型中随机选择的方式，在有限的题目数内（如15题）存在概率偏差。例如在 LV5 难度下，15题中理论上每种题型应为3题，但实际测试发现有余数除法仅有2题。虽然从统计学角度这种偏差是正常的，但对于教育软件来说，保证练习的全面性和公平性更重要，应确保在有限题目数内各题型尽可能均匀分布。

## What Changes
- 修改出题逻辑，在练习开始时预先分配每种题型的题目数量，确保均匀分布
- 使用"随机池"机制维护剩余题型列表，每次从池中随机抽取一种题型
- 当某种题型的配额耗尽时，从池中移除该题型
- 支持题目总数小于题型种类数的情况（如1题对应5种题型）
- 保持向后兼容，不影响现有的难度配置和验证逻辑

## Impact
- **Affected specs**: difficulty-selection, math-operations
- **Affected code**:
  - `UI/MainFormPresenter.cs` - 修改 GenerateProblem() 方法
  - `Core/GameStateManager.cs` - 可能需要新增题型分配状态管理
  - `Core/DifficultyConfiguration.cs` - 可能需要添加题型分配相关配置
