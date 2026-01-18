# Change: 初始化当前系统能力规范

## Why
根据 OpenSpec 规范原则 "Specs are truth"，需要将当前已实现的功能记录为正式的能力规范。这为后续重构提供基线，确保规范与代码实现保持同步，同时为项目文档提供完整的能力描述。

## What Changes
创建以下能力规范，准确反映当前系统的已实现功能：

- **math-operations** - 数学题目生成与答案验证（加减乘除、有余数除法）
- **scoring-system** - 金币计分与奖励系统
- **pause-mechanism** - 暂停机制（限次数/限时间）
- **process-monitoring** - 进程监控与黑名单管理
- **logging** - 日志记录与历史追踪
- **configuration-management** - 配置文件管理（INI格式）
- **exam-type-management** - 题型配置与管理
- **time-management** - 时间限制与超时惩罚
- **punishment-system** - 错误惩罚机制
- **help-system** - 辅助功能（检查答案、给出提示）

## Impact
- **Affected specs**: 
  - 新增 10 个能力规范到 `specs/` 目录
  - 无现有规范需要修改

- **Affected code**: 无代码修改，仅文档规范化

- **Breaking changes**: 无

- **Migration required**: 无
