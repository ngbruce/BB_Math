# Change: 修复余数输入框显示问题并优化日志输出

## Why
在LV4"(4)100以内加减乘除带余数"难度下，余数输入框(tbRemainder)的显示逻辑存在多个问题：
1. 无论题型是否为有余数除法，余数输入框都可能错误显示或隐藏
2. 程序启动时出现大量重复的同步日志(100+行)
3. 答题后的日志顺序混乱，容易导致误解
4. 偶尔出现生成题目失败的错误，需要更多测试定位根因

## What Changes
- **FIXED** 修复MainFormPresenter.GenerateProblem()中未同步currentTypeIndex导致余数输入框显示错误的问题
- **FIXED** 修复Form1.UpdateDisp()中余数输入框显示逻辑，移除helpBox的影响，仅根据题型决定是否显示
- **FIXED** 修复Form1.answer()中多个位置使用helpBox判断余数输入框逻辑的问题，改为判断题型
- **FIXED** 修复重复日志问题：将MainFormPresenter.GenerateProblem()中的递归重试改为do-while循环，避免每次重试都输出同步日志
- **FIXED** 优化日志顺序：调整Form1.answer()中的日志输出，使其更清晰地反映执行流程
- **FIXED** 修复DivisionNoRemainderStrategy中intBits使用不当的问题，确保生成正确的除法题目
- **FIXED** 修复LV4难度下乘法题目生成失败的问题：乘法使用intBits=1（个位数），避免结果总是>=100导致无限重试
- **ADDED** 在MainFormPresenter.GenerateProblem()中添加详细的调试日志，便于定位生成题目失败的原因

## Impact
- Affected specs: math-operations, logging
- Affected code:
  - UI/MainFormPresenter.cs - 生成题目逻辑，添加currentTypeIndex同步和详细日志
  - Form1.cs - 余数输入框显示逻辑、答题验证逻辑
  - Core/DivisionNoRemainderStrategy.cs - 除法生成策略
  - Core/GameStateManager.cs - 状态管理器（currentTypeIndex字段）
