## Purpose
惩罚系统在用户答错或超时触发惩罚机制，自动增加额外题目，确保用户完成所有必要练习，从而达到熟练掌握数学运算的目标。

## Requirements

### Requirement: 错误惩罚机制
(SHALL) 用户答错题目时，系统应增加惩罚题目，惩罚数量可配置。

#### Scenario: 答错增加惩罚题目
- **GIVEN** 当前剩余题目 10 题，错误惩罚 punishment=2
- **WHEN** 用户答错一题
- **THEN** 剩余题目增加为 12 题（10 + 2）

#### Scenario: 惩罚题目统计
- **GIVEN** 整数乘法题型，初始 TotalQty=20
- **WHEN** 用户答错 2 题（每次惩罚 2 题）
- **THEN** WrongQty=2, TotalQty=24（20 + 2×2）

#### Scenario: 惩罚后继续答题
- **GIVEN** 用户答错后剩余题目增加
- **WHEN** 生成新题目
- **THEN** 用户必须完成惩罚增加的题目才能通关

#### Scenario: 惩罚配置为 0
- **GIVEN** 错误惩罚 punishment=0
- **WHEN** 用户答错题目
- **THEN** 剩余题目数不变，只记录错误数

#### Scenario: 惩罚不影响金币
- **GIVEN** 用户答错题目，触发惩罚机制
- **WHEN** 增加惩罚题目
- **THEN** 金币数不变化（惩罚只增加题目数，不扣除金币）
