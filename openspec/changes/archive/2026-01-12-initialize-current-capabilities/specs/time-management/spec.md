## ADDED Requirements

### Requirement: 时间限制与超时管理
系统应为每道题目设置时间限制，超时自动判定为错误并增加惩罚题目。

#### Scenario: 题目时间限制
- **GIVEN** 整数乘法题型配置 TimeLimit=300 秒
- **WHEN** 生成该题型题目
- **THEN** 倒计时器初始化为 300 秒

#### Scenario: 倒计时正常进行
- **GIVEN** 当前题目剩余时间 150 秒
- **WHEN** 经过 1 秒
- **THEN** 剩余时间更新为 149 秒

#### Scenario: 在时间限制内答题
- **GIVEN** 题目剩余时间 60 秒
- **WHEN** 用户提交正确答案
- **THEN** 记录用时 = 初始时间 - 剩余时间，进入下一题

#### Scenario: 超时惩罚
- **GIVEN** 题目剩余时间 0 秒，超时惩罚 punishmentTimeOut=1
- **WHEN** 超时发生时
- **THEN** 剩余题目数增加 1 题，生成新题目，重置倒计时器

#### Scenario: 超时进度条显示
- **GIVEN** 题目时间限制 300 秒，当前剩余 150 秒
- **WHEN** 显示进度条
- **THEN** 进度条显示 50% 进度

#### Scenario: 时间限制随题型变化
- **GIVEN** 整数乘法题型 TimeLimit=300 秒，有余数除法 TimeLimit=400 秒
- **WHEN** 生成不同题型题目
- **THEN** 倒计时器使用对应题型的 TimeLimit 值
