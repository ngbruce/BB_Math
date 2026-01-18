# Change: 重构难度等级并修复余数显示bug

## Why
当前系统仅支持3个难度级别（Easy、Medium、Hard），且难度定义过于简单。用户需要更细粒度的难度分级以适应不同学习阶段。同时发现"100以内加减乘除"模式下的有余数除法时，余数专用控件(tbRemainder、lbRemainder)未正确显示，影响答题体验。

## What Changes
### Requirement: 难度等级扩展
系统**SHALL**将难度枚举从3个级别扩展为5个级别：LV1、LV2、LV3、LV4、LV5，以支持更细粒度的难度分级。

#### Scenario: 难度枚举包含5个级别
- **GIVEN** 系统启动
- **WHEN** 访问 Difficulty 枚举
- **THEN** 应包含五个枚举值：LV1、LV2、LV3、LV4、LV5

### Requirement: 难度配置更新
系统**SHALL**更新DifficultyConfigurationManager，为每个难度级别（LV1-LV5）定义明确的题目参数配置。

#### Scenario: LV1-LV5难度配置存在
- **GIVEN** 系统配置
- **WHEN** 查询各难度配置
- **THEN** 每个难度应包含数值范围、运算类型、整数位数等参数

### Requirement: 主窗体逻辑重构
系统**SHALL**修改Form1.cs，实现动态难度选择和出题逻辑。

#### Scenario: 动态初始化难度选择控件
- **GIVEN** 主窗体加载
- **WHEN** 初始化 cbDifficultySelect 控件
- **THEN** 应动态添加5个难度选项，而非硬编码

#### Scenario: 根据难度生成题目
- **GIVEN** 用户选择难度级别
- **WHEN** 生成题目
- **THEN** 应根据难度配置生成符合要求的题目

### Requirement: 余数控件显示修复
系统**SHALL**修复有余数除法时余数控件不显示的bug，确保currentTypeIndex正确同步。

#### Scenario: 有余数除法显示余数控件
- **GIVEN** 当前题型为 DivisionWithRemainder
- **WHEN** 显示题目
- **THEN** 余数输入框和标签应可见

### Requirement: 规范更新
系统**SHALL**更新difficulty-selection和math-operations规范，反映新的难度级别定义和出题逻辑。

## Impact
- Affected specs: difficulty-selection, math-operations
- Affected code:
  - Core/Difficulty.cs - 难度枚举定义
  - Core/DifficultyConfiguration.cs - 难度配置管理
  - Form1.cs - 主窗体逻辑，包含难度选择控件初始化、出题逻辑
  - Core/GameStateManager.cs - 状态管理器（currentDifficulty字段）
