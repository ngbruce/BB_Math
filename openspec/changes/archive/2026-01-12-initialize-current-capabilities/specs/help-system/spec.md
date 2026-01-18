## ADDED Requirements

### Requirement: 辅助功能系统
系统应提供辅助功能，帮助用户答题，包括检查答案和给出提示，使用金币购买辅助。

#### Scenario: 启用辅助框
- **GIVEN** 配置 helpBox=true
- **WHEN** 生成有余数除法题目
- **THEN** 显示辅助输入框，标签为"余数："

#### Scenario: 禁用辅助框
- **GIVEN** 配置 helpBox=false
- **WHEN** 生成题目
- **THEN** 不显示辅助输入框

#### Scenario: 免费检查辅助
- **GIVEN** 配置 helpBoxFree=true，当前有余数除法题目，余数为 4
- **WHEN** 用户在辅助框输入 4
- **THEN** 辅助框背景变为绿色，不消耗金币

#### Scenario: 付费检查辅助
- **GIVEN** 配置 helpBoxFree=false，helpBox=true，当前金币 50，花费 1，有余数除法题目余数为 4
- **WHEN** 用户点击检查按钮，输入余数 4
- **THEN** 金币减少为 49，辅助框背景变为绿色

#### Scenario: 给出答案提示
- **GIVEN** 配置 helpBox=true，当前有余数除法题目余数为 4
- **WHEN** 用户使用给出答案功能（花费 3 金币）
- **THEN**: 辅助框显示正确答案 4，背景变为绿色，金币减少 3

#### Scenario: 辅助框输入错误
- **GIVEN** 有余数除法题目余数为 4
- **WHEN** 用户在辅助框输入 3
- **THEN**: 辅助框背景变为红色（无论是否使用检查功能）

#### Scenario: 辅助功能对普通题目不可用
- **GIVEN** 当前为加法题目
- **WHEN** 生成题目
- **THEN** 辅助输入框隐藏或不可用
